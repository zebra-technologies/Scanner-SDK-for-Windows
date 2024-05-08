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
using STC;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        private void performProtocolSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProtocol.SelectedItem.Equals("Simple Serial Interface (SSI)"))
            {
                cmbHostName.Enabled = true;
                cmbHostName.SelectedIndex = 0;
            }
            else
            {
                cmbHostName.Enabled = false;
            }
            GetPairingBarcode();
        }
        private void performScannerTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbScannerType.SelectedIndex == 0)
            {
                cmbProtocol.Enabled = false;
                cmbHostName.Enabled = false;
                cmbDefaultOption.Enabled = false;
            }
            else
            {
                cmbProtocol.Enabled = true;
                if (cmbProtocol.SelectedIndex == 0)
                {
                    cmbHostName.Enabled = true;
                }
                else
                {
                    cmbHostName.Enabled = false;
                }
                cmbDefaultOption.Enabled = true;
            }
            GetPairingBarcode();
        }
        private void GetPairingBarcode()
        {            
            if (cmbScannerType.SelectedItem != null && cmbProtocol.SelectedItem != null && cmbHostName.SelectedItem != null)
            {
                Constants.DefaultOption defaultOption = GetSelectedDefaultOptionID(cmbDefaultOption.SelectedIndex);
                Constants.ImageSize imageSize = GetSelectedImageSizeID(cmbImageSize.SelectedIndex);
                Constants.ScannerType scannerType = GetSelectedScannerTypeID(cmbScannerType.SelectedIndex);
                Constants.ProtocolName protocol = GetSelectedProtocolID(cmbProtocol.SelectedIndex);
                Constants.HostName host = GetSelectedHostID(cmbHostName.SelectedIndex);

                int iProtocol = (int)protocol;
                int iDefaultOption = (int)defaultOption;
                int iImagesize = (int)imageSize;

                //Logic for protocol SSI and scanner
                if (scannerType == Constants.ScannerType.New && protocol == Constants.ProtocolName.SSI)
                {
                    iProtocol = (int)host;
                }

                int status = STATUS_FALSE;
                string outXml = "";
                string inXml = "";

                string parameters = string.Join(",", iProtocol, iDefaultOption, iImagesize);
                inXml = scanToConnect.GenerateInitXML(3, parameters);

                ExecCmd(Constants.GetPairingBarcode, ref inXml, out outXml, out status);
                DisplayResult(status, "GET_PAIRING_BARCODE");
            }
        }

        /// <summary>
        /// Get Scanner type value based on index
        /// </summary>
        /// <param name="SelectedIndex">Scanner type index</param>
        /// <returns>value</returns>
        private Constants.ScannerType GetSelectedScannerTypeID(int SelectedIndex)
        {
            if (SelectedIndex == 0)
            {
                return Constants.ScannerType.Legacy;
            }
            else
            {
                return Constants.ScannerType.New;
            }
        }
        /// <summary>
        /// Get Default option value based on index
        /// </summary>
        /// <param name="SelectedIndex">Default option index</param>
        /// <returns>value</returns>
        private Constants.DefaultOption GetSelectedDefaultOptionID(int SelectedIndex)
        {
            if (SelectedIndex == 2)
            {
                return Constants.DefaultOption.RestoreFactoryDefaults;
            }
            else if (SelectedIndex == 1)
            {
                return Constants.DefaultOption.SetFactoryDefaults;
            }
            else
            {
                return Constants.DefaultOption.NoDefaults;
            }
        }
        /// <summary>
        ///  Get Image size value based on index
        /// </summary>
        /// <param name="SelectedIndex">Image size index</param>
        /// <returns>value</returns>
        private Constants.ImageSize GetSelectedImageSizeID(int SelectedIndex)
        {
            if (SelectedIndex == 2)
            {
                return Constants.ImageSize.Large;
            }
            else if (SelectedIndex == 0)
            {
                return Constants.ImageSize.Small;
            }
            else
            {
                return Constants.ImageSize.Medium;
            }
        }
        /// <summary>
        /// Get protocol value based on index
        /// </summary>
        /// <param name="SelectedIndex">protocol index</param>
        /// <returns>value</returns>
        private Constants.ProtocolName GetSelectedProtocolID(int SelectedIndex)
        {
            if (SelectedIndex == 2)
            {
                return Constants.ProtocolName.HID;
            }
            else if (SelectedIndex == 1)
            {
                return Constants.ProtocolName.SPP;
            }
            else
            {
                return Constants.ProtocolName.SSI;
            }
        }
        /// <summary>
        ///  Get Host value based on index
        /// </summary>
        /// <param name="SelectedIndex">Host index</param>
        /// <returns>value</returns>
        private Constants.HostName GetSelectedHostID(int SelectedIndex)
        {
            if (SelectedIndex == 0)
            {
                return Constants.HostName.SSIBTClassic;
            }
            return Constants.HostName.SSIBTClassic;
        }
    }
}
