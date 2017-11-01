namespace GreenStar.Core.Resources
{
    /// <summary>
    /// An item in a resource amount. represent a resource and a value
    /// </summary>
    public sealed class ResourceAmountItem
    {
        public ResourceAmountItem(string resource, int value)
        {
            Resource = resource;
            Value = value;
        }

        /// <summary>
        /// The resource
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// The value
        /// </summary>
        public int Value { get; set; }
    }
}
