using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a complete EdgeOS Firewall configuration tree.</summary>
    public class Firewall
    {
        /// <summary>Firewall group</summary>
        [JsonProperty(PropertyName = "group")]
        public FirewallGroup Group;
    }
}
