using Newtonsoft.Json;

namespace EdgeOS.API.Types.Configuration
{
    /// <summary>A class representing a complete EdgeOS Firewall Group Address-Group configuration tree.</summary>
    public class FirewallAddressGroupEntry
    {
        /// <summary>Address-group member</summary>
        [JsonProperty(PropertyName = "address")]
        public string[] Address;

        /// <summary>Address-group description</summary>
        [JsonProperty(PropertyName = "description")]
        public string Description;
    }
}