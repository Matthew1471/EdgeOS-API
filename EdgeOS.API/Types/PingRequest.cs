using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EdgeOS.API.Types
{
    /// <summary>A class representing an ignored ping message to keep-alive the EdgeOS connection.</summary>
    public class PingRequest
    {
        /// <summary>EdgeOS empty string.</summary>
        [JsonProperty(PropertyName = "CLIENT_PING")]
        public string ClientPing = "";

        /// <summary>EdgeOS authenticates based off a PHP session ID.</summary>
        [JsonProperty(PropertyName = "SESSION_ID")]
        public string SessionID;

        /// <summary>Prevent the compiler from creating a default constructor without the SessionID.</summary>
        private PingRequest() { }

        /// <summary>Constructor for a PingRequest that sets the required SessionID field.</summary>
        /// <param name="sessionID"></param>
        public PingRequest(string sessionID)
        {
            SessionID = sessionID;
        }

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new StringEnumConverter());
        }
    }
}