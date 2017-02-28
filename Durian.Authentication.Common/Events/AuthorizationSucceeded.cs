namespace Durian.Authentication
{
    public class AuthorizationSucceeded
    {
        public string UserId { get; private set; }

        public string SessionId { get; private set; }

        public AuthorizationSucceeded(string userId)
        {
            UserId = userId;
        }
    }
}
