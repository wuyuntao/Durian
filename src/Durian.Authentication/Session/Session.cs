using Akka.Actor;
using System;

namespace Durian.Authentication
{
    public enum SessionState
    {
        Unauthenticated,
        Authenticating,
        Authenticated,
        Stopped,
    }

    public class Session : FSM<SessionState, object>
    {
        private readonly IActorRef connection;

        public Session(IActorRef connection)
        {
            this.connection = connection;

            StartWith(SessionState.Unauthenticated, null);

            When(SessionState.Unauthenticated, e =>
            {
                switch (e.FsmEvent)
                {
                    case Authorize a:
                        Sender.Tell(e.FsmEvent);
                        return GoTo(SessionState.Authenticating);

                    case StateTimeout t:
                        return GoTo(SessionState.Stopped);

                    default:
                        return null;
                }
            }, TimeSpan.FromSeconds(3));

            When(SessionState.Authenticating, e =>
            {
                switch (e.FsmEvent)
                {
                    case AuthorizationSucceeded r:
                        connection.Tell(r);
                        return GoTo(SessionState.Authenticated);

                    case AuthorizationFailed r:
                        connection.Tell(r);
                        return GoTo(SessionState.Unauthenticated);

                    default:
                        return GoTo(SessionState.Unauthenticated);
                }
            });

            When(SessionState.Authenticated, e =>
            {
                // TODO forward messages by predefined routines
                return null;
            });

            OnTransition((from, to) =>
            {
                if (to == SessionState.Stopped)
                {
                    Context.Stop(Self);
                    return;
                }
            });

            Initialize();
        }
    }
}
