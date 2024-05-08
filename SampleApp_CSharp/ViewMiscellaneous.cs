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
using System.Text.RegularExpressions;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        private string build_version_string(string strXml)
        {
            string msg = "";
            if ("" != strXml)
            {
                try
                {
                    XmlTextReader xmlRead = new XmlTextReader(new StringReader(strXml));
                    // Skip non-significant whitespace   
                    xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;
                    string sElementName = "", sElmValue = "";
                    while (xmlRead.Read())
                    {

                        switch (xmlRead.NodeType)
                        {
                            case XmlNodeType.Element:
                                sElementName = xmlRead.Name;
                                break;
                            case XmlNodeType.Text:
                                {
                                    sElmValue = xmlRead.Value;
                                    switch (sElementName)
                                    {
                                        case "arg-string":
                                            msg += "The Selected Scanner is running on Zebra CoreScanner Driver Version ";
                                            msg += sElmValue;
                                            msg += " API\n";
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
            return msg;
        }
        private void PerformBtnSdkVersionClick(object sender, EventArgs e)
        {
            if (chkAsync.Checked)
            {
                MessageBox.Show("'CoreScanner Version' is not supported in Asynchronous mode", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (IsMotoConnected())
            {
                string outXml= ExecuteActionCommandOffline(GET_VERSION, "GET_VERSION", "<inArgs></inArgs>");
                if (!string.IsNullOrEmpty(outXml))
                {
                    UpdateOutXml(outXml);
                    string respMsg = "";
                    respMsg = build_version_string(outXml);
                    MessageBox.Show(respMsg, "CoreScanner Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        private void PerformBtnGetDevTopologyClick(object sender, EventArgs e)
        {
            if (chkAsync.Checked)
            {
                MessageBox.Show("'Get Device Topology' is not supported in Asynchronous mode", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (IsMotoConnected())
            {
                string outXml = ExecuteActionCommandOffline(GET_DEVICE_TOPOLOGY, "GET_DEVICE_TOPOLOGY", "<inArgs></inArgs>");
                if (!string.IsNullOrEmpty(outXml))
                    UpdateOutXml(outXml);
                TopologyPopupForm form = new TopologyPopupForm();
                form.buildTopologyTree(outXml);
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Combo CDC Host mode settings
        /// </summary>
        private void CDCSwitchModeChangeAction()
        {
            try
            {
                string strHostMode = cmbSCdcSHostMode.SelectedValue.ToString();
                string strSilentChange = chkSCdcSIsSilent.Checked.ToString().ToUpper();
                string strPermChange = chkSCdcSIsPermanent.Checked.ToString().ToUpper();
                string inXml = BaseMethods.GetSwitchXml(GetOnlyScannerIDXml(), strHostMode, strSilentChange, strPermChange, false);
                ExecuteActionCommand(SWITCH_CDC_DEVICES, "SWITCH_CDC_DEVICES", inXml);
            }
            catch
            {
                UpdateResults("");
            }
        }
        /// <summary>
        /// Used to fill the CDC combo
        /// </summary>
        private void FillSCdcHostMode()
        {
            cmbSCdcSHostMode.DataSource = null;
            List<KeyValuePair<string, string>> hostModes = scannerAction.GetHostModes(true);
            cmbSCdcSHostMode.DataSource = new BindingSource(hostModes, null);
            cmbSCdcSHostMode.DisplayMember = "Key";
            cmbSCdcSHostMode.ValueMember = "Value";
        }
    }
}
