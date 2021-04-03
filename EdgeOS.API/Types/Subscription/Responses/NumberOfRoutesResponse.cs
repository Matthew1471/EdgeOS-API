using Newtonsoft.Json;

namespace EdgeOS.API.Types.Subscription.Responses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class NumberOfRoutesResponse : IResponse
    {
        /// <summary>The object containing counts of the EdgeOS network routes.</summary>
        [JsonProperty(PropertyName = "num-routes")]
        public NumberOfRoutesDetails NumberOfRoutes;

        /// <summary>An object containing counts of the EdgeOS network routes.</summary>
        public class NumberOfRoutesDetails
        {
            /// <summary>The number of routes that are currently connected.</summary>
            [JsonProperty(PropertyName = "connected")]
            public byte connectedRoutes;

            /// <summary>The number of static routes.</summary>
            [JsonProperty(PropertyName = "static")]
            public byte staticRoutes;

            /// <summary>The total number of routes (should be the number of static + connected routes).</summary>
            [JsonProperty(PropertyName = "total")]
            public byte totalRoutes;
        }
    }
}