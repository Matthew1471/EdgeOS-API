using Newtonsoft.Json;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class SystemStatsResponse : IResponse
    {
        /// <summary>The object containing system statistics about the EdgeOS device.</summary>
        [JsonProperty(PropertyName = "system-stats")]
        public SystemStatsDetails SystemStats;

        /// <summary>An object containing system statistics about the EdgeOS device.</summary>
        public class SystemStatsDetails
        {
            /// <summary>The CPU usage (as a percentage) of the EdgeOS device.</summary>
            public byte cpu;

            /// <summary>How long this EdgeOS device has been running for.</summary>
            public ushort uptime;

            /// <summary>The amount of memory in use (as a percentage) on the EdgeOS device.</summary>
            public byte mem;
        }
    }
}