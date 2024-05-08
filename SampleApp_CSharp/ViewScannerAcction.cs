using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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

        const int DEVICE_LED_OFF = 2009;
        const int DEVICE_LED_ON = 2010;
        const int DEVICE_SCAN_DISABLE = 2013;
        const int DEVICE_SCAN_ENABLE = 2014;
        const int DEVICE_BEEP_CONTROL = 2018;
        const int REBOOT_SCANNER = 2019;
        const int DISCONNECT_BT_SCANNER = 2023;

        // USBHIDKB //
        const int DEVICE_SWITCH_HOST_MODE = 6200;
        const int SWITCH_CDC_DEVICES = 6201;
        // USBHIDKB - end //

        /// <summary>
        /// Enable the Pager motor action
        /// </summary>

        private void EnablePagerMotorAction()
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
                    string inXml = scannerAction.GetPageMotorXML(GetOnlyScannerIDXml(), pagerMotorDuration);
                    ExecuteActionCommand(RSM_ATTR_SET, "START_PAGER_MOTOR", inXml);
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

        /// <summary>
        /// LED button related action
        /// </summary>
        /// <param name="isOn">ON/OFF</param>
        public void PerformLEDAction(bool isOn)
        {
            /*LEDs can be controlled with SET action command (RSM Action attribute) as well however currently some scanners like CS4070 does not support RSM action attributes.
             Following commented code segment can be used to control LEDs of other scanners*/
            bool isCS4070 = false;
            if (IsScannerConnected())
            {
                if (m_arScanners.Where(s => s.SCANNERID == GetSelectedScannerID() && s.MODELNO.Contains("PL3300")).FirstOrDefault() != null)
                    isCS4070 = true;

                int opCode = isOn ? DEVICE_LED_ON : DEVICE_LED_OFF;
                string opCodeName = isOn ? "DEVICE_LED_ON" : "DEVICE_LED_OFF";
                if (!isCS4070)
                {
                    opCode = SET_ACTION;
                    opCodeName = "SET_ACTION";
                }
                ExecuteActionCommand(opCode, opCodeName, scannerAction.GetLedIDXml(GetOnlyScannerIDXml(),isOn, cmbLed.SelectedIndex));
            }
        }

        /// <summary>
        /// Set Report Action based on mode
        /// </summary>
        private void SetReportAction()
        {
            if (!IsScannerConnected())
            {
                return;
            }
            try
            {
                string strHostMode = cmbMode.SelectedValue.ToString();
                string strSilentSwitch = chkShmSilentSwitch.Checked.ToString().ToUpper();
                string strPermChange = chkShmPermChange.Checked.ToString().ToUpper();

                string inXml = BaseMethods.GetSwitchXml(GetOnlyScannerIDXml(),strHostMode, strSilentSwitch, strPermChange);
                int opCode = DEVICE_SWITCH_HOST_MODE;
                string scnrMode = null;
                scnrMode = lstvScanners.SelectedItems[0].SubItems[1].Text;
                if ("USB-CDC Serial Emulation" == scnrMode || "USB-SSI over CDC" == scnrMode && "USB-CDC Serial Emulation" == strHostMode || "USB-SSI over CDC" == strHostMode)
                    opCode = SWITCH_CDC_DEVICES;
                ExecuteActionCommand(opCode, "DEVICE_SWITCH_HOST_MODE", inXml);
            }
            catch
            {
                UpdateResults("");
            }
        }

    }
}
