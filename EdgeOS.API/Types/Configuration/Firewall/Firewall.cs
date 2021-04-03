using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a complete EdgeOS Firewall configuration tree.</summary>
    public class Firewall
    {
        /// <summary>Firewall group</summary>
        [JsonProperty(PropertyName = "group")]
        public FirewallGroup Group;

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new StringEnumConverter());
        }
    }
}
