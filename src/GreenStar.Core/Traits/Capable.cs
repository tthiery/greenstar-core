using System;
using System.Linq;

using GreenStar.Persistence;

namespace GreenStar.Traits;

public class Capable : Trait
{
    private int[] _values;

    public string[] CapabilityNames { get; private set; }

    public Capable(string[] capabilityNames)
    {
        CapabilityNames = capabilityNames;
        _values = new int[capabilityNames.Length];
    }

    public override void Load(IPersistenceReader reader)
    {
        foreach (string property in reader.ReadPropertyNames(prefix: "Capability:"))
        {
            var capability = property.Substring(11);
            var value = reader.Read<int>(property);

            Of(capability, value);
        }
    }

    public override void Persist(IPersistenceWriter writer)
    {
        foreach (var capability in CapabilityNames)
        {
            writer.Write<int>("Capability:" + capability, Of(capability));
        }
    }

    public int Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"Argument {nameof(name)} is null or empty", nameof(name));
        }

        var position = Array.IndexOf(CapabilityNames, name);

        return position >= 0 ? _values[position] : 0;
    }

    public int Of(string name, int value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"Argument {nameof(name)} is null or empty", nameof(name));
        }

        var position = Array.IndexOf(CapabilityNames, name);

        if (position >= 0)
        {
            _values[position] = value;
        }

        return position >= 0 ? _values[position] : 0;
    }

    public void AddCapability(string name)
    {
        var newNames = new string[CapabilityNames.Length + 1];
        CapabilityNames.CopyTo(newNames, 0);
        newNames[CapabilityNames.Length] = name;

        var newValues = new int[CapabilityNames.Length + 1];
        _values.CopyTo(newValues, 0);
        newValues[CapabilityNames.Length] = 0;

        _values = newValues;
        CapabilityNames = newNames;
    }

    public string ToAbbreviatedString()
    {
        return string.Join("/", CapabilityNames.Select((name, index) => _values[index]));
    }
}
