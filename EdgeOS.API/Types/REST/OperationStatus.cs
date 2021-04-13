using Newtonsoft.Json;

namespace EdgeOS.API.Types.REST
{
    /// <summary>A class representing a single status item for an operation response from EdgeOS.</summary>
    public class OperationStatus
    {
        /// <summary>Whether the operation was successful.</summary>
        [JsonProperty(PropertyName = "success")]
        public byte Success;

        /// <summary>The error that occurred while trying to process this request.</summary>
        [JsonProperty(PropertyName = "error")]
        public string Error;

        /// <summary>Outputs a human friendly readable form of the fields and their relations contained in this object.</summary>
        /// <returns>A string showing the relation between all the fields in a human friendly readable format.</returns>
        public override string ToString() { return "Success : " + Success + (Error != null ? ", Error \": " + Error + "\"": null); }
    }
}