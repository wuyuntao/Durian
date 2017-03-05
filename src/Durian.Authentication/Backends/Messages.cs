namespace Durian.Authentication
{
    public sealed class Authorize
    {
        public string Backend { get; private set; }

        public string Uid { get; private set; }

        public string Token { get; private set; }

        public string Extra { get; private set; }

        public Authorize(string serviceName, string uid, string token, string extra)
        {
            Backend = serviceName;
            Uid = uid;
            Token = token;
            Extra = extra;
        }
    }

    public class AuthorizationSucceeded
    {
        public string UserId { get; private set; }

        public string SessionId { get; private set; }

        public AuthorizationSucceeded(string userId)
        {
            UserId = userId;
        }
    }

    public sealed class AuthorizationFailed
    {
        public string Reason { get; private set; }

        public AuthorizationFailed(string reason)
        {
            Reason = reason;
        }
    }
}
