using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace EdgeOS.API.Types
{
    /// <summary>The type of messages that EdgeOS should deliver.</summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionMessageType
    {
        // Returns JSON objects.

        /// <summary>Request messages containing status information when EdgeOS is processing a configuration change.</summary>
        [EnumMember(Value = "config-change")]
        ConfigurationChange,

        /// <summary>Request messages that contains discovered device information when EdgeOS has discovered another device (typically via UBNT Discovery).</summary>
        [EnumMember(Value = "discover")]
        DeviceDiscovery,

        /// <summary>Request periodic messages containing network interface information for each of the EdgeOS network interfaces.</summary>
        [EnumMember(Value = "interfaces")]
        Interfaces,

        /// <summary>Request periodic messages containing counts of the EdgeOS network routes.</summary>
        [EnumMember(Value = "num-routes")]
        NumberOfRoutes,

        /// <summary>Request periodic messages containing system statistics about the EdgeOS device.</summary>
        [EnumMember(Value = "system-stats")]
        SystemStatistics,

        /// <summary>Request periodic messages containing traffic analysis information for each host when EdgeOS has observed certain application types via Deep Packet Inspection (DPI).</summary>
        [EnumMember(Value = "export")]
        TrafficAnalysis,

        /// <summary>Request messages that contain the users logged into the EdgeOS device including SSH, Web and VPN.</summary>
        [EnumMember(Value = "users")]
        Users,

        /// <summary>Request periodic messages containing device statistics in a specific API format, potentially used by UNMS.</summary>
        [EnumMember(Value = "udapi-statistics")]
        UDAPIStatistics,

        // Returns RAW Console Output.

        /// <summary>Request periodic system log messages.</summary>
        [EnumMember(Value = "log-feed")]
        LogFeed,

        /// <summary>Request periodic Network Address Translation (NAT) statistics.</summary>
        [EnumMember(Value = "nat-stats")]
        NATStatistics,

        /// <summary>Request periodic port forwarding statistics.</summary>
        [EnumMember(Value = "pf-stats")]
        PortForwardingStatistics,

        // Returns RAW Console Output but requires additional parameters.

        /// <summary>Request a bandwidth test (using iperf) is performed or hosted.</summary>
        [EnumMember(Value = "bwtest-feed")]
        BandwidthTestFeed,

        /// <summary>Request periodic firewall statistics.</summary>
        [EnumMember(Value = "fw-stats")]
        FirewallStatistics,

        /// <summary>Request a packet capture (using tcpdump) is performed.</summary>
        [EnumMember(Value = "packets-feed")]
        PacketCaptureFeed,

        /// <summary>Request a ping is performed and the results are returned as they are available.</summary>
        [EnumMember(Value = "ping-feed")]
        PingFeed,

        /// <summary>Request to run tracert to trace a route to a host.</summary>
        [EnumMember(Value = "traceroute-feed")]
        TracerouteFeed,

        // Not Fully Implemented In This API Version.

        /// <summary>Request information about Link Layer Discovery Protocol (LLDP) connected neighbours.</summary>
        [EnumMember(Value = "lldp-detail")]
        LLDPDetail,

        /// <summary>List Network to Network Interface (NNI) statistics.</summary>
        [EnumMember(Value = "nni-stats")]
        NNIStatistics,

        /// <summary>List Optical Network Unit (ONU) details.</summary>
        [EnumMember(Value = "onu-list")]
        ONUList,

        /// <summary>List Passive Optical Network (PON) statistics.</summary>
        [EnumMember(Value = "pon-stats")]
        PONStatistics,
    };
}