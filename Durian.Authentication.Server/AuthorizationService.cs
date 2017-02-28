using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using System;
using System.Collections.Generic;

namespace Durian.Authentication
{
    public sealed class AuthorizationService : ReceiveActor
    {
        private readonly ILoggingAdapter log = Context.GetLogger();

        private readonly Dictionary<string, IActorRef> routers = new Dictionary<string, IActorRef>();

        public AuthorizationService()
        {
            var authenticationConfig = AuthenticationConfigFactory.Default();
            var settings = Context.System.Settings;
            settings.InjectTopLevelFallback(authenticationConfig);

            var deployer = new Deployer(settings);
            var config = settings.Config;
            var fallbackConfig = config.GetConfig("durian.authentication.backends.fallback");

            var backends = config.GetStringList("durian.authentication.backends.enabled");

            foreach (var backend in backends)
            {
                var backendConfig = config.GetConfig($"durian.authentication.backends.{backend}");
                if (backendConfig == null)
                    throw new ConfigurationException($"Unknown authentication backend '{backend}'");
                backendConfig = backendConfig.WithFallback(fallbackConfig);

                var backendTypeName = backendConfig.GetString("type");
                if (string.IsNullOrEmpty(backendTypeName))
                    throw new ConfigurationException($"Type not defined for backend '{backend}'");

                var backendType = Type.GetType(backendTypeName);
                if (backendType == null)
                    throw new ConfigurationException($"Backend not found: '{backendTypeName}'");

                var routerDeploy = deployer.ParseConfig(backend, backendConfig);
                var routerProps = Props.Create(backendType).WithRouter(routerDeploy.RouterConfig);
                var router = Context.ActorOf(routerProps, $"authentication/{backend}");

                routers.Add(backend, router);
            }

            ReceiveAsync<Authorize>(async m =>
            {
                IActorRef router;
                if (routers.TryGetValue(m.Backend, out router))
                {
                    var r = await router.Ask(m);

                    Sender.Tell(r, Self);
                }
                else
                    Sender.Tell(new AuthorizationFailed($"Unknown authentication backend '{m.Backend}'"));
            });
        }
    }
}
