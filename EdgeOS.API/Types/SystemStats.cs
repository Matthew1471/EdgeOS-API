namespace EdgeOS.API.Types
{
    /// <summary>An object containing system statistics about the EdgeOS device.</summary>
    public class SystemStats
    {
        /// <summary>The CPU usage (as a percentage) of the EdgeOS device.</summary>
        public byte cpu;

        /// <summary>How long this EdgeOS device has been running for.</summary>
        public ushort uptime;

        /// <summary>The amount of memory in use (as a percentage) on the EdgeOS device.</summary>
        public byte mem;
    }
}