using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Event;
using System;

namespace Durian.ServiceRegistry
{
    class ServiceRegistrator : ReceiveActor
    {
        private readonly ILoggingAdapter log = Logging.GetLogger(Context);

        private readonly string serviceName;
        private readonly IActorRef service;
        private readonly IActorRef registry;

        private ServiceRegistrator(Props serviceProps, string serviceName)
        {
            this.serviceName = serviceName;

            service = Context.ActorOf(serviceProps, serviceName);
            Context.Watch(service);

            registry = Context.ActorOf(ClusterSingletonProxy.Props(
                "/user/singleton",
                new ClusterSingletonProxySettings("service-registry-manager", null, TimeSpan.FromSeconds(3), 8192)
            ));

            ReceiveAsync<Terminated>(async m =>
            {
                var e = await registry.Ask(new Unregister(serviceName));

                log.Debug("Unregistered");
                Context.Stop(Self);
            }, m => m.ActorRef.Equals(service));

            ReceiveAsync<string>(async m =>
            {
                var meta = new ServiceMeta(serviceName, Self);

                var e = await registry.Ask(new Register(meta));

                log.Debug("Registered");
            }, s => s == "Register");

            // TODO Configure report health interval
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromSeconds(30), Self, "Register", null);
        }
    }
}
