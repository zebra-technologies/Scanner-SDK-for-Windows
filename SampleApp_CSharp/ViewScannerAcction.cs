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
            if (IsScannerConnected())
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
            if (IsScannerConnected())
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
            if (!IsScannerConnected())
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


        /// <summary>
        /// This method allows only digits to be entered as pager motor duration
        /// Sends DEVICE_PAGE_MOTOR_CONTROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PerformPagerMotorTxtKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void PerformPagerMotorEableClick(object sender, EventArgs e)
        {
            if (!IsScannerConnected())
            {
                return;
            }
            try
            {              
                double i = 0;
                string pagerMotorDuration= txtPagerMotorDuration.Text;

                if (pagerMotorDuration == "")
                {
                    pagerMotorDuration = "0";
                    txtPagerMotorDuration.Text = "0";
                }

                bool isNumeric = double.TryParse(pagerMotorDuration, out i);
                if (isNumeric)
                {
                    string inXml = "<inArgs>" +
                         GetOnlyScannerIDXml() +
                        "<cmdArgs>" +
                            "<arg-xml>" +
                                "<attrib_list>" +
                                    "<attribute>" +
                                        "<id>" + PAGER_MOTOR_ACTION + "</id>" +
                                        "<datatype>" + "X" + "</datatype>" +
                                        "<value>" + pagerMotorDuration + "</value>" +
                                    "</attribute>" +
                                "</attrib_list>" +
                            "</arg-xml>" +
                         "</cmdArgs>" +
                         "</inArgs>";

                    int opCode = RSM_ATTR_SET;
                    string outXml = "";
                    int status = STATUS_FALSE;
                    ExecCmd(opCode, ref inXml, out outXml, out status);

                    DisplayResult(status, "START_PAGER_MOTOR");
                }
                else
                {
                    UpdateResults("Please enter a numeric value for pager motor duration");
                }               
                
            }
            catch
            {
                UpdateResults("Please enter a numeric value for pager motor duration");
            }
        }

        private void PerformBtnRebootScannerClick(object sender, EventArgs e)
        {
            if (IsScannerConnected())
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
            if (IsScannerConnected())
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
            if (IsScannerConnected())
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
            bool isCS4070 = false;
            if (IsScannerConnected())
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
            if (!IsScannerConnected())
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

        private void PerformCDCSwitchModeClick(object sender, EventArgs e)
        {
            try
            {
                string strHostMode = comboSCdcSHostMode.Text;
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

                string strSilentChange = chkSCdcSIsSilent.Checked ? "TRUE" : "FALSE";
                string strPermChange = chkSCdcSIsPermanent.Checked ? "TRUE" : "FALSE";
                string inXml = "<inArgs>" +
                                    "<cmdArgs>" +
                                    "<arg-string>" + strHostMode + "</arg-string>" +
                                    "<arg-bool>" + strSilentChange + "</arg-bool>" +
                                    "<arg-bool>" + strPermChange + "</arg-bool>" +
                                    "</cmdArgs>" +
                                "</inArgs>";
                int opCode = SWITCH_CDC_DEVICES;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "SWITCH_CDC_DEVICES");
            }
            catch
            {
                UpdateResults("");
            }
        }


        private void OnEnableScanner()
        {
            if(IsScannerConnected())
            {
                int iOpcode = DEVICE_SCAN_ENABLE;
                int status = STATUS_FALSE;
                string strSerialNumber = lstvScanners.SelectedItems[0].SubItems[5].Text;
                string inXml = GetScannerIDXml();
                string outXml = "";
                string strCmd = "SCAN_ENABLE";

                ExecCmd(iOpcode, ref inXml, out outXml, out status);

                DisplayResult(status, strCmd);
            }
        }

        private void OnDisableScanner()
        {
            if(IsScannerConnected())
            {
                int iOpcode = DEVICE_SCAN_DISABLE;
                int status = STATUS_FALSE;
                string strSerialNumber = lstvScanners.SelectedItems[0].SubItems[5].Text;
                string inXml = GetScannerIDXml();
                string outXml = "";
                string strCmd = "SCAN_DISABLE";

                ExecCmd(iOpcode, ref inXml, out outXml, out status);

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
