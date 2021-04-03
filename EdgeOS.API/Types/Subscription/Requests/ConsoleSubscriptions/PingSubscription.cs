using EdgeOS.API.Types.Subscription.Requests;

namespace EdgeOS.API.Types.Subscription.Requests.ConsoleSubscriptions
{
    /// <summary>A request to ping a host.</summary>
    public class PingSubscription : ConsoleSubscription
    {
        /// <summary>The destination/host to ping.</summary>
        public string target;

        /// <summary>How many times to ping (1 - 4294967295).</summary>
        public uint? count;

        /// <summary>The packet size of each ping request (1 - 65507).</summary>
        public uint? size;
    }
}