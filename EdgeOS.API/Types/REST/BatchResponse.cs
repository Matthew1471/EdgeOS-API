using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a batch configuration response from EdgeOS.</summary>
    public class BatchResponse
    {
        /// <summary>Status values for the set request.</summary>
        [JsonProperty(PropertyName = "SET")]
        public BatchResponseStatus Set;

        /// <summary>Status values for the deletion request.</summary>
        [JsonProperty(PropertyName = "DELETE")]
        public BatchResponseStatus Delete;

        /// <summary>The authorisation string for this session that confirms the user is correctly authenticated.</summary>
        [JsonProperty(PropertyName = "SESSION_ID")]
        public string SessionID;

        /// <summary>The requested section of the configuration after the operation is performed (defaults to the requested section for deletion/setting by default).</summary>
        [JsonProperty(PropertyName = "GET")]
        public Configuration Get;

        /// <summary>Status values for the configuration commit request.</summary>
        [JsonProperty(PropertyName = "COMMIT")]
        public BatchResponseStatus Commit;

        /// <summary>Status values for the configuration save request.</summary>
        [JsonProperty(PropertyName = "SAVE")]
        public BatchResponseStatus Save;

        /// <summary>Whether the API request was successful..</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new StringEnumConverter());
        }
    }
}
