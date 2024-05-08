using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Scanner_SDK_Sample_Application
{
    /// <summary>
    /// Business Logic separation from Barcode tab UI part
    /// </summary>
    public class ScannerBarcode
    {
        private static ScannerBarcode _scannerBarcode;
        public static ScannerBarcode GetInstance()
        {
            if (_scannerBarcode == null)
                _scannerBarcode = new ScannerBarcode();
            return _scannerBarcode;
        }

        // Symbology types 

        public const int ST_NOT_APP = 0x00;
        public const int ST_CODE_39 = 0x01;
        public const int ST_CODABAR = 0x02;
        public const int ST_CODE_128 = 0x03;
        public const int ST_D2OF5 = 0x04;
        public const int ST_IATA = 0x05;
        public const int ST_I2OF5 = 0x06;
        public const int ST_CODE93 = 0x07;
        public const int ST_UPCA = 0x08;
        public const int ST_UPCE0 = 0x09;
        public const int ST_EAN8 = 0x0a;
        public const int ST_EAN13 = 0x0b;
        public const int ST_CODE11 = 0x0c;
        public const int ST_CODE49 = 0x0d;
        public const int ST_MSI = 0x0e;
        public const int ST_EAN128 = 0x0f;
        public const int ST_UPCE1 = 0x10;
        public const int ST_PDF417 = 0x11;
        public const int ST_CODE16K = 0x12;
        public const int ST_C39FULL = 0x13;
        public const int ST_UPCD = 0x14;
        public const int ST_TRIOPTIC = 0x15;
        public const int ST_BOOKLAND = 0x16;
        public const int ST_UPCA_W_CODE128 = 0x17; // For UPC-A w/Code 128 Supplemental
        public const int ST_JAN13_W_CODE128 = 0x78; // For EAN/JAN-13 w/Code 128 Supplemental
        public const int ST_NW7 = 0x18;
        public const int ST_ISBT128 = 0x19;
        public const int ST_MICRO_PDF = 0x1a;
        public const int ST_DATAMATRIX = 0x1b;
        public const int ST_QR_CODE = 0x1c;
        public const int ST_MICRO_PDF_CCA = 0x1d;
        public const int ST_POSTNET_US = 0x1e;
        public const int ST_PLANET_CODE = 0x1f;
        public const int ST_CODE_32 = 0x20;
        public const int ST_ISBT128_CON = 0x21;
        public const int ST_JAPAN_POSTAL = 0x22;
        public const int ST_AUS_POSTAL = 0x23;
        public const int ST_DUTCH_POSTAL = 0x24;
        public const int ST_MAXICODE = 0x25;
        public const int ST_CANADIN_POSTAL = 0x26;
        public const int ST_UK_POSTAL = 0x27;
        public const int ST_MACRO_PDF = 0x28;
        public const int ST_MACRO_QR_CODE = 0x29;
        public const int ST_MICRO_QR_CODE = 0x2c;
        public const int ST_AZTEC = 0x2d;
        public const int ST_AZTEC_RUNE = 0x2e;
        public const int ST_DISTANCE = 0x2f;
        public const int ST_GS1_DATABAR = 0x30;
        public const int ST_GS1_DATABAR_LIMITED = 0x31;
        public const int ST_GS1_DATABAR_EXPANDED = 0x32;
        public const int ST_PARAMETER = 0x33;
        public const int ST_USPS_4CB = 0x34;
        public const int ST_UPU_FICS_POSTAL = 0x35;
        public const int ST_ISSN = 0x36;
        public const int ST_SCANLET = 0x37;
        public const int ST_CUECODE = 0x38;
        public const int ST_MATRIX2OF5 = 0x39;
        public const int ST_UPCA_2 = 0x48;
        public const int ST_UPCE0_2 = 0x49;
        public const int ST_EAN8_2 = 0x4a;
        public const int ST_EAN13_2 = 0x4b;
        public const int ST_UPCE1_2 = 0x50;
        public const int ST_CCA_EAN128 = 0x51;
        public const int ST_CCA_EAN13 = 0x52;
        public const int ST_CCA_EAN8 = 0x53;
        public const int ST_CCA_RSS_EXPANDED = 0x54;
        public const int ST_CCA_RSS_LIMITED = 0x55;
        public const int ST_CCA_RSS14 = 0x56;
        public const int ST_CCA_UPCA = 0x57;
        public const int ST_CCA_UPCE = 0x58;
        public const int ST_CCC_EAN128 = 0x59;
        public const int ST_TLC39 = 0x5A;
        public const int ST_CCB_EAN128 = 0x61;
        public const int ST_CCB_EAN13 = 0x62;
        public const int ST_CCB_EAN8 = 0x63;
        public const int ST_CCB_RSS_EXPANDED = 0x64;
        public const int ST_CCB_RSS_LIMITED = 0x65;
        public const int ST_CCB_RSS14 = 0x66;
        public const int ST_CCB_UPCA = 0x67;
        public const int ST_CCB_UPCE = 0x68;
        public const int ST_SIGNATURE_CAPTURE = 0x69;
        public const int ST_MOA = 0x6A;
        public const int ST_PDF417_PARAMETER = 0x70;
        public const int ST_CHINESE2OF5 = 0x72;
        public const int ST_KOREAN_3_OF_5 = 0x73;
        public const int ST_DATAMATRIX_PARAM = 0x74;
        public const int ST_CODE_Z = 0x75;
        public const int ST_UPCA_5 = 0x88;
        public const int ST_UPCE0_5 = 0x89;
        public const int ST_EAN8_5 = 0x8a;
        public const int ST_EAN13_5 = 0x8b;
        public const int ST_UPCE1_5 = 0x90;
        public const int ST_MACRO_MICRO_PDF = 0x9A;
        public const int ST_OCRB = 0xA0;
        public const int ST_OCRA = 0xA1;
        public const int ST_PARSED_DRIVER_LICENSE = 0xB1;
        public const int ST_PARSED_UID = 0xB2;
        public const int ST_PARSED_NDC = 0xB3;
        public const int ST_DATABAR_COUPON = 0xB4;
        public const int ST_PARSED_XML = 0xB6;
        public const int ST_HAN_XIN_CODE = 0xB7;
        public const int ST_CALIBRATION = 0xC0;
        public const int ST_GS1_DATAMATRIX = 0xC1;
        public const int ST_GS1_QR = 0xC2;
        public const int BT_MAINMARK = 0xC3;
        public const int BT_DOTCODE = 0xC4;
        public const int BT_GRID_MATRIX = 0xC8;
        public const int BT_UDI_CODE = 0xCC;
        public const int ST_EPC_RAW = 0xE0;

        //End Symbology Types

        /// <summary>
        /// Barcode symbology
        /// </summary>
        /// <param name="Code">Symbology code</param>
        /// <returns>Symbology name</returns>
        public string GetSymbology(int Code)
        {
            switch (Code)
            {
                case ST_NOT_APP:
                    return "NOT APPLICABLE";
                case ST_CODE_39:
                    return "CODE 39";
                case ST_CODABAR:
                    return "CODABAR";
                case ST_CODE_128:
                    return "CODE 128";
                case ST_D2OF5:
                    return "DISCRETE 2 OF 5";
                case ST_IATA:
                    return "IATA";
                case ST_I2OF5:
                    return "INTERLEAVED 2 OF 5";
                case ST_CODE93:
                    return "CODE 93";
                case ST_UPCA:
                    return "UPC-A";
                case ST_UPCE0:
                    return "UPC-E0";
                case ST_EAN8:
                    return "EAN-8";
                case ST_EAN13:
                    return "EAN-13";
                case ST_CODE11:
                    return "CODE 11";
                case ST_CODE49:
                    return "CODE 49";
                case ST_MSI:
                    return "MSI";
                case ST_EAN128:
                    return "EAN-128";
                case ST_UPCE1:
                    return "UPC-E1";
                case ST_PDF417:
                    return "PDF-417";
                case ST_CODE16K:
                    return "CODE 16K";
                case ST_C39FULL:
                    return "CODE 39 FULL ASCII";
                case ST_UPCD:
                    return "UPC-D";
                case ST_TRIOPTIC:
                    return "CODE 39 TRIOPTIC";
                case ST_BOOKLAND:
                    return "BOOKLAND";
                case ST_UPCA_W_CODE128:
                    //return "UPC-A w/Code 128 Supplemental";
                    // 0x17 was the old style coupon code that was a specific UPC-A that could have or not have a Code 128 Supplemental. Switched to GS1 Databar.
                    return "Coupon Code";
                case ST_JAN13_W_CODE128:
                    return "EAN/JAN-13 w/Code 128 Supplemental";
                case ST_NW7:
                    return "NW-7";
                case ST_ISBT128:
                    return "ISBT-128";
                case ST_MICRO_PDF:
                    return "MICRO PDF";
                case ST_DATAMATRIX:
                    return "DATAMATRIX";
                case ST_QR_CODE:
                    return "QR CODE";
                case ST_MICRO_PDF_CCA:
                    return "MICRO PDF CCA";
                case ST_POSTNET_US:
                    return "POSTNET US";
                case ST_PLANET_CODE:
                    return "PLANET CODE";
                case ST_CODE_32:
                    return "CODE 32";
                case ST_ISBT128_CON:
                    return "ISBT-128 CON";
                case ST_JAPAN_POSTAL:
                    return "JAPAN POSTAL";
                case ST_AUS_POSTAL:
                    return "AUS POSTAL";
                case ST_DUTCH_POSTAL:
                    return "DUTCH POSTAL";
                case ST_MAXICODE:
                    return "MAXICODE";
                case ST_CANADIN_POSTAL:
                    return "CANADIAN POSTAL";
                case ST_UK_POSTAL:
                    return "UK POSTAL";
                case ST_MACRO_PDF:
                    return "MACRO PDF";
                case ST_MACRO_QR_CODE:
                    return "MACRO QR CODE";
                case ST_MICRO_QR_CODE:
                    return "MICRO QR CODE";
                case ST_AZTEC:
                    return "AZTEC";
                case ST_AZTEC_RUNE:
                    return "AZTEC RUNE";
                case ST_DISTANCE:
                    return "DISTANCE";
                case ST_GS1_DATABAR:
                    return "GS1 DATABAR";
                case ST_GS1_DATABAR_LIMITED:
                    return "GS1 DATABAR LIMITED";
                case ST_GS1_DATABAR_EXPANDED:
                    return "GS1 DATABAR EXPANDED";
                case ST_PARAMETER:
                    return "PARAMETER";
                case ST_USPS_4CB:
                    return "USPS 4CB";
                case ST_UPU_FICS_POSTAL:
                    return "UPU FICS POSTAL";
                case ST_ISSN:
                    return "ISSN";
                case ST_SCANLET:
                    return "SCANLET";
                case ST_CUECODE:
                    return "CUECODE";
                case ST_MATRIX2OF5:
                    return "MATRIX 2 OF 5";
                case ST_UPCA_2:
                    return "UPC-A + 2 SUPPLEMENTAL";
                case ST_UPCE0_2:
                    return "UPC-E0 + 2 SUPPLEMENTAL";
                case ST_EAN8_2:
                    return "EAN-8 + 2 SUPPLEMENTAL";
                case ST_EAN13_2:
                    return "EAN-13 + 2 SUPPLEMENTAL";
                case ST_UPCE1_2:
                    return "UPC-E1 + 2 SUPPLEMENTAL";
                case ST_CCA_EAN128:
                    return "CCA EAN-128";
                case ST_CCA_EAN13:
                    return "CCA EAN-13";
                case ST_CCA_EAN8:
                    return "CCA EAN-8";
                case ST_CCA_RSS_EXPANDED:
                    return "GS1 DATABAR EXPANDED COMPOSITE (CCA)";
                case ST_CCA_RSS_LIMITED:
                    return "GS1 DATABAR LIMITED COMPOSITE (CCA)";
                case ST_CCA_RSS14:
                    return "GS1 DATABAR COMPOSITE (CCA)";
                case ST_CCA_UPCA:
                    return "CCA UPC-A";
                case ST_CCA_UPCE:
                    return "CCA UPC-E";
                case ST_CCC_EAN128:
                    return "CCC EAN-128";
                case ST_TLC39:
                    return "TLC-39";
                case ST_CCB_EAN128:
                    return "CCB EAN-128";
                case ST_CCB_EAN13:
                    return "CCB EAN-13";
                case ST_CCB_EAN8:
                    return "CCB EAN-8";
                case ST_CCB_RSS_EXPANDED:
                    return "GS1 DATABAR EXPANDED COMPOSITE (CCB)";
                case ST_CCB_RSS_LIMITED:
                    return "GS1 DATABAR LIMITED COMPOSITE (CCB)";
                case ST_CCB_RSS14:
                    return "GS1 DATABAR COMPOSITE (CCB)";
                case ST_CCB_UPCA:
                    return "CCB UPC-A";
                case ST_CCB_UPCE:
                    return "CCB UPC-E";
                case ST_SIGNATURE_CAPTURE:
                    return "SIGNATURE CAPTUREE";
                case ST_MOA:
                    return "MOA";
                case ST_PDF417_PARAMETER:
                    return "PDF417 PARAMETER";
                case ST_CHINESE2OF5:
                    return "CHINESE 2 OF 5";
                case ST_KOREAN_3_OF_5:
                    return "KOREAN 3 OF 5";
                case ST_DATAMATRIX_PARAM:
                    return "DATAMATRIX PARAM";
                case ST_CODE_Z:
                    return "CODE Z";
                case ST_UPCA_5:
                    return "UPC-A + 5 SUPPLEMENTAL";
                case ST_UPCE0_5:
                    return "UPC-E0 + 5 SUPPLEMENTAL";
                case ST_EAN8_5:
                    return "EAN-8 + 5 SUPPLEMENTAL";
                case ST_EAN13_5:
                    return "EAN-13 + 5 SUPPLEMENTAL";
                case ST_UPCE1_5:
                    return "UPC-E1 + 5 SUPPLEMENTAL";
                case ST_MACRO_MICRO_PDF:
                    return "MACRO MICRO PDF";
                case ST_OCRB:
                    return "OCRB";
                case ST_OCRA:
                    return "OCRA";
                case ST_PARSED_DRIVER_LICENSE:
                    return "PARSED DRIVER LICENSE";
                case ST_PARSED_UID:
                    return "PARSED UID";
                case ST_PARSED_NDC:
                    return "PARSED NDC";
                case ST_DATABAR_COUPON:
                    return "DATABAR COUPON";
                case ST_PARSED_XML:
                    return "PARSED XML";
                case ST_HAN_XIN_CODE:
                    return "HAN XIN CODE";
                case ST_CALIBRATION:
                    return "CALIBRATION";
                case ST_GS1_DATAMATRIX:
                    return "GS1 DATA MATRIX";
                case ST_GS1_QR:
                    return "GS1 QR";
                case BT_MAINMARK:
                    return "MAIL MARK";
                case BT_DOTCODE:
                    return "DOT CODE";
                case BT_GRID_MATRIX:
                    return "GRID MATRIX";
                case ST_EPC_RAW:
                    return "EPC RAW";
                case BT_UDI_CODE:
                    return "UDI CODE";
                default:
                    return "";
            }

        }


        /// <summary>
        /// Get data label from the CoreScanner's barcode event XML
        /// </summary>
        /// <param name="scanDataXml">CoreScanner's barcode event XML</param>
        /// <returns>Content of datalabel tag</returns>
        public string GetScanDataLabel(string scanDataXml)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(scanDataXml);
                return xmlDocument.DocumentElement.GetElementsByTagName("datalabel").Item(0).InnerText;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Get the data type from CoreScanner's barcode event XML
        /// </summary>
        /// <param name="scanDataXml">CoreScanner's barcode event XML</param>
        /// <returns>Content of datatype tag</returns>
        public int GetScanDataType(string scanDataXml)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(scanDataXml);
                return (int)Convert.ToInt32(xmlDocument.DocumentElement.GetElementsByTagName("datatype").Item(0).InnerText.Trim());
            }
            catch
            {
                return ScannerBarcode.ST_NOT_APP;
            }

        }

        /// <summary>
        /// Get Barcode data for controls
        /// </summary>
        /// <param name="strXml">Barcode data XML</param>
        /// <param name="encoding">Encoding value</param>
        /// <param name="symbology">Barcode type</param>
        /// <returns></returns>
        public string GetBarcodelabel(string strXml, string encoding, out string symbology)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Initial XML" + strXml);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strXml);

                string strData = String.Empty;
                string barcode = xmlDoc.DocumentElement.GetElementsByTagName("datalabel").Item(0).InnerText;
                symbology = xmlDoc.DocumentElement.GetElementsByTagName("datatype").Item(0).InnerText;
                string[] numbers = barcode.Split(' ');

                List<byte> bytes = new List<byte>();
                foreach (string number in numbers)
                {
                    if (String.IsNullOrEmpty(number))
                    {
                        break;
                    }

                    bytes.Add(Convert.ToByte(number, 16));
                }

                return ConvertToEncodeString(encoding, bytes.ToArray());
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Convert the encode string
        /// </summary>
        /// <param name="encoding">Encoding value</param>
        /// <param name="bytes">converted encoding value</param>
        /// <returns></returns>
        private string ConvertToEncodeString(string encoding, byte[] bytes)
        {
            string encodedString;

            switch (encoding)
            {
                case "ASCII":
                    encodedString = System.Text.Encoding.ASCII.GetString(bytes);
                    break;
                case "UTF-8":
                    encodedString= System.Text.Encoding.UTF8.GetString(bytes);
                    break;
                case "UTF-16":
                    encodedString = System.Text.Encoding.Unicode.GetString(bytes);
                    break;
                case "UTF-32":
                    encodedString = System.Text.Encoding.UTF32.GetString(bytes);
                    break;
                default:
                    encodedString = System.Text.Encoding.Default.GetString(bytes);
                    break;
            }
            return encodedString;
        }


    }
}
