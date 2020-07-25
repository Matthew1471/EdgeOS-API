using Newtonsoft.Json;

namespace EdgeOS.API.Types.SubscriptionRequests.ConsoleSubscriptions
{
    /// <summary>A request to run iperf as a client to an existing iperf server to test network bandwidth.</summary>
    public class BandwidthTestClient : ConsoleSubscription
    {
        /// <summary>Receiver IP.</summary>
        public string server;

        /// <summary>Duration in seconds (0 - 1000 or null).</summary>
        public ushort? duration;

        /// <summary>The protocol (e.g. UDP, TCP or null).</summary>
        public string protocol;

        /// <summary>The UDP bandwidth in Kbps (500 - 1000000 or null).</summary>
        [JsonProperty(PropertyName = "udp-bandwidth")]
        public uint? udpBandwidth;

        /// <summary>The number of parallel flows (1 - 20 or null).</summary>
        [JsonProperty(PropertyName = "parallel-flows")]
        public byte? parallelFlows;

        /// <summary>The TCP window size (64 - 1024 or null).</summary>
        [JsonProperty(PropertyName = "tcp-window-size")]
        public ushort? tcpWindowSize;

        /// <summary>Whether to reverse the direction.</summary>
        [JsonProperty(PropertyName = "reverse-direction")]
        public bool? reverseDirection;
    }
}