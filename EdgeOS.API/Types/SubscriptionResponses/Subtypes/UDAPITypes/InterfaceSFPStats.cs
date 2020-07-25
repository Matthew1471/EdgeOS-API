namespace EdgeOS.API.Types.SubscriptionResponses.Subtypes.UDAPITypes
{
    /// <summary>An object that contains information on an interface's Small Form-factor Pluggable (SFP).</summary>
    public class InterfaceSFPStats
    {
        /// <summary>The temperature of the SFP module.</summary>
        public string temperature;

        /// <summary>The transmit power of the module.</summary>
        public string txPower;

        /// <summary>The receive power of the module.</summary>
        public string rxPower;

        /// <summary>The module voltage.</summary>
        public string voltage;

        /// <summary>The module current.</summary>
        public string current;
    }
}