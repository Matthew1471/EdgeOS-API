using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing the routes data response from EdgeOS.</summary>
    public class DataRoutesResponse
    {
        /// <summary>The output for the data request.</summary>
        [JsonProperty(PropertyName = "output")]
        public Routes[] Output;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>The EdgeOS routes.</summary>
        public class Routes
        {
            /// <summary>The route prefix.</summary>
            [JsonProperty(PropertyName = "pfx")]
            public string Prefix;

            /// <summary>The route next hop information.</summary>
            [JsonProperty(PropertyName = "nh")]
            public NextHopDetails[] NextHop;

            /// <summary>Contains route next hop information.</summary>
            public class NextHopDetails
            {
                /// <summary>The different EdgeOS route types.</summary>
                public enum RouteType
                {
                    /// <summary>Not defined or known.</summary>
                    Unknown,

                    /// <summary>A static route (used by blackhole, interface, gateway).</summary>
                    Static = 'S',

                    /// <summary>Connected route.</summary>
                    Connected = 'C',

                    /// <summary>Connected OSPF route.</summary>
                    OSPF = 'O',

                    /// <summary>Connected RIP route.</summary>
                    RIP = 'R',

                    /// <summary>Connected Kernel route.</summary>
                    Kernel = 'K'
                }

                /// <summary>The route type (in the format RouteType, Selected, FIB e.g. S>*).</summary>
                [JsonProperty(PropertyName = "t")]
                public string Type;

                /// <summary>The metric information.</summary>
                [JsonProperty(PropertyName = "metric")]
                public string Metric;

                /// <summary>Whether this interface is a blackhole.</summary>
                [JsonProperty(PropertyName = "bh")]
                public byte Blackhole;

                /// <summary>The next hop address information.</summary>
                [JsonProperty(PropertyName = "via")]
                public string Via;

                /// <summary>The next hop interface.</summary>
                [JsonProperty(PropertyName = "intf")]
                public string Interface;

                /// <summary>Parses the EdgeOS Route Type value into its component parts.</summary>
                /// <returns>The converted route type components.</returns>
                public ConvertedRouteType GetConvertedRouteType()
                {
                    return new ConvertedRouteType()
                    {
                        Type = (RouteType)Type[0],
                        Selected = Type[2] == '>',
                        FIB = Type[3] == '*'
                    };
                }

                /// <summary>The component parts of an EdgeOS route type.</summary>
                public class ConvertedRouteType
                {
                    /// <summary>The type of this route.</summary>
                    public RouteType Type;

                    /// <summary>Whether the route is able to be used (or just declared in the config file).</summary>
                    public bool Selected;

                    /// <summary>Whether the route is in the Forwarding Information Base (FIB).</summary>
                    public bool FIB;
                }
            }
        }
    }
}