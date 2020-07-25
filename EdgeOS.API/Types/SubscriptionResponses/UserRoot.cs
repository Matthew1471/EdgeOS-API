using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class UserRoot : RootObject
    {
        /// <summary>The object that contains EdgeOS connected user information.</summary>
        [JsonProperty(PropertyName = "users")]
        public Users users;
    }
}