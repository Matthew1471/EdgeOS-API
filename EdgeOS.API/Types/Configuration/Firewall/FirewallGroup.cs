using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.Configuration
{
    /// <summary>A class representing an EdgeOS Firewall Group configuration tree.</summary>
    public class FirewallGroup
    {
        /// <summary>Firewall address-group</summary>
        [JsonProperty(PropertyName = "address-group")]
        public Dictionary<string, FirewallAddressGroupEntry> AddressGroup;

        /// <summary>Firewall address-group</summary>
        [JsonProperty(PropertyName = "ipv6-address-group")]
        public Dictionary<string, object> IPv6AddressGroup;

        /// <summary>Firewall network-group</summary>
        [JsonProperty(PropertyName = "ipv6-network-group")]
        public Dictionary<string, object> IPv6NetworkGroup;

        /// <summary>Firewall network-group</summary>
        [JsonProperty(PropertyName = "network-group")]
        public Dictionary<string, object> NetworkGroup;

        /// <summary>Firewall port-group</summary>
        [JsonProperty(PropertyName = "port-group")]
        public Dictionary<string, object> PortGroup;
    }
}