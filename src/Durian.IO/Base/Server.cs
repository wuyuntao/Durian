using Akka.Actor;

namespace Durian.IO
{
    public abstract class Server : ReceiveActor
    {
        public static Props Props(TcpServerConfig config)
        {
            return Akka.Actor.Props.Create(() => new TcpServer(config));
        }
    }
}
