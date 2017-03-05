using Akka.Actor;

namespace Durian.Network
{
    public class Connected
    {
        public IActorRef Connection { get; private set; }

        public Connected(IActorRef connection)
        {
            Connection = connection;
        }
    }

    public class Disconnected
    {
        public IActorRef Connection { get; private set; }

        public Disconnected(IActorRef connection)
        {
            Connection = connection;
        }
    }

    public class MessageReceived
    {
        public object Payload { get; private set; }

        public MessageReceived(object payload)
        {
            Payload = payload;
        }
    }
}
