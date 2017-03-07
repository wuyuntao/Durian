using Akka.Actor;
using Durian.IO;
using System.Net;

namespace Durian.Authentication
{
    public class SessionService : ReceiveActor
    {
        IActorRef server;

        public SessionService()
        {
            // TODO configure server settings
            server = Context.ActorOf(Server.Props(null));
            Context.Watch(server);

            // TODO configure server settings
            server.Tell(new Bind(new IPEndPoint(IPAddress.Loopback, 1000)));

            Receive<Connected>(m =>
            {
                //Context.ActorOf()
            });
        }
    }
}
