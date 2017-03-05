using Akka.Actor;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;

namespace Durian.IO
{
    class TcpConnection : Connection
    {
        private readonly IChannelHandlerContext context;

        private TcpConnection(IChannelHandlerContext context)
        {
            this.context = context;
        }

        internal static Props Props(IChannelHandlerContext context)
        {
            return Akka.Actor.Props.Create(() => new TcpConnection(context));
        }

        internal override Task WriteAsync(object payload)
        {
            return context.WriteAndFlushAsync(payload);
        }
    }
}
