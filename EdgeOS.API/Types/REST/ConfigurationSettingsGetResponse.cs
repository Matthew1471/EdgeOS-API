using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a get configuration response from EdgeOS.</summary>
    public class ConfigurationSettingsGetResponse
    {
        /// <summary>The authorisation string for this session that confirms the user is correctly authenticated.</summary>
        [JsonProperty(PropertyName = "SESSION_ID")]
        public string SessionID;

        /// <summary>The requested section of the configuration.</summary>
        [JsonProperty(PropertyName = "GET")]
        public Configuration.Configuration Get;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;
    }
}