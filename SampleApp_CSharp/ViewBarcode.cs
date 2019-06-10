using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using CoreScanner;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        /// <summary>
        /// Flush Macro PDF
        /// </summary>
        private void PerformBtnFlushMacroPdfClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                string inXml = GetScannerIDXml(), outXml = String.Empty;
                int iOpCode = FLUSH_MACROPDF, iStatus = STATUS_FALSE;
                
                ExecCmd(iOpCode, ref inXml, out outXml, out iStatus);
                DisplayResult(iStatus, "FLUSH_MACROPDF");
            }
        }

        /// <summary>
        /// Abort Macro PDF
        /// </summary>
        private void PerformBtnAbortMacroPdfClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                string inXml = GetScannerIDXml(), outXml = String.Empty;
                int iOpCode = ABORT_MACROPDF, iStatus = STATUS_FALSE;
                
                ExecCmd(iOpCode, ref inXml, out outXml, out iStatus);
                DisplayResult(iStatus, "ABORT_MACROPDF");
            }
        }

        /// <summary>
        /// Clear barcode event data populations
        /// </summary>
        private void PerformClearBarcodeDataClick(object sender, EventArgs e)
        {
            txtBarcode.Clear();
            txtBarcodeLbl.Clear();
            txtSyblogy.Clear();
        }

        /// <summary>
        /// Barcode symbology
        /// </summary>
        /// <param name="Code">Symbology code</param>
        /// <returns>Symbology name</returns>
        private string GetSymbology(int Code)
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
                case ST_COUPON:
                    return "COUPON CODE";
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
                    return "CCA EAN-128";
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
                case ST_OCR:
                    return "OCR";
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
                default:
                    return "";
            }
            
        }

        /// <summary>
        /// Populate Barcode data controls
        /// </summary>
        /// <param name="strXml">Barcode data XML</param>
        private void ShowBarcodeLabel(string strXml)
        {
            System.Diagnostics.Debug.WriteLine("Initial XML" + strXml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strXml);

            string strData = String.Empty;
            string barcode = xmlDoc.DocumentElement.GetElementsByTagName("datalabel").Item(0).InnerText;
            string symbology = xmlDoc.DocumentElement.GetElementsByTagName("datatype").Item(0).InnerText;
            string[] numbers = barcode.Split(' ');

            foreach (string number in numbers)
            {
                if (String.IsNullOrEmpty(number))
                {
                    break;
                }

                strData += ((char)Convert.ToInt32(number, 16)).ToString();
            }

            if (txtBarcodeLbl.InvokeRequired)
            {
                txtBarcodeLbl.Invoke(new MethodInvoker(delegate
                {
                    txtBarcodeLbl.Clear();
                    txtBarcodeLbl.Text = strData;
                }));
            }

            if (txtSyblogy.InvokeRequired)
            {
                txtSyblogy.Invoke(new MethodInvoker(delegate
                {
                    txtSyblogy.Text = GetSymbology((int)Convert.ToInt32(symbology));
                }));
            }
        }

        /// <summary>
        /// BarcodeEvent received
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="scanData">Barcode string</param>
        void OnBarcodeEvent(short eventType, ref string scanData)
        {
            try
            {
                string tmpScanData = scanData;

                UpdateResults("Barcode Event fired");
                ShowBarcodeLabel(tmpScanData);

                if(txtBarcode.InvokeRequired)
                {
                    txtBarcode.Invoke(new MethodInvoker(delegate
                    {
                        txtBarcode.Text = IndentXmlString(tmpScanData);
                    }));
                }
            }
            catch (Exception)
            {
            }
        }


        private string m_DadfSource = "";
        private string m_DadfPath = "";

        private void ResetDADF()
        {
            int opcode = RESET_DADF;
            string inXML = "<inArgs></inArgs>";
            string outXml = "";
            int status = STATUS_FALSE;
            ExecCmd(opcode, ref inXML, out outXml, out status);
            DisplayResult(status, "RESET_DADF");
        }

        private void SetDADF()
        {
            int opcode = CONFIGURE_DADF;
            string inXML = "<inArgs><cmdArgs><arg-string>" + m_DadfPath + "</arg-string></cmdArgs></inArgs>";
            string outXml = "";
            int status = STATUS_FALSE;
            ExecCmd(opcode, ref inXML, out outXml, out status);
            DisplayResult(status, "CONFIGURE_DADF");
        }

        private void OnChkChangedDADF(object sender, EventArgs e)
        {
            if(chkBoxAppADF.CheckState == CheckState.Unchecked)
	        {
		        ResetDADF();
                chkBoxAppADF.Text = "Not Set";
                chkBoxAppADF.Enabled = false;
                m_DadfSource = "";
	        }
        }

        private void PerformBtnBrowseScriptClick(object sender, EventArgs e)
        {
            if (openFileDialogDADF.ShowDialog() == DialogResult.OK)
            {
                m_DadfPath = openFileDialogDADF.FileName;
                if (m_DadfPath == "") return;

                SetDADF();

                chkBoxAppADF.Enabled = true;
                chkBoxAppADF.Checked = true;
                chkBoxAppADF.Text = "Unload";
                m_DadfSource = "";
            }
        }

        private void PerformBtnScriptEditorClick(object sender, EventArgs e)
        {
            DadfScriptEditor frm = new DadfScriptEditor();
            frm.ScriptSource = m_DadfSource;

            DialogResult result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_DadfSource = frm.ScriptSource;
                if (m_DadfSource == "")
                {
                    chkBoxAppADF.Checked = false;
                    chkBoxAppADF.Text = "Not Set";
                    chkBoxAppADF.Enabled = false;

                    ResetDADF();

                    m_DadfPath = "";
                    m_DadfSource = "";
                }
                else
                {
                    chkBoxAppADF.Enabled = true;
                    chkBoxAppADF.Checked = true;
                    chkBoxAppADF.Text = "Unload";

                    //XML encode entities
                    string scriptSource = m_DadfSource;
                    scriptSource = scriptSource.Replace("&", "&amp;");
                    scriptSource = scriptSource.Replace("<", "&lt;");
                    scriptSource = scriptSource.Replace(">", "&gt;");
                    scriptSource = scriptSource.Replace("\'", "&apos;");
                    scriptSource = scriptSource.Replace("\"", "&quot;");
                    //m_DadfSource = scriptSource;
                    m_DadfPath = scriptSource;
                    SetDADF();
                    m_DadfPath = "";
                }
            }
        }

    }
}
