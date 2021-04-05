using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST.Responses
{
    /// <summary>A class representing an operation response from EdgeOS.</summary>
    public class OperationResponse
    {
        /// <summary>Status values for the operation request.</summary>
        [JsonProperty(PropertyName = "OPERATION")]
        public OperationStatus Operation;

        /// <summary>Whether the API request was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;
    }
}