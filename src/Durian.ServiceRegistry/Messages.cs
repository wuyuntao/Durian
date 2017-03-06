using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Durian.ServiceRegistry
{
    public class ServiceMeta
    {
        public string Name { get; private set; }

        public IActorRef Registrator { get; private set; }

        public DateTime RegistrationTime { get; private set; }

        public ServiceMeta(string name, IActorRef registrator)
        {
            Name = name;
            Registrator = registrator;
            RegistrationTime = DateTime.UtcNow;
        }
    }

    public class Register
    {
        public ServiceMeta ServiceMeta { get; private set; }

        public Register(ServiceMeta serviceMeta)
        {
            ServiceMeta = serviceMeta;
        }
    }

    public class Registered
    {
        public ServiceMeta ServiceMeta { get; private set; }

        public Registered(ServiceMeta serviceMeta)
        {
            ServiceMeta = serviceMeta;
        }
    }

    public class Unregister
    {
        public string ServiceName { get; private set; }

        public Unregister(string serviceName)
        {
            ServiceName = serviceName;
        }
    }

    public class Unregistered
    {
        public string ServiceName { get; private set; }

        public Unregistered(string serviceName)
        {
            ServiceName = serviceName;
        }
    }

    public class QueryAll
    {
    }

    public class QueryAllResponse
    {
        public IEnumerable<ServiceMeta> Services { get; private set; }

        public QueryAllResponse(IEnumerable<ServiceMeta> services)
        {
            Services = services?.ToArray();
        }
    }

    public class ServiceRegistryState
    {
        public Dictionary<string, ServiceMeta> Services { get; private set; }

        public ServiceRegistryState()
        {
            Services = new Dictionary<string, ServiceMeta>();
        }

        public ServiceRegistryState(IDictionary<string, ServiceMeta> services)
        {
            Services = new Dictionary<string, ServiceMeta>(services);
        }

        public ServiceRegistryState Apply(Registered e)
        {
            Services[e.ServiceMeta.Name] = e.ServiceMeta;

            return new ServiceRegistryState(Services);
        }

        public ServiceRegistryState Apply(Unregistered e)
        {
            Services.Remove(e.ServiceName);

            return new ServiceRegistryState(Services);
        }
    }
}
