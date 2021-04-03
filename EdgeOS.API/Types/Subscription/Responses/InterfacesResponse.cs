using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdgeOS.API.Types.Subscription.Responses
{
    /// <summary>A JSON message from EdgeOS that is serialised into this class.</summary>
    public class InterfacesResponse : IResponse
    {
        /// <summary>The object that contains network interface information for each of the EdgeOS network interfaces.</summary>
        [JsonProperty(PropertyName = "interfaces")]
        public Dictionary<string, Interface> Interfaces;

        /// <summary>An object that contains network interface information for a specific EdgeOS network interface.</summary>
        public class Interface
        {
            /// <summary>Whether or not the interface is currently enabled.</summary>
            public bool up;

            /// <summary>Whether the physical layer is currently plugged-in.</summary>
            public bool l1up;

            /// <summary>Whether this interface is set to auto-negotiate.</summary>
            public bool autoneg;

            /// <summary>The duplex setting for this interface.</summary>
            public string duplex;

            /// <summary>The speed this interface is operating at.</summary>
            public ushort speed;

            /// <summary>Whether this interface is virtual.</summary>
            public bool on_switch;

            /// <summary>Whether this is an interface that can operate in either copper or in fiber mode.</summary>
            public bool is_combo;

            /// <summary>Whether this is an interface that has Small Form-factor Pluggable (SFP) support.</summary>
            public bool is_sfp;

            /// <summary>Whether a small form-factor pluggable module is physically present.</summary>
            public bool sfp_present;

            /// <summary>The MAC address for this interface.</summary>
            public string mac;

            /// <summary>The Maximum Transmission Unit (MTU) for this interface.</summary>
            public uint mtu;

            /// <summary>Any addresses assigned to this interface.</summary>
            public string[] addresses;

            /// <summary>A breakdown of statistics for this interface.</summary>
            public InterfaceStats stats;

            /// <summary>An object that contains network interface statistics information for a specific EdgeOS network interface.</summary>
            public class InterfaceStats
            {
                /// <summary>The number of packets this interface has received.</summary>
                public ulong rx_packets;

                /// <summary>The number of packets this interface has trasmitted.</summary>
                public ulong tx_packets;

                /// <summary>The number of bytes this interface has received.</summary>
                public ulong rx_bytes;

                /// <summary>The number of bytes this interface has trasmitted.</summary>
                public ulong tx_bytes;

                /// <summary>The number of frames this interface has failed to recieve.</summary>
                public ulong rx_errors;

                /// <summary>The number of frames this interface has failed to transmit.</summary>
                public ulong tx_errors;

                /// <summary>The number of packets received that were ultimately dropped.</summary>
                public ulong rx_dropped;

                /// <summary>The number of packets transmitted that were ultimately dropped.</summary>
                public ulong tx_dropped;

                /// <summary>The number of multicast packets processed by this interface.</summary>
                public ulong multicast;

                /// <summary>The current bits per second this interface is receiving.</summary>
                public ulong rx_bps;

                /// <summary>The current bits per second this interface is transmitting.</summary>
                public ulong tx_bps;
            }
        }
    }
}