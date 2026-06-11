using GreenStar.Persistence;

namespace GreenStar;

public abstract class Trait
{
    public Actor Self { get; set; } = default!;

    public virtual void Persist(IPersistenceWriter writer)
    { }
    public virtual void Load(IPersistenceReader reader)
    { }
}
