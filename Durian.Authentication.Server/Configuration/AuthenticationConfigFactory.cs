using Akka.Configuration;
using System.Diagnostics;
using System.IO;

namespace Durian.Authentication
{
    internal static class AuthenticationConfigFactory
    {
        public static Config Default()
        {
            return FromResource("Durian.Authentication.Configuration.Authentication.conf");
        }

        internal static Config FromResource(string resourceName)
        {
            var assembly = typeof(AuthenticationConfigFactory).Assembly;

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
