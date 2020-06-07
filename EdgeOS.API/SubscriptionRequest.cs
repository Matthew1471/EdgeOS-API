using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EdgeOS.API
{
    /// <summary>The type of messages that EdgeOS should deliver.</summary>
    public enum SubscriptionMessageType { PONStatistics, NNIStatistics, ONUList, Interfaces, DeviceDiscovery, SystemStatistics, NumberOfRoutes, ConfigurationChange, Users, TrafficAnalysis, NATStatistics, LogFeed, FirewallStatistics, PFStatistics, LLDPDetail, UDAPIStatistics, PingFeed, TracerouteFeed, BandwidthTestFeed, PacketsFeed };

    /// <summary>A class representing the required subscription and unsubscription of EdgeOS.</summary>
    public class SubscriptionRequest
    {
        /// <summary>EdgeOS message types that we wish to subscribe to.</summary>
        public List<SubscriptionMessageType> Subscribe = new List<SubscriptionMessageType>();

        /// <summary>EdgeOS message types that we wish to unsubscribe from.</summary>
        public List<SubscriptionMessageType> Unsubscribe = new List<SubscriptionMessageType>();

        /// <summary>EdgeOS authenticates based off a PHP session ID.</summary>
        public string SessionID;

        /// <summary>Converts our C# friendly <see cref="SubscriptionMessageType"/> to an EdgeOS compliant string.</summary>
        /// <param name="subscriptionMessageType">The C# subscription message type.</param>
        /// <returns>The EdgeOS message type.</returns>
        private static string GetEdgeOSFriendlySubscriptionMessageType(SubscriptionMessageType subscriptionMessageType)
        {
            switch (subscriptionMessageType)
            {
                // Status
                case SubscriptionMessageType.ONUList: return "onu-list";
                case SubscriptionMessageType.Interfaces: return "interfaces";
                case SubscriptionMessageType.DeviceDiscovery: return "discover";
                case SubscriptionMessageType.NumberOfRoutes: return "num-routes";
                case SubscriptionMessageType.ConfigurationChange: return "config-change";
                case SubscriptionMessageType.Users: return "users";
                case SubscriptionMessageType.TrafficAnalysis: return "export";
                case SubscriptionMessageType.LLDPDetail: return "lldp-detail";

                // Statistics
                case SubscriptionMessageType.FirewallStatistics: return "fw-stats";
                case SubscriptionMessageType.PFStatistics: return "pf-stats";
                case SubscriptionMessageType.PONStatistics: return "pon-stats";
                case SubscriptionMessageType.NNIStatistics: return "nni-stats";
                case SubscriptionMessageType.SystemStatistics: return "system-stats";
                case SubscriptionMessageType.UDAPIStatistics: return "udapi-statistics";
                case SubscriptionMessageType.NATStatistics: return "nat-stats";

                // Feeds
                case SubscriptionMessageType.LogFeed: return "log-feed";
                case SubscriptionMessageType.PingFeed: return "ping-feed";
                case SubscriptionMessageType.TracerouteFeed: return "traceroute-feed";
                case SubscriptionMessageType.PacketsFeed: return "packets-feed";
                case SubscriptionMessageType.BandwidthTestFeed: return "bwtest-feed";

                default: throw new NotImplementedException(subscriptionMessageType.ToString());
            }
        }

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            StringWriter stringWriter = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(stringWriter);

            // {
            writer.WriteStartObject();

            // "SUBSCRIBE":
            writer.WritePropertyName("SUBSCRIBE");

            // [
            writer.WriteStartArray();

            foreach (SubscriptionMessageType subscription in Subscribe)
            {
                // {
                writer.WriteStartObject();

                // "name":"interfaces"
                writer.WritePropertyName("name");
                writer.WriteValue(GetEdgeOSFriendlySubscriptionMessageType(subscription));

                // },
                writer.WriteEndObject();
            }

            // ]
            writer.WriteEndArray();

            // "UNSUBSCRIBE":
            writer.WritePropertyName("UNSUBSCRIBE");

            // [
            writer.WriteStartArray();

            foreach (SubscriptionMessageType subscription in Unsubscribe)
            {
                // {
                writer.WriteStartObject();

                // "name":"interfaces"
                writer.WritePropertyName("name");
                writer.WriteValue(GetEdgeOSFriendlySubscriptionMessageType(subscription));

                // },
                writer.WriteEndObject();
            }

            // ]
            writer.WriteEndArray();

            // "SESSION_ID":"ac47ffb3b3014f089abc24e4b9dc6bff"
            writer.WritePropertyName("SESSION_ID");
            writer.WriteValue(SessionID);

            // }
            writer.WriteEndObject();

            return stringWriter.ToString();
        }
    }
}