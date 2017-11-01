using GreenStar.Core;
using GreenStar.Core.Resources;

namespace GreenStar.Core.Traits
{
    /// <summary>
    /// The amount of resources found in this actor
    /// </summary>
    public class Resourceful : Trait
    {
        /// <summary>
        /// The resources in the actor.
        /// </summary>
        public ResourceAmount Resources { get; set; } = new ResourceAmount();
    }
}
