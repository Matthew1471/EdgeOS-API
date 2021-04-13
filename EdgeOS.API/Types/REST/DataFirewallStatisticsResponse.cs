using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a Firewall statistics data response from EdgeOS.</summary>
    public class DataFirewallStatisticsResponse
    {
        /// <summary>The output for the data request.</summary>
        [JsonProperty(PropertyName = "output")]
        public DataFirewallStatistics Output;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>An object that contains each of the firewall types (e.g. IPv4 and IPv6).</summary>
        public class DataFirewallStatistics
        {
            /// <summary>An array of dictionary items that contains each of the IPv4 firewall rulesets and their rule statistics.</summary>
            [JsonProperty(PropertyName = "name")]
            public Dictionary<string, DataFirewallRuleStatistics[]>[] Name;

            /// <summary>An array of dictionary items that contains each of the IPv6 firewall rulesets and their rule statistics.</summary>
            [JsonProperty(PropertyName = "ipv6-name")]
            public Dictionary<string, DataFirewallRuleStatistics[]>[] IPv6Name;

            /// <summary>Outputs a human friendly readable form of the fields and their relations contained in this object.</summary>
            /// <returns>A string showing the relation between all the fields in a human friendly readable format.</returns>
            public override string ToString()
            {
                return "Name: " + Name.Length + " Rulesets, IPv6Name: " + IPv6Name.Length + " Rulesets";
            }

            /// <summary>An object that represents a particular firewall rule’s statistics..</summary>
            public class DataFirewallRuleStatistics
            {
                /// <summary>The rule number for this particular firewall entry.</summary>
                [JsonProperty(PropertyName = "rule")]
                public uint Rule;

                /// <summary>The number of packets that this firewall rule has matched.</summary>
                [JsonProperty(PropertyName = "pkts")]
                public uint Packets;

                /// <summary>The number of bytes that this firewall rule has matched.</summary>
                [JsonProperty(PropertyName = "bytes")]
                public uint Bytes;

                /// <summary>Outputs a human friendly readable form of the fields and their relations contained in this object.</summary>
                /// <returns>A string showing the relation between all the fields in a human friendly readable format.</returns>
                public override string ToString()
                {
                    return "Rule #" + Rule + " - Packets: " + Packets + ", Bytes: " + Bytes;
                }
            }
        }
    }
}