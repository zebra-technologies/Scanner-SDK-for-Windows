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
        private void PerformFWBrowseClick(object sender, EventArgs e)
        {
            if (openFileDialogFW.ShowDialog() == DialogResult.OK)
            {
                txtFWFile.Text = openFileDialogFW.FileName;
                progressBarFWUpdate.Value = 0;
            }
        }
        private void PerformFWUpdateClick(object sender, EventArgs e)
        {
            if (IsScannerConnected())
            {
                progressBarFWUpdate.Value = 0;

                byte[] filePath = new byte[1024];
                filePath = (new ASCIIEncoding()).GetBytes(txtFWFile.Text);

                if (txtFWFile.Text.CompareTo("") == 1) // To select correct firmware file
                {
                    this.UpdateFWThreadFunction();
                }
                else
                {
                    MessageBox.Show("Please browse correct firmware file before update firmware", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void PerformAbortFWUpdateClick(object sender, EventArgs e)
        {
            if (IsScannerConnected())
            {
                progressBarFWUpdate.Value = 0;
                ExecuteActionCommand(ABORT_UPDATE_FIRMWARE, "ABORT_UPDATE_FIRMWARE");
            }
        }

        private void PerformWavFileBrowseClick(object sender, EventArgs e)
        {
            openFileDialogWavFile.Title = "Decode Tone File";
            openFileDialogWavFile.FileName = "";
            if (openFileDialogWavFile.ShowDialog() == DialogResult.OK)
            {
                txtWavFile.Text = openFileDialogWavFile.FileName;
            }
        }
        private void PerformWavFileUploadClick(object sender, EventArgs e)
        {
            if (txtWavFile.Text != "")
            {
                string inXML = GetInXmlForAdvancedTab(txtWavFile.Text); 
                ExecuteActionCommandOffline(UPDATE_DECODE_TONE, "UPDATE_DECODE_TONE", inXML);
            }
        }
        private void PerformElectricFenceWavFileBrowseClick(object sender, EventArgs e)
        {
            openFileDialogWavFile.Title = "Electric Fence Custom Tone";
            openFileDialogWavFile.FileName = "";

            if (openFileDialogWavFile.ShowDialog() == DialogResult.OK)
            {
                txtElectricFenceWaveFile.Text = openFileDialogWavFile.FileName;
            }
        }

        private void PerformElectricFenceWavFileUploadClick(object sender, EventArgs e)
        {
            if (txtElectricFenceWaveFile.Text != "")
            {
                string inXML = GetInXmlForAdvancedTab(txtElectricFenceWaveFile.Text);
                ExecuteActionCommandOffline(UPDATE_ELECTRIC_FENCE_CUSTOM_TONE, "UPDATE_ELECTRIC_FENCE_CUSTOM_TONE", inXML);
            }
        }

        private string GetInXmlForAdvancedTab(string inputValues, string strCheckblock="")
        {
            string inXml = "<inArgs>" +
                             GetOnlyScannerIDXml() +
                             "<cmdArgs>" +
                             "<arg-string>" + inputValues + "</arg-string>" +
                             strCheckblock +
                             "</cmdArgs>" +
                             "</inArgs>";
            return inXml;

        }
        /// <summary>
        /// Sends UPDATE_FIRMWARE. FW update thread function
        /// </summary>
        private void UpdateFWThreadFunction()
        {
            if ("" == txtFWFile.Text)
            {
                MessageBox.Show("Please select a Firmware update file", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string strblk;
            if (chkBulk.Checked) strblk = "<arg-int>2</arg-int>"; else strblk = "<arg-int>1</arg-int>";
            string inXml = GetInXmlForAdvancedTab(txtFWFile.Text, strblk); 
            int opCode = UPDATE_FIRMWARE;

            if (txtFWFile.Text.EndsWith(".SCNPLG"))
            {
                opCode = UPDATE_FIRMWARE_FROM_PLUGIN;
            }
            ExecuteActionCommandOffline(opCode, "UPDATE_FIRMWARE", inXml);
        }

        /// <summary>
        /// Claims or releases a device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClaimScanner(object sender, EventArgs e)
        {
            if ((false == m_bIgnoreIndexChange) && IsScannerConnected())
            {
                int opCode = CLAIM_DEVICE;
                string strCode = "CLAIM_DEVICE";
                string serialno = lstvScanners.SelectedItems[0].SubItems[5].Text;

                if (!chkClaim.Checked)
                {
                    opCode = RELEASE_DEVICE;
                    strCode = "RELEASE_DEVICE";
                    if (claimlist.Contains(serialno))
                    {
                        claimlist.Remove(serialno);
                    }

                    int nSelIndex = lstvScanners.SelectedItems[0].Index;
                    if (-1 < nSelIndex && m_nTotalScanners > nSelIndex)
                    {
                        Scanner scanr = (Scanner)m_arScanners.GetValue(nSelIndex);
                        scanr.CLAIMED = chkClaim.Checked;
                    }
                }

                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "</inArgs>";
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, strCode);
                

                if (status == STATUS_LOCKED)
                {
                    m_bIgnoreIndexChange = true;
                    chkClaim.Checked = !chkClaim.Checked;

                    return;
                }
                else if (!chkClaim.Checked)
                    return;
                else
                {
                    int nSelIndex = lstvScanners.SelectedItems[0].Index;
                    if (-1 < nSelIndex && m_nTotalScanners > nSelIndex)
                    {
                        Scanner scanr = (Scanner)m_arScanners.GetValue(nSelIndex);
                        scanr.CLAIMED = chkClaim.Checked;
                        if (!claimlist.Contains(serialno))
                        {
                            claimlist.Add(serialno);
                        }
                    }
                }
            }
            m_bIgnoreIndexChange = false;
        }
    }
}
