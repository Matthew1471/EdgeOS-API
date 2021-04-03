using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EdgeOS.API.Types.REST
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

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new StringEnumConverter());
        }
    }
}