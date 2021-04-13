using Newtonsoft.Json;

namespace EdgeOS.API.Types.Configuration
{
    /// <summary>A class representing a complete EdgeOS Firewall configuration tree.</summary>
    public class Firewall
    {
        /// <summary>Policy for handling of all IPv4 ICMP echo requests</summary>
        [JsonProperty(PropertyName = "all-ping")]
        public ConfigurationBool AllPing;

        /// <summary>Policy for handling broadcast IPv4 ICMP echo and timestamp requests</summary>
        [JsonProperty(PropertyName = "broadcast-ping")]
        public ConfigurationBool BroadcastPing;

        /// <summary>Firewall group</summary>
        [JsonProperty(PropertyName = "group")]
        public FirewallGroup Group;

        /// <summary>IPv6 modify rule-set name</summary>
        [JsonProperty(PropertyName = "ipv6-modify")]
        public object IPv6Modify;

        /// <summary>IPv6 firewall rule-set name</summary>
        [JsonProperty(PropertyName = "ipv6-name")]
        public object IPv6Name;

        /// <summary>Policy for handling received ICMPv6 redirect messages</summary>
        [JsonProperty(PropertyName = "ipv6-receive-redirects")]
        public ConfigurationBool IPv6ReceiveRedirects;

        /// <summary>Policy for handling IPv6 packets with routing extension header</summary>
        [JsonProperty(PropertyName = "ipv6-src-route")]
        public ConfigurationBool IPv6SrcRoute;

        /// <summary>Policy for handling IPv4 packets with source route option</summary>
        [JsonProperty(PropertyName = "ip-src-route")]
        public ConfigurationBool IPSrcRoute;

        /// <summary>Policy for logging IPv4 packets with invalid addresses</summary>
        [JsonProperty(PropertyName = "log-martians")]
        public ConfigurationBool LogMartians;

        /// <summary>IPv4 modify rule-set name</summary>
        [JsonProperty(PropertyName = "modify")]
        public object Modify;

        /// <summary>IPv4 firewall rule-set name</summary>
        [JsonProperty(PropertyName = "name")]
        public object Name;

        /// <summary>Firewall options</summary>
        [JsonProperty(PropertyName = "options")]
        public object Options;

        /// <summary>Policy for handling received IPv4 ICMP redirect messages</summary>
        [JsonProperty(PropertyName = "receive-redirects")]
        public ConfigurationBool ReceiveRedirects;

        /// <summary>Policy for sending IPv4 ICMP redirect messages</summary>
        [JsonProperty(PropertyName = "send-redirects")]
        public ConfigurationBool SendRedirects;

        /// <summary>Policy for source validation by reversed path, as specified in RFC3704</summary>
        [JsonProperty(PropertyName = "source-validation")]
        public ConfigurationBool SourceValidation;

        /// <summary>Policy for using TCP SYN cookies with IPv4</summary>
        [JsonProperty(PropertyName = "syn-cookies")]
        public ConfigurationBool SYNCookies;
    }
}