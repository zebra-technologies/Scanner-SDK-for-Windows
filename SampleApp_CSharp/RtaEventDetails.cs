using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    /// <summary>
    /// This class defines the properties in the RTA Registration details. 
    /// </summary>
    public class RtaEventDetails
    {
        public RtaEventDetails()
        {

        }

        public RtaEventDetails(int number, bool registered, string eventString, string stat, string onlimit, string offlimit)
        {
            ItemNumber = number;
            Registered = registered;
            Event = eventString;
            Stat = stat;
            OnLimit = onlimit;
            OffLimit = offlimit; 
        }
            

        public int ItemNumber { get; set; } = 0; 
        public bool Registered { get; set; } = false;
        public string Event { get; set; } = string.Empty;
        public string Stat { get; set; } = string.Empty;
        public string OnLimit { get; set; } = string.Empty;
        public string OffLimit { get; set; } = string.Empty;

    }
}
