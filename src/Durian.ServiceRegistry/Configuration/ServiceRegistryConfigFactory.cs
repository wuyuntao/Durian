using Akka.Configuration;
using System.Diagnostics;
using System.IO;

namespace Durian.ServiceRegistry
{
    internal static class ServiceRegistryConfigFactory
    {
        public static Config Default()
        {
            return FromResource("Durian.ServiceRegistry.Configuration.ServiceRegistry.conf");
        }

        internal static Config FromResource(string resourceName)
        {
            var assembly = typeof(ServiceRegistryConfigFactory).Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                Debug.Assert(stream != null, "stream != null");
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();

                    return ConfigurationFactory.ParseString(result);
                }
            }
        }
    }
}
