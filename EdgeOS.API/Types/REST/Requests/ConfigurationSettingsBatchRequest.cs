using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST.Requests
{
    /// <summary>A class representing a batch configuration request to EdgeOS.</summary>
    public class ConfigurationSettingsBatchRequest
    {
        /// <summary>The JSON configuration attributes to set.</summary>
        [JsonProperty(PropertyName = "SET")]
        public Configuration.Configuration Set;

        /// <summary>The JSON configuration attributes to delete.</summary>
        [JsonProperty(PropertyName = "DELETE")]
        public Configuration.Configuration Delete;

        /// <summary>The JSON configuration attributes to get.</summary>
        [JsonProperty(PropertyName = "GET")]
        public Configuration.Configuration Get;
    }
}