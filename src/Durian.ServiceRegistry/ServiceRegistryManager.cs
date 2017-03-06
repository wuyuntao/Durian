using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using System;

namespace Durian.ServiceRegistry
{
    public sealed class ServiceRegistryManager : ReceivePersistentActor
    {
        private readonly ILoggingAdapter log = Logging.GetLogger(Context);

        private ServiceRegistryState state = new ServiceRegistryState();

        public override string PersistenceId => $"{GetType().Name}-{Self.Path.Name}";

        public ServiceRegistryManager()
        {
            Recover<SnapshotOffer>(so => state = (ServiceRegistryState)so.Snapshot);
            Recover<Registered>(e => state = state.Apply(e));
            Recover<Unregistered>(e => state = state.Apply(e));

            Command<Register>(c =>
            {
                PersistAsync(new Registered(c.ServiceMeta), e =>
                {
                    state = state.Apply(e);
                    SaveSnapshot(state);
                    Sender.Tell(e);
                });
            });
            Command<Unregister>(c =>
            {
                if (state.Services.ContainsKey(c.ServiceName))
                {
                    PersistAsync(new Unregistered(c.ServiceName), e =>
                    {
                        state = state.Apply(e);
                        SaveSnapshot(state);
                        Sender.Tell(e);
                    });
                }
            });
            Command<string>(s =>
            {
                // TODO Configure expiration time
                var expirationTime = DateTime.UtcNow - TimeSpan.FromSeconds(30);

                foreach (var service in state.Services.Values)
                {
                    if (service.RegistrationTime < expirationTime)
                        Self.Tell(new Unregister(service.Name));
                }
            }, s => s == "CheckHealth");

            Command<QueryAll>(q => Sender.Tell(new QueryAllResponse(state.Services.Values)));

            // TODO Configure check health interval
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30), Self, "CheckHealth", null);
        }
    }
}
