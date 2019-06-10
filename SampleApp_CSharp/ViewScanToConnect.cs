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
        


        private int GetSizeInt()
        {
            if (cmbImageSize.SelectedItem != null)
            {
                if (cmbImageSize.SelectedItem.Equals("Small"))
                {
                    return 1;
                }
                else if (cmbImageSize.SelectedItem.Equals("Large"))
                {
                    return 3;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                return 2;
            }
        }

        private int GetDefaultOptionInt()
        {
            if (cmbDefaultOption.SelectedItem != null)
            {
                if (cmbDefaultOption.SelectedItem.Equals("Set Factory Defaults"))
                {
                    return 1;
                }
                else if (cmbDefaultOption.SelectedItem.Equals("Restore Factory Defaults"))
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private void cmbProtocol_SelectedIndexChanged(object sender, EventArgs e)
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

        private void GetPairingBarcode()
        {
            int protocol = 1;
            int defaultOption = GetDefaultOptionInt();
            int size = GetSizeInt();
            if (cmbScannerType.SelectedItem != null && cmbProtocol.SelectedItem != null && cmbHostName.SelectedItem != null)
            {
                if (cmbScannerType.SelectedItem.Equals("Legacy"))
                {
                    protocol = 1;
                }
                else
                {
                    if (cmbProtocol.SelectedItem.Equals("Serial Port Profile (SPP)"))
                    {
                        protocol = 14;
                    }
                    else if (cmbProtocol.SelectedItem.Equals("Human Interface Device (HID)"))
                    {
                        protocol = 17;
                    }
                    else
                    {
                        if (cmbHostName.SelectedItem.Equals("SSI BT Classic (Non-Discoverable)"))
                        {
                            protocol = 22;
                        }
                    }
                }


                int status = STATUS_FALSE;
                string outXml = "";
                string inXml = "";

                inXml = "<inArgs>"
                             + " <cmdArgs>"
                                + "<arg-int>3</arg-int>"  //number of parameters
                                + "<arg-int>"
                                + protocol + ","// Protocol
                                + defaultOption + ","// Default Option
                                + size + ","// Image Size
                                + "</arg-int>"
                              + " </cmdArgs>"
                          + "</inArgs>";

                ExecCmd(GET_PAIRING_BARCODE, ref inXml, out outXml, out status);
                DisplayResult(status, "GET_PAIRING_BARCODE");
            }
        }

        private void cmbDefaultOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetPairingBarcode();
        }

        private void cmbImageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetPairingBarcode();
        }

        private void txtFolderPath_TextChanged(object sender, EventArgs e)
        {
            GetPairingBarcode();
        }

        private void cmbHostName_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetPairingBarcode();
        }

        private void cmbScannerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbScannerType.SelectedItem.Equals("Legacy"))
            {
                cmbProtocol.Enabled = false;
                cmbHostName.Enabled = false;
                cmbDefaultOption.Enabled = false;
            }
            else
            {
                cmbProtocol.Enabled = true;
                if (cmbProtocol.SelectedItem.Equals("Simple Serial Interface (SSI)"))
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
    }
}
