using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a DHCP statistics data response from EdgeOS.</summary>
    public class DataDHCPStatisticsResponse
    {
        /// <summary>The output for the data request.</summary>
        [JsonProperty(PropertyName = "output")]
        public DataDHCPServerStatistics Output;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>An object that contains each of the DHCP servers and their pool statistics.</summary>
        public class DataDHCPServerStatistics
        {
            /// <summary>A dictionary that contains each of the DHCP servers and their pool statistics.</summary>
            [JsonProperty(PropertyName = "dhcp-server-stats")]
            public Dictionary<string, DataDHCPPoolStatistics> DHCPServerStatistics;

            /// <summary>An object that represents a particular DHCP server’s pool statistics.</summary>
            public class DataDHCPPoolStatistics
            {
                /// <summary>The total size of the dynamic DHCP pool.</summary>
                [JsonProperty(PropertyName = "pool_size")]
                public uint PoolSize;

                /// <summary>The number of dynamically leased addresses.</summary>
                [JsonProperty(PropertyName = "leased")]
                public uint Leased;

                /// <summary>The number of remaining available addresses in the dynamic DHCP pool.</summary>
                [JsonProperty(PropertyName = "available")]
                public uint Available;
            }
        }
    }
}