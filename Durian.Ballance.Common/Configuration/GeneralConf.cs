using Durian.Configuration.Attributes;

namespace Durian.Ballance.Common.Configuration
{
    [Coniguration]
    public sealed class GeneralConf
    {
        public GeneralConf(float maxMass, float maxFraction, float maxBounce)
        {
            MaxMass = maxMass;
            MaxFraction = maxFraction;
            MaxBounce = maxBounce;
        }

        public float MaxMass { get; private set; }

        public float MaxFraction { get; private set; }

        public float MaxBounce { get; private set; }
    }
}
