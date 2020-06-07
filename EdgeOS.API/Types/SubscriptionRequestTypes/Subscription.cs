namespace EdgeOS.API.Types.SubscriptionRequestTypes
{
    /// <summary>Represents a standard subscription with no parameters. Can be overriden to provide further parameters.</summary>
    public class Subscription
    {
        /// <summary>The name of the type of subscription we are requesting.</summary>
        public SubscriptionMessageType name;
    }
}