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
}
