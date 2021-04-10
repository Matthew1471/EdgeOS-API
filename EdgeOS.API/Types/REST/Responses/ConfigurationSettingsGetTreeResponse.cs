using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.REST.Responses
{
    /// <summary>A class representing a configuration tree response from EdgeOS.</summary>
    public class ConfigurationSettingsGetTreeResponse
    {
        /// <summary>The requested tree section of the configuration.</summary>
        [JsonProperty(PropertyName = "GETCFG")]
        public GetConfig Get;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;

        public class GetConfig
        {
            /// <summary>The requested children of the configuration.</summary>
            [JsonProperty(PropertyName = "children")]
            public object[] Children;

            /// <summary>The definitions of the requested configuration.</summary>
            [JsonProperty(PropertyName = "defs")]
            public Dictionary<string, Definition> Definitions;

            /// <summary>An array of values contained within this configuration node.</summary>
            [JsonProperty(PropertyName = "tags")]
            public string[] Tags;

            /// <summary>Whether the operation was successful.</summary>
            [JsonProperty(PropertyName = "success")]
            public byte Success;

            /// <summary>The error that occurred while trying to process this GETCFG request.</summary>
            [JsonProperty(PropertyName = "error")]
            public string Error;

            public class Definition
            {
                public enum ValueType {
                    /// <summary>This value has not been set.</summary>
                    Unknown,

                    /// <summary>A string of text.</summary>
                    txt,

                    /// <summary>Unsigned 32bit number.</summary>
                    u32,

                    /// <summary>An IPv4 Address.</summary>
                    ipv4,

                    /// <summary>An IPv6 Address.</summary>
                    ipv6,

                    /// <summary>A true or false.</summary>
                    @bool,

                    /// <summary>A MAC Address.</summary>
                    macaddr,

                    /// <summary>An IPv4 Network (CIDR).</summary>
                    ipv4net,

                    /// <summary>An IPv6 Network (CIDR).</summary>
                    ipv6net
                };

                /// <summary>The type of the configurable value (in the format <see cref="ValueType"/>).</summary>
                [JsonProperty(PropertyName = "type")]
                public ValueType Type;

                /// <summary>An additional permitted type of the configurable value (in the format <see cref="ValueType"/>).</summary>
                [JsonProperty(PropertyName = "type2")]
                public ValueType Type2;

                /// <summary>A default value for this setting.</summary>
                [JsonProperty(PropertyName = "default")]
                public string Default;

                /// <summary>Whether this value can be specified multiple times.</summary>
                [JsonProperty(PropertyName = "multi")]
                public bool Multiple;

                /// <summary>Whether this is a tag (list in web UI) that can contain children.</summary>
                [JsonProperty(PropertyName = "tag")]
                public bool Tag;

                /// <summary>An explanation of the key or value that is used in the web UI as the tooltip text.</summary>
                [JsonProperty(PropertyName = "help")]
                public string Help;
            }
        }
    }
}