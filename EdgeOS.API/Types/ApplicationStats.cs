using Newtonsoft.Json;

namespace EdgeOS.API.Types
{
    /// <summary>An object that contains traffic statistics for a certain observed application type via Deep Packet Inspection (DPI).</summary>
    public class ApplicationStats
    {
        /// <summary>The application's transmitted bytes.</summary>
        [JsonProperty(PropertyName = "tx_bytes")]
        uint tx_bytes;

        /// <summary>The application's current transmit rate.</summary>
        [JsonProperty(PropertyName = "tx_rate")]
        uint tx_rate;

        /// <summary>The application's received bytes.</summary>
        [JsonProperty(PropertyName = "rx_bytes")]
        uint rx_bytes;

        /// <summary>The application's current receive rate.</summary>
        [JsonProperty(PropertyName = "rx_rate")]
        uint rx_rate;
    }
}