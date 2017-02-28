using System;

namespace Durian.Authentication
{
    class DevelopmentBackend : AuthorizationBackend
    {
        protected override void OnAuthorize(Authorize msg)
        {
            Sender.Tell(new AuthorizationSucceeded($"{msg.Backend}-{msg.Uid}"), Self);
        }
    }
}
