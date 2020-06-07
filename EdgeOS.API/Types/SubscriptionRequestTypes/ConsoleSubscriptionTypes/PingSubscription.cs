namespace EdgeOS.API.Types.SubscriptionRequestTypes.ConsoleSubscriptionTypes
{
    /// <summary>A request to ping a host.</summary>
    class PingSubscription : ConsoleSubscription
    {
        /// <summary>The destination/host IP.</summary>
        public string target;

        /// <summary>How many times to ping (1 - 4294967295).</summary>
        public uint? count;

        /// <summary>The packet size (1 - 65507).</summary>
        public uint? size;
    }
}