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

        private void PerformBtnAimOnClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                string inXml = GetScannerIDXml();
                int opCode = DEVICE_AIM_ON;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "AIM_ON");
            }
        }

        private void PerformBtnAimOffClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                string inXml = GetScannerIDXml();
                int opCode = DEVICE_AIM_OFF;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "AIM_OFF");
            }
        }

        private void PerformBtnSoundBeeperClick(object sender, EventArgs e)
        {
            if (!IsMotoConnectedWithScanners())
            {
                return;
            }
            try
            {
                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                    "<arg-int>" + comboBeep.SelectedIndex 
                                    + "</arg-int>" +
                                    "</cmdArgs>" +
                                    "</inArgs>";

                int opCode = DEVICE_BEEP_CONTROL;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "SET_ACTION");
            }
            catch
            {
                UpdateResults("Please select a beep from the list, default ONESHORTHI");
            }
        }

        private void PerformBtnRebootScannerClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                string inXml = "<inArgs>" +
                                 GetOnlyScannerIDXml() +
                                 "</inArgs>";
                int opCode = REBOOT_SCANNER;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "REBOOT_SCANNER");
            }
        }

        private void PerformBtnDisconnectScannerClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                string inXml = "<inArgs>" +
                                 GetOnlyScannerIDXml() +
                                 "</inArgs>";
                int opCode = DISCONNECT_BT_SCANNER;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "DISCONNECT_BT_SCANNER");
            }
        }


        private void PerformBtnLedOnClick(object sender, EventArgs e)
        {
            /*LEDs can be controlled with SET action command (RSM Action attribute) as well however currently some scanners like CS4070 does not support RSM action attributes.
            Following commented code segment can be used to control LEDs of other scanners*/
            bool isCS4070 = false;
            if (IsMotoConnectedWithScanners())
            {
                foreach (var item in m_arScanners)
                {
                    if (item.SCANNERID == GetSelectedScannerID())
                    {
                        if (item.MODELNO.Contains("PL3300"))
                        {
                            isCS4070 = true;
                        }
                    }
                }

                if (!isCS4070)
                {
                    int onLed = GetOnLedID();
                    string inXml = "<inArgs>" +
                                        GetOnlyScannerIDXml() +
                                        "<cmdArgs>" +
                                        "<arg-int>" + onLed.ToString() + "</arg-int>" +
                                        "</cmdArgs>" +
                                        "</inArgs>";

                    int opCode = SET_ACTION;
                    string outXml = "";
                    int status = STATUS_FALSE;
                    ExecCmd(opCode, ref inXml, out outXml, out status);
                    DisplayResult(status, "SET_ACTION");
                }
                else
                {
                    int onLed = GetOnLedID();
                    string inXml = "<inArgs>" +
                                        GetOnlyScannerIDXml() +
                                        "<cmdArgs>" +
                                        "<arg-int>" + onLed.ToString()
                                        + "</arg-int>" +
                                        "</cmdArgs>" +
                                        "</inArgs>";

                    int opCode = DEVICE_LED_ON;
                    string outXml = "";
                    int status = STATUS_FALSE;
                    ExecCmd(opCode, ref inXml, out outXml, out status);
                    DisplayResult(status, "DEVICE_LED_ON");
                }
            }

        }

        private void PerformBtnLedOffClick(object sender, EventArgs e)
        {
            /*LEDs can be controlled with SET action command (RSM Action attribute) as well however currently some scanners like CS4070 does not support RSM action attributes.
             Following commented code segment can be used to control LEDs of other scanners*/

            //if (IsMotoConnectedWithScanners())
            //{
            //    int offLed = GetOffLedID();
            //    string inXml = "<inArgs>" +
            //                       GetOnlyScannerIDXml() +
            //                       "<cmdArgs>" +
            //                       "<arg-int>" + offLed.ToString() + "</arg-int>" +
            //                       "</cmdArgs>" +
            //                       "</inArgs>";

            //    int opCode = SET_ACTION;
            //    string outXml = "";
            //    int status = STATUS_FALSE;
            //    ExecCmd(opCode, ref inXml, out outXml, out status);
            //    DisplayResult(status, "SET_ACTION");
            //}

            

            //if (!IsMotoConnectedWithScanners())
            //{
            //    return;
            //}
            //try
            //{
            //    int offLed = GetOffLedID();
            //    string inXml = "<inArgs>" +
            //                        GetOnlyScannerIDXml() +
            //                        "<cmdArgs>" +
            //                        "<arg-int>" + offLed.ToString()
            //                        + "</arg-int>" +
            //                        "</cmdArgs>" +
            //                        "</inArgs>";

            //    int opCode = DEVICE_LED_OFF;
            //    string outXml = "";
            //    int status = STATUS_FALSE;
            //    ExecCmd(opCode, ref inXml, out outXml, out status);
            //    DisplayResult(status, "DEVICE_LED_OFF");
            //}
            //catch
            //{
            //    UpdateResults("Please select a beep from the list, default ONESHORTHI");
            //}

            bool isCS4070 = false;
            if (IsMotoConnectedWithScanners())
            {
                foreach (var item in m_arScanners)
                {
                    if (item.SCANNERID == GetSelectedScannerID())
                    {
                        if (item.MODELNO.Contains("PL3300"))
                        {
                            isCS4070 = true;
                        }
                    }
                }

                if (!isCS4070)
                {
                    int onLed = GetOffLedID();
                    string inXml = "<inArgs>" +
                                        GetOnlyScannerIDXml() +
                                        "<cmdArgs>" +
                                        "<arg-int>" + onLed.ToString() + "</arg-int>" +
                                        "</cmdArgs>" +
                                        "</inArgs>";

                    int opCode = SET_ACTION;
                    string outXml = "";
                    int status = STATUS_FALSE;
                    ExecCmd(opCode, ref inXml, out outXml, out status);
                    DisplayResult(status, "SET_ACTION");
                }
                else
                {
                    int onLed = GetOffLedID();
                    string inXml = "<inArgs>" +
                                        GetOnlyScannerIDXml() +
                                        "<cmdArgs>" +
                                        "<arg-int>" + onLed.ToString()
                                        + "</arg-int>" +
                                        "</cmdArgs>" +
                                        "</inArgs>";

                    int opCode = DEVICE_LED_OFF;
                    string outXml = "";
                    int status = STATUS_FALSE;
                    ExecCmd(opCode, ref inXml, out outXml, out status);
                    DisplayResult(status, "DEVICE_LED_ON");
                }
            }
        }

        private void PerformBtnSetReportClick(object sender, EventArgs e)
        {
            if (!IsMotoConnectedWithScanners())
            {
                return;
            }
            try
            {
                string strHostMode = cmbMode.Text;
                if ("USB-IBMHID" == strHostMode)
                {
                    strHostMode = "XUA-45001-1";
                }
                else if ("USB-IBMTT" == strHostMode)
                {
                    strHostMode = "XUA-45001-2";
                }
                else if ("USB-HIDKB" == strHostMode)
                {
                    strHostMode = "XUA-45001-3";
                }
                else if ("USB-OPOS" == strHostMode)
                {
                    strHostMode = "XUA-45001-8";
                }
                else if ("USB-SNAPI with Imaging" == strHostMode)
                {
                    strHostMode = "XUA-45001-9";
                }
                else if ("USB-SNAPI without Imaging" == strHostMode)
                {
                    strHostMode = "XUA-45001-10";
                }
                else if ("USB-CDC Serial Emulation" == strHostMode)
                {
                    strHostMode = "XUA-45001-11";
                }
                else if ("USB-SSI over CDC" == strHostMode)
                {
                    strHostMode = "XUA-45001-14";
                }

                string strSilentSwitch = chkShmSilentSwitch.Checked ? "TRUE" : "FALSE";
                string strPermChange = chkShmPermChange.Checked ? "TRUE" : "FALSE";
                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                    "<arg-string>" + strHostMode + "</arg-string>" +
                                    "<arg-bool>" + strSilentSwitch + "</arg-bool>" +
                                    "<arg-bool>" + strPermChange + "</arg-bool>" +
                                    "</cmdArgs>" +
                                    "</inArgs>";
                int opCode = DEVICE_SWITCH_HOST_MODE;
                string scnrMode = null; 
                scnrMode = lstvScanners.SelectedItems[0].SubItems[1].Text;
                if("USB-CDC Serial Emulation" == scnrMode || "USB-SSI over CDC" == scnrMode && "USB-CDC Serial Emulation" == strHostMode || "USB-SSI over CDC" == strHostMode)
                    opCode = SWITCH_CDC_DEVICES;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "DEVICE_SWITCH_HOST_MODE");
            }
            catch
            {
                UpdateResults("");
            }
        }

        private void PerformOnScannerEnable(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                int opCode = DEVICE_SCAN_ENABLE;
                string serialno = lstvScanners.SelectedItems[0].SubItems[5].Text;
                if (chkScannerEnable.Checked)
                {
                    opCode = DEVICE_SCAN_DISABLE;
                    if (!scanrdisablelist.Contains(serialno))
                    {
                        scanrdisablelist.Add(lstvScanners.SelectedItems[0].SubItems[5].Text);//
                    }
                }

                string inXml = GetScannerIDXml();
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                string strCmd = "SCAN_DISABLE";
                if (DEVICE_SCAN_ENABLE == opCode)
                {
                    strCmd = "SCAN_ENABLE";
                    if (scanrdisablelist.Contains(serialno) && !chkScannerEnable.Checked)
                    {
                        scanrdisablelist.Remove(serialno);
                    }
                }
                DisplayResult(status, strCmd);
            }
        }

        private int GetOnLedID()
        {
            int onLed = LED_1_ON;
            if(cmbLed.SelectedIndex == 0)
            {
                onLed = LED_1_ON;
            }
            if (cmbLed.SelectedIndex == 1)
            {
                onLed = LED_2_ON;
            }
            if (cmbLed.SelectedIndex == 2)
            {
                onLed = LED_3_ON;
            }
            
            return onLed;
        }

        private int GetOffLedID()
        {
            int offLed = LED_1_OFF;
            if (cmbLed.SelectedIndex == 0)
            {
                offLed = LED_1_OFF;
            }
            if (cmbLed.SelectedIndex == 1)
            {
                offLed = LED_2_OFF;
            }
            if (cmbLed.SelectedIndex == 2)
            {
                offLed = LED_3_OFF;
            }

            return offLed;
        }
    }
}
