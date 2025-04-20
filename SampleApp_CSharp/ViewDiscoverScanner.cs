using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        private void PullTriggerClick(object sender, EventArgs e)
        {

            if (IsScannerConnected())
            {
                pbxImageVideo.Image = null;
                pbxImageVideo.Refresh();
                txtOutXml.Clear();
                ExecuteActionCommand(DEVICE_PULL_TRIGGER, "PULL_TRIGGER");
            }
        }
        private void performGetScanner()
        {
            m_arSelectedTypes = scanToConnect.GetSelectedTypes();
            discoverScanner.FilterScannerList(ref m_arSelectedTypes, cmbFilterScnrs.SelectedIndex);
            MakeConnectCtrl();
            registerForEvents();
            ShowScanners();
            GetPairingBarcode();
            GetRtaPublicEvents();
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
            grpElectricFenceCustomTone.Enabled = bEnable;
            grpHVS.Enabled = bEnable;

            //RTA Button States
            btnGetSuppRTAEvents.Enabled = bEnable;
            btnGetRegRTAEvents.Enabled = bEnable;
            btnRegisterRTAEvents.Enabled = bEnable;
            btnSetRTAEventStatus.Enabled = bEnable;
            btnGetRTAEventStatus.Enabled = bEnable;
            btnClean.Enabled = bEnable;
            btnCleanEvents.Enabled = bEnable; 
            cbSuspend.Enabled = bEnable;
            btnGetRTAState.Enabled = bEnable;
                       
            ToolTip suspendStateTooltip = new ToolTip();
            suspendStateTooltip.ShowAlways = true;
            suspendStateTooltip.SetToolTip(cbSuspend, "To suspend RTA alerts, place a check in the box and click on \"Set RTA Alert Status\"");
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
            m_nNumberOfTypes = scanToConnect.GetSelectedScannerTypes();
            m_arScannerTypes = scanToConnect.GetScannerTypes();
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
        private void registerForEvents()
        {
            if (IsMotoConnected())
            {
                int nEvents = 0;
                string strEvtIDs = scanToConnect.GetRegUnRegisterIDs(out nEvents);
                string inXml = scanToConnect.GenerateInitXML(nEvents, strEvtIDs);

                int opCode = REGISTER_FOR_EVENTS;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "REGISTER_FOR_EVENTS");
            }
        }

        /// <summary>
        /// Get RTA public events to store in globally to filter out the private events when required.
        /// </summary>
        private void GetRtaPublicEvents()
        {            
            List<RtaAlertStatusDetails> rtaAlertsStatus = new List<RtaAlertStatusDetails>();
            bool rtaSupportedDeviceSeen = false;
           
            for (int i = 0; i < m_nTotalScanners; i++)
            {

                rtaAlertsStatus = GetRtaAlertStatus(m_arScanners[i].SCANNERID);
                

                if (rtaAlertsStatus != null && rtaAlertsStatus.Count > 0)
                {
                    rtaSupportedDeviceSeen = true;

                    if (!tabCtrl.TabPages.Contains(tabRta))
                    {
                        if (tabCtrl.InvokeRequired)
                        {
                            tabCtrl.Invoke(new MethodInvoker(delegate
                            {
                                tabCtrl.TabPages.Add(tabRta);
                            }));
                        }
                        else
                        {
                            tabCtrl.TabPages.Add(tabRta);
                        }
                    }
                }
                else
                {
                    if (tabCtrl.TabPages.Contains(tabRta) && !rtaSupportedDeviceSeen)
                    {
                        if (tabCtrl.InvokeRequired)
                        {
                            tabCtrl.Invoke(new MethodInvoker(delegate
                            {
                                tabCtrl.TabPages.Remove(tabRta);
                            }));
                        }
                        else
                        {
                            tabCtrl.TabPages.Remove(tabRta);
                        }
                    }
                    
                }
               
            }                
                
        }

        /// <summary>
        /// Calls GetScanners command
        /// </summary>
        private void ShowScanners()
        {
            string inXml = String.Empty;
            int status = STATUS_FALSE;
            lstvScanners.Items.Clear();
            cmbSlcrScnr.Items.Clear();

            m_arScanners.Initialize();
            if (IsMotoConnected())
            {
                m_nTotalScanners = 0;
                short numOfScanners = 0;
                string outXML = "";

                try
                {
                    m_arScanners = discoverScanner.GetScanners(ref numOfScanners, ref outXML, ref status, claimlist);
                    DisplayResult(status, "GET_SCANNERS");
                    if (STATUS_SUCCESS == status)
                    {
                        m_nTotalScanners = numOfScanners;
                        FillScannerList();
                        UpdateOutXml(outXML);
                        discoverScanner.ClaimDevice(m_arScanners, numOfScanners, claimlist, chkAsync.Checked);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error GETSCANNERS - " + ex.Message, APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
