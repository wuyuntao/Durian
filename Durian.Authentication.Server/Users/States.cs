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

        public UserState Apply(object e)
        {
            if (e is SessionCreated)
            {
                var sc = (SessionCreated)e;

                return new UserState(UserId, sc.SessionId, sc.SessionExpirationTime);
            }
            else
                return this;
        }
    }
}
