using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.Subscription.Responses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class UserResponse : IResponse
    {
        /// <summary>The object that contains EdgeOS connected user information.</summary>
        [JsonProperty(PropertyName = "users")]
        public Users users;

        /// <summary>An object that contains connected EdgeOS user information.</summary>
        public class Users
        {
            /// <summary>The users currently connected.</summary>
            [JsonProperty(PropertyName = "local")]
            public Dictionary<string, LocalUserInfo>[] local;

            /// <summary>An object that contains locally connected EdgeOS user information.</summary>
            public class LocalUserInfo
            {
                /// <summary>The method the user is currently connected.</summary>
                public string tty;

                /// <summary>How long the user has been idle for.</summary>
                public string idle;

                /// <summary>The hostname the user has been resolved to (if known).</summary>
                public string host;

                /// <summary>How long the user has been connected for.</summary>
                public string uptime;
            }
        }
    }
}