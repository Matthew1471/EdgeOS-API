namespace EdgeOS.API.Types
{
    /// <summary>An object that contains locally connected EdgeOS user information.</summary>
    public class LocalUserInfo
    {
        /// <summary>The method the user is currently connected.</summary>
        public string tty;

        /// <summary>How long the user has been idle for.</summary>
        public string idle;

        /// <summary>The hostname the user has been resolved to (if known).</summary>
        public string host;

        /// <summary>How long the user has been connected for.</summary>
        public string uptime;
    }
}