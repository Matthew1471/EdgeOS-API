using Newtonsoft.Json;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class DiscoverRoot : RootObject
    {
        /// <summary>The object that contains discovered device information when EdgeOS has discovered another device (typically via UBNT Discovery).</summary>
        [JsonProperty(PropertyName = "discover")]
        public string Discover;
    }
}