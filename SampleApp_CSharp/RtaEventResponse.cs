using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    /// <summary>
    /// This class defines the properties in the RTA event response. 
    /// </summary>
    public class RtaEventResponse
    {
        public RtaEventResponse() 
        { 

        }
        public RtaEventResponse(string eventString, string stat, string data1, string data2, string rawData) 
        {
            Event = eventString;
            Stat = stat;
            Data1 = data1;
            Data2 = data2;
            RawData = rawData;
            EventTimeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"); 
        }


        public string Event { get; set; } = string.Empty;
        public string Stat { get; set; } = string.Empty;
        public string Data1 { get; set; } = string.Empty;
        public string Data2 { get; set; } = string.Empty;
        public string RawData { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string EventTimeStamp { get; set; } = string.Empty;

    }
}