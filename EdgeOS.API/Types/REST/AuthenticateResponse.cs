using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing an authentication response from EdgeOS.</summary>
    public class AuthenticateResponse
    {
        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public string Success;

        /// <summary>The error that occurred while trying to process this authentication request.</summary>
        [JsonProperty(PropertyName = "error")]
        public string Error;

        /// <summary>A value that represents the role / privilege level of the authenticated user.</summary>
        [JsonProperty(PropertyName = "level")]
        public PermissionLevel Level;

        /// <summary>Device uptime in seconds.</summary>
        [JsonProperty(PropertyName = "started")]
        public string Started;

        /// <summary>Whether the device is running the factory default configuration.</summary>
        [JsonProperty(PropertyName = "default-config")]
        public bool DefaultConfig;

        /// <summary>Contains information about the current device capabilities.</summary>
        [JsonProperty(PropertyName = "platform")]
        public EdgeOSPlatform Platform;

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new StringEnumConverter());
        }

        /// <summary>A value that represents the role / privilege level of a user.</summary>
        public enum PermissionLevel
        {
            /// <summary>Not defined.</summary>
            unknown,

            /// <summary>The user can make changes to the EdgeRouter configuration.</summary>
            admin,

            /// <summary>The user can view the EdgeRouter configuration but cannot make changes.</summary>
            @operator
        }

        /// <summary>Contains information about the device capabilities.</summary>
        public class EdgeOSPlatform
        {
            /// <summary>A string that represents a device model from devices.json.</summary>
            [JsonProperty(PropertyName = "model")]
            public string Model;

            /// <summary>A dictionary that represents the device Power over Ethernet (PoE) capabilities.</summary>
            [JsonProperty(PropertyName = "poe_cap")]
            public Dictionary<string, short> PoECapabilities;

            /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
            /// <returns>An EdgeOS friendly JSON string.</returns>
            public string ToJson()
            {
                return JsonConvert.SerializeObject(this, new StringEnumConverter());
            }
        }
    }
}