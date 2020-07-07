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
        private void PerformButtonFWBrowseClick(object sender, EventArgs e)
        {
            if (openFileDialogFW.ShowDialog() == DialogResult.OK)
            {
                txtFWFile.Text = openFileDialogFW.FileName;
                progressBarFWUpdate.Value = 0;
            }
        }

        private void PerformbtnFWUpdateClick(object sender, EventArgs e)
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

        private void PerformBtnAbortFWUpdateClick(object sender, EventArgs e)
        {
            if (IsScannerConnected())
            {
                progressBarFWUpdate.Value = 0;

                string inXml = GetScannerIDXml();
                int opCode = ABORT_UPDATE_FIRMWARE;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "ABORT_UPDATE_FIRMWARE");
            }
        }

        private void PerformBtnStartNewFWClick(object sender, EventArgs e)
        {
            string inXml = GetScannerIDXml();
            int opCode = START_NEW_FIRMWARE;
            string outXml = "";
            int status = STATUS_FALSE;
            ExecCmd(opCode, ref inXml, out outXml, out status);
            DisplayResult(status, "START_NEW_FIRMWARE");
        }
    }
}
