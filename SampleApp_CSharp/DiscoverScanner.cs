using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreScanner;
using STC;

namespace Scanner_SDK_Sample_Application
{
    class DiscoverScanner
    {
        CCoreScannerClass coreScanner;
        // Scanner types
        public const short SCANNER_TYPES_ALL = 1;
        public const short SCANNER_TYPES_SNAPI = 2;
        public const short SCANNER_TYPES_SSI = 3;
        public const short SCANNER_TYPES_RSM = 4;
        public const short SCANNER_TYPES_IMAGING = 5;
        public const short SCANNER_TYPES_IBMHID = 6;
        public const short SCANNER_TYPES_NIXMODB = 7;
        public const short SCANNER_TYPES_HIDKB = 8;
        public const short SCANNER_TYPES_IBMTT = 9;
        public const short SCALE_TYPES_IBM = 10;
        public const short SCALE_TYPES_SSI_BT = 11;
        public const short CAMERA_TYPES_UVC = 14;

        /// <summary>
        /// Used to Assign the Index for the ScannerType
        /// </summary>
        public enum ScannerType
        {
            SNAPI = 0,
            SSI = 1,
            USBOPOS = 2,
            USBIBMHID = 2,
            NIXMODB = 3,
            USBHIDKB = 4,
            USBIBMTT =5,
            USBIBMSCALE=6,
            SSI_BT=7,
            UVC_CAMERA = 8,
            SSI_IP =9,
    }

        // Total number of scanner types
        public const short TOTAL_SCANNER_TYPES = CAMERA_TYPES_UVC;
        const int MAX_NUM_DEVICES = 255; /* Maximum number of scanners to be connected*/
        const int CLAIM_DEVICE = 1500;

        private static DiscoverScanner _discoverScanner;

        public DiscoverScanner(CCoreScannerClass m_pCoreScanner)
        {
            coreScanner = m_pCoreScanner;
        }

        public static DiscoverScanner GetInstance(CCoreScannerClass m_pCoreScanner)
        {
            if (_discoverScanner == null)
                _discoverScanner = new DiscoverScanner(m_pCoreScanner);
            return _discoverScanner;

        }

        /// <summary>
        /// Used to get Scanner by using CoreScanner
        /// </summary>
        /// <param name="numberOfScanners">0</param>
        /// <param name="outXML">Xml format</param>
        /// <param name="status">Status</param>
        /// <param name="claimlist">ClaimList</param>
        /// <returns></returns>
        public Scanner[] GetScanners(ref short numberOfScanners, ref string outXML, ref int status, List<string> claimlist)
        {
            XmlReader xml = new XmlReader();
            Scanner[] arScanners = new Scanner[MAX_NUM_DEVICES];
            for (int i = 0; i < MAX_NUM_DEVICES; i++)
            {
                Scanner scanr = new Scanner();
                arScanners.SetValue(scanr, i);
            }
            arScanners.Initialize();
            int[] scannerIdList = new int[MAX_NUM_DEVICES];
            int nScannerCount = 0;
            try
            {
                coreScanner.GetScanners(out numberOfScanners, scannerIdList, out outXML, out status);
                if (Constants.StatusSuccess == status)
                {
                    xml.ReadXmlString_GetScanners(outXML, arScanners, numberOfScanners, out nScannerCount);
                    
                        for (int index = 0; index < arScanners.Length && claimlist.Count > 0; index++) // Noticed looping 255 times even the claim list = 0
                        {
                            for (int i = 0; i < claimlist.Count; i++)
                            {
                                if (string.Compare(claimlist[i], arScanners[index].SERIALNO) == 0)
                                {
                                    Scanner objScanner = (Scanner)arScanners.GetValue(index);
                                    objScanner.CLAIMED = true;
                                }
                            }
                        }
                    
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return arScanners;
        }

        /// <summary>
        /// Used to Claim Device based on the ScannerID
        /// </summary>
        /// <param name="arScanners">GetScanner()</param>
        /// <param name="numOfScanners">ScannerCount</param>
        /// <param name="claimlist">ClaimList</param>
        /// <param name="async">ChkAsync value</param>
        public void ClaimDevice(Scanner[] arScanners, short numOfScanners, List<string> claimlist, bool async)
        {
            int status = Constants.StatusFail;
            string outXML = "";
            try
            {
                for (int index = 0; index < numOfScanners; index++)
                {
                    Scanner objScanner = (Scanner)arScanners.GetValue(index);
                    string inXml = "<inArgs><scannerID>" + objScanner.SCANNERID + "</scannerID></inArgs>";

                    for (int i = 0; i < claimlist.Count; i++)
                    {
                        if (string.Compare(claimlist[i], objScanner.SERIALNO) == 0)
                        {
                            if (async)
                                coreScanner.ExecCommandAsync(CLAIM_DEVICE, inXml, out status);
                            else
                                coreScanner.ExecCommand(CLAIM_DEVICE, inXml, out outXML, out status);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Used to Filter Scanner List
        /// </summary>
        /// <param name="m_arSelectedTypes">Selected Scanner type</param>
        /// <param name="indexFilter">Filter Scanner Index</param>

        public void FilterScannerList(ref bool[] m_arSelectedTypes, int indexFilter)
        {
            switch (indexFilter)
            {
                case 0:
                    m_arSelectedTypes[SCANNER_TYPES_ALL - 1] = true;
                    break;

                case 1:
                    m_arSelectedTypes[SCANNER_TYPES_HIDKB - 1] = true;
                    break;

                case 2:
                    m_arSelectedTypes[SCANNER_TYPES_IBMHID - 1] = true;
                    break;

                case 3:
                    m_arSelectedTypes[SCANNER_TYPES_SNAPI - 1] = true;
                    break;
            }
        }

        /// <summary>
        /// Get Scanner Item based on ScannerID
        /// </summary>
        /// <param name="objScanner">object Scanner</param>
        /// <param name="strItems">SCanner Item</param>
        /// <returns></returns>
        public string[] GetScannerItems(Scanner objScanner, string[] strItems)
        {
            switch (objScanner.SCANNERTYPE)
            {
                case Scanner.SCANNER_IBMHID:
                    strItems = new string[] { objScanner.SCANNERID, "IBM HANDHELD", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERCONFIG, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                    break;

                case Scanner.SCANNER_OPOS:
                    strItems = new string[] { objScanner.SCANNERID, "USB OPOS", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERCONFIG, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                    break;

                case Scanner.SCANNER_HIDKB:
                    strItems = new string[] { objScanner.SCANNERID, "HID KEYBOARD", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERCONFIG, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                    break;

                case Scanner.SCANNER_IBMTT:
                    strItems = new string[] { objScanner.SCANNERID, "IBM TABLETOP", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERCONFIG, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                    break;

                case Scanner.SCALE_IBM:
                    strItems = new string[] { objScanner.SCANNERID, "IBM SCALE", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERCONFIG, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                    break;

                case Scanner.SCANNER_SNAPI:
                case Scanner.SCANNER_SSI:
                case Scanner.SCANNER_NIXMODB:
                case Scanner.SCANNER_SSI_BT:
                case Scanner.CAMERA_UVC:
                case Scanner.SCANNER_SSI_IP:
                    strItems = new string[] { objScanner.SCANNERID, objScanner.SCANNERTYPE, objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERCONFIG, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                    break;

            }
            return strItems;
        }

        /// <summary>
        /// Used to Incease Scanner Count
        /// </summary>
        /// <param name="scannerType">Scanner type</param>
        /// <param name="m_nArTotalScannersInType">ref m_nArTotalScannersInType</param>

        public void IncreaseScannerCount(string scannerType, ref int[] m_nArTotalScannersInType)
        {
            ScannerType scannerTypeValue;
            Enum.TryParse(scannerType, out scannerTypeValue);
            int index = Convert.ToInt32(scannerTypeValue);
            m_nArTotalScannersInType[index]++;
        }

    }
}
