using Akka.Actor;

namespace Durian.Network
{
    public abstract class Server : ReceiveActor
    {
        protected void Connected(Connection connection)
        {
        }
    }
}
