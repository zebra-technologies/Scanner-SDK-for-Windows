using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        public event EventHandler RtaEventReceived;
        public List<RtaEventResponse> RtaResponseList { get; set; } = new List<RtaEventResponse>();

        /// <summary>
        /// Clears RTA Event Data. 
        /// </summary>
        public void ClearRtaEventData()
        {
            RtaResponseList.Clear();
            RtaEventReceived?.Invoke(this, null);
        }

        /// <summary>
        /// Returns the Rta Event response details list. 
        /// </summary>
        /// <returns>List of RtaEventResponse objects</returns>
        public List<RtaEventResponse> GetRtaEventResponseDetails()
        {
            return RtaResponseList;
        }

        /// <summary>
        /// Retrieves the RTA event details 
        /// </summary>
        /// <param name="getSupportedList">Is true, calls the RTA Get Supported event list attribute (5427)</param>
        /// <returns>Returns the list of supported RTA events</returns>
        public List<RtaEventDetails> GetRTAEventDetails(bool getSupportedList)
        {
            try
            {
                if (IsScannerConnected())
                {
                    string outXML = String.Empty;
                    int iStatus = STATUS_FALSE;
                    string inXML = string.Empty;

                    if (getSupportedList)
                    {
                        inXML = GenerateOpCode_InXML(); // Generating the OpCode to be sent into the CoreScanner
                        ExecCmd(RTA_GET_SUPPORTED, ref inXML, out outXML, out iStatus); // Retrieving the supported RTA events from the device using the CoreScanner
                        DisplayResult(iStatus, "RTA_GET_SUPPORTED");
                    }
                    else
                    {
                        inXML = GenerateOpCode_InXML(); // Generating the OpCode to be sent into the CoreScanner
                        ExecCmd(RTA_GET_EVENT_STATUS, ref inXML, out outXML, out iStatus); // Retrieving the Registered RTA events from the device using the CoreScanner
                        DisplayResult(iStatus, "RTA_GET_REGISTERED");
                        if (iStatus == STATUS_SUCCESS) cbSuspend.Checked = m_xml.ReadRTAOpCodeXML_GetSuspendStatus(outXML); //Retrieving the RTA suspend status from the device using the same outxml of RTA_GET_EVENT_STATUS opcode

                    }

                    UpdateOutXml(outXML); //Updating the LOG 

                    if (iStatus == STATUS_SUCCESS)
                    {
 
                        if (!m_xml.ReadRTAOpCodeXML_ValidateEventSupport(outXML)) // Checking the RTA support of the device
                        {
                            UpdateResults("ATTR_GET" + " - null return");
                        }
                        else
                        {
                            List<RtaEventDetails> rtaDetailsList = new List<RtaEventDetails>();
                            rtaDetailsList = m_xml.ReadRTAOpCodeXML_ExtractRTAEventsList(outXML); // Adding the RTA events which are retrieved from the device as a list

                            if (rtaDetailsList.Count == 0) return rtaDetailsList; // Checking if the device list is empty

                            foreach (RtaEventDetails rtaItem in rtaDetailsList)
                            {

                                if (rtaItem.OnLimit.Equals(RtaNotApplicableLimit)) rtaItem.OnLimit = RtaLimitNotSupported;
                                if (rtaItem.OffLimit.Equals(RtaNotApplicableLimit)) rtaItem.OffLimit = RtaLimitNotSupported;

                                if (rtaItem.Event.Trim().Equals(GiftedBatteryPercentageAttribute))
                                {
                                    if (rtaItem.OnLimit.Equals(RtaMaxOnOffLimit)) rtaItem.OnLimit = RtaLimitNotSet;
                                    if (rtaItem.OffLimit.Equals(RtaMaxOnOffLimit)) rtaItem.OffLimit = RtaLimitNotSet;

                                }
                                else if (rtaItem.Event.Trim().Equals(ScannerOutOfCradleAttribute))
                                {
                                    if (rtaItem.OnLimit.Equals(RtaMaxOnOffLimit)) rtaItem.OnLimit = RtaLimitNotSet;
                                    if (rtaItem.OffLimit.Equals(RtaMaxOnOffLimit)) rtaItem.OffLimit = RtaLimitNotSet;

                                }
                                else if (rtaItem.Event.Trim().Equals(ScannerIdleAttribute))
                                {
                                    if (rtaItem.OnLimit.Equals(RtaMaxOnOffLimit)) rtaItem.OnLimit = RtaLimitNotSet;
                                    if (rtaItem.OffLimit.Equals(RtaMaxOnOffLimit)) rtaItem.OffLimit = RtaLimitNotSet;
                                }
                                else
                                {
                                    rtaItem.OnLimit = RtaLimitNotSupported;
                                    rtaItem.OffLimit = RtaLimitNotSupported;
                                }
                            }
                            return rtaDetailsList;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Registers the list of RTA events configured in the UI
        /// </summary>
        /// <param name="eventList">List of RTA events to be registered</param>
        public void RegisterRTAEvents(List<RtaEventDetails> eventList)
        {
            try
            {
                if (eventList != null && eventList.Count > 0)
                {
                    if (IsScannerConnected())
                    {
                        string outXML = String.Empty;
                        int iStatus = STATUS_FALSE;
                        string inXML = string.Empty;

                        foreach (RtaEventDetails rtaEvent in eventList)
                        {
                            inXML += "<rtaevent>" +
                                "<id>" + rtaEvent.Event + "</id>" +
                                "<stat>" + rtaEvent.Stat + "</stat>" +
                                "<onlimit>" + rtaEvent.OnLimit + "</onlimit>" +
                                "<offlimit>" + rtaEvent.OffLimit + "</offlimit>" +
                                "</rtaevent>";
                        }

                        inXML = GenerateOpCode_InXML(inXML);
                        ExecCmd(RTA_REGISTER,ref inXML,out outXML, out iStatus);
                        DisplayResult(iStatus, "RTA_REGISTER_EVENTS");

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the RTA Alert Status
        /// </summary>
        /// <returns>Returns a list of RTA alert status details. </returns>
        public List<RtaAlertStatusDetails> GetRtaAlertStatus() // Checking for the RTA Events suspend state
        {
            try
            {

                if (IsScannerConnected())
                {
                    string outXML = String.Empty;
                    int iStatus = STATUS_FALSE;
                    string inXML = string.Empty;

                    inXML = GenerateOpCode_InXML(); // Generating the OpCode to get the RTA Event Status - 5503

                    ExecCmd(RTA_GET_EVENT_STATUS, ref inXML, out outXML, out iStatus); 
                    if (iStatus == STATUS_SUCCESS && outXML != null) cbSuspend.Checked = m_xml.ReadRTAOpCodeXML_GetSuspendStatus(outXML); //Setting the suspend state checkbox value

                    ExecCmd(RTA_GET_EVENT_STATUS, ref inXML, out outXML, out iStatus);
                    DisplayResult(iStatus, "RTA_GET_EVENT_STATUS");


                    if (iStatus == STATUS_SUCCESS)
                    {
                        UpdateOutXml(outXML); //Updating the LOG
                        List<RtaAlertStatusDetails> rtaDetailsList = new List<RtaAlertStatusDetails>();
                        rtaDetailsList =  m_xml.ReadRTAOpCodeXML_ExtractRTAEventStatus(outXML);
                        return rtaDetailsList;
                    }
                   

                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RtaAlertStatusDetails> GetRtaAlertStatus(string scannerID) // Checking for the RTA Events
        {
            try
            {
                List<RtaAlertStatusDetails> rtaDetailsList = new List<RtaAlertStatusDetails>();
        
                string outXML = String.Empty;
                int iStatus = STATUS_FALSE;
                string inXML = string.Empty;

                inXML = GenerateOpCode_InXML_SpecificID(scannerID); // Generating the OpCode to get the RTA Event Status with specific scanner ID

                ExecCmd(RTA_GET_EVENT_STATUS, ref inXML, out outXML, out iStatus);
                DisplayResult(iStatus, "RTA_GET_EVENT_STATUS");


                if (iStatus == STATUS_SUCCESS)
                {
                    UpdateOutXml(outXML); //Updating the LOG
                    rtaDetailsList = m_xml.ReadRTAOpCodeXML_ExtractRTAEventStatus(outXML);
                }

                return rtaDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Registers the list of RTA events configured in the UI
        /// </summary>
        /// <param name="eventList">List of RTA events to be registered</param>
        public void SetRtaAlertStatus(List<RtaAlertStatusDetails> eventList, bool suspendStatus)
        {
            try
            {
                if (eventList != null && eventList.Count > 0)
                {

                    if (IsScannerConnected())
                    {
                        setSuspendState(suspendStatus); // Setting the suspend state

                        string outXML = String.Empty;
                        int iStatus = STATUS_FALSE;
                        string inXML = string.Empty;

                        foreach (RtaAlertStatusDetails rtaEvent in eventList) // Adding every RTA event with respective reporting state
                        {
                            inXML += "<rtaevent>" +
                                "<id>" + rtaEvent.Event + "</id>" +
                                "<stat>" + rtaEvent.Stat + "</stat>" +
                                "<reported>" + (rtaEvent.Reported ? 1 : 0) + "</reported>" +
                                "</rtaevent>";
                        }

                        inXML = GenerateOpCode_InXML(inXML); // Generating the InXML for OpCode

                        ExecCmd(RTA_SET_EVENT_STATUS, ref inXML, out outXML, out iStatus);
                        DisplayResult(iStatus, "RTA_SET_EVENT_STATUS");

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UnregisterRTAEvents(List<RtaEventDetails> rtaEvents)
        {
            try
            {
                string outXML = String.Empty;
                int iStatus = STATUS_FALSE;
                string inXML = string.Empty;

                if (rtaEvents != null && rtaEvents.Count > 0 && IsScannerConnected())
                {
                    foreach (RtaEventDetails rtaEvent in rtaEvents) // Adding every RTA event which needs to be unregistered
                    {
                        inXML += "<rtaevent>" +
                            "<id>" + rtaEvent.Event + "</id>" +
                            "<stat>" + rtaEvent.Stat + "</stat>" +
                            "</rtaevent>";
                    }

                    inXML = GenerateOpCode_InXML(inXML); // Generating the InXML for OpCode

                    ExecCmd(RTA_UNREGISTER, ref inXML, out outXML, out iStatus);

                    if (iStatus != STATUS_SUCCESS)
                    {
                        throw new Exception("Failed to unregister RTA events.");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Processes the RTA event data and display the update in the RTA UI. 
        /// </summary>
        /// <param name="rtaData"></param>
        public void UpdateRtaEvent(string rtaData)
        {
            RtaEventResponse extractedRtaData = new RtaEventResponse();
            m_xml.ReadXmlString_RtaEventResponse(rtaData, out extractedRtaData);

            RtaResponseList.Add(extractedRtaData);

            RtaEventReceived?.Invoke(this, null);
        }

        /// <summary>
        /// Retrieves the RTA event state 
        /// </summary>
        /// <returns>Returns a String of RTA State</returns>
        public string getRtaState()
        {
            try
            {
                if (!IsScannerConnected())
                {
                    return null;
                }

                string inXML = GenerateOpCode_InXML(); // Generating the InXML for OpCode
                ExecCmd(RTA_GETSTATE, ref inXML, out string outXML, out int iStatus);
                DisplayResult(iStatus, "RTA_GET_STATE");

                if (iStatus != STATUS_SUCCESS)
                {
                    return null;
                }

                UpdateOutXml(outXML);
                string rtaState = m_xml.ReadRTAOpCodeXML_ExtractRTAState(outXML); // Retrieving RTA State from the OutXML
                if (rtaState == null)
                {
                    UpdateResults("ATTR_GET - null return");
                    return string.Empty;
                }

                if (m_xml.RtaState.ContainsKey(Convert.ToInt32(rtaState)))
                {
                    return m_xml.RtaState[Convert.ToInt32(rtaState)];
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setSuspendState(bool suspend)
        {
            try
            {
                string inXML = "<inArgs>" +
                                GetOnlyScannerIDXml() +
                                "<cmdArgs>" +
                                    "<arg-bool>" + suspend + "</arg-bool>" +
                                "</cmdArgs>" +
                            "</inArgs>";

                ExecCmd(RTA_SUSPEND, ref inXML, out string outXML, out int iStatus);
                DisplayResult(iStatus, "SET_RTA_SUSPEND");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Adding OpCode RTA Events
        private string GenerateOpCode_InXML() // Using only when the ScannerID is required
        {
            string inXml = "<inArgs>" +
                            GetOnlyScannerIDXml() +
                            "</inArgs>";

            return inXml;
        }

        private string GenerateOpCode_InXML_SpecificID(string scannerID) // Using to create an inXML with specific scanner ID
        {
            string inXml = "<inArgs>" +
                           "<scannerID>" + scannerID + "</scannerID>" +
                            "</inArgs>";

            return inXml;
        }

        private string GenerateOpCode_InXML(string rtaevent_list)// Generating full InXML for RTA OpCodes
        {
            string inXml = "<inArgs>" +
            GetOnlyScannerIDXml() +
            "<cmdArgs><arg-xml><rtaevent_list>" +
            rtaevent_list +
            "</rtaevent_list></arg-xml></cmdArgs></inArgs>";

            return inXml;
        }
    }
}
