using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a configuration download preparation response from EdgeOS.</summary>
    public class ConfigurationDownloadPrepareResponse
    {
        /// <summary>Status values for the configuration download preparation request.</summary>
        [JsonProperty(PropertyName = "CONFIG")]
        public ConfigurationDownloadPrepareStatus Configuration;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;

        /// <summary>A class representing a single status item for a configuration download preparation response from EdgeOS.</summary>
        public class ConfigurationDownloadPrepareStatus
        {
            /// <summary>Whether the configuration save was successful.</summary>
            [JsonProperty(PropertyName = "success")]
            public byte Success;

            /// <summary>A temporary file path on the local device where the configuration was saved.</summary>
            [JsonProperty(PropertyName = "Path")]
            public string Path;

            /// <summary>Outputs a human friendly readable form of the fields and their relations contained in this object.</summary>
            /// <returns>A string showing the relation between all the fields in a human friendly readable format.</returns>
            public override string ToString() { return "Success : " + Success + (Path != null ? ", Path : \"" + Path + "\"": null); }
        }
    }
}