using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing an ONU reboot response from EdgeOS.</summary>
    public class ONURebootResponse
    {
        /// <summary>Status text for the reboot ONU request.</summary>
        [JsonProperty(PropertyName = "REBOOT_ONU")]
        public string RebootONU;

        /// <summary>The authorisation string for this session that confirms the user is correctly authenticated.</summary>
        [JsonProperty(PropertyName = "SESSION_ID")]
        public string SessionID;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;
    }
}