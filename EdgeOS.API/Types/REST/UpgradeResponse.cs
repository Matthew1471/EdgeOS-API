using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing an upgrade response from EdgeOS.</summary>
    public class UpgradeResponse
    {
        /// <summary>Status values for the upgrade request.</summary>
        [JsonProperty(PropertyName = "UPGRADE")]
        public OperationStatus Upgrade;

        /// <summary>The error(s) that occurred while trying to process this request.</summary>
        [JsonProperty(PropertyName = "errors")]
        public string[] Errors;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;
    }
}