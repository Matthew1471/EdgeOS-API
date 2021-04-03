using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.REST.Configuration
{
    /// <summary>A class representing a complete EdgeOS Firewall Group configuration tree.</summary>
    public class FirewallGroup
    {
        /// <summary>Firewall address-group</summary>
        [JsonProperty(PropertyName = "address-group")]
        public Dictionary<string, FirewallAddressGroupEntry> AddressGroup;
    }
}
