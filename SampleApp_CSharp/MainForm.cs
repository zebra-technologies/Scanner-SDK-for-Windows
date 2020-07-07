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
using System.Text.RegularExpressions;
using CoreScanner;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp : Form
    {
        Image imgCapturedImage;
        Bitmap idcImage;
        CCoreScannerClass m_pCoreScanner;
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
            comboFilterScnrs.SelectedIndex = 0;
            comboBaudRate.SelectedIndex = 5; // 9600
            comboDataBits.SelectedIndex = 7; // 8
            comboParity.SelectedIndex = 0; // NOPARITY
            comboStpBits.SelectedIndex = 0; // ONESTOPBIT
            comboBeep.SelectedIndex = 0;
            comboSCdcSHostMode.SelectedIndex = 0; //USB-IBMHID
            cmbLed.SelectedIndex = 0;
            cmbImageSize.SelectedIndex = 1;
            cmbDefaultOption.SelectedIndex = 0;
            cmbProtocol.SelectedIndex = 0;
            cmbScannerType.SelectedIndex = 0;
            InitISO_15434_tab();

            tabCtrl.TabPages.Remove(tabSSW);
        }

        private void OnParameterBarcodeEvent(short eventType, int size, short imageFormat, ref object sfImageData, ref string pData)
        {
            if (eventType == 1)
            {
                Array arr = (Array)sfImageData;
                long len = arr.LongLength;
                byte[] byImage = new byte[len];
                arr.CopyTo(byImage, 0);

                MemoryStream ms = new MemoryStream();
                ms.Write(byImage, 0, byImage.Length);

                Image img = Image.FromStream(ms);
                if (picBBarcode.Width < img.Width)
                {
                    picBBarcode.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    picBBarcode.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                picBBarcode.Image = img;
                if (picBBarcode.Image != null)
                {
                    btnSaveBarcode.Enabled = true;
                }
                UpdateResults("ParameterBarcode Event fired");
            }
        }

        /// <summary>
        /// Calls when a Scanner Notification Event Received
        /// </summary>
        /// <param name="notificationType">Notification type</param>
        /// <param name="pScannerData">Scanner that notification sent</param>
        void OnScannerNotification(short notificationType, ref string pScannerData)
        {
            try
            {
                switch (notificationType)
                {
                    case IMAGE_MODE:
                        UpdateResults("Scanner Notification : Image Mode");
                        break;

                    case VIDEO_MODE:
                        UpdateResults("Scanner Notification : Video Mode");
                        break;

                    case BARCODE_MODE:
                        UpdateResults("Scanner Notification : Barcode Mode");
                        break;

                    case DEVICE_ENABLED:
                        UpdateResults("Scanner Notification : Device Enabled");
                        break;

                    case DEVICE_DISABLED:
                        UpdateResults("Scanner Notification : Device Disabled");
                        break;
                }

                UpdateOutXml(pScannerData);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Calls when PnPEvent is received
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="ppnpData">PnP info string</param>
        void OnPNPEvent(short eventType, ref string ppnpData)
        {
            try
            {
                UpdateOutXml(ppnpData);
                Scanner[] arScanr;
                string sStatus = String.Empty;
                m_xml.ReadXmlString_AttachDetachMulti(ppnpData, out arScanr, out sStatus);

                foreach (Scanner scnTmp in arScanr)
                {
                    if (null != scnTmp)
                    {
                        if ((SCANNER_ATTACHED == eventType && sStatus.Equals("1")) ||
                            (SCANNER_DETTACHED == eventType && sStatus.Equals("0")))
                        {
                            int nAdd = 0;
                            if (0 == m_nTotalScanners)//when there's no scanners in the list
                            {
                                nAdd = 1;
                            }
                            for (int i = 0; i < m_nTotalScanners; i++)
                            {
                                nAdd = 1;

                                Scanner scan = (Scanner)m_arScanners.GetValue(i);
                                if (scan.SCANNERID == scnTmp.SCANNERID)
                                {
                                    nAdd = 2;//already exist ...don't add
                                    if (SCANNER_ATTACHED == eventType)
                                    {
                                    }
                                    else if (SCANNER_DETTACHED == eventType)
                                    {
                                        UpdateResults("Scanner Detached Event fired");
                                        scan.ClearValues();
                                        if (i < (m_nTotalScanners - 1))//not the last item of array
                                        {
                                            for (int k = i; k < m_nTotalScanners; k++)
                                            {
                                                if (k == (m_nTotalScanners - 1))//last item of array
                                                {
                                                    m_arScanners.SetValue((Scanner)scan, k);//empty scanner object
                                                }
                                                else
                                                {
                                                    Scanner tmp = (Scanner)m_arScanners.GetValue(k + 1);
                                                    m_arScanners.SetValue((Scanner)tmp, k);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_arScanners.SetValue((Scanner)scan, i);//empty scanner object
                                        }
                                        m_nTotalScanners--;
                                        FillScannerList();
                                        SetControls();
                                    }
                                    break;
                                }
                            }
                            if (1 == nAdd)
                            {
                                UpdateResults("Scanner Attached Event fired");
                                FillDeviceProperties(scnTmp);
                                m_arScanners.SetValue((Scanner)scnTmp, m_nTotalScanners);
                                m_nTotalScanners++;
                                FillScannerList();
                                SetControls();
                            }
                        }
                    }
                }
                progressBarFWUpdate.Value = 0;
            }
            catch (Exception)
            {
            }
        }

        void On_BinaryDataEvent(short eventType, int size, short dataFormat, ref object sfBinaryData, ref string pScannerData)
        {
            Array arr = (Array)sfBinaryData;
            long len = arr.LongLength;
            byte[] byImage = new byte[len];
            arr.CopyTo(byImage, 0);
            txtDocCapDecodeData.Text = "";
            txtDocCapDecodeDataSymbol.Text = "";
            string channel = "";


            checkUseHID.CheckedChanged -= this.checkUseHID_CheckedChanged;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(pScannerData);

            string txchannel = xmlDoc.DocumentElement.GetElementsByTagName("channel").Item(0).InnerText;
            if (txchannel == "usb_HID")
            {
                checkUseHID.CheckState = CheckState.Checked;
                channel = "Data transfered through HID channel";

            }
            else if (txchannel == "usb_BULK")
            {
                checkUseHID.CheckState = CheckState.Unchecked;
                channel = "Data transfered through Bulk channel";
            }
            checkUseHID.CheckedChanged += this.checkUseHID_CheckedChanged;


            UpdateResults("Binary Data Event Fired. " + channel);

            if (dataFormat == 0xB5)
            {
                m_docCapMsg.ParseMessage(new UnknownBulkEventArg(byImage));
            }
            else if (dataFormat == 0x69)
            {
                m_docCapMsg.ParseMessage(new DecodeData(DecodeData.CodeTypes.SignatureCapture, byImage));
            }
        }
        void OnDocCapDecode(object sender, EventArgs e)
        {
            DecodeData d = e as DecodeData;

            if (txtDocCapDecodeData.InvokeRequired)
            {
                txtDocCapDecodeData.Invoke(new MethodInvoker(delegate
                {
                    txtDocCapDecodeData.Text = d.Text;
                }));
            }

            if (txtDocCapDecodeDataSymbol.InvokeRequired)
            {
                txtDocCapDecodeDataSymbol.Invoke(new MethodInvoker(delegate
                {
                    txtDocCapDecodeDataSymbol.Text = d.CodeType.ToString();
                }));
            }

        }

        void OnDocCapImage(object sender, DocCapImageArgs e)
        {

            //throw new Exception("The method or operation is not implemented.");
            try
            {
                // Make a bitmap for display and to extract information from it
                //this.m_rawData = ube.Data;
                MemoryStream ms = new MemoryStream(e.ImgData);
                Bitmap bm = new Bitmap(ms);
                idcImage = bm;
                this.pbxISO15434Image.Image = bm;
                if (pbxISO15434Image.MaximumSize.Height < bm.Height || pbxISO15434Image.MaximumSize.Width < bm.Width)
                {
                    pbxISO15434Image.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    pbxISO15434Image.SizeMode = PictureBoxSizeMode.Normal;
                }
                ms.Dispose();
            }
            catch
            {
                throw new Exception("Unable to convert raw bytes to bitmap");
            }

            if(idcImage != null)
            {
                btnSaveIdc.Enabled = true; ;
            }
            else
            {
                btnSaveIdc.Enabled = false;
            }
        }

        private void FillDeviceProperties(Scanner scnTmp)
        {
            string inXml = "<inArgs><scannerID>" +
                    scnTmp.SCANNERID +
                    "</scannerID><cmdArgs>" +
                    "<arg-xml>" +
                    "<attrib_list>20004</attrib_list>" +
                    "</arg-xml>" +
                    "</cmdArgs>" +
                    "</inArgs>";

            int opCode = RSM_ATTR_GET;
            string outXml = "";
            int status = STATUS_FALSE;
            ExecCmd(opCode, ref inXml, out outXml, out status);

            if (STATUS_SUCCESS == status)
            {
                scnTmp.SCANNERFIRMWARE = m_xml.GetAttribXMLValue(outXml);
            }

            inXml = "<inArgs><scannerID>" +
                    scnTmp.SCANNERID +
                    "</scannerID><cmdArgs>" +
                    "<arg-xml>" +
                    "<attrib_list>535</attrib_list>" +
                    "</arg-xml>" +
                    "</cmdArgs>" +
                    "</inArgs>";

            opCode = RSM_ATTR_GET;
            outXml = "";
            status = STATUS_FALSE;
            ExecCmd(opCode, ref inXml, out outXml, out status);

            if (STATUS_SUCCESS == status)
            {
                scnTmp.SCANNERMNFDATE = m_xml.GetAttribXMLValue(outXml);
            }
        }

        /// <summary>
        /// Calls when CommandResponseEvent is received
        /// </summary>  
        /// <param name="eventType">Type of event</param>
        /// <param name="prspData">CommandResponse info string</param>
        void OnCommandResponseEvent(short status, ref string prspData)
        {
            try
            {
                UpdateResults("CommandResponse Event fired");
                UpdateOutXml(prspData);
                Scanner scanr = null;
                int nIndex = -1;
                int nAttrCount = 0;
                int nOpCode = -1;
                m_xml.ReadXmlString_RsmAttr(prspData, m_arScanners, out scanr, out nIndex, out nAttrCount, out nOpCode);
                switch (nOpCode)
                {
                    case RSM_ATTR_GETALL:
                        FillRsmList_Numbers(nAttrCount, scanr);
                        UpdateResults("RSM_ATTR_GETALL Event");
                        break;

                    case RSM_ATTR_GET:
                        FillRsmList_Attributes(scanr);
                        UpdateResults("RSM_ATTR_GET Event");
                        break;

                    case RSM_ATTR_GETNEXT:
                        FillRsmList_Attribute(scanr, nIndex);
                        UpdateResults("RSM_ATTR_GETNEXT Event");
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Calls when ScanRMDEvent is received
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="prmdData">ScanRMD info string</param>
        void OnScanRMDEvent(short eventType, ref string prmdData)
        {
            try
            {
                int iMax = 0, iProgress = 0, iErrorStatus = 0;
                string strStatus = String.Empty, strScannerID = String.Empty;

                UpdateOutXml(prmdData);

                switch (eventType)
                {
                    case SCANNER_UF_SESS_START:
                        //int nMax = 0, nProgress = 0;
                        //string csStatus = "", csScannerID = "";
                        m_xml.ReadXmlString_FW(prmdData, out iMax, out iProgress, out strStatus, out strScannerID);
                        if (GetSelectedScannerID().Equals(strScannerID))
                        {
                            if (progressBarFWUpdate.InvokeRequired)
                            {
                                progressBarFWUpdate.Invoke(new MethodInvoker(delegate
                                {
                                    progressBarFWUpdate.Minimum = 0;
                                    progressBarFWUpdate.Maximum = iMax;
                                }));
                            }
                            UpdateResults("ScanRMD Event fired - SCANNER_UF_SESS_START Max = " + iMax.ToString());
                        }
                        break;

                    case SCANNER_UF_DL_START:
                        //int nMax = 0, nProgress = 0;
                        //string csStatus = "", csScannerID = "";
                        m_xml.ReadXmlString_FW(prmdData, out iMax, out iProgress, out strStatus, out strScannerID);
                        if (GetSelectedScannerID().Equals(strScannerID))
                        {
                            if (progressBarFWUpdate.InvokeRequired)
                            {
                                progressBarFWUpdate.Invoke(new MethodInvoker(delegate
                                {
                                    progressBarFWUpdate.Value = 0;
                                }));
                            }

                            UpdateResults("ScanRMD Event fired - SCANNER_UF_DL_START");
                        }
                        break;

                    case SCANNER_UF_DL_PROGRESS:
                        //int nMax = 0, nProgress = 0;
                        //string csStatus = "", csScannerID = "";
                        m_xml.ReadXmlString_FW(prmdData, out iMax, out iProgress, out strStatus, out strScannerID);
                        if (GetSelectedScannerID().Equals(strScannerID))
                        {
                            if (progressBarFWUpdate.InvokeRequired)
                            {
                                progressBarFWUpdate.Invoke(new MethodInvoker(delegate
                                {
                                    progressBarFWUpdate.Value = iProgress;
                                }));
                            }
                        }
                        break;

                    case SCANNER_UF_DL_END:
                        //int nErrorStatus = 0, nProgress = 0;
                        //string csStatus = "", csScannerID = "";
                        m_xml.ReadXmlString_FW(prmdData, out iErrorStatus, out iProgress, out strStatus, out strScannerID);
                        if (GetSelectedScannerID().Equals(strScannerID))
                        {
                            UpdateResults("ScanRMD Event fired - SCANNER_UF_DL_END");
                        }
                        break;

                    case SCANNER_UF_SESS_END:
                        //int nErrorStatus = 0, nProgress = 0;
                        //string csStatus = "", csScannerID = "";
                        m_xml.ReadXmlString_FW(prmdData, out iErrorStatus, out iProgress, out strStatus, out strScannerID);
                        if (GetSelectedScannerID().Equals(strScannerID))
                        {
                            if (progressBarFWUpdate.InvokeRequired)
                            {
                                progressBarFWUpdate.Invoke(new MethodInvoker(delegate
                                {
                                    progressBarFWUpdate.Value = 0;
                                }));
                            }

                            UpdateResults("ScanRMD Event fired - SCANNER_UF_SESS_END");
                            btnFWUpdate.Enabled = true;
                        }
                        break;

                    case SCANNER_UF_STATUS:
                        //int nErrorStatus = 0, nProgress = 0;
                        //string csStatus = "", csScannerID = "";
                        m_xml.ReadXmlString_FW(prmdData, out iErrorStatus, out iProgress, out strStatus, out strScannerID);
                        UpdateResults("ScanRMD Event fired - SCANNER_UF_STATUS " + strStatus);
                        break;
                }
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
            }
        }

        /// <summary>
        /// Calls when IOEvent is received
        /// </summary>
        /// <param name="type">Type of event</param>
        /// <param name="data">IO info</param>
        void OnIOEvent(short type, byte data)
        {
            try
            {
                UpdateResults("IO Event fired");
            }
            catch (Exception)
            {
            }
        }

        private void DisplayResult(int status)
        {
            switch (status)
            {
                case STATUS_SUCCESS:
                    UpdateResults("Command success.");
                    break;
                case STATUS_LOCKED:
                    UpdateResults("Command failed. Device is locked by another application.");
                    break;
                default:
                    UpdateResults("Command failed. Error:" + status.ToString());
                    break;
            }
        }

        private void DisplayResult(int status, string strCmd)
        {
            switch (status)
            {
                case STATUS_SUCCESS:
                    UpdateResults(strCmd + " - Command success.");
                    break;
                case STATUS_LOCKED:
                    UpdateResults(strCmd + " - Command failed. Device is locked by another application.");
                    break;
                case ERROR_CDC_SCANNERS_NOT_FOUND:
                    UpdateResults(strCmd + " - No CDC device found. - Error:" + status.ToString());
                    break;
                case ERROR_UNABLE_TO_OPEN_CDC_COM_PORT:
                    UpdateResults(strCmd + " - Unable to open CDC port. - Error:" + status.ToString());
                    break;
                default:
                    UpdateResults(strCmd + " - Command failed. Error:" + status.ToString());
                    break;
            }
        }

        private void UpdateResults(string strOut)
        {
            m_nResultLineCount++;

            if (txtResults.InvokeRequired)
            {
                txtResults.Invoke(new MethodInvoker(delegate
                {
                    txtResults.AppendText(m_nResultLineCount.ToString() + ". " + strOut + Environment.NewLine);
                }));
            }
            else
            {
                txtResults.AppendText(m_nResultLineCount.ToString() + ". " + strOut + Environment.NewLine);
            }

            toolStripStatusLbl.Text = strOut + "        ";
        }

        private void UpdateOutXml(string strOut)
        {
            if (txtOutXml.InvokeRequired)
            {
                txtOutXml.Invoke(new MethodInvoker(delegate
                {
                    txtOutXml.Text = IndentXmlString(strOut);
                }));
            }
            else
            {
                txtOutXml.Text = IndentXmlString(strOut);
            }
        }

        private static string IndentXmlString(string strXml)
        {
            string outXml = string.Empty;
            MemoryStream ms = new MemoryStream();
            // Create a XMLTextWriter that will send its output to a memory stream (file)
            XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode);
            XmlDocument doc = new XmlDocument();

            try
            {
                // Load the unformatted XML text string into an instance
                // of the XML Document Object Model (DOM)
                doc.LoadXml(strXml);

                // Set the formatting property of the XML Text Writer to indented
                // the text writer is where the indenting will be performed
                xtw.Formatting = Formatting.Indented;

                // write dom xml to the xmltextwriter
                doc.WriteContentTo(xtw);
                // Flush the contents of the text writer
                // to the memory stream, which is simply a memory file
                xtw.Flush();

                // set to start of the memory stream (file)
                ms.Seek(0, SeekOrigin.Begin);
                // create a reader to read the contents of
                // the memory stream (file)
                StreamReader sr = new StreamReader(ms);
                // return the formatted string to caller
                return sr.ReadToEnd();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void btnGetScanners_Click(object sender, EventArgs e)
        {
            performGetScanner();
        }

        private void performGetScanner()
        {
            FilterScannerList();
            MakeConnectCtrl();
            registerForEvents();
            ShowScanners();
            GetPairingBarcode();
        }

        private void SetControlsForScannerSelection(bool bEnable)
        {
            grpScannerProp.Enabled = bEnable;
            grpTrigger.Enabled = bEnable;

            txtBarcode.Enabled = bEnable;
            grpImageVideo.Enabled = bEnable;
            grpScnActions.Enabled = bEnable;

            grpRSM.Enabled = bEnable; //get line disable, select line enable
            gbAdvanced.Enabled = bEnable;
            grpFrmWrUpdate.Enabled = bEnable;
            grpCustomDecodeTone.Enabled = bEnable;
            grpBaudrate.Enabled = bEnable;
            grpHVS.Enabled = bEnable;
        }

        private string GetScannerIDXml()
        {
            string strInXml = "";
            if (0 < lstvScanners.Items.Count && 1 == lstvScanners.SelectedItems.Count && -1 < lstvScanners.SelectedItems[0].Index)
            {
                strInXml = "<inArgs>" +
                                "<scannerID>" + GetSelectedScannerID() + "</scannerID>" +
                                "</inArgs>";
            }
            return strInXml;
        }

        private string GetOnlyScannerIDXml()
        {
            string strInXml = "";
            if (0 < lstvScanners.Items.Count &&
                 1 == lstvScanners.SelectedItems.Count &&
                 -1 < lstvScanners.SelectedItems[0].Index)
            {
                strInXml = "<scannerID>" + GetSelectedScannerID() + "</scannerID>";
            }
            return strInXml;
        }

        /// <summary>
        /// Sends ExecCommand(Sync) or ExecCommandAsync
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="inXml"></param>
        /// <param name="outXml"></param>
        /// <param name="status"></param>
        private void ExecCmd(int opCode, ref string inXml, out string outXml, out int status)
        {
            outXml = "";
            status = STATUS_FALSE;
            if (m_bSuccessOpen)
            {
                try
                {
                    if (!chkAsync.Checked)
                    {
                        m_pCoreScanner.ExecCommand(opCode, ref inXml, out outXml, out status);
                    }
                    else
                    {
                        m_pCoreScanner.ExecCommandAsync(opCode, ref inXml, out status);
                    }
                }
                catch (Exception ex)
                {
                    DisplayResult(status, "EXEC_COMMAND");
                    UpdateResults("..." + ex.Message.ToString());
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
            if (IsScannerConnected())
            {
                pbxImageVideo.Image = null;
                pbxImageVideo.Refresh();
                txtOutXml.Clear();

                string inXml = GetScannerIDXml();
                int opCode = DEVICE_PULL_TRIGGER;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "PULL_TRIGGER");
            }
        }

        /// <summary>
        /// Sends DEVICE_RELEASE_TRIGGER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReleaseTrigger_Click(object sender, EventArgs e)
        {
            if (IsScannerConnected())
            {
                string inXml = GetScannerIDXml();
                int opCode = DEVICE_RELEASE_TRIGGER;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "RELEASE_TRIGGER");
            }
        }

        /// <summary>
        /// Sends DEVICE_SCAN_ENABLE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScannerEnable_Click(object sender, EventArgs e)
        {
            OnEnableScanner();
        }

        /// <summary>
        /// Sends DEVICE_SCAN_DISABLE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScannerDisable_Click(object sender, EventArgs e)
        {
            OnDisableScanner();
        }

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

        /// <summary>
        /// Sends DEVICE_SET_PARAMETERS with VIDEOVIEWFINDER_ON/OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVideoViewFinderEnable(object sender, EventArgs e)
        {
            PerformOnVideoViewFinderEnable(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_SET_PARAMETERS with JPEG
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnJpg(object sender, EventArgs e)
        {
            PerformOnJpg(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_SET_PARAMETERS with TIFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTiff(object sender, EventArgs e)
        {
            PerformOnTiff(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_SET_PARAMETERS with BMP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBmp(object sender, EventArgs e)
        {
            PerformOnBmp(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_AIM_ON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAimOn_Click(object sender, EventArgs e)
        {
            PerformBtnAimOnClick(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_AIM_OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAimOff_Click(object sender, EventArgs e)
        {
            PerformBtnAimOffClick(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_BEEP_CONTROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSoundBeeper_Click(object sender, EventArgs e)
        {
            PerformBtnSoundBeeperClick(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_LED_ON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLedOn_Click(object sender, EventArgs e)
        {
            PerformBtnLedOnClick(sender, e);
        }

        /// <summary>
        /// Sends DEVICE_LED_OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLedOff_Click(object sender, EventArgs e)
        {
            PerformBtnLedOffClick(sender, e);
        }

        private void buttonFWBrowse_Click(object sender, EventArgs e)
        {
            PerformButtonFWBrowseClick(sender, e);
        }

        /// <summary>
        /// Starts UpdateFWThreadFunction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFWUpdate_Click(object sender, EventArgs e)
        {
            PerformbtnFWUpdateClick(sender, e);
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
            string inXml = "<inArgs>" +
                              GetOnlyScannerIDXml() +
                              "<cmdArgs>" +
                              "<arg-string>" + txtFWFile.Text + "</arg-string>" +
                              strblk +
                              "</cmdArgs>" +
                              "</inArgs>";
            int opCode = UPDATE_FIRMWARE;

            if (txtFWFile.Text.EndsWith(".SCNPLG"))
            {
                opCode = UPDATE_FIRMWARE_FROM_PLUGIN;
            }

            string outXml = "";
            int status = STATUS_FALSE;
            ExecCmd(opCode, ref inXml, out outXml, out status);
            DisplayResult(status, "UPDATE_FIRMWARE");
        }

        /// <summary>
        /// Sends ABORT_UPDATE_FIRMWARE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbortFWUpdate_Click(object sender, EventArgs e)
        {
            PerformBtnAbortFWUpdateClick(sender, e);
        }

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

        /// <summary>
        /// Sends GET_VERSION
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSdkVersion_Click(object sender, EventArgs e)
        {
            PerformBtnSdkVersionClick(sender, e);
        }

        /// <summary>
        /// Sends FLUSH_MACROPDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlushMacroPdf_Click(object sender, EventArgs e)
        {
            PerformBtnFlushMacroPdfClick(sender, e);
        }

        private void btnAbortMacroPdf_Click(object sender, EventArgs e)
        {
            PerformBtnAbortMacroPdfClick(sender, e);
        }

        private void btnClearStatusArea_Click(object sender, EventArgs e)
        {
            m_nResultLineCount = 0;
            txtResults.Clear();
        }

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
        /// Sends RSM_ATTR_GET
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGet_Click(object sender, EventArgs e)
        {
            GetAttributes();
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

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectAllAttributes();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            ClearAllRsmData();
        }

        private void btnClearAllValues_Click(object sender, EventArgs e)
        {
            PerformBtnClearAllValuesClick(sender, e);
        }

        private void btnClearValue_Click(object sender, EventArgs e)
        {
            PerformBtnClearValueClick(sender, e);
        }

        private string GetRegUnregIDs(out int nEvents)
        {
            string strIDs = "";
            nEvents = NUM_SCANNER_EVENTS;
            strIDs = SUBSCRIBE_BARCODE.ToString();
            strIDs += "," + SUBSCRIBE_IMAGE.ToString();
            strIDs += "," + SUBSCRIBE_VIDEO.ToString();
            strIDs += "," + SUBSCRIBE_RMD.ToString();
            strIDs += "," + SUBSCRIBE_PNP.ToString();
            strIDs += "," + SUBSCRIBE_OTHER.ToString();
            return strIDs;
        }

        /// <summary>
        /// Is a scanner selected
        /// </summary>
        /// <returns></returns>
        private bool IsScannerSelected()
        {
            if (0 < lstvScanners.Items.Count &&
                 1 == lstvScanners.SelectedItems.Count &&
                 -1 < lstvScanners.SelectedItems[0].Index)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is Open successful & scanners available
        /// </summary>
        /// <returns></returns>
        private bool IsScannerConnected()
        {
            return (m_bSuccessOpen && (0 < m_nTotalScanners) && IsScannerSelected());
        }

        /// <summary>
        /// Is Open successful
        /// </summary>
        /// <returns></returns>
        private bool IsMotoConnected()
        {
            return m_bSuccessOpen;
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
                combSlcrScnr.SelectedIndex = nSelIndex;
            }
            SetControlsForScannerSelection(bEnable);

            if (lstvScanners.SelectedItems.Count > 0)
            {
                scnrMode = lstvScanners.SelectedItems[0].SubItems[1].Text;
                cmbMode.Items.Clear();
                if (scnrMode.CompareTo("HID KEYBOARD") == 0)
                {
                    cmbMode.Items.Add("USB-HIDKB");
                    cmbMode.Items.Add("USB-IBMHID");
                    cmbMode.Items.Add("USB-OPOS");
                    cmbMode.Items.Add("USB-SNAPI with Imaging");
                    //cmbMode.Items.Add("USB-SNAPI without Imaging");
                    cmbMode.Items.Add("USB-CDC Serial Emulation");
                    cmbMode.Items.Add("USB-SSI over CDC");
                    cmbMode.Items.Add("USB-IBMTT");
                }
                //else if (scnrMode.CompareTo("IBM HANDHELD") == 0)
                else if (scnrMode.CompareTo("SSI") == 0 || scnrMode.CompareTo("SSI_BT") == 0 || scnrMode.CompareTo("NIXMODB") == 0)
                {
                    //cmbMode.Items.Add("USB-HIDKB");
                    //cmbMode.Items.Add("USB-IBMHID");
                    //cmbMode.Items.Add("USB-OPOS");
                    //cmbMode.Items.Add("USB-SNAPI with Imaging");
                    //cmbMode.Items.Add("USB-SNAPI without Imaging");
                    //cmbMode.Items.Add("USB-CDC Serial Emulation");
                    //cmbMode.Items.Add("USB-SSI over CDC");
                    //cmbMode.Items.Add("USB-IBMTT");
                    cmbMode.Items.Add("");

                }
                else
                {
                    cmbMode.Items.Add("USB-HIDKB");
                    cmbMode.Items.Add("USB-IBMHID");
                    cmbMode.Items.Add("USB-OPOS");
                    cmbMode.Items.Add("USB-SNAPI with Imaging");
                    cmbMode.Items.Add("USB-SNAPI without Imaging");
                    cmbMode.Items.Add("USB-CDC Serial Emulation");
                    cmbMode.Items.Add("USB-SSI over CDC");
                    cmbMode.Items.Add("USB-IBMTT");
                }
                cmbMode.SelectedIndex = 0;
                if (scnrMode.CompareTo("SNAPI") == 0 || scnrMode.CompareTo("SSI") == 0 || scnrMode.CompareTo("SSI_BT") == 0)
                {
                    btnPullTrigger.Enabled = true;
                    btnReleaseTrigger.Enabled = true;
                    btnAbortImageXfer.Enabled = true;
                }
                else
                {
                    btnPullTrigger.Enabled = false;
                    btnReleaseTrigger.Enabled = false;
                    btnAbortImageXfer.Enabled = false;
                }
            }
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

        private void frmCoreScannerApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        private void btnAbortImageXfer_Click(object sender, EventArgs e)
        {
            PerformBtnAbortImageXferClick(sender, e);
        }

        private string GetSelectedScannerID()
        {
            string strScannerID = String.Empty;

            if (lstvScanners.InvokeRequired)
            {
                lstvScanners.Invoke(new MethodInvoker(delegate
                {
                    strScannerID = lstvScanners.SelectedItems[0].SubItems[0].Text;
                }));
            }
            else
            {
                strScannerID = lstvScanners.SelectedItems[0].SubItems[0].Text; ;
            }

            return strScannerID;
        }

        private void btnSetReport_Click(object sender, EventArgs e)
        {
            PerformBtnSetReportClick(sender, e);
        }

        private void btnGetDevTopology_Click(object sender, EventArgs e)
        {
            PerformBtnGetDevTopologyClick(sender, e);
        }

        private void btnRebootScanner_Click(object sender, EventArgs e)
        {
            PerformBtnRebootScannerClick(sender, e);
        }

        private void btnStartNewFW_Click(object sender, EventArgs e)
        {
            PerformBtnStartNewFWClick(sender, e);
        }

        private void btnClearXmlArea_Click(object sender, EventArgs e)
        {
            txtOutXml.Clear();
        }

        private string RemoveWhiteSpaces(string dirtyXml)
        {
            Regex regex = new Regex(@"\s*");
            return regex.Replace(dirtyXml, "");
        }

        private void btnBarcodeClear_Click(object sender, EventArgs e)
        {
            PerformClearBarcodeDataClick(sender, e);
        }

        private void lstvScanners_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            btnClearAll_Click(sender, null);
        }

        private void btnClearLogsArea_Click(object sender, EventArgs e)
        {
            m_nResultLineCount = 0;
            txtResults.Clear();
        }

        private void combSlcrScnr_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstvScanners.Items[combSlcrScnr.SelectedIndex].Selected = true;
            lstvScanners_SelectedIndexChanged(sender, e);
        }

        private void SetOpenEnable()
        {
            bool bRet = false;

            for (int i = 0; i < TOTAL_SCANNER_TYPES; i++)
            {
                if (m_arSelectedTypes[i])
                {
                    bRet = true;
                    break;
                }
            }
            btnGetScanners.Enabled = bRet;
        }

        private void comboFilterScnrs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (STR_FIND == btnGetScanners.Text)
            {
                return;
            }
            btnGetScanners_Click(sender, e);
        }

        private void frmScannerApp_Load(object sender, EventArgs e)
        {
            GetLanguageConfigInfo();
        }

        private void btnSetSrilInfce_Click(object sender, EventArgs e)
        {
            PerformBtnSetSrilInfceClick(sender, e);
        }

        private void btnSveImge_Click(object sender, EventArgs e)
        {
            PerformBtnSveImgeClick(sender, e);
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            SetStoreAttributeValue(RSM_ATTR_SET);
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            SetStoreAttributeValue(RSM_ATTR_STORE);
        }

        private void FilterScannerList()
        {
            for (int index = 0; index < TOTAL_SCANNER_TYPES; index++)
            {
                m_arSelectedTypes[index] = false;
            }

            switch (comboFilterScnrs.SelectedIndex)
            {
                case 0:
                    m_arSelectedTypes[SCANNER_TYPES_ALL - 1] = true;
                    break;

                case 1:
                    m_arSelectedTypes[SCANNER_TYPES_HIDKB - 1] = true;
                    break;

                case 2:
                    m_arSelectedTypes[SCANNER_TYPES_IBMHID - 1] = true;
                    break;

                case 3:
                    m_arSelectedTypes[SCANNER_TYPES_SNAPI - 1] = true;
                    break;
            }
        }

        private void MakeConnectCtrl()
        {
            if (STR_FIND == btnGetScanners.Text)
            {
                Connect();
                if (m_bSuccessOpen)
                {
                    SetControls();
                    btnGetScanners.Text = STR_REFRESH;
                }
            }
            else if (STR_REFRESH == btnGetScanners.Text)
            {
                Disconnect();
                if (!m_bSuccessOpen)
                {
                    SetControls();
                    btnGetScanners.Text = STR_FIND;
                    ClearAllRsmData();
                }
                Connect();
                if (m_bSuccessOpen)
                {
                    SetControls();
                    btnGetScanners.Text = STR_REFRESH;
                }
            }
        }

        /// <summary>
        /// Calls Open command
        /// </summary>
        private void Connect()
        {
            if (m_bSuccessOpen)
            {
                return;
            }
            int appHandle = 0;
            GetSelectedScannerTypes();
            int status = STATUS_FALSE;

            try
            {
                m_pCoreScanner.Open(appHandle, m_arScannerTypes, m_nNumberOfTypes, out status);
                DisplayResult(status, "OPEN");
                if (STATUS_SUCCESS == status)
                {
                    m_bSuccessOpen = true;
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Error OPEN - " + exp.Message, APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (STATUS_SUCCESS == status)
                {
                    SetControls();
                }
            }
        }

        /// <summary>
        /// Calls Close command
        /// </summary>
        private void Disconnect()
        {
            if (m_bSuccessOpen)
            {
                int appHandle = 0;
                int status = STATUS_FALSE;
                try
                {
                    m_pCoreScanner.Close(appHandle, out status);
                    DisplayResult(status, "CLOSE");
                    if (STATUS_SUCCESS == status)
                    {
                        m_bSuccessOpen = false;
                        lstvScanners.Items.Clear();
                        combSlcrScnr.Items.Clear();
                        m_nTotalScanners = 0;
                        InitScannersCount();
                        UpdateScannerCountLabels();
                        SetControls();
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("CLOSE Error - " + exp.Message, APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetControls()
        {
            bool bEnable = IsMotoConnected();
            filterScnrs.Enabled = bEnable;
            grpScanners.Enabled = bEnable;
            grpAsync.Enabled = bEnable;
            grpScannerProp.Enabled = bEnable;
            grpTrigger.Enabled = bEnable;
            grpboxBarcodeLbl.Enabled = bEnable;
            txtBarcode.Enabled = bEnable;
            grpImageVideo.Enabled = bEnable;
            grpScnActions.Enabled = bEnable;
            grpRSM.Enabled = bEnable;
            gbAdvanced.Enabled = bEnable;
            grpFrmWrUpdate.Enabled = bEnable;
            grpCustomDecodeTone.Enabled = bEnable;
            grpMiscOther.Enabled = bEnable;
            grpScale.Enabled = bEnable;
            grpIDC.Enabled = bEnable;
            grpScan2Connect.Enabled = bEnable;
            pbxImageVideo.Image = null;
        }

        private void GetSelectedScannerTypes()
        {
            m_nNumberOfTypes = 0;
            for (int index = 0, k = 0; index < TOTAL_SCANNER_TYPES; index++)
            {
                if (m_arSelectedTypes[index])
                {
                    m_nNumberOfTypes++;
                    switch (index + 1)
                    {
                        case SCANNER_TYPES_ALL:
                            m_arScannerTypes[k++] = SCANNER_TYPES_ALL;
                            return;

                        case SCANNER_TYPES_SNAPI:
                            m_arScannerTypes[k++] = SCANNER_TYPES_SNAPI;
                            break;

                        case SCANNER_TYPES_SSI:
                            m_arScannerTypes[k++] = SCANNER_TYPES_SSI;
                            break;

                        case SCANNER_TYPES_NIXMODB:
                            m_arScannerTypes[k++] = SCANNER_TYPES_NIXMODB;
                            break;

                        case SCANNER_TYPES_RSM:
                            m_arScannerTypes[k++] = SCANNER_TYPES_RSM;
                            break;

                        case SCANNER_TYPES_IMAGING:
                            m_arScannerTypes[k++] = SCANNER_TYPES_IMAGING;
                            break;

                        case SCANNER_TYPES_IBMHID:
                            m_arScannerTypes[k++] = SCANNER_TYPES_IBMHID;
                            break;

                        case SCANNER_TYPES_HIDKB:
                            m_arScannerTypes[k++] = SCANNER_TYPES_HIDKB;
                            break;

                        case SCALE_TYPES_SSI_BT:
                            m_arScannerTypes[k++] = SCALE_TYPES_SSI_BT;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void registerForEvents()
        {
            if (IsMotoConnected())
            {
                int nEvents = 0;
                string strEvtIDs = GetRegUnregIDs(out nEvents);
                string inXml = "<inArgs>" +
                                    "<cmdArgs>" +
                                    "<arg-int>" + nEvents + "</arg-int>" +
                                    "<arg-int>" + strEvtIDs + "</arg-int>" +
                                    "</cmdArgs>" +
                                    "</inArgs>";

                int opCode = REGISTER_FOR_EVENTS;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "REGISTER_FOR_EVENTS");
            }
        }

        /// <summary>
        /// Calls GetScanners command
        /// </summary>
        private void ShowScanners()
        {
            int opCode = CLAIM_DEVICE;
            string inXml = String.Empty;
            string outXml = "";
            int status = STATUS_FALSE;
            lstvScanners.Items.Clear();
            combSlcrScnr.Items.Clear();

            m_arScanners.Initialize();
            if (m_bSuccessOpen)
            {
                m_nTotalScanners = 0;
                short numOfScanners = 0;
                int nScannerCount = 0;
                string outXML = "";
                int[] scannerIdList = new int[MAX_NUM_DEVICES];
                try
                {
                    m_pCoreScanner.GetScanners(out numOfScanners, scannerIdList, out outXML, out status);
                    DisplayResult(status, "GET_SCANNERS");
                    if (STATUS_SUCCESS == status)
                    {
                        m_nTotalScanners = numOfScanners;
                        m_xml.ReadXmlString_GetScanners(outXML, m_arScanners, numOfScanners, out nScannerCount);
                        for (int index = 0; index < m_arScanners.Length; index++)
                        {
                            for (int i = 0; i < claimlist.Count; i++)
                            {
                                if (string.Compare(claimlist[i], m_arScanners[index].SERIALNO) == 0)
                                {
                                    Scanner objScanner = (Scanner)m_arScanners.GetValue(index);
                                    objScanner.CLAIMED = true;
                                }
                            }
                        }

                        FillScannerList();
                        UpdateOutXml(outXML);
                        for (int index = 0; index < m_nTotalScanners; index++)
                        {
                            Scanner objScanner = (Scanner)m_arScanners.GetValue(index);
                            string[] strItems = new string[] { "", "", "", "", "" };

                            inXml = "<inArgs><scannerID>" + objScanner.SCANNERID + "</scannerID></inArgs>";

                            for (int i = 0; i < claimlist.Count; i++)
                            {
                                if (string.Compare(claimlist[i], objScanner.SERIALNO) == 0)
                                {
                                    ExecCmd(opCode, ref inXml, out outXml, out status);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error GETSCANNERS - " + ex.Message, APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Populate the connected scanner list
        /// </summary>
        private void FillScannerList()
        {
            if (lstvScanners.InvokeRequired)
            {
                lstvScanners.Invoke(new MethodInvoker(delegate
                {
                    lstvScanners.Items.Clear();
                }));
            }
            else
            {
                lstvScanners.Items.Clear();
            }

            if (combSlcrScnr.InvokeRequired)
            {
                combSlcrScnr.Invoke(new MethodInvoker(delegate
                {
                    combSlcrScnr.Items.Clear();
                }));
            }
            else
            {
                combSlcrScnr.Items.Clear();
            }

            InitScannersCount();

            bool deviceFound = false;
            if (tabCtrl.TabPages.Contains(tabSSW))
            {
                if (tabCtrl.InvokeRequired)
                {
                    tabCtrl.Invoke(new MethodInvoker(delegate
                    {
                        tabCtrl.TabPages.Remove(tabSSW);
                    }));
                }
                else
                {
                    tabCtrl.TabPages.Remove(tabSSW);
                }                
            }

            for (int index = 0; index < m_nTotalScanners; index++)
            {
                Scanner objScanner = (Scanner)m_arScanners.GetValue(index);
                if (!deviceFound)
                {
                    deviceFound = objScanner.MODELNO.StartsWith("DS9908");
                }

                string[] strItems = new string[] { "", "", "", "", "" };

                if (combSlcrScnr.InvokeRequired)
                {
                    combSlcrScnr.Invoke(new MethodInvoker(delegate
                    {
                        combSlcrScnr.Items.Add(objScanner.SCANNERID + "        " + objScanner.MODELNO);
                    }));
                }
                else
                {
                    combSlcrScnr.Items.Add(objScanner.SCANNERID + "        " + objScanner.MODELNO);
                }

                switch (objScanner.SCANNERTYPE)
                {
                    case Scanner.SCANNER_SNAPI:
                        m_nArTotalScannersInType[0]++;
                        strItems = new string[] { objScanner.SCANNERID, objScanner.SCANNERTYPE, objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;

                    case Scanner.SCANNER_SSI:
                        m_nArTotalScannersInType[1]++;
                        strItems = new string[] { objScanner.SCANNERID, objScanner.SCANNERTYPE, objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;

                    case Scanner.SCANNER_IBMHID:
                        m_nArTotalScannersInType[2]++;
                        strItems = new string[] { objScanner.SCANNERID, "IBM HANDHELD", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;

                    case Scanner.SCANNER_OPOS:
                        m_nArTotalScannersInType[2]++;
                        strItems = new string[] { objScanner.SCANNERID, "USB OPOS", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;

                    case Scanner.SCANNER_NIXMODB:
                        m_nArTotalScannersInType[3]++;
                        strItems = new string[] { objScanner.SCANNERID, objScanner.SCANNERTYPE, objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;

                    case Scanner.SCANNER_HIDKB:
                        m_nArTotalScannersInType[4]++;
                        strItems = new string[] { objScanner.SCANNERID, "HID KEYBOARD", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;

                    case Scanner.SCANNER_IBMTT:
                        m_nArTotalScannersInType[5]++;
                        strItems = new string[] { objScanner.SCANNERID, "IBM TABLETOP", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;

                    case Scanner.SCALE_IBM:
                        m_nArTotalScannersInType[6]++;
                        strItems = new string[] { objScanner.SCANNERID, "IBM SCALE", objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;

                    case Scanner.SCANNER_SSI_BT:
                        m_nArTotalScannersInType[7]++;
                        strItems = new string[] { objScanner.SCANNERID, objScanner.SCANNERTYPE, objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;
                    case Scanner.CAMERA_UVC:
                        m_nArTotalScannersInType[8]++;
                        strItems = new string[] { objScanner.SCANNERID, objScanner.SCANNERTYPE, objScanner.MODELNO, objScanner.SCANNERFIRMWARE, objScanner.SCANNERMNFDATE, objScanner.SERIALNO, objScanner.GUID };
                        break;
                }

                ListViewItem lstVItm = new ListViewItem(strItems);
                if (lstvScanners.InvokeRequired)
                {
                    lstvScanners.Invoke(new MethodInvoker(delegate
                    {
                        lstvScanners.Items.Insert(index, lstVItm);
                    }));
                }
                else
                {
                    lstvScanners.Items.Insert(index, lstVItm);
                }
            }

            if (deviceFound)
            {
                if (tabCtrl.InvokeRequired)
                {
                    tabCtrl.Invoke(new MethodInvoker(delegate
                    {
                        tabCtrl.TabPages.Add(tabSSW);
                    }));
                }
                else
                {
                    tabCtrl.TabPages.Add(tabSSW);
                }                
            }


            if (m_nTotalScanners > 0)
            {
                if (lstvScanners.InvokeRequired)
                {
                    lstvScanners.Invoke(new MethodInvoker(delegate
                    {
                        lstvScanners.Items[0].Selected = true;
                    }));
                }
                else
                {
                    lstvScanners.Items[0].Selected = true;
                }
                m_bIgnoreIndexChange = false;
            }
            UpdateScannerCountLabels();
        }

        private void InitScannersCount()
        {
            for (int index = 0; index < 6; index++)
            {
                m_nArTotalScannersInType[index] = 0;
            }
        }

        private void UpdateScannerCountLabels()
        {
            toolStripStatusLblTotal.Text = "Total = " + m_nTotalScanners.ToString();
            toolStripStatusLblSnapi.Text = "SNAPI = " + m_nArTotalScannersInType[0].ToString();
            toolStripStatusLblSsi.Text = "SSI = " + m_nArTotalScannersInType[1].ToString();
            toolStripStatusLblIbmhid.Text = "IBMHID = " + m_nArTotalScannersInType[2].ToString();
            toolStripStatusLblNxmdb.Text = "NXMODB = " + m_nArTotalScannersInType[3].ToString();
            toolStripStatusLblHidkb.Text = "HIDKB = " + m_nArTotalScannersInType[4].ToString();
            toolStripStatusIBMTT.Text = "IBMTT = " + m_nArTotalScannersInType[5].ToString();
        }

        /// <summary>
        /// Manage HID Keyboard Emulator current configuration
        /// </summary>
        private void GetLanguageConfigInfo()
        {
            string outXML = String.Empty;
            string inXML = "<inArgs></inArgs>";
            int iStatus = STATUS_FALSE;

            try
            {
                m_pCoreScanner.ExecCommand(KEYBOARD_EMULATOR_GET_CONFIG, ref inXML, out outXML, out iStatus);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(outXML);

                string strEnable = xmlDoc.DocumentElement.GetElementsByTagName("KeyEnumState").Item(0).InnerText;
                string strLanguage = xmlDoc.DocumentElement.GetElementsByTagName("KeyEnumLocale").Item(0).InnerText;

                cmbEmulation.SelectedIndex = int.Parse(strLanguage);
                if (strEnable.Equals("1"))
                {
                    chkBoxEmulation.Checked = true;
                }
                else
                {
                    chkBoxEmulation.Checked = false;
                    cmbEmulation.Enabled = false;
                }
            }
            catch (Exception)
            {
            }

            DisplayResult(iStatus, "KEYBOARD_EMULATOR_GET_CONFIG");
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

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            PerformBtnBarcodeClick(sender, e);
        }

        //------------- Scale related code-------------------------------------
        byte[] ScaleConfig = new byte[5];


        private void btnReadWeight_Click(object sender, EventArgs e)
        {
            int status = STATUS_FALSE;
            string inXml = "";
            string outXml = "";
            int opCode = SCALE_READ_WEIGHT;

            inXml = GetScannerIDXml();
            ExecCmd(opCode, ref inXml, out outXml, out status);
            UpdateOutXml(outXml);

            string weight = "";
            string weightMode = "";
            int scalStat = -1;
            m_xml.ReadXmlString_Scale(outXml, out weight, out weightMode, out scalStat);
            txtWeight.Text = weight;
            txtWeightUnit.Text = weightMode;

            switch (scalStat)
            {
                case 0:
                    lblScalStatusDesc.Text = "Scale Not Enabled";
                    break;
                case 1:
                    lblScalStatusDesc.Text = "Scale Not Ready";
                    break;
                case 2:
                    lblScalStatusDesc.Text = "Stable Weight OverLimit";
                    break;
                case 3:
                    lblScalStatusDesc.Text = "Stable Weight Under Zero";
                    break;
                case 4:
                    lblScalStatusDesc.Text = "Non Stable Weight";
                    break;
                case 5:
                    lblScalStatusDesc.Text = "Stable Zero Weight";
                    break;
                case 6:
                    lblScalStatusDesc.Text = "Stable NonZero Weight";
                    break;
                default:
                    lblScalStatusDesc.Text = "Scale Unknown Status";
                    break;
            }

            DisplayResult(status, "SCALE_READ_WEIGHT");
        }

        private void btnZeroScale_Click(object sender, EventArgs e)
        {
            int status = STATUS_FALSE;
            string inXml = "";
            string outXml = "";
            int opCode = SCALE_ZERO_SCALE;
            inXml = GetScannerIDXml();
            ExecCmd(opCode, ref inXml, out outXml, out status);
            DisplayResult(status, "SCALE_ZERO_SCALE");
        }

        private void btnSystemRest_Click(object sender, EventArgs e)
        {
            int status = STATUS_FALSE;
            string inXml = "";
            string outXml = "";
            int opCode = SCALE_SYSTEM_RESET;
            inXml = GetScannerIDXml();
            ExecCmd(opCode, ref inXml, out outXml, out status);
            DisplayResult(status, "SCALE_SYSTEM_RESET");
        }

        byte[] responseArray = new byte[8];

        private void checkUseHID_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsScannerConnected())
            {
                return;
            }

            Scanner.RSMAttribute rsmAttr;
            PerformRSMGetAttribute(PARAM_USE_HID, out rsmAttr);
            if (null == rsmAttr)
                return;
            rsmAttr.value = checkUseHID.Checked ? "True" : "False";
            performRSMSetStoreAttribute(rsmAttr, false);
        }

        /// <summary>
        /// Fill parameter combobox with the known entries
        /// </summary>
        private void FillParamCombo()
        {
            using (TextReader sr = OpenParamFile())
            {
                List<ParamDefs> scanParams = new List<ParamDefs>();
                char[] delims = { '\t', ',' };

                cmbSnapiParams.Sorted = false;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                        continue;

                    // Line format: ParamName, ParamId, ParamSection, ParamType, Min, Max
                    string[] items = line.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length >= 6)
                    {
                        scanParams.Add(new ParamDefs(items));
                    }
                }
                scanParams.Sort();
                BindingSource mySource = new BindingSource(scanParams, null);

                cmbSnapiParams.DataSource = mySource;
                cmbSnapiParams.DisplayMember = "Name";

                if (cmbSnapiParams.Items.Count > 0)
                    cmbSnapiParams.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Get the known parameter definitions from the internal resource or external file
        /// </summary>
        /// <returns>text stream of parameter definitions</returns>
        private TextReader OpenParamFile()
        {
            if (File.Exists(".\\paramdef.txt")) return new StreamReader(".\\paramdef.txt");
            return new StringReader(Properties.Resources.DocCapParams);
        }

        private void cmbSnapiParams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ParamDefs)((ComboBox)sender).SelectedItem) == null)
            {
                return;
            }
            string snapiParamselected = ((ParamDefs)((ComboBox)sender).SelectedItem).Name;
            int paramID = ((ParamDefs)((ComboBox)sender).SelectedItem).Id;

            Scanner.RSMAttribute rsmAttr;
            PerformRSMGetAttribute(paramID, out rsmAttr);
            if (rsmAttr == null)
                return;
            cmbSnapiParamValue.Text = rsmAttr.value;
        }

        private void InitISO_15434_tab()
        {
            FillParamCombo();

            this.checkUseHID.CheckedChanged += new System.EventHandler(this.checkUseHID_CheckedChanged);
            this.cmbSnapiParams.SelectedIndexChanged += new System.EventHandler(this.cmbSnapiParams_SelectedIndexChanged);
        }

        private int GetParameterValue(int paramID)
        {
            string scannerID = GetOnlyScannerIDXml();
            string inXml = "<inArgs>  " + scannerID + "  <cmdArgs> <arg-xml> <attrib_list>" + paramID + "</attrib_list>   </arg-xml>  </cmdArgs>  </inArgs>";
            int opCode = GET_PARAMETERS;
            string outXml = "";
            int status = STATUS_FALSE;
            ExecCmd(opCode, ref inXml, out outXml, out status);

            DisplayResult(status, "GET_PARAMETERS");

            int paramValu;
            m_xml.ReadXmlString_Snapi(outXml, out paramValu);
            return paramValu;
        }

        private bool SetParameterValue(int parameID, int paramValue, bool persist)
        {
            bool ret = false;
            if (IsScannerConnected())
            {
                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                        "<arg-xml>" +
                                            "<attrib_list>" +
                                                "<attribute>" +
                                                    "<id>" +
                                                      parameID +
                                                    "</id>" +
                                                    "<datatype>I</datatype>" +
                                                    "<value>" +
                                                    paramValue +
                                                    "</value>" +
                                                 "</attribute>" +
                                              "</attrib_list>" +
                                           "</arg-xml>" +
                                       "</cmdArgs>" +
                                     "</inArgs>";

                int opCode = persist ? SET_PARAMETER_PERSISTANCE : DEVICE_SET_PARAMETERS;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                if (status == STATUS_SUCCESS)
                {
                    ret = true;
                }
                DisplayResult(status, "SET_PARAMETERS");
            }
            return ret;
        }

        private void btnSnapiGet_Click(object sender, EventArgs e)
        {
            if ((ParamDefs)(cmbSnapiParams.SelectedItem) == null)
            {
                return;
            }
            int paramID = ((ParamDefs)(cmbSnapiParams.SelectedItem)).Id;
            Scanner.RSMAttribute rsmAttr;
            if (!PerformRSMGetAttribute(paramID, out rsmAttr))
            {
                return;
            }
            if (null == rsmAttr)
                return;
            cmbSnapiParamValue.Text = rsmAttr.value;

        }

        private void btnSnapiSet_Click(object sender, EventArgs e)
        {
            if ((ParamDefs)(cmbSnapiParams.SelectedItem) == null)
            {
                return;
            }
            int paramID = ((ParamDefs)(cmbSnapiParams.SelectedItem)).Id;
            Scanner slctdScanner = GetScannerFromID(GetSelectedScannerID());
            slctdScanner.m_rsmAttribute.value = cmbSnapiParamValue.Text.Trim();
            performRSMSetStoreAttribute(slctdScanner.m_rsmAttribute, false);
        }
        private void btnSnapiStore_Click(object sender, EventArgs e)
        {
            if ((ParamDefs)(cmbSnapiParams.SelectedItem) == null)
            {
                return;
            }
            int paramID = ((ParamDefs)(cmbSnapiParams.SelectedItem)).Id;
            Scanner slctdScanner = GetScannerFromID(GetSelectedScannerID());
            slctdScanner.m_rsmAttribute.value = cmbSnapiParamValue.Text.Trim();
            performRSMSetStoreAttribute(slctdScanner.m_rsmAttribute, true);
        }

        private void btnClearpbx_Click(object sender, EventArgs e)
        {
            pbxISO15434Image.Image = null;
            txtDocCapDecodeDataSymbol.Text = "";
            txtDocCapDecodeData.Text = "";
            idcImage = null;
        }

        private void buttonWavFileBrowse_Click(object sender, EventArgs e)
        {
            openFileDialogWavFile.FileName = "";
            if (openFileDialogWavFile.ShowDialog() == DialogResult.OK)
            {
                textBoxWavFile.Text = openFileDialogWavFile.FileName;
            }
        }

        private void buttonWavFileUpload_Click(object sender, EventArgs e)
        {
            if (textBoxWavFile.Text != "")
            {
                int opcode = UPDATE_DECODE_TONE;
                string outXML; // XML Output
                string inXML = "<inArgs>" +
                             GetOnlyScannerIDXml() +
                             "<cmdArgs>" +
                             "<arg-string>" + textBoxWavFile.Text + "</arg-string>" +
                             "</cmdArgs>" +
                             "</inArgs>";

                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opcode, ref inXML, out outXml, out status);
                DisplayResult(status, "UPDATE_DECODE_TONE");
            }
        }

        private void btnEraseTone_Click(object sender, EventArgs e)
        {
            int opcode = ERASE_DECODE_TONE;
            string outXML; // XML Output
            string inXML = GetScannerIDXml();

            string outXml = "";
            int status = STATUS_FALSE;
            ExecCmd(opcode, ref inXML, out outXml, out status);
            DisplayResult(status, "ERASE_DECODE_TONE");
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

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            PerformBtnDisconnectScannerClick(sender, e);
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

        private void btnSaveIdc_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveIdcImageDialog = new SaveFileDialog();
            saveIdcImageDialog.Filter = "jpeg files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
            if (idcImage != null)
            {
                if (saveIdcImageDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap newBitmap = new Bitmap(idcImage);
                    newBitmap.Save(saveIdcImageDialog.FileName);
                    newBitmap.Dispose();
                }
            }
            saveIdcImageDialog.Dispose();
        }

        private void btnSCdcSwitchDevices_Click(object sender, EventArgs e)
        {
            PerformCDCSwitchModeClick(sender, e);
        }
    }
}