using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a configuration response from EdgeOS.</summary>
    public class ConfigurationResponse
    {
        /// <summary>Status values for the configuration request.</summary>
        [JsonProperty(PropertyName = "CONFIG")]
        public OperationStatus Configuration;

        /// <summary>The error(s) that occurred while trying to process this request.</summary>
        [JsonProperty(PropertyName = "errors")]
        public string[] Errors;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;
    }
}