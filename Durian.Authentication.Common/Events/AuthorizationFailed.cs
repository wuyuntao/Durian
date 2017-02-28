namespace Durian.Authentication
{
    public sealed class AuthorizationFailed
    {
        public string Reason { get; private set; }

        public AuthorizationFailed(string reason)
        {
            Reason = reason;
        }
    }
}
