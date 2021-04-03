using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST.Responses
{
    /// <summary>A class representing a batch configuration response from EdgeOS.</summary>
    public class ConfigurationSettingsBatchResponse
    {
        /// <summary>Status values for the set request.</summary>
        [JsonProperty(PropertyName = "SET")]
        public ConfigurationSettingsStatus Set;

        /// <summary>Status values for the deletion request.</summary>
        [JsonProperty(PropertyName = "DELETE")]
        public ConfigurationSettingsStatus Delete;

        /// <summary>The authorisation string for this session that confirms the user is correctly authenticated.</summary>
        [JsonProperty(PropertyName = "SESSION_ID")]
        public string SessionID;

        /// <summary>The requested section of the configuration after the operation is performed (defaults to the requested section for deletion/setting by default).</summary>
        [JsonProperty(PropertyName = "GET")]
        public Configuration.Configuration Get;

        /// <summary>Status values for the configuration commit request.</summary>
        [JsonProperty(PropertyName = "COMMIT")]
        public ConfigurationSettingsStatus Commit;

        /// <summary>Status values for the configuration save request.</summary>
        [JsonProperty(PropertyName = "SAVE")]
        public ConfigurationSettingsStatus Save;

        /// <summary>Whether the API request was successful..</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;
    }
}