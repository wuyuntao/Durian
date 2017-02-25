using Durian.Configuration.Attributes;

namespace Durian.Ballance.Common.Configuration
{
    [ConfigurationElement]
    public sealed class ItemConf
    {
        public ItemConf(uint id, string property, float offset)
        {
            Id = id;
            Property = property;
            Offset = offset;
        }

        public uint Id { get; private set; }

        public string Property { get; private set; }

        public float Offset { get; private set; }
    }
}
