using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    /// <summary>
    /// Business Logic separation from Action tab UI part
    /// </summary>
    class ScannerAction
    {
        private const int LED_1_ON = 43; /* Green  LED On */
        private const int LED_2_ON = 45; /* Yellow  LED On */
        private const int LED_3_ON = 47; /* Red  LED On */

        private const int LED_1_OFF = 42; /* Green  LED Off */
        private const int LED_2_OFF = 46; /* Yellow  LED Off */
        private const int LED_3_OFF = 48; /* Red  LED Off */

        const int PAGER_MOTOR_ACTION = 6033;

        private static ScannerAction _scannerAction;
        public ScannerAction()
        {
            CreateHostModeInstance();

        }
        public static ScannerAction GetInstance()
        {
            if (_scannerAction == null)
            {
                _scannerAction = new ScannerAction();
            }
            return _scannerAction;
        }


        private Dictionary<string, string> HostModes;

        /// <summary>
        /// Hostmode having the combo values
        /// </summary>

        private void CreateHostModeInstance()
        {
            HostModes = new Dictionary<string, string>();
            HostModes.Add("USB-HIDKB", "XUA-45001-3");
            HostModes.Add("USB-IBMHID", "XUA-45001-1");
            HostModes.Add("USB-OPOS", "XUA-45001-8");
            HostModes.Add("USB-SNAPI with Imaging", "XUA-45001-9");
            HostModes.Add("USB-SNAPI without Imaging", "XUA-45001-10");
            HostModes.Add("USB-CDC Serial Emulation", "XUA-45001-11");
            HostModes.Add("USB-SSI over CDC", "XUA-45001-14");
            HostModes.Add("USB-IBMTT", "XUA-45001-2");
        }

        /// <summary>
        /// Assigning the combo values to the corresponding combo based on sorting
        /// </summary>
        /// <param name="sort">True/False</param>
        /// <returns>Hostmode values</returns>

        public List<KeyValuePair<string, string>> GetHostModes(bool sort = false)
        {
            if (sort)
            {
                return HostModes.OrderBy(s => Convert.ToInt32(s.Value.Split('-')[2])).ToList();
            }
            return HostModes.ToList();
        }

        /// <summary>
        /// Generate the LED Xml Format
        /// </summary>
        /// <param name="inXml">GetScanneronlyxml()</param>
        /// <param name="isOn">True/false</param>
        /// <param name="LedIndex">Combo value</param>
        /// <returns></returns>

        public string GetLedIDXml(string inXml, bool isOn, int LedIndex)
        {
            int LedID;
            switch (LedIndex)
            {
                case 0:
                    LedID = isOn ? LED_1_ON : LED_1_OFF;
                    break;
                case 1:
                    LedID = isOn ? LED_2_ON : LED_2_OFF;
                    break;
                case 2:
                    LedID = isOn ? LED_3_ON : LED_3_OFF;
                    break;
                default:
                    LedID = isOn ? LED_1_ON : LED_1_OFF;
                    break;
            }

            return "<inArgs>" +
                    inXml +
                    "<cmdArgs>" +
                    "<arg-int>" + LedID.ToString()
                    + "</arg-int>" +
                    "</cmdArgs>" +
                    "</inArgs>";
        }

        /// <summary>
        /// Generate the Beep Xml format
        /// </summary>
        /// <param name="inXML">GetOnlyScannerXml()</param>
        /// <param name="BeepIndex">Beep Combo value</param>
        /// <returns></returns>

        public string GetBeepXml(string inXML, int BeepIndex)
        {
            return "<inArgs>" +
                    inXML +
                    "<cmdArgs>" +
                    "<arg-int>" + BeepIndex
                    + "</arg-int>" +
                    "</cmdArgs>" +
                    "</inArgs>";
        }
        /// <summary>
        /// Generate the PageMotor XML format
        /// </summary>
        /// <param name="inXml">GetOnlyScannerXml()</param>
        /// <param name="pagerMotorDuration">PageMotor Duration</param>
        /// <returns></returns>
        public string GetPageMotorXML(string inXml, string pagerMotorDuration)
        {
            return "<inArgs>" +
                        inXml +
                    "<cmdArgs>" +
                        "<arg-xml>" +
                            "<attrib_list>" +
                                "<attribute>" +
                                    "<id>" + PAGER_MOTOR_ACTION + "</id>" +
                                    "<datatype>" + "X" + "</datatype>" +
                                    "<value>" + pagerMotorDuration + "</value>" +
                                "</attribute>" +
                            "</attrib_list>" +
                        "</arg-xml>" +
                        "</cmdArgs>" +
                        "</inArgs>";
        }
    }
}