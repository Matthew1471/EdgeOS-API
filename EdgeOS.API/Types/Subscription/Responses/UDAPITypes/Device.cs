namespace EdgeOS.API.Types.Subscription.Responses.UDAPITypes
{
    /// <summary>An object that represents a device.</summary>
    public class Device
    {
        /// <summary>Information about all the CPUs this device has.</summary>
        public CPU[] cpu;

        /// <summary>Information on this device's RAM.</summary>
        public RAM ram;

        /// <summary>Information about all the power supplys this device has.</summary>
        public Power[] power;

        /// <summary>Information about all the storage this device has.</summary>
        public Storage[] storage;

        /// <summary>Temperature readings from all of the temperature sensors on this device.</summary>
        public Temperature[] temperatures;

        /// <summary>Information about all the fans this device has.</summary>
        public FanSpeed[] fanSpeeds;

        /// <summary>How long this device has been powered for in seconds.</summary>
        public string uptime;
    }
}