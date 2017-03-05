using Akka.Actor;
using System.Net;

namespace Durian.Network
{
    public class Bind
    {
        public IPEndPoint LocalAddress { get; private set; }

        public Bind(IPEndPoint localAddress)
        {
            LocalAddress = localAddress;
        }
    }

    public class Bound
    {
        public IPEndPoint LocalAddress { get; private set; }

        public Bound(IPEndPoint localAddress)
        {
            LocalAddress = localAddress;
        }
    }

    public class Unbind
    {
    }

    public class Unbound
    {
    }

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
        public Disconnected()
        {
        }
    }

    public class Received
    {
        public object Payload { get; private set; }

        public Received(object payload)
        {
            Payload = payload;
        }
    }

    public class Send
    {
        public object Payload { get; private set; }

        public Send(object payload)
        {
            Payload = payload;
        }
    }

    public class Register
    {
        public IActorRef Handler { get; private set; }

        public Register(IActorRef handler)
        {
            Handler = handler;
        }
    }

    public class Unregister
    {
        public IActorRef Handler { get; private set; }

        public Unregister(IActorRef handler)
        {
            Handler = handler;
        }
    }
}
