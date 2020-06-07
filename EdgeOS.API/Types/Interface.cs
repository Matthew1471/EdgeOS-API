using System.Collections.Generic;

namespace EdgeOS.API.Types
{
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
        public List<string> addresses;

        /// <summary>A breakdown of statistics for this interface.</summary>
        public InterfaceStats stats;
    }
}