using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class RootObject
    {
        /// <summary>The object that contains status information when EdgeOS is processing a configuration change.</summary>
        [JsonProperty(PropertyName = "config-change")]
        public ConfigurationChange ConfigurationChange;

        /// <summary>A message that contains discovered device information when EdgeOS has discovered another device (typically via UBNT Discovery).</summary>
        [JsonProperty(PropertyName = "discover")]
        public string Discover;

        /// <summary>The object that contains traffic analysis information for each host when EdgeOS has observed certain application types via Deep Packet Inspection (DPI).</summary>
        [JsonProperty(PropertyName = "export")]
        public Dictionary<string, Dictionary<string, ApplicationStats>> TrafficAnalysis;
        
        /// <summary>The object that contains network interface information for each of the EdgeOS network interfaces.</summary>
        [JsonProperty(PropertyName = "interfaces")]
        public Dictionary<string, Interface> Interfaces;

        /// <summary>The object containing counts of the EdgeOS network routes.</summary>
        [JsonProperty(PropertyName = "num-routes")]
        public NumberOfRoutes NumberOfRoutes;

        /// <summary>The object containing system statistics about the EdgeOS device.</summary>
        [JsonProperty(PropertyName = "system-stats")]
        public SystemStats SystemStats;

        /// <summary>The object containing device API statistics, potentially used by UNMS.</summary>
        [JsonProperty(PropertyName = "udapi-statistics")]
        public UDAPIStatistics[] UDAPIStatistics;

        /// <summary>A generic JSON message from either a tool or an error message.</summary>
        [JsonProperty(PropertyName = "")]
        public string Message;
    }
}