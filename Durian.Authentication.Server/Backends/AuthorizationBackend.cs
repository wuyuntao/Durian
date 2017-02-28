using Akka.Actor;
using System.Threading.Tasks;
using System;

namespace Durian.Authentication
{
    public abstract class AuthorizationBackend : ReceiveActor
    {
        protected AuthorizationBackend()
        {
            Receive<Authorize>(OnAuthorize, null);
        }

        protected abstract void OnAuthorize(Authorize msg);

        protected sealed class Envelope
        {
            public IActorRef Sender { get; private set; }

            public object Message { get; private set; }

            public Envelope(IActorRef sender, object message)
            {
                Sender = sender;
                Message = message;
            }
        }
    }
}
