using Akka.Actor;
using DotNetty.Transport.Channels;

namespace Durian.Network
{
    public class TcpConnection : Connection
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
    }
}
