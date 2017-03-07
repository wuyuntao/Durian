using Akka.Configuration;
using Akka.Event;
using Akka.Persistence;
using System;

namespace Durian.Authentication
{
    class UserState
    {
        public string UserId { get; private set; }

        public string SessionId { get; private set; }

        public DateTime? SessionExpirationTime { get; private set; }

        public UserState(string userId, string sesionId, DateTime? sessionExpirationTime)
        {
            UserId = userId;
            SessionId = sesionId;
            SessionExpirationTime = sessionExpirationTime;
        }

        public UserState Apply(SessionCreated e)
        {
            return new UserState(UserId, e.SessionId, e.SessionExpirationTime);
        }
    }

    public class User : ReceivePersistentActor
    {
        private readonly ILoggingAdapter log = Logging.GetLogger(Context);

        private UserState state;

        public User()
        {
            var ttl = Context.System.Settings.Config.GetInt("durian.authentication.session-ttl");
            if (ttl <= 0)
                throw new ConfigurationException($"Invalid session TTL: {ttl}");

            state = new UserState(Self.Path.Name, null, null);

            Recover<SnapshotOffer>(so => state = (UserState)so.Snapshot);
            Recover<SessionCreated>(e => state = state.Apply(e));

            Command<CreateSession>(c =>
            {
                // Not necessary to reset session while it is not expired
                if (state.SessionId != null && state.SessionExpirationTime > DateTime.UtcNow)
                {
                    Sender.Tell(new SessionCreated(state.SessionId, state.SessionExpirationTime.Value), Self);
                }
                else
                {
                    var sender = Sender;
                    var sessionId = Guid.NewGuid().ToString();
                    var expirationTime = DateTime.UtcNow + TimeSpan.FromSeconds(ttl);

                    Persist(new SessionCreated(sessionId, expirationTime), e =>
                    {
                        log.Info("Session is updated '{1}' and will be expired at {2}", state.UserId, sessionId, expirationTime);
                        state = state.Apply(e);
                        sender.Tell(e, Self);
                    });
                }
            });
        }

        public override string PersistenceId
        {
            get { return $"{GetType().Name}-{state.UserId}"; }
        }
    }
}
