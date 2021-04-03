namespace EdgeOS.API.Types.SubscriptionResponses.UDAPITypes
{
    /// <summary>An object that contains information on the Random Access Memory (RAM).</summary>
    public class RAM
    {
        /// <summary>The current RAM usage on the device (as a percentage).</summary>
        public string usage;

        /// <summary>How many bytes of RAM are free.</summary>
        public string free;

        /// <summary>How much total RAM is installed on the device.</summary>
        public string total;
    }
}