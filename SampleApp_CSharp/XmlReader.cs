using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace Scanner_SDK_Sample_Application
{
    class XmlReader
    {
        const string TAG_MAXCOUNT = "maxcount";
        const string TAG_PROGRESS = "progress";
        const string TAG_PNP = "pnp";

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

        public void ReadXmlString_RSMIDProperty(string strXML, out List<KeyValuePair<int, string[]>> lstProperty)
        {
            lstProperty = new List<KeyValuePair<int, string[]>>();
            XmlDocument xmlDoc = new XmlDocument();
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