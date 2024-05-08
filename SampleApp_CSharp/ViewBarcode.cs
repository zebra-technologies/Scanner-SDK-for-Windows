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
        /// Clear barcode event data populations
        /// </summary>
        private void PerformClearBarcodeDataClick(object sender, EventArgs e)
        {
            txtBarcode.Clear();
            txtBarcodeLbl.Clear();
            txtSyblogy.Clear();
        }
        
        /// <summary>
        /// Populate Barcode data controls
        /// </summary>
        /// <param name="strXml">Barcode data XML</param>
        private void ShowBarcodeLabel(string strXml)
        {
            string symbology = "";
           
            if (txtBarcodeLbl.InvokeRequired)
            {
                txtBarcodeLbl.Invoke(new MethodInvoker(delegate
                {
                    string LabelText = scannerBarcode.GetBarcodelabel(strXml, cmbEncoding.SelectedItem.ToString(), out symbology);
                    txtBarcodeLbl.Clear();
                    txtBarcodeLbl.Text = LabelText;
                    
                }));
            }

            if (txtSyblogy.InvokeRequired)
            {
                txtSyblogy.Invoke(new MethodInvoker(delegate
                {
                    txtSyblogy.Text = scannerBarcode.GetSymbology((int)Convert.ToInt32(symbology));
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
                        txtBarcode.Text = m_xml.IndentXmlString(tmpScanData);
                    }));
                }

                if(GetSelectedTabName().Equals(SSW_TAB_NAME))
                {
                    if (scannerBarcode.GetScanDataType(tmpScanData) == ScannerBarcode.ST_UPCA)
                    {
                        currentUpca = scannerBarcode.GetScanDataLabel(tmpScanData);
                        currentUpca = BaseMethods.GetReadableScanDataLabel(currentUpca);
                        SetTextboxText(txtUpcaBarcode, currentUpca);

                        ExtractUpcData();
                    }
                    if(scannerBarcode.GetScanDataType(tmpScanData) == ScannerBarcode.ST_EPC_RAW) 
                    {
                        currentEpcId = scannerBarcode.GetScanDataLabel(tmpScanData);
                        currentEpcId = BaseMethods.GetReadableScanDataLabel(currentEpcId);
                        SetTextboxText(txtEpcId, currentEpcId);

                        ExtractEpcData();
                    }                   
                    CreateNewEpcId();
                }
            }
            catch (Exception e)
            {
                UpdateResults("Failed to scan data  "+e.Message);
            }
        }

        /// <summary>
        /// Get the selected tab name
        /// </summary>
        /// <returns>Name of the selected tab</returns>
        private string GetSelectedTabName()
        {
            string selectedTabName = String.Empty;
            if (tabCtrl.InvokeRequired)
            {
                tabCtrl.Invoke(new MethodInvoker(delegate
                {
                    selectedTabName = tabCtrl.SelectedTab.Name;
                }));
            }
            else
            {
                selectedTabName = tabCtrl.SelectedTab.Name;
            }
            return selectedTabName;
        }

        

        private string m_DadfSource = "";
        private string m_DadfPath = "";

        private void SetDADF()
        {
            string inXML = "<inArgs><cmdArgs><arg-string>" + m_DadfPath + "</arg-string></cmdArgs></inArgs>";
            ExecuteActionCommand(CONFIGURE_DADF, "CONFIGURE_DADF", inXML);
        }

        private void OnChkChangedDADF(object sender, EventArgs e)
        {
            if(chkBoxAppADF.CheckState == CheckState.Unchecked)
	        {
                ExecuteActionCommand(RESET_DADF, "RESET_DADF", "<inArgs></inArgs>");
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

                    ExecuteActionCommand(RESET_DADF, "RESET_DADF", "<inArgs></inArgs>");

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
