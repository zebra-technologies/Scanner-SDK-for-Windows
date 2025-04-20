using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    /// <summary>
    /// This class holds the details of the public RTA events details.
    /// </summary>
    class RtaPublicEventDetails
    {
        public RtaPublicEventDetails()
        {

        }
        public RtaPublicEventDetails(string eventString, string stat)
        {
            Event = eventString;
            Stat = stat;           
        }

        public string Event { get; set; } = string.Empty;
        public string Stat { get; set; } = string.Empty;
      
    }
}
