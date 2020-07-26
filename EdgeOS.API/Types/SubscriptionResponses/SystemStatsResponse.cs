using Newtonsoft.Json;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class SystemStatsResponse : IResponse
    {
        /// <summary>The object containing system statistics about the EdgeOS device.</summary>
        [JsonProperty(PropertyName = "system-stats")]
        public SystemStats SystemStats;
    }
}