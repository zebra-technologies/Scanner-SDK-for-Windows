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
using System.Text.RegularExpressions;
using CoreScanner;
using STC;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp : Form
    {
        Image imgCapturedImage;
        Bitmap idcImage;
        CCoreScannerClass m_pCoreScanner;
		ScanToConnect scanToConnect;
        ScannerAction scannerAction;
        DiscoverScanner discoverScanner;
        ScannerBarcode scannerBarcode;
        bool m_bSuccessOpen;//Is open success
		
        Scanner[] m_arScanners;
        XmlReader m_xml;
        bool m_bIgnoreIndexChange;
        short m_nNumberOfTypes;
        short[] m_arScannerTypes;
        bool[] m_arSelectedTypes;
        int m_nTotalScanners;
        static ushort[] m_arParamsList; //Parameter information list 
        static ushort[] m_arViewFindParamsList; //Parameter information list - viewFinder
        int m_nRsmAttributeCount;
        long m_nResultLineCount;
        int[] m_nArTotalScannersInType;//total scanners in types of SCANNER_TYPES_SNAPI,SCANNER_TYPES_SSI,SCANNER_TYPES_IBMHID,SCANNER_TYPES_NIXMODB,SCANNER_TYPES_HIDKB
        DocCapMessage m_docCapMsg = new DocCapMessage();
        byte[] m_wavFile;
        List<string> claimlist = new List<string>();

        public frmScannerApp()
        {
            InitializeComponent();

            m_nResultLineCount = 0;
            m_bSuccessOpen = false;
            m_nTotalScanners = 0;
            m_nArTotalScannersInType = new int[TOTAL_SCANNER_TYPES];
            InitScannersCount();
            m_arScanners = new Scanner[MAX_NUM_DEVICES];
            for (int i = 0; i < MAX_NUM_DEVICES; i++)
            {
                Scanner scanr = new Scanner();
                m_arScanners.SetValue(scanr, i);
            }
            m_arParamsList = new ushort[MAX_PARAM_LEN];
            m_arViewFindParamsList = new ushort[MAX_PARAM_LEN];
            m_xml = new XmlReader();

            m_nRsmAttributeCount = 0;
            m_nNumberOfTypes = 0;
            m_arScannerTypes = new short[TOTAL_SCANNER_TYPES];
            m_arSelectedTypes = new bool[TOTAL_SCANNER_TYPES];
            SetControls();
            m_bIgnoreIndexChange = false;

            m_wavFile = new byte[WAV_FILE_MAX_SIZE];
            scanToConnect = ScanToConnect.GetInstance();
            scannerAction = ScannerAction.GetInstance();
            scannerBarcode = ScannerBarcode.GetInstance();


            try
            {
                m_pCoreScanner = new CoreScanner.CCoreScannerClass();
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                m_pCoreScanner = new CoreScanner.CCoreScannerClass();
            }

            // Register events for COM services
            m_pCoreScanner.ImageEvent += new CoreScanner._ICoreScannerEvents_ImageEventEventHandler(OnImageEvent);
            m_pCoreScanner.VideoEvent += new CoreScanner._ICoreScannerEvents_VideoEventEventHandler(OnVideoEvent);
            m_pCoreScanner.BarcodeEvent += new CoreScanner._ICoreScannerEvents_BarcodeEventEventHandler(OnBarcodeEvent);
            m_pCoreScanner.PNPEvent += new CoreScanner._ICoreScannerEvents_PNPEventEventHandler(OnPNPEvent);
            m_pCoreScanner.ScanRMDEvent += new CoreScanner._ICoreScannerEvents_ScanRMDEventEventHandler(OnScanRMDEvent);
            m_pCoreScanner.CommandResponseEvent += new CoreScanner._ICoreScannerEvents_CommandResponseEventEventHandler(OnCommandResponseEvent);
            m_pCoreScanner.IOEvent += new CoreScanner._ICoreScannerEvents_IOEventEventHandler(OnIOEvent);
            m_pCoreScanner.ScannerNotificationEvent += new _ICoreScannerEvents_ScannerNotificationEventEventHandler(OnScannerNotification);
            m_pCoreScanner.BinaryDataEvent += new _ICoreScannerEvents_BinaryDataEventEventHandler(On_BinaryDataEvent);
            m_pCoreScanner.ParameterBarcode += new _ICoreScannerEvents_ParameterBarcodeEventHandler(OnParameterBarcodeEvent);
            m_docCapMsg.DocCapImage += new DocCapMessage.DocCapImageHandler(OnDocCapImage);
            m_docCapMsg.DocCapDecode += new DocCapMessage.DocCapDecodeHandler(OnDocCapDecode);

            discoverScanner = DiscoverScanner.GetInstance(m_pCoreScanner);

            cmbFilterScnrs.SelectedIndex = 0;
            cmbBeep.SelectedIndex = 0;
            FillSCdcHostMode();
            cmbSCdcSHostMode.SelectedIndex = 0; //USB-IBMHID
            cmbLed.SelectedIndex = 0;
            cmbImageSize.SelectedIndex = 1;
            cmbDefaultOption.SelectedIndex = 0;
            cmbProtocol.SelectedIndex = 0;
            cmbScannerType.SelectedIndex = 0;
            cmbEncoding.SelectedIndex = 0;
            InitISO_15434_tab();

            tabCtrl.TabPages.Remove(tabSSW);
        }
        private void frmScannerApp_Load(object sender, EventArgs e)
        {
            GetLanguageConfigInfo();
        }
        private void frmCoreScannerApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        #region "DiscoverScanner"
        private void btnGetScanners_Click(object sender, EventArgs e)
        {
            performGetScanner();
        }

        private void cmbSlcrScnr_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstvScanners.Items[cmbSlcrScnr.SelectedIndex].Selected = true;
            lstvScanners_SelectedIndexChanged(sender, e);
        }

        private void lstvScanners_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            btnClearAll_Click(sender, null);
        }

        private void lstvScanners_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bEnable = false;
            rdoJPG.Checked = false;
            rdoTIFF.Checked = false;
            rdoBMP.Checked = false;
            string scnrMode = null;

            if (1 == lstvScanners.SelectedItems.Count)
            {
                int nSelIndex = lstvScanners.SelectedItems[0].Index;
                if (-1 < nSelIndex && m_nTotalScanners > nSelIndex)
                {
                    bEnable = true;
                    Scanner scanr = (Scanner)m_arScanners.GetValue(nSelIndex);
                    chkClaim.Checked = scanr.CLAIMED;
                }
                cmbSlcrScnr.SelectedIndex = nSelIndex;
            }
            SetControlsForScannerSelection(bEnable);

            if (lstvScanners.SelectedItems.Count > 0)
            {
                scnrMode = lstvScanners.SelectedItems[0].SubItems[1].Text;
                if (cmbMode.DataSource != null)
                    cmbMode.DataSource = null;

                List<KeyValuePair<string, string>> hostModes = scannerAction.GetHostModes();
                if (scnrMode.CompareTo("HID KEYBOARD") == 0)
                    hostModes.Remove(new KeyValuePair<string, string>("USB-SNAPI without Imaging", "XUA-45001-10"));
                else if (scnrMode.CompareTo("SSI") == 0 || scnrMode.CompareTo("SSI_BT") == 0 || scnrMode.CompareTo("NIXMODB") == 0)
                {
                    hostModes.RemoveRange(0, hostModes.Count);
                    hostModes.Add(new KeyValuePair<string, string>("", ""));
                }

                cmbMode.DataSource = new BindingSource(hostModes, null);
                cmbMode.DisplayMember = "Key";
                cmbMode.ValueMember = "Value";

                cmbMode.SelectedIndex = 0;
                if (scnrMode.CompareTo("SNAPI") == 0 || scnrMode.CompareTo("SSI") == 0 || scnrMode.CompareTo("SSI_BT") == 0)
                {
                    btnAbortImageXfer.Enabled = true;
                }
                else
                {
                    btnAbortImageXfer.Enabled = false;
                }
            }
        }
        /// <summary>
        /// Sends DEVICE_PULL_TRIGGER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPullTrigger_Click(object sender, EventArgs e)
        {
            PullTriggerClick(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_RELEASE_TRIGGER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReleaseTrigger_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(DEVICE_RELEASE_TRIGGER, "RELEASE_TRIGGER");
        }
        #endregion
        #region "BarCode"
        /// <summary>
        /// Sends FLUSH_MACROPDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlushMacroPdf_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(FLUSH_MACROPDF, "FLUSH_MACROPDF");
        }

        /// <summary>
        /// Abort Macro PDF
        /// </summary>
        private void btnAbortMacroPdf_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(ABORT_MACROPDF, "ABORT_MACROPDF");
        }

        /// <summary>
        /// Clear barcode event data populations
        /// </summary>
        private void btnBarcodeClear_Click(object sender, EventArgs e)
        {
            PerformClearBarcodeDataClick(sender, e);
        }
        /// <summary>
        /// Browse for a DADF script file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowseScript_Click(object sender, EventArgs e)
        {
            PerformBtnBrowseScriptClick(sender, e);
        }

        /// <summary>
        /// Open the simple Script dialog for input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScriptEditor_Click(object sender, EventArgs e)
        {
            PerformBtnScriptEditorClick(sender, e);
        }
        private void cmbEmulation_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iStatus = STATUS_FALSE;
            string outXml = String.Empty;

            try
            {
                string inXml = "<inArgs><cmdArgs><arg-int>";
                inXml += cmbEmulation.SelectedIndex.ToString();
                inXml += "</arg-int></cmdArgs></inArgs>";

                m_pCoreScanner.ExecCommand(KEYBOARD_EMULATOR_SET_LOCALE, ref inXml, out outXml, out iStatus);
            }
            catch (Exception)
            {
            }

            DisplayResult(iStatus, "KEYBOARD_EMULATOR_SET_LOCALE");
        }
        private void chkBoxEmulation_CheckedChanged(object sender, EventArgs e)
        {
            int iStatus = STATUS_FALSE;
            string outXml = String.Empty;

            try
            {
                string inXml = "<inArgs><cmdArgs><arg-bool>";
                if (chkBoxEmulation.Checked)
                {
                    inXml += "TRUE";
                    cmbEmulation.Enabled = true;
                }
                else
                {
                    inXml += "FALSE";
                    cmbEmulation.Enabled = false;
                }
                inXml += "</arg-bool></cmdArgs></inArgs>";
                m_pCoreScanner.ExecCommand(KEYBOARD_EMULATOR_ENABLE, ref inXml, out outXml, out iStatus);
            }
            catch (Exception)
            {
            }

            DisplayResult(iStatus, "KEYBOARD_EMULATOR_ENABLE");
        }
        #endregion
        #region "Image&Video"
        /// <summary>
        /// Sends DEVICE_CAPTURE_IMAGE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImage_Click(object sender, EventArgs e)
        {
            // PerformOnJpg(sender, e);//set default image type
            PerformBtnImageClick(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_CAPTURE_VIDEO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVideo_Click(object sender, EventArgs e)
        {
            PerformBtnVideoClick(sender, e);
        }

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(DEVICE_CAPTURE_BARCODE, "SET_BARCODE_MODE");
        }
        private void btnAbortImageXfer_Click(object sender, EventArgs e)
        {
            PerformBtnAbortImageXferClick(sender, e);
        }
        private void btnSveImge_Click(object sender, EventArgs e)
        {
            PerformBtnSveImgeClick(sender, e);
        }
        /// <summary>
        /// Sends DEVICE_SET_PARAMETERS with JPEG
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnJpg(object sender, EventArgs e)
        {
            if (rdoJPG.Checked)
                PerformOnImageType(JPEG_FILE_SELECTION);
        }

        /// <summary>
        /// Sends DEVICE_SET_PARAMETERS with TIFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTiff(object sender, EventArgs e)
        {
            if (rdoTIFF.Checked)
                PerformOnImageType(TIFF_FILE_SELECTION);
        }

        /// <summary>
        /// Sends DEVICE_SET_PARAMETERS with BMP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBmp(object sender, EventArgs e)
        {
            if (rdoBMP.Checked)
                PerformOnImageType(BMP_FILE_SELECTION);
        }
        /// <summary>
        /// Sends DEVICE_SET_PARAMETERS with VIDEOVIEWFINDER_ON/OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVideoViewFinderEnable(object sender, EventArgs e)
        {
            PerformOnVideoViewFinderEnable(sender, e);
        }

        #endregion
        #region "IDC"
        private void checkUseHID_CheckedChanged(object sender, EventArgs e)
        {
            PerformCheckUserHIDChanged(sender, e);
        }

        private void cmbSnapiParams_SelectedIndexChanged(object sender, EventArgs e)
        {
            PerformcmbSnapiParamsSelectedIndexChanged(sender, e);
        }

        private void btnSnapiGet_Click(object sender, EventArgs e)
        {
            PerformSnapiGetClick(sender, e);
        }

        private void btnSnapiSet_Click(object sender, EventArgs e)
        {
            PerformSnapiSetClick(sender, e);
        }

        private void btnSnapiStore_Click(object sender, EventArgs e)
        {
            PerformSnapiStoreClick(sender, e);
        }

        private void btnSaveIdc_Click(object sender, EventArgs e)
        {
            PerformSaveIdcClick(sender, e);
        }

        private void btnClearpbx_Click(object sender, EventArgs e)
        {
            PerformClearpbxClick(sender, e);
        }
        #endregion
        #region "Action"
        /// <summary>
        /// Sends DEVICE_SCAN_ENABLE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScannerEnable_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(DEVICE_SCAN_ENABLE, "SCAN_ENABLE");
        }

        /// <summary>
        /// Sends DEVICE_SCAN_DISABLE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScannerDisable_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(DEVICE_SCAN_DISABLE, "SCAN_DISABLE");
        }
        /// <summary>
        /// Sends DEVICE_AIM_ON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAimOn_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(DEVICE_AIM_ON, "AIM_ON");
        }

        /// <summary>
        /// Sends DEVICE_AIM_OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAimOff_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(DEVICE_AIM_OFF, "AIM_OFF");
        }
        /// <summary>
        /// Sends DEVICE_BEEP_CONTROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSoundBeeper_Click(object sender, EventArgs e)
        {
            try
            {
                ExecuteActionCommand(DEVICE_BEEP_CONTROL, "SET_ACTION", scannerAction.GetBeepXml(GetOnlyScannerIDXml(), cmbBeep.SelectedIndex));
            }
            catch
            {
                UpdateResults("Please select a beep from the list, default ONESHORTHI");
            }
        }
        private void btnSetReport_Click(object sender, EventArgs e)
        {
            SetReportAction();
        }
        private void btnRebootScanner_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(REBOOT_SCANNER, "REBOOT_SCANNER");
        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            ExecuteActionCommand(DISCONNECT_BT_SCANNER, "DISCONNECT_BT_SCANNER");
        }
        /// <summary>
        /// Sends DEVICE_LED_ON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLedOn_Click(object sender, EventArgs e)
        {
            PerformLEDAction(isOn: true);
        }

        /// <summary>
        /// Sends DEVICE_LED_OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLedOff_Click(object sender, EventArgs e)
        {
            PerformLEDAction(isOn: false);
        }
        /// <summary>
        /// Sends DEVICE_PAGE_MOTOR_CONTROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnablePageMotor_Click(object sender, EventArgs e)
        {
            EnablePagerMotorAction();
        }
        /// <summary>
        /// This method allows only digits to be entered as pager motor duration
        /// Sends DEVICE_PAGE_MOTOR_CONTROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPagerMotorDuration_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        #endregion
        #region "RSM"
        ///  RSM Commands   ////

        /// <summary>
        /// Sends RSM_ATTR_GETALL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetAll_Click(object sender, EventArgs e)
        {
            GetAllAttributes();
        }

        /// <summary>
        /// Sends RSM_ATTR_GETNEXT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetNext_Click(object sender, EventArgs e)
        {
            GetNextAttribute();
        }
        private void btnStore_Click(object sender, EventArgs e)
        {
            SetStoreAttributeValue(RSM_ATTR_STORE);
        }
        /// <summary>
        /// Sends RSM_ATTR_GET
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGet_Click(object sender, EventArgs e)
        {
            GetAttributes();
        }
        private void btnSet_Click(object sender, EventArgs e)
        {
            SetStoreAttributeValue(RSM_ATTR_SET);
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectAllAttributes();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            ClearAllRsmData();
        }
        #endregion
        #region "Advanced"
        private void btnFWBrowse_Click(object sender, EventArgs e)
        {
            PerformFWBrowseClick(sender, e);
        }
        /// <summary>
        /// Starts UpdateFWThreadFunction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFWUpdate_Click(object sender, EventArgs e)
        {
            PerformFWUpdateClick(sender, e);
        }
        private void btnLaunchNewFW_Click(object sender, EventArgs e)
        {
            ExecuteActionCommandOffline(START_NEW_FIRMWARE, "START_NEW_FIRMWARE");
        }
        /// <summary>
        /// Sends ABORT_UPDATE_FIRMWARE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbortFWUpdate_Click(object sender, EventArgs e)
        {
            PerformAbortFWUpdateClick(sender, e);
        }
        private void cmbFilterScnrs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (STR_FIND == btnGetScanners.Text)
            {
                return;
            }
            btnGetScanners_Click(sender, e);
        }
        private void btnWavFileBrowse_Click(object sender, EventArgs e)
        {
            PerformWavFileBrowseClick(sender, e);
        }

        private void btnWavFileUpload_Click(object sender, EventArgs e)
        {
            PerformWavFileUploadClick(sender, e);
        }
        private void btnEraseTone_Click(object sender, EventArgs e)
        {
            ExecuteActionCommandOffline(ERASE_DECODE_TONE, "ERASE_DECODE_TONE");
        }
        private void btnElectricFenceWavFileBrowse_Click(object sender, EventArgs e)
        {
            PerformElectricFenceWavFileBrowseClick(sender, e);
        }
        private void btnElectricFenceWavFileUpload_Click(object sender, EventArgs e)
        {
            PerformElectricFenceWavFileUploadClick(sender, e);
        }
        private void btnElectricFenceEraseTone_Click(object sender, EventArgs e)
        {
            ExecuteActionCommandOffline(ERASE_ELECTRIC_FENCE_CUSTOM_TONE, "ERASE_ELECTRIC_FENCE_CUSTOM_TONE");
        }
        #endregion
        #region "ScanToConnect"
        private void cmbScannerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            performScannerTypeSelectedIndexChanged(sender, e);
        }
        private void cmbProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            performProtocolSelectedIndexChanged(sender, e);
        }
        private void cmbHostName_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetPairingBarcode();
        }
        private void cmbDefaultOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetPairingBarcode();
        }
        private void cmbImageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetPairingBarcode();
        }
        private void btnSaveBarcode_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdBC = new SaveFileDialog();
            sfdBC.Filter = "jpeg files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
            var result = sfdBC.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                picBBarcode.Image.Save(sfdBC.FileName);
            }
            sfdBC.Dispose();
        }
        #endregion
        #region "Miscellaneous"
        /// <summary>
        /// Sends GET_VERSION
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSdkVersion_Click(object sender, EventArgs e)
        {
            PerformBtnSdkVersionClick(sender, e);
        }
        private void btnGetDevTopology_Click(object sender, EventArgs e)
        {
            PerformBtnGetDevTopologyClick(sender, e);
        }
        private void btnSCdcSwitchDevices_Click(object sender, EventArgs e)
        {
            CDCSwitchModeChangeAction();
        }
        #endregion
        #region "Scale"
        private void btnReadWeight_Click(object sender, EventArgs e)
        {
            PerformReadWeightClick(sender, e);
        }

        private void btnZeroScale_Click(object sender, EventArgs e)
        {
            ExecuteActionCommandOffline(SCALE_ZERO_SCALE, "SCALE_ZERO_SCALE");
        }

        private void btnSystemRest_Click(object sender, EventArgs e)
        {
            ExecuteActionCommandOffline(SCALE_SYSTEM_RESET, "SCALE_SYSTEM_RESET");
        }
        #endregion
        #region "Logs"
        private void btnClearXmlArea_Click(object sender, EventArgs e)
        {
            txtOutXml.Clear();
        }

        private void btnClearLogsArea_Click(object sender, EventArgs e)
        {
            m_nResultLineCount = 0;
            txtResults.Clear();
        }
        #endregion
    } 
}