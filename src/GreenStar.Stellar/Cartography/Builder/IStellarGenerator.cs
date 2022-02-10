using System;
using System.Collections.Generic;

namespace GreenStar.Cartography.Builder;

public record StellarType(string Name, string DisplayName, StellarGeneratorArgument[] Arguments);
public record StellarGeneratorArgument(string Name, string DisplayName, double Value);

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class StellarGeneratorArgumentAttribute : Attribute
{
    public StellarGeneratorArgumentAttribute(string name, string displayName, double value)
    {
        Name = name;
        DisplayName = displayName;
        Value = value;
    }

    public string Name { get; }
    public string DisplayName { get; }
    public double Value { get; }
}

public interface IStellarGenerator
{
    void Generate(IActorContext actorContext, GeneratorMode mode, StellarGeneratorArgument[] arguments);
}
