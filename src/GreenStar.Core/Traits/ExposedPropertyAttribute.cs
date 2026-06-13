using System;

namespace GreenStar.Traits;

[AttributeUsage(AttributeTargets.Property)]
public class ExposedPropertyAttribute : Attribute
{
    public ExposedPropertyAttribute(DiscoveryLevel level)
    {
        Level = level;
    }

    public DiscoveryLevel Level { get; }
}