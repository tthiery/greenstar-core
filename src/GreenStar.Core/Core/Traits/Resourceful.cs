using GreenStar.Core;
using GreenStar.Core.Persistence;
using GreenStar.Core.Resources;

namespace GreenStar.Core.Traits
{
    /// <summary>
    /// The amount of resources found in this actor
    /// </summary>
    public class Resourceful : Trait
    {
        public override void Load(IPersistenceReader reader)
        {
            if (reader == null)
            {
                throw new System.ArgumentNullException(nameof(reader));
            }

            Resources = reader.Read<string>(nameof(Resources));
        }

        public override void Persist(IPersistenceWriter writer)
        {
            if (writer == null)
            {
                throw new System.ArgumentNullException(nameof(writer));
            }

            writer.Write<string>(nameof(Resources), Resources.ToString());
        }
        
        /// <summary>
        /// The resources in the actor.
        /// </summary>
        public ResourceAmount Resources { get; set; } = new ResourceAmount();
    }
}
