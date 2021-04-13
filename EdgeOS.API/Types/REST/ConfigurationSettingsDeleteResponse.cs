using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a delete configuration response from EdgeOS.</summary>
    public class ConfigurationSettingsDeleteResponse
    {
        /// <summary>Status values for the deletion request.</summary>
        [JsonProperty(PropertyName = "DELETE")]
        public ConfigurationSettingsStatus Delete;

        /// <summary>The authorisation string for this session that confirms the user is correctly authenticated.</summary>
        [JsonProperty(PropertyName = "SESSION_ID")]
        public string SessionID;

        /// <summary>The parent section of the deleted value after the deletion.</summary>
        [JsonProperty(PropertyName = "GET")]
        public Configuration.Configuration Get;

        /// <summary>Status values for the configuration commit request.</summary>
        [JsonProperty(PropertyName = "COMMIT")]
        public ConfigurationSettingsStatus Commit;

        /// <summary>Status values for the configuration save request.</summary>
        [JsonProperty(PropertyName = "SAVE")]
        public ConfigurationSettingsStatus Save;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;
    }
}