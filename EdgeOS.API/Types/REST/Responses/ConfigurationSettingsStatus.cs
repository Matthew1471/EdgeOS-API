using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST.Responses
{
    /// <summary>A class representing a single status item for the configuration settings response from EdgeOS.</summary>
    public class ConfigurationSettingsStatus
    {
        /// <summary>Whether the operation failed.</summary>
        [JsonProperty(PropertyName = "failure")]
        public byte Failure;

        /// <summary>Whether the operation was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>Outputs a human friendly readable form of the fields and their relations contained in this object.</summary>
        /// <returns>A string showing the relation between all the fields in a human friendly readable format.</returns>
        public override string ToString() { return "Failure : " + Failure + ", Success : " + Success; }
    }
}
