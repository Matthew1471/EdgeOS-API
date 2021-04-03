using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a single status item for the batch configuration response from EdgeOS.</summary>
    public class BatchResponseStatus
    {
        /// <summary>Whether the operation failed.</summary>
        [JsonProperty(PropertyName = "failure")]
        public string Failure;

        /// <summary>Whether the operation was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public string Success;

        /// <summary>Represents the class as a JSON string suitable for sending to the EdgeOS device.</summary>
        /// <returns>An EdgeOS friendly JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new StringEnumConverter());
        }
    }
}