using Newtonsoft.Json;

namespace EdgeOS.API.Types.Configuration
{
    /// <summary>A class representing an EdgeOS Firewall Group AddressGroup Entry configuration tree.</summary>
    public class AddressGroupEntry
    {
        /// <summary>Address-group member</summary>
        [JsonProperty(PropertyName = "address")]
        public string[] Address;

        /// <summary>Address-group description</summary>
        [JsonProperty(PropertyName = "description")]
        public string Description;
    }
}