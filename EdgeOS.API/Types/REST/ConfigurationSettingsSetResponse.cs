using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a set configuration response from EdgeOS.</summary>
    public class ConfigurationSettingsSetResponse
    {
        /// <summary>Status values for the insertion request.</summary>
        [JsonProperty(PropertyName = "SET")]
        public ConfigurationSettingsStatus Set;

        /// <summary>The authorisation string for this session that confirms the user is correctly authenticated.</summary>
        [JsonProperty(PropertyName = "SESSION_ID")]
        public string SessionID;

        /// <summary>The parent section of the set value after the set.</summary>
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