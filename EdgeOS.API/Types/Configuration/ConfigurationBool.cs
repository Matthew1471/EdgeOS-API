using System.Runtime.Serialization;

namespace EdgeOS.API.Types.Configuration
{
    /// <summary>EdgeOS Configuration Boolean.</summary>
    public enum ConfigurationBool : byte
    {
        /// <summary>This setting has not been configured.</summary>
        [EnumMember(Value = null)]
        NotConfigured,

        /// <summary>This setting is enabled.</summary>
        [EnumMember(Value = "enable")]
        Enable,

        /// <summary>This setting is disabled.</summary>
        [EnumMember(Value = "disable")]
        Disable
    }
}