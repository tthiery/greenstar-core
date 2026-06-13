using GreenStar.Persistence;

namespace GreenStar.Traits;

public class Nameable : Trait
{
    [ExposedProperty(DiscoveryLevel.Known)]
    public string Name { get; set; } = string.Empty;

    public override void Load(IPersistenceReader reader)
    {
        Name = reader.Read<string>(nameof(Name));
    }

    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write<string>(nameof(Name), Name);
    }
}