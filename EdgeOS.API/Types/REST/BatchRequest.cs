using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a batch configuration request to EdgeOS.</summary>
    public class BatchRequest
    {
        /// <summary>The JSON configuration attributes to set.</summary>
        [JsonProperty(PropertyName = "SET")]
        public Configuration Set;

        /// <summary>The JSON configuration attributes to delete.</summary>
        [JsonProperty(PropertyName = "DELETE")]
        public Configuration Delete;

        /// <summary>The JSON configuration attributes to get.</summary>
        [JsonProperty(PropertyName = "GET")]
        public Configuration Get;
    }
}