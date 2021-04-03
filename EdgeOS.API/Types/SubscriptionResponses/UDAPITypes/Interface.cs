namespace EdgeOS.API.Types.SubscriptionResponses.UDAPITypes
{
    /// <summary>An object that contains Interface information on a single interface.</summary>
    public class Interface
    {
        /// <summary>The interface name.</summary>
        public string name;

        /// <summary>The ID for this interface.</summary>
        public string id;

        /// <summary>A breakdown of statistics for this interface.</summary>
        public UDAPIInterfaceStats statistics;
    }
}