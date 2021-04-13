using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a default configuration status data response from EdgeOS.</summary>
    public class DataDefaultConfigurationStatusResponse
    {
        /// <summary>The output for the data request.</summary>
        [JsonProperty(PropertyName = "output")]
        public DefaultConfigurationStatus Output;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>The status of the configuration.</summary>
        public class DefaultConfigurationStatus
        {
            /// <summary>Whether the device is running the factory default configuration.</summary>
            [JsonProperty(PropertyName = "is_default")]
            public byte IsDefault;
        }
    }
}