using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EdgeOS.API.Types.SubscriptionRequests
{
    /// <summary>A class representing the required subscription and unsubscription of EdgeOS.</summary>
    public class SubscriptionRequest
    {
        /// <summary>EdgeOS message types that we wish to subscribe to.</summary>
        [JsonProperty(PropertyName = "SUBSCRIBE")]
        public Subscription[] Subscribe;

        /// <summary>EdgeOS message types that we wish to unsubscribe from.</summary>
        [JsonProperty(PropertyName = "UNSUBSCRIBE")]
        public Subscription[] Unsubscribe;

        /// <summary>EdgeOS authenticates based off a PHP session ID.</summary>
        [JsonProperty(PropertyName = "SESSION_ID")]
        public string SessionID;

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new StringEnumConverter());
        }
    }
}