using GreenStar.Core.Persistence;

namespace GreenStar.Core
{
    public abstract class Trait
    {
        public Actor Self { get; set; }
        
        public virtual void Persist(IPersistenceWriter writer)
        { }
        public virtual void Load(IPersistenceReader reader)
        { }
    }
}
