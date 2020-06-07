using Newtonsoft.Json;

namespace EdgeOS.API.Types.UDAPITypes
{
    /// <summary>An object that contains information on a FanSpeed.</summary>
    public class FanSpeed
    {
        /// <summary>Unknown.</summary>
        [JsonProperty(PropertyName = "&ubnt_arr_type;")]
        public string ArrayType;
    }
}