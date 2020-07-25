namespace EdgeOS.API.Types.SubscriptionResponses.Subtypes.UDAPITypes
{
    /// <summary>An object that contains information on the storage.</summary>
    public class Storage
    {
        /// <summary>The name of this storage.</summary>
        public string name;

        /// <summary>The type of storage this is (e.g. flash).</summary>
        public string type;

        /// <summary>The system's name for the storage (often the system device name such as /dev/root).</summary>
        public string sysName;

        /// <summary>How many bytes are used of the storage device.</summary>
        public string used;

        /// <summary>What the capacity of this storage device is.</summary>
        public string size;

        /// <summary>The physical temperature of this storage device (if available).</summary>
        public string temperature;
    }
}