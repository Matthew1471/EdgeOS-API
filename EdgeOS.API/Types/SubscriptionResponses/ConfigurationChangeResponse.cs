using Newtonsoft.Json;

namespace EdgeOS.API.Types.SubscriptionResponses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class ConfigurationChangeResponse : IResponse
    {
        /// <summary>The object that contains status information when EdgeOS is processing a configuration change.</summary>
        [JsonProperty(PropertyName = "config-change")]
        public ConfigurationChangeDetails ConfigurationChange;

        /// <summary>An object that contains status information when EdgeOS is processing a configuration change.</summary>
        public class ConfigurationChangeDetails
        {
            /// <summary>The configuration change status message.</summary>
            public string commit;
        }
    }
}