using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>An object that contains connected EdgeOS user information.</summary>
    public class Users
    {
        /// <summary>The users currently connected.</summary>
        [JsonProperty(PropertyName = "local")]
        public Dictionary<string, LocalUserInfo>[] local;
    }
}