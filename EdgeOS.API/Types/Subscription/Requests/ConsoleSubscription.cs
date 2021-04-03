namespace EdgeOS.API.Types.Subscription.Requests
{
    /// <summary>Represents a subscription that calls an application (with no parameters) that returns the data as a value to a sub_id JSON property. Can be overridden for applications that require parameters.</summary>
    public class ConsoleSubscription : Subscription
    {
        /// <summary>The subscription ID that the JSON will contain as the property name for responses to this subscription request.</summary>
        public string sub_id;
    }
}