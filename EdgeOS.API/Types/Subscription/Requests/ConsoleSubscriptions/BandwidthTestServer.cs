using EdgeOS.API.Types.Subscription.Requests;
using Newtonsoft.Json;

namespace EdgeOS.API.Types.Subscription.Requests.ConsoleSubscriptions
{
    /// <summary>A request to run iperf as a server to test network bandwidth.</summary>
    public class BandwidthTestServer : ConsoleSubscription
    {
        /// <summary>Whether to run the bandwidth test as a server.</summary>
        [JsonProperty(PropertyName = "server-mode")]
        public bool serverMode;
    }
}