using EdgeOS.API.Types.Subscription.Requests;

namespace EdgeOS.API.Types.Subscription.Requests.ConsoleSubscriptions
{
    /// <summary>Requests statistics from the firewall, optionally for a specific chain.</summary>
    public class FirewallStatistics : ConsoleSubscription
    {
        /// <summary>The firewall chain to specify.</summary>
        public string chain;
    }
}
