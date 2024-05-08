using CoreScanner; //Add this reference from COM library
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace STC
{
    public static class Constants
    {
        public const int StatusFail = 1;
        public const int StatusSuccess = 0;
        public const int StatusLocked = 10;
        public const int ErrorCDCScannersNotFound = 150;
        public const int ErrorUnableToOpenCDCComPort = 151;

        public const int RegisterForEvents = 1001;
        public const int GetPairingBarcode = 1005;

        public const int NumScannerEvents = 6;
        public const int SubscribeBarcode = 1;
        public const int SubscribeImage = 2;
        public const int SubscribeVideo = 4;
        public const int SubscribeRMD = 8;
        public const int SubscribePNP = 16;
        public const int SubscribeOther = 32;

        public const string AppTitle = "Scanner to Connect";

        public const short ScannerTypesAll = 1;
        public const short ScannerTypesSNAPI = 2;
        public const short ScannerTypesSSI = 3;
        public const short ScannerTypesRSM = 4;
        public const short ScannerTypesIMAGING = 5;
        public const short ScannerTypesIBMHID = 6;
        public const short ScannerTypesNIXMODB = 7;
        public const short ScannerTypesHIDKB = 8;
        public const short ScannerTypesIBMTT = 9;
        public const short ScaleTypesIBM = 10;
        public const short ScaleTypesSSI_BT = 11;
        public const short CameraTypesUVC = 14;
        public const short TotalScannerTypes = CameraTypesUVC;

        public enum DefaultOption
        {
            [Description("No Defaults")]
            NoDefaults = 0,
            [Description("Set Factory Defaults")]
            SetFactoryDefaults = 1,
            [Description("Restore Factory Defaults")]
            RestoreFactoryDefaults = 2
        }

        public enum ImageSize
        {
            [Description("Small")]
            Small = 1,
            [Description("Medium")]
            Medium = 2,
            [Description("Large")]
            Large = 3
        }
        public enum ScannerType
        {
            [Description("Legacy")]
            Legacy = 0,
            [Description("New")]
            New = 1
        }
        public enum ProtocolName
        {
            [Description("Simple Serial Interface (SSI)")]
            SSI = 1,
            [Description("Serial Port Profile(SPP)")]
            SPP = 14,
            [Description("Human Interface Device(HID)")]
            HID = 17
        }

        public enum HostName
        {
            [Description("SSI BT Classic (Non-Discoverable)")]
            SSIBTClassic = 22
        }
    }
    public class ScanToConnect
    {
        //Resolve error: refernce -> CoreScanner -> properties: Embed Interop types should be false.
        public short NoOfTypes { get; }
        private short[] scannerTypes;
        private bool[] selectedTypes;
        static ScanToConnect _scanToConnect;

        public ScanToConnect()
        {
            scannerTypes = new short[Constants.TotalScannerTypes];
            selectedTypes = new bool[Constants.TotalScannerTypes];
        }

        public static ScanToConnect GetInstance()
        {
            if(_scanToConnect == null)            
                _scanToConnect = new ScanToConnect();

            return _scanToConnect;
            
        }
        /// <summary>
        /// Getter method for scanner types
        /// </summary>
        /// <returns>Array of scanners types</returns>
        public short[] GetScannerTypes()
        {
            return scannerTypes;
        }

        /// <summary>
        /// Getter method for selected types
        /// </summary>
        /// <returns>Array of selected types</returns>
        public bool[] GetSelectedTypes()
        {
            for (int index = 0; index < Constants.TotalScannerTypes; index++)
            {
                selectedTypes[index] = false;
            }
            return selectedTypes;
        }

        ///// <summary>
        ///// Intialize the scanner type from constants.
        ///// </summary>
        //private void FilterScannerList()
        //{
            
        //}
        
        /// <summary>
        /// Generate the XML based on parameters
        /// </summary>
        /// <param name="NoOfParameters">Number of parameters</param>
        /// <param name="Parameters">Parameters</param>
        /// <returns>Generated InXml</returns>
        public string GenerateInitXML(int NoOfParameters, string Parameters)
        {
            return "<inArgs>"
                     + " <cmdArgs>"
                        + "<arg-int>" + NoOfParameters + "</arg-int>" //number of parameters
                        + "<arg-int>" + Parameters + "</arg-int>"
                      + " </cmdArgs>"
                  + "</inArgs>";

        }

        /// <summary>
        /// Get the resgiter/unregisterIDs
        /// </summary>
        /// <param name="nEvents">Number of scanner events</param>
        /// <returns>IDs</returns>
        public string GetRegUnRegisterIDs(out int nEvents)
        {
            string strIDs = "";
            nEvents = Constants.NumScannerEvents;
            strIDs = Constants.SubscribeBarcode.ToString();
            strIDs += "," + Constants.SubscribeImage.ToString();
            strIDs += "," + Constants.SubscribeVideo.ToString();
            strIDs += "," + Constants.SubscribeRMD.ToString();
            strIDs += "," + Constants.SubscribePNP.ToString();
            strIDs += "," + Constants.SubscribeOther.ToString();
            return strIDs;
        }


        /// <summary>
        /// Get scanner types and its values.
        /// </summary>
        /// <returns>Number of total scanner types.</returns>
        public short GetSelectedScannerTypes()
        {
            short _numberOfTypes = 0;
            for (int index = 0, k = 0; index < Constants.TotalScannerTypes; index++)
            {
                if (selectedTypes[index])
                {
                    _numberOfTypes++;
                    switch (index + 1)
                    {
                        case Constants.ScannerTypesAll:
                            scannerTypes[k++] = Constants.ScannerTypesAll;
                            return _numberOfTypes;

                        case Constants.ScannerTypesSNAPI:
                            scannerTypes[k++] = Constants.ScannerTypesSNAPI;
                            break;

                        case Constants.ScannerTypesSSI:
                            scannerTypes[k++] = Constants.ScannerTypesSNAPI;
                            break;

                        case Constants.ScannerTypesNIXMODB:
                            scannerTypes[k++] = Constants.ScannerTypesNIXMODB;
                            break;

                        case Constants.ScannerTypesRSM:
                            scannerTypes[k++] = Constants.ScannerTypesRSM;
                            break;

                        case Constants.ScannerTypesIMAGING:
                            scannerTypes[k++] = Constants.ScannerTypesIMAGING;
                            break;

                        case Constants.ScannerTypesIBMHID:
                            scannerTypes[k++] = Constants.ScannerTypesIBMHID;
                            break;

                        case Constants.ScannerTypesHIDKB:
                            scannerTypes[k++] = Constants.ScannerTypesHIDKB;
                            break;

                        case Constants.ScaleTypesSSI_BT:
                            scannerTypes[k++] = Constants.ScaleTypesSSI_BT;
                            break;

                        default:
                            break;
                    }
                }
            }
            return _numberOfTypes;
        }   
        
    }   
}
