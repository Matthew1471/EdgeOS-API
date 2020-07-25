using Newtonsoft.Json;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class UDAPIStatisticsRoot : RootObject
    {
        /// <summary>The object containing device API statistics, potentially used by UNMS.</summary>
        [JsonProperty(PropertyName = "udapi-statistics")]
        public UDAPIStatistics[] UDAPIStatistics;
    }
}