using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class InterfacesResponse : IResponse
    {
        /// <summary>The object that contains network interface information for each of the EdgeOS network interfaces.</summary>
        [JsonProperty(PropertyName = "interfaces")]
        public Dictionary<string, Interface> Interfaces;
    }
}