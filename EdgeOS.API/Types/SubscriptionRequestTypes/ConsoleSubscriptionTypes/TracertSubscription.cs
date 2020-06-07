namespace EdgeOS.API.Types.SubscriptionRequestTypes.ConsoleSubscriptionTypes
{
    /// <summary>A request to run tracert to trace a route to a host.</summary>
    class TracertSubscription : ConsoleSubscription
    {
        /// <summary>The destination host.</summary>
        public string target;

        /// <summary>Resolve IP addresses.</summary>
        public bool resolve;
    }
}