using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class TrafficAnalysisRoot : RootObject
    {
        /// <summary>The object that contains traffic analysis information for each host when EdgeOS has observed certain application types via Deep Packet Inspection (DPI).</summary>
        [JsonProperty(PropertyName = "export")]
        public Dictionary<string, Dictionary<string, ApplicationStats>> TrafficAnalysis;
    }
}