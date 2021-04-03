using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.Subscription.Responses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class TrafficAnalysisResponse : IResponse
    {
        /// <summary>The object that contains traffic analysis information for each host when EdgeOS has observed certain application types via Deep Packet Inspection (DPI).</summary>
        [JsonProperty(PropertyName = "export")]
        public Dictionary<string, Dictionary<string, ApplicationStats>> TrafficAnalysis;

        /// <summary>An object that contains traffic statistics for a certain observed application type via Deep Packet Inspection (DPI).</summary>
        public class ApplicationStats
        {
            /// <summary>The number of bytes transmitted from this observed application.</summary>
            [JsonProperty(PropertyName = "tx_bytes")]
            public uint tx_bytes;

            /// <summary>The rate at which bytes are currently being transmitted from this observed application.</summary>
            [JsonProperty(PropertyName = "tx_rate")]
            public uint tx_rate;

            /// <summary>The number of bytes received by this observed application.</summary>
            [JsonProperty(PropertyName = "rx_bytes")]
            public uint rx_bytes;

            /// <summary>The rate at which bytes are currently being received by this observed application.</summary>
            [JsonProperty(PropertyName = "rx_rate")]
            uint rx_rate;
        }
    }
}