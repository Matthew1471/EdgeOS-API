using Newtonsoft.Json;

namespace EdgeOS.API.Types.SubscriptionRequestTypes.ConsoleSubscriptionTypes
{
    /// <summary>A request to run tcpdump and capture packets.</summary>
    class PacketCaptureFeed : ConsoleSubscription
    {
        /// <summary>The interface to capture on.</summary>
        [JsonProperty(PropertyName = "interface")]
        public string captureInterface;

        /// <summary>The packet capture limit (in number of packets).</summary>
        public uint pkt_count;

        /// <summary>Whether to resolve addresses.</summary>
        public bool resolve;

        /// <summary>The filter protocol.</summary>
        public string f_proto;

        /// <summary>The filter address.</summary>
        public string f_address;

        /// <summary>The filter port.</summary>
        public string f_port;

        /// <summary>Whether to negate the filter.</summary>
        public bool f_neg;
    }
}