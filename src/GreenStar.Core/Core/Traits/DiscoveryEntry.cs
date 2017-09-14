using System;

namespace GreenStar.Core.Traits
{
    public class DiscoveryEntry
    {
        /// <summary>
        /// The player id of the person who discovered the planet.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The level of discoverage
        /// </summary>
        public DiscoveryLevel Level { get; set; }
        
        /// <summary>
        /// The turn age of the discovery entry
        /// </summary>
        public int Turn { get; set; }
    }
}
