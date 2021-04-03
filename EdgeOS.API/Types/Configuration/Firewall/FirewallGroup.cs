using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a complete EdgeOS Firewall Group configuration tree.</summary>
    public class FirewallGroup
    {
        /// <summary>Firewall address-group</summary>
        [JsonProperty(PropertyName = "address-group")]
        public Dictionary<string, FirewallAddressGroupEntry> AddressGroup;

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new StringEnumConverter());
        }
    }
}
