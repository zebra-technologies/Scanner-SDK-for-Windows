using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        #region "CoreScanner Events"
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
        private void OnParameterBarcodeEvent(short eventType, int size, short imageFormat, ref object sfImageData, ref string pData)
        {
            if (eventType == 1)
            {
                Image img = BaseMethods.ProcessImageData(sfImageData);
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
        #endregion
        #region "CoreScannerUsedFunction"
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

            if (cmbSlcrScnr.InvokeRequired)
            {
                cmbSlcrScnr.Invoke(new MethodInvoker(delegate
                {
                    cmbSlcrScnr.Items.Clear();
                }));
            }
            else
            {
                cmbSlcrScnr.Items.Clear();
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

                if (cmbSlcrScnr.InvokeRequired)
                {
                    cmbSlcrScnr.Invoke(new MethodInvoker(delegate
                    {
                        cmbSlcrScnr.Items.Add(objScanner.SCANNERID + "        " + objScanner.MODELNO);
                    }));
                }
                else
                {
                    cmbSlcrScnr.Items.Add(objScanner.SCANNERID + "        " + objScanner.MODELNO);
                }

                strItems = discoverScanner.GetScannerItems(objScanner, strItems);
                if (strItems.Length > 0)
                    discoverScanner.IncreaseScannerCount(objScanner.SCANNERTYPE, ref m_nArTotalScannersInType);

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
                        cmbSlcrScnr.Items.Clear();
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
            grpElectricFenceCustomTone.Enabled = bEnable;
            grpCustomDecodeTone.Enabled = bEnable;
            grpMiscOther.Enabled = bEnable;
            grpScale.Enabled = bEnable;
            grpIDC.Enabled = bEnable;
            grpScan2Connect.Enabled = bEnable;
            pbxImageVideo.Image = null;
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
            toolStripStatusLblSnapi.Text = "SNAPI = " + m_nArTotalScannersInType[(int)DiscoverScanner.ScannerType.SNAPI].ToString();
            toolStripStatusLblSsi.Text = "SSI = " + m_nArTotalScannersInType[(int)DiscoverScanner.ScannerType.SSI].ToString();
            toolStripStatusLblIbmhid.Text = "IBMHID = " + m_nArTotalScannersInType[(int)DiscoverScanner.ScannerType.USBIBMHID].ToString();
            toolStripStatusLblNxmdb.Text = "NXMODB = " + m_nArTotalScannersInType[(int)DiscoverScanner.ScannerType.NIXMODB].ToString();
            toolStripStatusLblHidkb.Text = "HIDKB = " + m_nArTotalScannersInType[(int)DiscoverScanner.ScannerType.USBHIDKB].ToString();
            toolStripStatusIBMTT.Text = "IBMTT = " + m_nArTotalScannersInType[(int)DiscoverScanner.ScannerType.USBIBMTT].ToString();
        }
        #endregion
        #region "ExecCommandFunctions"
        private string ExecuteActionCommand(int opCode, string strCmd, string inXml = "", bool isDisplayResultRequired = true)
        {
            string outXml = "";
            try
            {
                if (IsScannerConnected())
                {
                    if (inXml == "")
                        inXml = GetScannerIDXml();
                    int status = STATUS_FALSE;
                    ExecCmd(opCode, ref inXml, out outXml, out status);
                    if (isDisplayResultRequired)
                        DisplayResult(status, strCmd);
                    return outXml;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return outXml;
        }

        private string ExecuteActionCommandOffline(int opCode, string strCmd, string inXml = "", bool isDisplayResultRequired = true)
        {
            string outXml = "";
            try
            {
                if (inXml == "")
                    inXml = GetScannerIDXml();
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                if (isDisplayResultRequired)
                    DisplayResult(status, strCmd);
                return outXml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        #endregion
    }
}
