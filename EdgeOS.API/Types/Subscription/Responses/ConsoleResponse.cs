namespace EdgeOS.API.Types.Subscription.Responses
{
    /// <summary>A string message from EdgeOS that is serialised into this class.</summary>
    public class ConsoleResponse : IResponse
    {
        /// <summary>The subscription ID that the JSON will contain as the property name for responses to this subscription request.</summary>
        public string sub_id;

        /// <summary>A generic JSON message from either a tool or an error message.</summary>
        public string Message;
    }
}