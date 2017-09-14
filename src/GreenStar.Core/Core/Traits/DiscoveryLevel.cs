namespace GreenStar.Core.Traits
{
    public enum DiscoveryLevel
    {
        /// <summary>
        /// Element is unknown to the player
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// The location of the actor is aware to the player
        /// </summary>
        LocationAware = 10,
        
        /// <summary>
        /// The details
        /// </summary>
        PropertyAware = 20,
    }
}
