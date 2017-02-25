using Durian.Configuration.Attributes;

namespace Durian.Ballance.Common.Configuration
{
    [ConfigurationElement]
    public class BallConf
    {
        public BallConf(uint id, float mass, float fraction, float bounce)
        {
            Id = id;
            Mass = mass;
            Fraction = fraction;
            Bounce = bounce;
        }

        public uint Id { get; private set; }

        public uint Level { get; private set; }

        public float Mass { get; private set; }

        public float Fraction { get; private set; }

        public float Bounce { get; private set; }
    }
}
