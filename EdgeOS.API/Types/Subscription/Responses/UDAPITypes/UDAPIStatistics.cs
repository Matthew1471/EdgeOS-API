namespace EdgeOS.API.Types.Subscription.Responses.UDAPITypes
{
    /// <summary>An object containing device statistics in a specific API format, potentially used by UNMS.</summary>
    public class UDAPIStatistics
    {
        /// <summary>When this information was collected (in epoch).</summary>
        public string timestamp;

        /// <summary>Contains physical information about this device.</summary>
        public Device device;

        /// <summary>Contains information about this device's interfaces.</summary>
        public Interface[] interfaces;
    }
}