using System.Collections.Generic;

namespace EdgeOS.API.Types
{
    /// <summary>An object containing device API statistics, potentially used by UNMS.</summary>
    public class UDAPIStatistics
    {
        public string timestamp;

        public Dictionary<string, string[]> device;
    }
}