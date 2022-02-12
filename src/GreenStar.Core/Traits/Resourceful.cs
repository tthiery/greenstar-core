using GreenStar;
using GreenStar.Persistence;
using GreenStar.Resources;

namespace GreenStar.Traits;

/// <summary>
/// The amount of resources found in this actor
/// </summary>
public class Resourceful : Trait
{
    public ResourceAmount Resources { get; set; } = new ResourceAmount();

    public override void Load(IPersistenceReader reader)
    {
        Resources = reader.Read<string>(nameof(Resources));
    }

    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write<string>(nameof(Resources), Resources.ToString());
    }
}
