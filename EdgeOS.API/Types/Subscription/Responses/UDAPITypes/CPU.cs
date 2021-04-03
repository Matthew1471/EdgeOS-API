namespace EdgeOS.API.Types.Subscription.Responses.UDAPITypes
{
    /// <summary>An object that contains information about a processor.</summary>
    public class CPU
    {
        /// <summary>How to refer to this CPU (typically its make and model).</summary>
        public string identifier;

        /// <summary>The current CPU load usage (as a percentage).</summary>
        public string usage;

        /// <summary>The physical temperature of this CPU if available.</summary>
        public string temperature;
    }
}