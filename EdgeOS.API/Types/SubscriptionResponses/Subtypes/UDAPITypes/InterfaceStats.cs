namespace EdgeOS.API.Types.SubscriptionResponses.Subtypes.UDAPITypes
{
    /// <summary>An object that contains statistics information on a single interface.</summary>
    public class UDAPIInterfaceStats
    {
        /// <summary>The current bits per second this interface is receiving.</summary>
        public string rxRate;

        /// <summary>The current bits per second this interface is transmitting.</summary>
        public string txRate;

        /// <summary>The number of bytes this interface has received.</summary>
        public string rxBytes;

        /// <summary>The number of bytes this interface has trasmitted.</summary>
        public string txBytes;

        /// <summary>The number of packets this interface has received.</summary>
        public string rxPackets;

        /// <summary>The number of packets this interface has trasmitted.</summary>
        public string txPackets;

        /// <summary>The total number of packets that were ultimately dropped.</summary>
        public string dropped;

        /// <summary>The number of packets received that were ultimately dropped.</summary>
        public string rxDropped;

        /// <summary>The number of packets transmitted that were ultimately dropped.</summary>
        public string txDropped;

        /// <summary>The total number of frames this interface has failed to process.</summary>
        public string errors;

        /// <summary>The number of frames this interface has failed to recieve.</summary>
        public string rxErrors;

        /// <summary>The number of frames this interface has failed to transmit.</summary>
        public string txErrors;

        /// <summary>Statistics on the Power over Ethernet (PoE) power.</summary>
        public string poePower;

        /// <summary>Statistics on the SFP port.</summary>
        public InterfaceSFPStats sfp;
    }
}