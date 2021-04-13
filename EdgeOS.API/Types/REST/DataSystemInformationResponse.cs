using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a system information data response from EdgeOS.</summary>
    public class DataSystemInformationResponse
    {
        /// <summary>The output for the data request.</summary>
        [JsonProperty(PropertyName = "output")]
        public DataSystemInformation Output;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>An object that contains to details of the EdgeOS System Information.</summary>
        public class DataSystemInformation
        {
            /// <summary>The installed software version.</summary>
            [JsonProperty(PropertyName = "sw_ver")]
            public string SoftwareVersion;

            /// <summary>The UNMS status information.</summary>
            [JsonProperty(PropertyName = "unms")]
            public UNMSStatus UNMS;

            /// <summary>The latest known firmware information.</summary>
            [JsonProperty(PropertyName = "fw-latest")]
            public LatestFirmwareDetails LatestFirmware;

            /// <summary>An object containing UNMS status information.</summary>
            public class UNMSStatus
            {
                /// <summary>An object containing all the available EdgeOS UNMS daemon service states.</summary>
                public enum DaemonStatus : byte
                {
                    /// <summary>Not defined or configured.</summary>
                    Unknown,

                    /// <summary>The UNMS daemon is running.</summary>
                    Running,

                    /// <summary>The UNMS daemon is not running.</summary>
                    [EnumMember(Value = "Not running")]
                    NotRunning
                }

                /// <summary>The UNMS daemon status.</summary>
                public DaemonStatus Daemon;

                /// <summary>The UNMS status information.</summary>
                [JsonProperty(PropertyName = "status")]
                public string Status;

                /// <summary>The time the UNMS' status was last updated.</summary>
                [JsonProperty(PropertyName = "last")]
                public string LastUpdated;

            }

            /// <summary>An object containing the latest known firmware information.</summary>
            public class LatestFirmwareDetails
            {
                /// <summary>An object containing all the available EdgeOS device firmware states.</summary>
                public enum FirmwareState : byte
                {
                    /// <summary>Not defined or configured.</summary>
                    Unknown,

                    /// <summary>The EdgeOS device is running the latest firmware.</summary>
                    [EnumMember(Value = "up-to-date")]
                    UpToDate,

                    /// <summary>The EdgeOS device could upgrade to a newer version available online.</summary>
                    [EnumMember(Value = "can-upgrade")]
                    CanUpgrade,

                    /// <summary>The EdgeOS device is downloading new firmware.</summary>
                    [EnumMember(Value = "downloading")]
                    Downloading,

                    /// <summary>The EdgeOS device is currently upgrading.</summary>
                    [EnumMember(Value = "upgrading")]
                    Upgrading,

                    /// <summary>The EdgeOS device requires a reboot for changes to take effect.</summary>
                    [EnumMember(Value = "reboot-needed")]
                    RebootNeeded,
                }

                /// <summary>The latest software version available online.</summary>
                [JsonProperty(PropertyName = "version")]
                public string version;

                /// <summary>The URL to download the latest firmware from.</summary>
                [JsonProperty(PropertyName = "url")]
                public string URL;

                /// <summary>The MD5 hash of the latest firmware.</summary>
                [JsonProperty(PropertyName = "md5")]
                public string MD5;

                /// <summary>The firmware upgrade status information.</summary>
                [JsonProperty(PropertyName = "state")]
                public FirmwareState State;
            }
        }
    }
}