using Akka.Actor;
using DotNetty.Codecs.Protobuf;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;

namespace Durian.IO
{
    class TcpServer : Server
    {
        private readonly TcpServerConfig config;
        private IActorRef monitor;
        private IChannel channel;

        public TcpServer(TcpServerConfig config)
        {
            this.config = config;

            ReceiveAsync<Bind>(async m =>
            {
                try
                {
                    var bootstrap = new ServerBootstrap();
                    bootstrap
                        // TODO configure to use seperate event loop groups
                        .Group(new MultithreadEventLoopGroup())
                        .Channel<TcpServerSocketChannel>()
                        // TODO configure socket options
                        .Option(ChannelOption.SoBacklog, 100)
                        .Option(ChannelOption.SoRcvbuf, 1024 * 16)
                        .Option(ChannelOption.SoSndbuf, 1024 * 16)
                        // TODO configure logging
                        //.Handler(new LoggingHandler("ping-pong-server"))
                        .ChildHandler(new ChannelInitializer(Context.System, Self));

                    // TODO Any threading risk here?
                    channel = await bootstrap.BindAsync(m.LocalAddress);
                    monitor = Sender;

                    Sender.Tell(new Bound(m.LocalAddress));

                    Become(Bound);
                }
                catch (Exception)
                {
                    // TODO Handler exceptions
                    channel = null;
                    monitor = null;
                }
            });
        }

        private void Bound()
        {
            ReceiveAsync<Unbind>(async m =>
            {
                // TODO Handler exceptions
                // TODO Any threading risk here?
                try
                {
                    await channel.CloseAsync();

                    Sender.Tell(new Unbound());
                }
                catch (Exception)
                {

                }
                finally
                {
                    Context.Stop(Self);
                }
            });

            Receive<Connected>(m => monitor.Tell(m, m.Connection));
        }

        class ChannelInitializer : ChannelInitializer<ISocketChannel>
        {
            private readonly ActorSystem system;
            private readonly IActorRef server;

            public ChannelInitializer(ActorSystem system, IActorRef server)
            {
                this.system = system;
                this.server = server;
            }

            protected override void InitChannel(ISocketChannel channel)
            {
                channel.Pipeline
                    // TODO configure logging
                    //.AddLast(new LoggingHandler("ping-pong-client"))
                    .AddLast("frame-decoder", new ProtobufVarint32FrameDecoder())
                    // TODO Initialize message parser
                    .AddLast("protobuf-decoder", new ProtobufDecoder(null))
                    .AddLast("frame-encoder", new ProtobufVarint32LengthFieldPrepender())
                    .AddLast("protobuf-encoder", new ProtobufEncoder())
                    .AddLast("message-handler", new ChannelHandler(system, server));
            }
        }

        class ChannelHandler : ChannelHandlerAdapter
        {
            private readonly ActorSystem system;
            private readonly IActorRef server;
            private IActorRef connection;

            public ChannelHandler(ActorSystem system, IActorRef server)
            {
                this.system = system;
                this.server = server;
            }

            public override void ChannelActive(IChannelHandlerContext context)
            {
                // TODO Assign connection name
                connection = system.ActorOf(TcpConnection.Props(context));

                server.Tell(new Connected(connection));
            }

            public override void ChannelInactive(IChannelHandlerContext context)
            {
                connection.Tell(new Disconnected());
            }

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
                connection.Tell(new Received(message));
            }
        }
    }
}
