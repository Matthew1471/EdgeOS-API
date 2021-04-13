using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a Network Address Translation (NAT) statistics data response from EdgeOS.</summary>
    public class DataNATStatisticsResponse
    {
        /// <summary>The output for the data request.</summary>
        [JsonProperty(PropertyName = "output")]
        public NATStatistics Output;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>The status of the configuration.</summary>
        public class NATStatistics
        {
        }
    }
}