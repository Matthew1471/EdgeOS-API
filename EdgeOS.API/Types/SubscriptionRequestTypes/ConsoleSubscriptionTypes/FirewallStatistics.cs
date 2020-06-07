namespace EdgeOS.API.Types.SubscriptionRequestTypes.ConsoleSubscriptionTypes
{
    /// <summary>Requests statistics from the firewall, optionally for a specific chain.</summary>
    class FirewallStatistics : ConsoleSubscription
    {
        /// <summary>The firewall chain to specify.</summary>
        public string chain;
    }
}
