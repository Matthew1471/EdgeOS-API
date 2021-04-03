namespace EdgeOS.API.Types.Subscription.Responses.UDAPITypes
{
    /// <summary>An object that contains information on a power supply.</summary>
    public class Power
    {
        /// <summary>The PSU type (AC, DC etc.).</summary>
        public string psuType;

        /// <summary>The PSU power (if available).</summary>
        public string power;

        /// <summary>The PSU voltage (if available).</summary>
        public string voltage;

        /// <summary>The PSU current (if available).</summary>
        public string current;

        /// <summary>The PSU temperature (if available).</summary>
        public string temperature;
    }
}