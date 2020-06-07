namespace EdgeOS.API.Types
{
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