namespace Durian.Authentication
{
    class DevelopmentBackend : AuthorizationBackend
    {
        public DevelopmentBackend()
        {
            Receive<Authorize>(msg =>
            {
                Sender.Tell(new AuthorizationSucceeded($"{msg.Backend}-{msg.Uid}", null), Self);
            });
        }
    }
}
