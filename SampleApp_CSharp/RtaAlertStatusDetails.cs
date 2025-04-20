using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    /// <summary>
    /// This class defines the properties in the RTA Alert Status
    /// </summary>
    public class RtaAlertStatusDetails
    {
        public RtaAlertStatusDetails()
        {

        }

        public RtaAlertStatusDetails(int number, bool registered, string eventString, string stat, bool reported, bool measuring, bool initialized)
        {
            ItemNumber = number;
            Registered = registered;
            Event = eventString;
            Stat = stat;
            Reported = reported;
            Measuring = measuring;
            Initialized = initialized; 
        }


        public int ItemNumber { get; set; } = 0;
        public bool Registered { get; set; } = false;
        public string Event { get; set; } = string.Empty;
        public string Stat { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public bool Reported { get; set; } = false;
        public bool Measuring { get; set; } = false;
        public bool Initialized { get; set; } = false;
        public bool SuspendState { get; set; } = false; 
    }
}
