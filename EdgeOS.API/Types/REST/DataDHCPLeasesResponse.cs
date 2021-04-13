using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a DHCP leases data response from EdgeOS.</summary>
    public class DataDHCPLeasesResponse
    {
        /// <summary>The output for the data request.</summary>
        [JsonProperty(PropertyName = "output")]
        public DataDHCPLeases Output;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>An object that contains each of the DHCP servers and their leases.</summary>
        public class DataDHCPLeases
        {
            /// <summary>A dictionary that contains each of the DHCP servers and their leases.</summary>
            [JsonProperty(PropertyName = "dhcp-server-leases")]
            public Dictionary<string, DataDHCPLeaseDetails> DHCPServerLeases;

            /// <summary>An object that represents a particular DHCP server lease.</summary>
            public class DataDHCPLeaseDetails
            {
                /// <summary>The date of a specific lease expiry.</summary>
                [JsonProperty(PropertyName = "expiration")]
                public string Expiration;

                /// <summary>The pool this lease corresponds to (appears to be a repetition of the DHCP server name).</summary>
                [JsonProperty(PropertyName = "pool")]
                public string Pool;

                /// <summary>The MAC address of this particular DHCP server lease.</summary>
                [JsonProperty(PropertyName = "mac")]
                public string MAC;

                /// <summary>The client supplied hostname for a DHCP server lease.</summary>
                [JsonProperty(PropertyName = "client-hostname")]
                public string ClientHostname;
            }
        }
    }
}