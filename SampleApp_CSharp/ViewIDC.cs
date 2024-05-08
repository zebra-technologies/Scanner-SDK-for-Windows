using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Scanner_SDK_Sample_Application
{
    public partial  class frmScannerApp
    {

        #region "CoreScanner Events"
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
                if (idcImage != null)
                {
                    btnSaveIdc.Enabled = true; ;
                }
                else
                {
                    btnSaveIdc.Enabled = false;
                }
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
        }
        #endregion

        #region "Control Events"

        private void PerformCheckUserHIDChanged(object sender, EventArgs e)
        {
            if (!IsScannerConnected())
            {
                return;
            }

            Scanner.RSMAttribute rsmAttr;
            PerformRSMGetAttribute(PARAM_USE_HID, out rsmAttr);
            if (rsmAttr == null)
                return;
            rsmAttr.value = checkUseHID.Checked ? "True" : "False";
            performRSMSetStoreAttribute(rsmAttr, false);
        }

        private void PerformcmbSnapiParamsSelectedIndexChanged(object sender, EventArgs e)
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

        private void PerformSnapiGetClick(object sender, EventArgs e)
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

        private void PerformSnapiSetClick(object sender, EventArgs e)
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

        private void PerformSnapiStoreClick(object sender, EventArgs e)
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

        private void PerformSaveIdcClick(object sender, EventArgs e)
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

        private void PerformClearpbxClick(object sender, EventArgs e)
        {
            pbxISO15434Image.Image = null;
            txtDocCapDecodeDataSymbol.Text = "";
            txtDocCapDecodeData.Text = "";
            idcImage = null;
        }

        #endregion

        #region "Functions"

        private void InitISO_15434_tab()
        {
            FillParamCombo();

            this.checkUseHID.CheckedChanged += new System.EventHandler(this.checkUseHID_CheckedChanged);
            this.cmbSnapiParams.SelectedIndexChanged += new System.EventHandler(this.cmbSnapiParams_SelectedIndexChanged);
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
    /// <summary>
    /// Fill parameter combobox with the known entries
    /// </summary>
    private void FillParamCombo()
        {
            using (TextReader sr =OpenParamFile())
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
       
        private bool performRSMSetStoreAttribute(Scanner.RSMAttribute rsmAttribute, bool isStore)
        {
            if (IsScannerConnected())
            {

                string inXml = GetInXmlForGetSetStore("<attribute>" + 
                                            "<id>" + rsmAttribute.ID + "</id>" +
                                            "<datatype>" + rsmAttribute.Type + "</datatype>" +
                                            "<value>" + rsmAttribute.value + "</value>" +
                                           "</attribute>");


                string outXml = "";
                if (isStore)
                    outXml = ExecuteActionCommand(RSM_ATTR_STORE, "RSM_ATTR_STORE", inXml);
                else
                    outXml = ExecuteActionCommand(RSM_ATTR_SET, "RSM_ATTR_SET", inXml);

                UpdateOutXml(outXml);
            }
            return false;
        }

        private string GetInXmlForGetSetStore(string inputValues)
        {
            string inXml = "<inArgs>" +
                                       GetOnlyScannerIDXml() +
                                       "<cmdArgs>" +
                                       "<arg-xml>" +
                                       "<attrib_list>" +
                                              inputValues +
                                       "</attrib_list>" +
                                       "</arg-xml>" +
                                       "</cmdArgs>" +
                                       "</inArgs>";
            return inXml;

        }
        private bool PerformRSMGetAttribute(int attributeNo, out Scanner.RSMAttribute attribute)
        {
            value = null;
            attribute = null;
            if (IsScannerConnected())
            {
                string inXml = GetInXmlForGetSetStore(attributeNo.ToString());
                string outXml = ExecuteActionCommand(RSM_ATTR_GET, "RSM_ATTR_GET", inXml, isDisplayResultRequired: false);
                UpdateOutXml(outXml);

                if (!chkAsync.Checked)
                {
                    Scanner scanr = null;
                    int nIndex = -1;
                    int nAttrCount = 0;
                    int nOpCode = -1;

                    m_xml.ReadXmlString_RsmAttrGet(outXml, m_arScanners, out scanr, out nIndex, out nAttrCount, out nOpCode);
                    if (scanr == null)
                    {
                        DisplayResult(STATUS_FALSE, "RSM_ATTR_GET");
                        return false;
                    }
                    DisplayResult(STATUS_SUCCESS, "RSM_ATTR_GET");

                    attribute = scanr.m_rsmAttribute;
                    return true;
                }
            }

            return false;
        }

        private Scanner GetScannerFromID(string scannerID)
        {
            foreach (Scanner scanrTmp in m_arScanners)
            {
                if ((null != scanrTmp) &&
                     (scannerID == scanrTmp.SCANNERID))
                {
                    return scanrTmp;
                }
            }
            return null;
        }
        #endregion

    }
}
