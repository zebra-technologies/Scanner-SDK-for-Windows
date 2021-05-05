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
        private void PerformBtnSdkVersionClick(object sender, EventArgs e)
        {
            if (chkAsync.Checked)
            {
                MessageBox.Show("'CoreScanner Version' is not supported in Asynchronous mode", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (IsMotoConnected())
            {
                int opCode = GET_VERSION;
                string inXml = "<inArgs></inArgs>";
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                UpdateOutXml(outXml);
                DisplayResult(status, "GET_VERSION");
                string respMsg = "";
                respMsg = build_version_string(outXml);
                MessageBox.Show(respMsg, "CoreScanner Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                string inXml = "<inArgs></inArgs>";
                int opCode = GET_DEVICE_TOPOLOGY;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                UpdateOutXml(outXml);
                DisplayResult(status, "GET_DEVICE_TOPOLOGY");


                TopologyPopupForm form = new TopologyPopupForm();
                form.buildTopologyTree(outXml);
                form.ShowDialog();
            }
        }
    }
}
