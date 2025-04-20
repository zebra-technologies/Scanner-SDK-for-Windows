using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;

namespace Scanner_SDK_Sample_Application
{
    class XmlReader
    {
        const string TAG_MAXCOUNT = "maxcount";
        const string TAG_PROGRESS = "progress";
        const string TAG_PNP = "pnp";
        const string HEX_PREFIX = "0x";
        const string SPACE = " ";
        const string RTA_RESPONSE_END = "0xff 0xff";
        const int WORD_LENGTH = 4;
        const int BYTE_LENGTH = 2; 
        const int RTA_EVENT_INFO_LENGTH = 24;
        const int RTA_ALERT_INFO_LENGTH = 16;

        public Dictionary<int, List<string>> RtaEventMap = new Dictionary<int, List<string>>()
        {
            {30012, new List<string>(){"Gifted Batt. Percentage","A value between 5% to 95% can be set", "A value between 5% to 95% can be set" }},
            {38004, new List<string>(){"RTA scanner out of cradle", "A value between 5 mins to 600 mins can be set", "A value between 5 mins to 600 mins can be set" }},
            {38001, new List<string>(){"RTA scanner idle", "A value between 5 mins to 600 mins can be set", "A value between 5 mins to 600 mins can be set" }},
            {38003, new List<string>(){"RTA virtual tether alert", "Value cannot be modified", "Value cannot be modified" }},
            {616, new List<string>(){"Config file", "Value cannot be modified", "Value cannot be modified" }},
        };

        public Dictionary<int, string> RtaStatType = new Dictionary<int, string>()
        {
            {1, "Count above max" },
            {2, "State any" },
            {7, "Value above max" },
            {9, "Value below min" },
            {11, "Out of cradle" },
            {12, "In cradle" },
            {13, "Alarm" },
            {14, "Fault" }
        };

        public Dictionary<int, string> RtaState = new Dictionary<int, string>()
        {
            {0, "RTA Suspended" },
            {1, "RTA Awaiting Registration" },
            {2, "RTA Awaiting Context Address" },
            {3, "RTA Fully Operational" }
        };

        public void ReadXmlString_GetScanners(string strXml, Scanner[] arScanner, int nTotal, out int nScannerCount)
        {
            nScannerCount = 0;
            if (1 > nTotal || String.IsNullOrEmpty(strXml))
            {
                return;
            }
            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                xmlRead.XmlResolver = null;
                // Skip non-significant whitespace   
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;

                string sElementName = "", sElmValue = "";
                Scanner scanr = null;
                int nIndex = 0;
                bool bScanner = false;
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElementName = xmlRead.Name;
                            if (Scanner.TAG_SCANNER == sElementName)
                            {
                                bScanner = false;
                            }

                            string strScannerType = xmlRead.GetAttribute(Scanner.TAG_SCANNER_TYPE);
                            if (xmlRead.HasAttributes && (
                                (Scanner.TAG_SCANNER_SNAPI == strScannerType) ||
                                (Scanner.TAG_SCANNER_SSI == strScannerType) ||
                                (Scanner.TAG_SCANNER_NIXMODB == strScannerType) ||
                                (Scanner.TAG_SCANNER_IBMHID == strScannerType) ||
                                (Scanner.TAG_SCANNER_OPOS == strScannerType) ||
                                (Scanner.TAG_SCANNER_IMBTT == strScannerType) ||
                                (Scanner.TAG_SCALE_IBM == strScannerType) ||
                                (Scanner.SCANNER_SSI_BT == strScannerType) ||
                                (Scanner.CAMERA_UVC == strScannerType) ||
                                (Scanner.TAG_SCANNER_HIDKB == strScannerType) ||
                                (Scanner.SCANNER_PRODUCE == strScannerType) ||
                                (Scanner.SCANNER_SSI_IP == strScannerType)))//n = xmlRead.AttributeCount;
                            {
                                if (arScanner.GetLength(0) > nIndex)
                                {
                                    bScanner = true;
                                    scanr = (Scanner)arScanner.GetValue(nIndex++);
                                    if (null != scanr)
                                    {
                                        scanr.ClearValues();
                                        nScannerCount++;
                                        scanr.SCANNERTYPE = strScannerType;
                                    }
                                }
                            }
                            break;

                        case XmlNodeType.Text:
                            if (bScanner && (null != scanr))
                            {
                                sElmValue = xmlRead.Value;
                                switch (sElementName)
                                {
                                    case Scanner.TAG_SCANNER_ID:
                                        scanr.SCANNERID = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_SERIALNUMBER:
                                        scanr.SERIALNO = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_MODELNUMBER:
                                        scanr.MODELNO = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_GUID:
                                        scanr.GUID = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_PORT:
                                        scanr.PORT = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_FW:
                                        scanr.SCANNERFIRMWARE = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_CN:
                                        scanr.SCANNERCONFIG= sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_DOM:
                                        scanr.SCANNERMNFDATE = sElmValue;
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /*
        public void ReadXmlString_AttachDetachSingle(string strXml, out Scanner scanr, out string sStatus)
        {
            scanr = null;
            sStatus = "";
            string sPnp = "";
            if ("" == strXml || null == strXml)
                return;
            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                // Skip non-significant whitespace   
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;

                string sElementName = "", sElmValue = "";
                bool bScanner = false;
                int nScannerCount = 0;//for multiple scanners as in cradle+cascaded
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElementName = xmlRead.Name;
                            if (Scanner.TAG_SCANNER_ID == sElementName)
                            {
                                nScannerCount++;
                                scanr = new Scanner();
                                bScanner = true;
                            }
                            break;
                        case XmlNodeType.Text:
                            if (bScanner && (null != scanr))
                            {
                                sElmValue = xmlRead.Value;
                                switch (sElementName)
                                {
                                    case Scanner.TAG_SCANNER_ID:
                                        scanr.SCANNERID = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_SERIALNUMBER:
                                        scanr.SERIALNO = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_MODELNUMBER:
                                        scanr.MODELNO = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_GUID:
                                        scanr.GUID = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_TYPE:
                                        scanr.SCANNERTYPE = sElmValue;
                                        break;
                                    case Scanner.TAG_STATUS:
                                        sStatus = sElmValue;
                                        break;
                                    case TAG_PNP:
                                        sPnp = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_FW:
                                        scanr.SCANNERFIRMWARE = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_DOM:
                                        scanr.SCANNERMNFDATE = sElmValue;
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
*/
        public void ReadXmlString_AttachDetachMulti(string strXml, out Scanner[] arScanr, out string sStatus)
        {
            arScanr = new Scanner[8];
            for (int index = 0; index < 5; index++)
            {
                arScanr.SetValue(null, index);
            }

            sStatus = "";
            if (String.IsNullOrEmpty(strXml))
            {
                return;
            }

            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                xmlRead.XmlResolver = null;
                // Skip non-significant whitespace   
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;

                string sElementName = "", sElmValue = "";
                bool bScanner = false;
                int nScannerCount = 0; //for multiple scanners as in cradle+cascaded
                int nIndex = 0;
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElementName = xmlRead.Name;
                            string strScannerType = xmlRead.GetAttribute(Scanner.TAG_SCANNER_TYPE);
                            if (xmlRead.HasAttributes && (
                                (Scanner.TAG_SCANNER_SNAPI == strScannerType) ||
                                (Scanner.TAG_SCANNER_SSI == strScannerType) ||
                                (Scanner.TAG_SCANNER_IBMHID == strScannerType) ||
                                (Scanner.TAG_SCANNER_OPOS == strScannerType) ||
                                (Scanner.TAG_SCANNER_IMBTT == strScannerType) ||
                                (Scanner.TAG_SCALE_IBM == strScannerType) ||
                                (Scanner.SCANNER_SSI_BT == strScannerType) ||
                                (Scanner.CAMERA_UVC == strScannerType) ||
                                (Scanner.TAG_SCANNER_HIDKB == strScannerType) ||
                                (Scanner.SCANNER_SSI_IP == strScannerType)))//n = xmlRead.AttributeCount;
                            {
                                nIndex = nScannerCount;
                                arScanr.SetValue(new Scanner(), nIndex);
                                nScannerCount++;
                                arScanr[nIndex].SCANNERTYPE = strScannerType;
                            }
                            if ((null != arScanr[nIndex]) && Scanner.TAG_SCANNER_ID == sElementName)
                            {
                                bScanner = true;
                            }
                            break;

                        case XmlNodeType.Text:
                            if (bScanner && (null != arScanr[nIndex]))
                            {
                                sElmValue = xmlRead.Value;
                                switch (sElementName)
                                {
                                    case Scanner.TAG_SCANNER_ID:
                                        arScanr[nIndex].SCANNERID = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_SERIALNUMBER:
                                        arScanr[nIndex].SERIALNO = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_MODELNUMBER:
                                        arScanr[nIndex].MODELNO = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_GUID:
                                        arScanr[nIndex].GUID = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_TYPE:
                                        arScanr[nIndex].SCANNERTYPE = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_FW:
                                        arScanr[nIndex].SCANNERFIRMWARE = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_CN:
                                        arScanr[nIndex].SCANNERCONFIG= sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_DOM:
                                        arScanr[nIndex].SCANNERMNFDATE = sElmValue;
                                        break;
                                    case Scanner.TAG_STATUS:
                                        sStatus = sElmValue;
                                        break;
                                    case TAG_PNP:
                                        if ("0" == sElmValue)
                                        {
                                            arScanr[nIndex] = null;
                                            nScannerCount--;
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void GetAttributePos(string strNumber, Scanner scanr, out int nIndex)
        {
            nIndex = -1;
            for (int index = 0; index < Scanner.MAX_ATTRIBUTE_COUNT; index++)
            {
                string strInNumber = scanr.m_arAttributes.GetValue(index, Scanner.POS_ATTR_ID).ToString();
                if (strNumber == strInNumber)
                {
                    nIndex = index;
                    break;
                }
            }
        }

        public void ReadXmlString_RsmAttrGet(string strXml, Scanner[] arScanner, out Scanner scanr, out int nIndex, out int nAttrCount, out int nOpCode)
        {
            nIndex = -1;
            nAttrCount = 0;
            scanr = null;
            nOpCode = -1;
            if (String.IsNullOrEmpty(strXml))
            {
                return;
            }

            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                xmlRead.XmlResolver = null;
                // Skip non-significant whitespace   
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;

                string sElementName = "", sElmValue = "";
                bool bValid = false, bFirst = false;
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElementName = xmlRead.Name;
                            if (Scanner.TAG_SCANNER_ID == sElementName)
                            {
                                bValid = true;
                                bFirst = true;
                            }
                            // for old att_getall.xml ....since name is not used(user can refer data-dictionary)  
                            else if (bValid && Scanner.TAG_ATTRIBUTE == sElementName && xmlRead.HasAttributes && (1 == xmlRead.AttributeCount))
                            {
                                sElmValue = xmlRead.GetAttribute("name");
                                if (null != scanr)
                                {
                                    scanr.m_arAttributes.SetValue(sElmValue, nAttrCount, Scanner.POS_ATTR_NAME);
                                }
                            }
                            break;

                        case XmlNodeType.Text:
                            if (bValid)
                            {
                                sElmValue = xmlRead.Value;
                                if (bFirst && Scanner.TAG_SCANNER_ID == sElementName)
                                {
                                    bFirst = false;
                                    foreach (Scanner scanrTmp in arScanner)
                                    {
                                        if ((null != scanrTmp) &&
                                             (sElmValue == scanrTmp.SCANNERID))
                                        {
                                            scanr = scanrTmp;
                                            break;
                                        }
                                    }
                                }
                                else if (null != scanr)
                                {
                                    switch (sElementName)
                                    {
                                        case Scanner.TAG_OPCODE:
                                            nOpCode = Int32.Parse(sElmValue);
                                            if (!(frmScannerApp.RSM_ATTR_GET == nOpCode))
                                            {
                                                return;
                                            }
                                            break;

                                        case Scanner.TAG_ATTRIBUTE:
                                            if (frmScannerApp.RSM_ATTR_GETALL == nOpCode)
                                            {
                                                //scanr.m_arAttributes.SetValue(sElmValue, nAttrCount, Scanner.POS_ATTR_ID);
                                                //scanr.rsmAttribute.ID = sElmValue;
                                                nAttrCount++;
                                            }
                                            break;

                                        case Scanner.TAG_ATTR_ID:
                                            nIndex = -1;
                                            //GetAttributePos(sElmValue, scanr, out nIndex);
                                            scanr.m_rsmAttribute.ID = sElmValue;
                                            break;

                                        case Scanner.TAG_ATTR_NAME:
                                            //if (-1 != nIndex)
                                            //{
                                            //    scanr.m_arAttributes.SetValue(sElmValue, nIndex, Scanner.POS_ATTR_NAME);
                                            //}
                                            scanr.m_rsmAttribute.name = sElmValue;
                                            break;

                                        case Scanner.TAG_ATTR_TYPE:
                                            //if (-1 != nIndex)
                                            //{
                                            //    scanr.m_arAttributes.SetValue(sElmValue, nIndex, Scanner.POS_ATTR_TYPE);
                                            //}
                                            scanr.m_rsmAttribute.Type = sElmValue;
                                            break;

                                        case Scanner.TAG_ATTR_PROPERTY:
                                           // if (-1 != nIndex)
                                           // {
                                            //    scanr.m_arAttributes.SetValue(sElmValue, nIndex, Scanner.POS_ATTR_PROPERTY);
                                           // }
                                            scanr.m_rsmAttribute.property = sElmValue;
                                            break;

                                        case Scanner.TAG_ATTR_VALUE:
                                            //if (-1 != nIndex)
                                            //{
                                            //    scanr.m_arAttributes.SetValue(sElmValue, nIndex, Scanner.POS_ATTR_VALUE);
                                            //}
                                            scanr.m_rsmAttribute.value = sElmValue;
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        public void ReadXmlString_RsmAttr(string strXml, Scanner[] arScanner, out Scanner scanr, out int nIndex, out int nAttrCount, out int nOpCode)
        {
            nIndex = -1;
            nAttrCount = 0;
            scanr = null;
            nOpCode = -1;
            if (String.IsNullOrEmpty(strXml))
            {
                return;
            }

            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                xmlRead.XmlResolver = null;
                // Skip non-significant whitespace   
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;

                string sElementName = "", sElmValue = "";
                bool bValid = false, bFirst = false;
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElementName = xmlRead.Name;
                            if (Scanner.TAG_SCANNER_ID == sElementName)
                            {
                                bValid = true;
                                bFirst = true;
                            }
                            // for old att_getall.xml ....since name is not used(user can refer data-dictionary)  
                            else if (bValid && Scanner.TAG_ATTRIBUTE == sElementName && xmlRead.HasAttributes && (1 == xmlRead.AttributeCount))
                            {
                                sElmValue = xmlRead.GetAttribute("name");
                                if (null != scanr)
                                {
                                    scanr.m_arAttributes.SetValue(sElmValue, nAttrCount, Scanner.POS_ATTR_NAME);
                                }
                            }
                            break;

                        case XmlNodeType.Text:
                            if (bValid)
                            {
                                sElmValue = xmlRead.Value;
                                if (bFirst && Scanner.TAG_SCANNER_ID == sElementName)
                                {
                                    bFirst = false;
                                    foreach (Scanner scanrTmp in arScanner)
                                    {
                                        if ((null != scanrTmp) &&
                                             (sElmValue == scanrTmp.SCANNERID))
                                        {
                                            scanr = scanrTmp;
                                            break;
                                        }
                                    }
                                }
                                else if (null != scanr)
                                {
                                    switch (sElementName)
                                    {
                                        case Scanner.TAG_OPCODE:
                                            nOpCode = Int32.Parse(sElmValue);
                                            if (!(frmScannerApp.RSM_ATTR_GETALL == nOpCode ||
                                                    frmScannerApp.RSM_ATTR_GET == nOpCode ||
                                                    frmScannerApp.RSM_ATTR_GETNEXT == nOpCode))
                                            {
                                                return;
                                            }
                                            break;

                                        case Scanner.TAG_ATTRIBUTE:
                                            if (frmScannerApp.RSM_ATTR_GETALL == nOpCode)
                                            {
                                                scanr.m_arAttributes.SetValue(sElmValue, nAttrCount, Scanner.POS_ATTR_ID);
                                                nAttrCount++;
                                            }
                                            break;

                                        case Scanner.TAG_ATTR_ID:
                                            nIndex = -1;
                                            GetAttributePos(sElmValue, scanr, out nIndex);
                                            break;

                                        case Scanner.TAG_ATTR_NAME:
                                            if (-1 != nIndex)
                                            {
                                                scanr.m_arAttributes.SetValue(sElmValue, nIndex, Scanner.POS_ATTR_NAME);
                                            }
                                            break;

                                        case Scanner.TAG_ATTR_TYPE:
                                            if (-1 != nIndex)
                                            {
                                                scanr.m_arAttributes.SetValue(sElmValue, nIndex, Scanner.POS_ATTR_TYPE);
                                            }
                                            break;

                                        case Scanner.TAG_ATTR_PROPERTY:
                                            if (-1 != nIndex)
                                            {
                                                scanr.m_arAttributes.SetValue(sElmValue, nIndex, Scanner.POS_ATTR_PROPERTY);
                                            }
                                            break;

                                        case Scanner.TAG_ATTR_VALUE:
                                            if (-1 != nIndex)
                                            {
                                                scanr.m_arAttributes.SetValue(sElmValue, nIndex, Scanner.POS_ATTR_VALUE);
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        public void clear_scanner_attributes(Scanner scanr)
        {
            try
            {
                int length = scanr.m_arAttributes.Length / 5;
                for (int index = 0; index < length; index++)
                {
                    scanr.m_arAttributes.SetValue(null, index, Scanner.POS_ATTR_NAME);
                    scanr.m_arAttributes.SetValue(null, index, Scanner.POS_ATTR_ID);
                    scanr.m_arAttributes.SetValue(null, index, Scanner.POS_ATTR_TYPE);
                    scanr.m_arAttributes.SetValue(null, index, Scanner.POS_ATTR_PROPERTY);
                    scanr.m_arAttributes.SetValue(null, index, Scanner.POS_ATTR_VALUE);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void ReadXmlString_FW(string strXml, out int nMax, out int nProgress, out string sStatus, out string csScannerID)
        {
            nMax = 0;
            nProgress = 0;
            sStatus = "";
            csScannerID = "";
            if (String.IsNullOrEmpty(strXml))
            {
                return;
            }

            string csSerial = "", csModel = "", csGuid = "";
            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                xmlRead.XmlResolver = null;
                // Skip non-significant whitespace   
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;

                string sElementName = "", sElmValue = "";
                bool bScanner = false;
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElementName = xmlRead.Name;
                            if (Scanner.TAG_SCANNER_ID == sElementName)
                            {
                                bScanner = true;
                            }
                            break;

                        case XmlNodeType.Text:
                            if (bScanner)
                            {
                                sElmValue = xmlRead.Value;
                                switch (sElementName)
                                {
                                    case Scanner.TAG_SCANNER_ID:
                                        csScannerID = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_SERIALNUMBER:
                                        csSerial = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_MODELNUMBER:
                                        csModel = sElmValue;
                                        break;
                                    case Scanner.TAG_SCANNER_GUID:
                                        csGuid = sElmValue;
                                        break;
                                    case Scanner.TAG_STATUS:
                                        sStatus = sElmValue;
                                        break;
                                    case TAG_MAXCOUNT:
                                        nMax = Int32.Parse(sElmValue);
                                        break;
                                    case TAG_PROGRESS:
                                        nProgress = Int32.Parse(sElmValue);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        internal string GetAttribXMLValue(string outXml)
        {
            string ret = String.Empty;
            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(outXml));
                xmlRead.XmlResolver = null;
                xmlRead.Read();
                xmlRead.ReadToFollowing("value");
                ret = xmlRead.ReadString();
            }
            catch (Exception)
            {
            }

            return ret;
        }

        public void ReadXmlString_Scale(string strXml, out string weight, out string weightMode, out int Scalestatus)
        {

            weight = "";
            weightMode = "";
            Scalestatus = 0;
            if (String.IsNullOrEmpty(strXml))
            {
                return;
            }

            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                xmlRead.XmlResolver = null;
                // Skip non-significant whitespace   
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;

                string sElementName = "", sElmValue = "";
                bool bScanner = false;
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElementName = xmlRead.Name;
                            if (Scanner.TAG_SCANNER_ID == sElementName)
                            {
                                bScanner = true;
                            }
                            break;

                        case XmlNodeType.Text:
                            if (bScanner)
                            {
                                sElmValue = xmlRead.Value;
                                switch (sElementName)
                                {
                                    case Scanner.TAG_SCALE_WEIGHT:
                                        weight = sElmValue;
                                        break;
                                    case Scanner.TAG_SCALE_WEIGHT_MODE:
                                        weightMode = sElmValue;
                                        break;
                                    case Scanner.TAG_SCALE_STATUS:
                                        Scalestatus = Int32.Parse(sElmValue);
                                        break;
                                    //case Scanner.TAG_SCANNER_GUID:
                                    //    csGuid = sElmValue;
                                    //    break;
                                    //case Scanner.TAG_STATUS:
                                    //    sStatus = sElmValue;
                                    //    break;
                                    //case TAG_MAXCOUNT:
                                    //    nMax = Int32.Parse(sElmValue);
                                    //    break;
                                    //case TAG_PROGRESS:
                                    //    nProgress = Int32.Parse(sElmValue);
                                    //    break;
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public void ReadXmlString_Snapi(string strXml, out int value)
        {

            value = -1;
            if (String.IsNullOrEmpty(strXml))
            {
                return;
            }

            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                xmlRead.XmlResolver = null;
                // Skip non-significant whitespace   
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;

                string sElementName = "", sElmValue = "";
                bool bScanner = false;
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            sElementName = xmlRead.Name;
                            if (Scanner.TAG_SCANNER_ID == sElementName)
                            {
                                bScanner = true;
                            }
                            break;

                        case XmlNodeType.Text:
                            if (bScanner)
                            {
                                sElmValue = xmlRead.Value;
                                switch (sElementName)
                                {
                                    case Scanner.TAG_SNAPI_PARAM_VAL:
                                        value = int.Parse(sElmValue);
                                        break;
                                    //case Scanner.TAG_SCALE_WEIGHT_MODE:
                                    //    weightMode = sElmValue;
                                    //    break;
                                    //case Scanner.TAG_SCALE_STATUS:
                                    //    Scalestatus = Int32.Parse(sElmValue);
                                    //    break;
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void ReadXmlString_RSMIDList(string strXML, out List<KeyValuePair<int, string>> lstAttrList)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            lstAttrList = new List<KeyValuePair<int, string>>();

            try
            {
                xmlDoc.LoadXml(strXML);

                lstAttrList = xmlDoc.SelectNodes("/outArgs/arg-xml/response/attrib_list/attribute").Cast<XmlNode>()
                                    .Select(xn => new KeyValuePair<int, string>(Convert.ToInt32(xn.InnerText),xn.Attributes[0].Value)).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Extracts the RTA alert mode list details
        /// </summary>
        /// <param name="strXml">OutXML for configured RTA alerts</param>
        /// <param name="alertList">List of RTA alerts</param>
        public void ReadXmlString_RTAAlertStatusDetails(string strXml, out List<RtaAlertStatusDetails> alertList)
        {
            alertList = new List<RtaAlertStatusDetails>();
            try
            {
                if (!string.IsNullOrEmpty(strXml))
                {
                    string responseXml = string.Empty;

                    //Extract content to process using the RTA response end notation "0xff 0xff". 
                    if (strXml.Contains(RTA_RESPONSE_END)) responseXml = strXml.Substring(0, strXml.IndexOf(RTA_RESPONSE_END));

                    //Clean out the "0x" from the response string. 
                    if (responseXml.Contains(HEX_PREFIX)) responseXml = responseXml.Replace(HEX_PREFIX, string.Empty);
                    //Clean out spaces from the response string. 
                    if (responseXml.Contains(SPACE)) responseXml = responseXml.Replace(SPACE, string.Empty);


                    //Extract the number of RTA events supported/registered from the response. 
                    int numOfRTAEvents = Convert.ToInt32(responseXml.Substring(0, BYTE_LENGTH), 16);
                    responseXml = responseXml.Remove(0, BYTE_LENGTH);

                    //Extract the event suspend status
                    int suspendValue = Convert.ToInt32(responseXml.Substring(0, BYTE_LENGTH), 16);
                    responseXml = responseXml.Remove(0, BYTE_LENGTH);

                    if (numOfRTAEvents > 0)
                    {
                        List<string> eventDetails = new List<string>();
                        //Check if RTA data has expected length of string
                        if (responseXml.Length % RTA_ALERT_INFO_LENGTH != 0) alertList = null;
                        else
                        {
                            while (responseXml.Length > 0)
                            {
                                //Extract RTA response details into a List<string> 
                                eventDetails.Add(responseXml.Substring(0, RTA_ALERT_INFO_LENGTH));
                                responseXml = responseXml.Remove(0, RTA_ALERT_INFO_LENGTH);
                            }

                            int iterator = 0;

                            //Extract RTA details
                            foreach (string rtaItem in eventDetails)
                            {
                                iterator++;

                                RtaAlertStatusDetails rtaExtractedItem = new RtaAlertStatusDetails();
                                //Extract RTA event attribute

                                int eventAttribute = Int32.Parse(rtaItem.Substring(0, WORD_LENGTH), System.Globalization.NumberStyles.HexNumber);
                                //Exclude following event types as it's not required to expose through the test app. (This can be removed since we using the scope filtering)
                                //if ((eventAttribute.Equals(StatScaleDisplayCommError)) || (eventAttribute.Equals(RtaLostHostConnection)) || (eventAttribute.Equals(RtaFirmwareUpdate))) continue;
                                rtaExtractedItem.Event = eventAttribute.ToString();

                                //Extract RTA event stat type
                                int statType = Int32.Parse(rtaItem.Substring(4, WORD_LENGTH), System.Globalization.NumberStyles.HexNumber);
                                rtaExtractedItem.Stat = statType.ToString();

                                //Extract RTA event Scope
                                int scope = Int32.Parse(rtaItem.Substring(8, WORD_LENGTH), System.Globalization.NumberStyles.HexNumber);
                                rtaExtractedItem.Scope = scope.ToString();

                                //Extract RTA event state bits
                                int stateBits = Int32.Parse(rtaItem.Substring(12, WORD_LENGTH), System.Globalization.NumberStyles.HexNumber);

                                //Convert state bits to binary and represent in 8-bits (Pad 0 to fill into a representation of 8 bits). 
                                string binaryString = Convert.ToString(stateBits, 2).PadLeft(8, '0');
                                if (!string.IsNullOrEmpty(binaryString))
                                {
                                    rtaExtractedItem.Registered = Convert.ToBoolean(Convert.ToInt32(binaryString.Substring(binaryString.Length - 1, 1)));
                                    rtaExtractedItem.Reported = Convert.ToBoolean(Convert.ToInt32(binaryString.Substring(binaryString.Length - 2, 1)));
                                    rtaExtractedItem.Initialized = Convert.ToBoolean(Convert.ToInt32(binaryString.Substring(binaryString.Length - 3, 1)));
                                    rtaExtractedItem.Measuring = Convert.ToBoolean(Convert.ToInt32(binaryString.Substring(binaryString.Length - 4, 1)));
                                }

                                //Set RTA event number
                                rtaExtractedItem.ItemNumber = iterator;

                                //Set Suspend state
                                rtaExtractedItem.SuspendState = (suspendValue == 1) ? true : false; 

                                alertList.Add(rtaExtractedItem);
                            }
                            eventDetails.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                alertList = null;
                throw ex;
            }
        }

        /// <summary>
        /// Extracts the RTA events list details
        /// </summary>
        /// <param name="strXml">OutXML for Supported/Registered RTA</param>
        /// <param name="supportedList">List of supported RTA events</param>
        public void ReadXmlString_RTAListDetails(string strXml, out List<RtaEventDetails> supportedList)
        {
            supportedList = new List<RtaEventDetails>();
            try
            {                
                if (!string.IsNullOrEmpty(strXml))
                {
                    string responseXml = string.Empty;

                    //Extract content to process using the RTA response end notation "0xff 0xff". 
                    if (strXml.Contains(RTA_RESPONSE_END)) responseXml = strXml.Substring(0, strXml.IndexOf(RTA_RESPONSE_END));

                    //Clean out the "0x" from the response string. 
                    if (responseXml.Contains(HEX_PREFIX)) responseXml = responseXml.Replace(HEX_PREFIX, string.Empty);
                    //Clean out spaces from the response string. 
                    if (responseXml.Contains(SPACE)) responseXml = responseXml.Replace(SPACE, string.Empty);

                   
                    //Extract the number of RTA events supported/registered from the response. 
                    int numOfRTAEvents = Convert.ToInt32(responseXml.Substring(0, WORD_LENGTH), 16);
                    responseXml = responseXml.Remove(0, WORD_LENGTH);

                    if (numOfRTAEvents > 0)
                    {
                        List<string> eventDetails = new List<string>();
                        //Check if RTA data has expected length of string
                        if (responseXml.Length % RTA_EVENT_INFO_LENGTH != 0) supportedList = null;
                        else
                        {
                            while (responseXml.Length > 0)
                            {
                                //Extract RTA response details into a List<string> 
                                eventDetails.Add(responseXml.Substring(0, RTA_EVENT_INFO_LENGTH));
                                responseXml = responseXml.Remove(0, RTA_EVENT_INFO_LENGTH);
                            }

                            int iterator = 0;

                            //Extract RTA details
                            foreach (string rtaItem in eventDetails)
                            {
                                iterator++;

                                RtaEventDetails rtaExtractedItem = new RtaEventDetails();
                                //Extract RTA event attribute

                                int eventAttribute = Int32.Parse(rtaItem.Substring(0, WORD_LENGTH), System.Globalization.NumberStyles.HexNumber);

                                //Exclude following event types as it's not required to expose through the test app. (This can be removed since we using the scope filtering)
                                //if ((eventAttribute.Equals(StatScaleDisplayCommError)) || (eventAttribute.Equals(RtaLostHostConnection)) || (eventAttribute.Equals(RtaFirmwareUpdate))) continue; 
                                rtaExtractedItem.Event = eventAttribute.ToString();

                                //Extract RTA event stat type
                                int statType = Int32.Parse(rtaItem.Substring(4, WORD_LENGTH), System.Globalization.NumberStyles.HexNumber);
                                rtaExtractedItem.Stat = statType.ToString();

                                //Extract RTA event on-limit
                                int onLimit = Int32.Parse(rtaItem.Substring(8, WORD_LENGTH), System.Globalization.NumberStyles.HexNumber);
                                rtaExtractedItem.OnLimit = onLimit.ToString();

                                //Extract RTA event off-limit
                                int offLimit = Int32.Parse(rtaItem.Substring(12, WORD_LENGTH), System.Globalization.NumberStyles.HexNumber);
                                rtaExtractedItem.OffLimit = offLimit.ToString();

                                //Set RTA event number
                                rtaExtractedItem.ItemNumber = iterator;

                                supportedList.Add(rtaExtractedItem);
                            }
                            eventDetails.Clear();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                supportedList = null;
                throw ex; 
            }
        }

        /// <summary>
        /// Converts the WORD string to little endian format. 
        /// </summary>
        /// <param name="item">string</param>
        /// <returns>little-endian converted string</returns>
        private string ConvertToLittleEndian(string item)
        {
            int number = Convert.ToInt32(item, 16);
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            int i = 0; 
            foreach (byte b in bytes)
            {
                i++;
                if (i > 2) break; 
                retval += b.ToString("X2");                
            }
            return retval;
        }

        public void ReadXmlString_RtaEventResponse(string strXml, out RtaEventResponse eventData)
        { 
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            eventData = null;

            try
            {
                if (String.IsNullOrEmpty(strXml))
                {
                    return;
                }

                xmlDoc.LoadXml(strXml);
                eventData = xmlDoc.SelectNodes("/outArgs/arg-xml/rta").Cast<XmlNode>()
                                .Select(xn => new
                                {
                                    Event = xn.ChildNodes[0].InnerText,
                                    Stat = xn.ChildNodes[1].InnerText,
                                    Data1 = xn.ChildNodes[2].InnerText,
                                    Data2 = xn.ChildNodes[3].InnerText,
                                    RawData = xn.ChildNodes[4].InnerText
                                }).Select(x => new RtaEventResponse(x.Event,x.Stat,x.Data1,x.Data2, x.RawData)).FirstOrDefault();

                if (eventData != null)
                {
                    eventData.Model = xmlDoc.DocumentElement.GetElementsByTagName("modelnumber").Item(0).InnerText;
                    eventData.SerialNumber = xmlDoc.DocumentElement.GetElementsByTagName("serialnumber").Item(0).InnerText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // RTA Events OpCode outXML Read - To check whether output includes at least one RTA Event
        public bool ReadRTAOpCodeXML_ValidateEventSupport(string outXML)
        {
            if (outXML != null)
            {
                XDocument doc = XDocument.Parse(outXML);
                var rtaEvents = doc.Descendants("rtaevent");
                return rtaEvents.Any();
            }
            return false;
        }

        public bool ReadRTAOpCodeXML_GetSuspendStatus(string outXML)// Extracting RTA suspend state from the OutXML of Get RTA Events Status
        {
            if (outXML != null)
            {
                XDocument doc = XDocument.Parse(outXML);
                var rtaSuspendState = doc.Descendants("suspend").FirstOrDefault();
                return (rtaSuspendState != null && rtaSuspendState.Value == "TRUE");
            }

            return false;
        }

        public List<RtaEventDetails> ReadRTAOpCodeXML_ExtractRTAEventsList(string outXML)// Extracting RTA events from the OutXML of Get Supported RTA Event
        {
            var rtaEventDetailsList = new List<RtaEventDetails>();

            if (outXML != null)
            {
                XDocument doc = XDocument.Parse(outXML);
                var rtaEvents = doc.Descendants("rtaevent");

                foreach (var item in rtaEvents.Select((rtaEvent, index) => new { rtaEvent, index }))
                {
                    var registeredElement = item.rtaEvent.Element("registered");
                    if ((registeredElement != null && registeredElement.Value.Equals("true", StringComparison.OrdinalIgnoreCase)) || registeredElement == null)// Only add events which are registered or doesn't include the registered tag. Ignores events which contains the false value for registered tag (Unregistered events).
                    {
                        RtaEventDetails rtaExtractedItem = new RtaEventDetails
                        {
                            ItemNumber = (int)item.index + 1,
                            Event = (string)item.rtaEvent.Element("id"),
                            Stat = (string)item.rtaEvent.Element("stat"),
                            OnLimit = (string)item.rtaEvent.Element("onlimit") ?? "Not applicable",
                            OffLimit = (string)item.rtaEvent.Element("offlimit") ?? "Not applicable"
                        };


                        rtaEventDetailsList.Add(rtaExtractedItem);
                    }
                }
            }
            
            return rtaEventDetailsList;
        }

        public List<RtaAlertStatusDetails> ReadRTAOpCodeXML_ExtractRTAEventStatus(string outXML)// Extracting RTA events from the OutXML of Get RTA Event Status
        {
            var rtaEventDetailsList = new List<RtaAlertStatusDetails>();

            if (outXML != null)
            {
                XDocument doc = XDocument.Parse(outXML);
                var rtaEvents = doc.Descendants("rtaevent");

                foreach (var item in rtaEvents.Select((rtaEvent, index) => new { rtaEvent, index }))
                {

                    RtaAlertStatusDetails rtaExtractedItem = new RtaAlertStatusDetails
                    {
                        ItemNumber = (int)item.index,
                        Event = (string)item.rtaEvent.Element("id"),
                        Stat = (string)item.rtaEvent.Element("stat"),
                        Registered = (bool)item.rtaEvent.Element("registered"),
                        Reported = (bool)item.rtaEvent.Element("reported"),
                        Initialized = (bool)item.rtaEvent.Element("initialized"),
                        Measuring = (bool)item.rtaEvent.Element("measuring"),
                        Scope = (string)item.rtaEvent.Element("scope")
                    };


                    rtaEventDetailsList.Add(rtaExtractedItem);

                }
            }
            return rtaEventDetailsList;
        }

        public string ReadRTAOpCodeXML_ExtractRTAState(string outXML) // Extracting RTA State from the OutXML of Get RTA State
        {
            if (outXML != null)
            {
                XDocument doc = XDocument.Parse(outXML);
                var rtaEvent = doc.Descendants("state").FirstOrDefault();

                return rtaEvent?.Value;
            }

            return null;
        }


        public void ReadXmlString_RSMIDProperty(string strXML, out List<KeyValuePair<int, string[]>> lstProperty)
        {
            lstProperty = new List<KeyValuePair<int, string[]>>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            if (String.IsNullOrEmpty(strXML))
            {
                return;
            }
            try
            {
                xmlDoc.LoadXml(strXML);

                 lstProperty = xmlDoc.SelectNodes("/outArgs/arg-xml/response/attrib_list/attribute").Cast<XmlNode>()
                                .Select(xn => new
                                {
                                  iID = Convert.ToInt32(xn.ChildNodes[0].InnerText),
                                  sDataType = xn.ChildNodes[2].InnerText,
                                  sPermission = xn.ChildNodes[3].InnerText,
                                  sValue = xn.ChildNodes[4].InnerText
                                 }).Select(x => new KeyValuePair<int, string[]>(x.iID, new string[] { x.sDataType, x.sPermission, x.sValue })).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public string IndentXmlString(string strXml)
        {
            string outXml = string.Empty;
            MemoryStream ms = new MemoryStream();
            // Create a XMLTextWriter that will send its output to a memory stream (file)
            XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode);
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;

            try
            {
                // Load the unformatted XML text string into an instance
                // of the XML Document Object Model (DOM)
                doc.LoadXml(strXml);

                // Set the formatting property of the XML Text Writer to indented
                // the text writer is where the indenting will be performed
                xtw.Formatting = Formatting.Indented;

                // write dom xml to the xmltextwriter
                doc.WriteContentTo(xtw);
                // Flush the contents of the text writer
                // to the memory stream, which is simply a memory file
                xtw.Flush();

                // set to start of the memory stream (file)
                ms.Seek(0, SeekOrigin.Begin);
                // create a reader to read the contents of
                // the memory stream (file)
                StreamReader sr = new StreamReader(ms);
                // return the formatted string to caller
                return sr.ReadToEnd();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}