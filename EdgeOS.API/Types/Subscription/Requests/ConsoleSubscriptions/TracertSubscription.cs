namespace EdgeOS.API.Types.Subscription.Requests.ConsoleSubscriptions
{
    /// <summary>A request to run tracert to trace a route to a host.</summary>
    public class TracertSubscription : ConsoleSubscription
    {
        /// <summary>The destination host.</summary>
        public string target;

        /// <summary>Resolve IP addresses.</summary>
        public bool resolve;
    }
}