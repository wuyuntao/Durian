using Akka.Actor;
using DotNetty.Codecs.Protobuf;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Durian.Network
{
    public class TcpServer : Server
    {
        private readonly TcpServerConfig config;
        private IChannel channel;

        private TcpServer(TcpServerConfig config)
        {
            this.config = config;
        }

        public static Props Props(TcpServerConfig config)
        {
            return Akka.Actor.Props.Create(() => new TcpServer(config));
        }

        protected override void PreStart()
        {
            base.PreStart();

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

            // TODO Handler exceptions
            var t = bootstrap.BindAsync(config.Address);
            t.RunSynchronously();
            channel = t.Result;
        }

        protected override void PostStop()
        {
            // TODO Handler exceptions
            channel.CloseAsync().RunSynchronously();

            base.PostStop();
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

                connection.Tell(new Connected(connection), server);
            }

            public override void ChannelInactive(IChannelHandlerContext context)
            {
                connection.Tell(new Disconnected(connection), server);
            }

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
                connection.Tell(new MessageReceived(message), server);
            }
        }
    }
}
