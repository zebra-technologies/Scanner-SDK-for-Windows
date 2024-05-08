using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Scanner_SDK_Sample_Application
{
    public enum CommandStatus
    {
        None = 0,
        Failed = 1,
        Success = 2,
    }

    public enum RfidBank
    {
        Reserved = 0,
        Epc = 1,
        Tid = 2,
        User = 3
    }

    public enum RfidCommand
    {
        Read = 1,
        Write = 2,
        Lock = 3,
        Kill = 4
    }

    public class SGTINPartitionValue
    {
        public int PartitionValue { get; set; }
        public int CompanyPrefixBits { get; set; }
        public int CompanyPrefixDigits { get; set; }
        public int ItemReferenceBits { get; set; }
        public int ItemReferenceDigits { get; set; }
    }

    public partial class frmScannerApp
    {
        private const int UPC_LENGTH = 12;
        private const int SGTIN_LENGTH = 96;
        private const int EPC_HEADER_LENGTH = 8;
        private const int EPC_FILTER_LENGTH = 3;
        private const int EPC_PARTITION_LENGTH = 3;
        private const int EPC_PREFIX_REFERENCE_LENGTH = 44;
        private const int EPC_SERIAL_NUMBER_LENGTH = 38;
        private const int SGTIN_HEADER_VALUE = 48;

        private const Int64 SERIAL_MAX_VALUE = 274877906943;
        private const string RFID_HEX_PREFIX = "0x00 0x40";

        private const int ATTRIBUTE_RFID_CMD_STATUS = 35009;
        private const int ATTRIBUTE_RFID_DATA = 35004;
        private const int ERROR_BEEP_UIF_CODE = 73;
        private const string INCORRECT_ARGUMENT = "Incorrect arguments";
        private const string UNABLE_TO_READ_USER_BANK = "Unable to read user bank";
        private const string SSW_TAB_NAME = "tabSSW";

        private string currentUpca = string.Empty;
        private string currentEpcId = string.Empty;

        private string userBankASCII = string.Empty;
        private string userBankHex = string.Empty;

        private int CurrentFilterValue = 0;
        private int CurrentPartitionValue = 0;
        private Int64 CurrentSerialNumber = 0;

        private Int64 CurrentUpcConpanyPrefix = 0;
        private Int64 CurrentUpcItemReference = 0;

        private string CurrentEPCsUPCPortionInBinary = "0";

        private bool initSSW = true;
        private bool overrideValues = true;
        private bool overrideSerial = true;


        private SGTINPartitionValue CurrentPartitionRecord = null;

        private string RFID_READ_XML_TEMPLATE = "<attribute><id>35002</id><datatype>A</datatype><value>TAGID</value></attribute>" +
                                                "<attribute><id>35003</id><datatype>B</datatype><value>BANK</value></attribute>" +
                                                "<attribute><id>35005</id><datatype>W</datatype><value>OFFSET</value></attribute>" +
                                                "<attribute><id>35006</id><datatype>W</datatype><value>LENGTH</value></attribute>" +
                                                "<attribute><id>35008</id><datatype>B</datatype><value>COMMAND</value></attribute>";

        private string RFID_WRITE_XML_TEMPLATE = "<attribute><id>35002</id><datatype>A</datatype><value>TAGID</value></attribute>" +
                                                 "<attribute><id>35003</id><datatype>B</datatype><value>BANK</value></attribute>" +
                                                 "<attribute><id>35004</id><datatype>A</datatype><value>DATA</value></attribute>" +
                                                 "<attribute><id>35005</id><datatype>W</datatype><value>OFFSET</value></attribute>" +
                                                 "<attribute><id>35008</id><datatype>B</datatype><value>COMMAND</value></attribute>";

        private List<SGTINPartitionValue> SGTINPartitionValueTable = new List<SGTINPartitionValue>()
        {
            new SGTINPartitionValue(){ PartitionValue =0, CompanyPrefixBits = 40, CompanyPrefixDigits = 12, ItemReferenceBits = 4, ItemReferenceDigits = 1 },
            new SGTINPartitionValue(){ PartitionValue =1, CompanyPrefixBits = 37, CompanyPrefixDigits = 11, ItemReferenceBits = 7, ItemReferenceDigits = 2 },
            new SGTINPartitionValue(){ PartitionValue =2, CompanyPrefixBits = 34, CompanyPrefixDigits = 10, ItemReferenceBits = 10, ItemReferenceDigits = 3 },
            new SGTINPartitionValue(){ PartitionValue =3, CompanyPrefixBits = 30, CompanyPrefixDigits = 9, ItemReferenceBits = 14, ItemReferenceDigits = 4 },
            new SGTINPartitionValue(){ PartitionValue =4, CompanyPrefixBits = 27, CompanyPrefixDigits = 8, ItemReferenceBits = 17, ItemReferenceDigits = 5 },
            new SGTINPartitionValue(){ PartitionValue =5, CompanyPrefixBits = 24, CompanyPrefixDigits = 7, ItemReferenceBits = 20, ItemReferenceDigits = 6 },
            new SGTINPartitionValue(){ PartitionValue =6, CompanyPrefixBits = 20, CompanyPrefixDigits = 6, ItemReferenceBits = 24, ItemReferenceDigits = 7 },
        };



        /// <summary>
        /// Get value of ATTRIBUTE_RFID_DATA attribute
        /// </summary>
        /// <returns>Value of ATTRIBUTE_RFID_DATA attribute. Empty string on failures. </returns>
        private string GetRfidData()
        {
            List<ScannerAttribute> attributes = GetAttributes(new List<int> { ATTRIBUTE_RFID_DATA });
            if (attributes != null)
            {
                ScannerAttribute commandStatusAttribute = attributes[0];
                if (commandStatusAttribute.Id == ATTRIBUTE_RFID_DATA)
                {
                    var data = (string)commandStatusAttribute.Value;
                    if (data.StartsWith(RFID_HEX_PREFIX))
                    {
                        data = data.Substring(10, (data.Length - 10));
                    }
                    return data;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Get scanner attributes
        /// </summary>
        /// <param name="attributeIds">List of attribute ids</param>
        /// <returns>List of cref="ScannerAttribute" </returns>
        private List<ScannerAttribute> GetAttributes(List<int> attributeIds)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<inArgs>");
            stringBuilder.Append("<scannerID>");
            stringBuilder.Append(GetSelectedScannerID());
            stringBuilder.Append("</scannerID>");
            stringBuilder.Append("<cmdArgs><arg-xml><attrib_list>");

            foreach (int attributeId in attributeIds)
            {
                stringBuilder.Append(attributeId);
                stringBuilder.Append(",");
            }

            if (attributeIds.Count > 0)
                stringBuilder.Length -= 1; // remove last ,

            stringBuilder.Append("</attrib_list></arg-xml></cmdArgs></inArgs>");
            string inXml = stringBuilder.ToString();
            string outXml = String.Empty;
            int status = STATUS_FALSE;
            m_pCoreScanner.ExecCommand(RSM_ATTR_GET, ref inXml, out outXml, out status);
            if (status == STATUS_SUCCESS && !string.IsNullOrEmpty(outXml))
            {
                return GetAttributesFromOutXml(outXml);
            }
            else
            {
                MessageBox.Show("Reader busy.", "Error", MessageBoxButtons.OK);
                return null;
            }
        }

        /// <summary>
        /// Get cref="ScannerAttribute" object list from CoreScanner output XML
        /// </summary>
        /// <param name="outXml">Output XML received from CoreScanner</param>
        /// <returns>List of cref="ScannerAttribute" objects</returns>
        private List<ScannerAttribute> GetAttributesFromOutXml(string outXml)
        {
            if (String.IsNullOrEmpty(outXml))
            {
                throw new ArgumentException(INCORRECT_ARGUMENT);
            }
            List<ScannerAttribute> scannerAttributes = new List<ScannerAttribute>();

            try
            {
                XmlTextReader xmlRead = new XmlTextReader(new StringReader(outXml));
                xmlRead.WhitespaceHandling = WhitespaceHandling.Significant;
                ScannerAttribute scannerAttribute = null;
                string elementName = String.Empty, elementValue = String.Empty;
                while (xmlRead.Read())
                {
                    switch (xmlRead.NodeType)
                    {
                        case XmlNodeType.Element:
                            elementName = xmlRead.Name;
                            break;

                        case XmlNodeType.Text:
                            elementValue = xmlRead.Value;
                            switch (elementName)
                            {
                                case Scanner.TAG_ATTR_ID:
                                    scannerAttribute = new ScannerAttribute();
                                    scannerAttribute.Id = Convert.ToInt32(elementValue);
                                    break;

                                case Scanner.TAG_ATTR_TYPE:
                                    scannerAttribute.Type = elementValue;
                                    break;

                                case Scanner.TAG_ATTR_PROPERTY:
                                    scannerAttribute.Permission = elementValue;
                                    break;

                                case Scanner.TAG_ATTR_VALUE:
                                    scannerAttribute.Value = elementValue;
                                    scannerAttributes.Add(scannerAttribute);
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return scannerAttributes;
        }

        /// <summary>
        /// Send a UIF code to currently selected scanner
        /// </summary>
        /// <param name="uifCode">UIF code</param>
        private void SendUif(int uifCode)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<inArgs>");
            stringBuilder.Append("<scannerID>");
            stringBuilder.Append(GetSelectedScannerID());
            stringBuilder.Append("</scannerID>");
            stringBuilder.Append("<cmdArgs><arg-int>");
            stringBuilder.Append(uifCode);
            stringBuilder.Append("</arg-int></cmdArgs></inArgs>");
            string inXml = stringBuilder.ToString();
            string outXml = String.Empty;
            int status = STATUS_FALSE;
            m_pCoreScanner.ExecCommand(SET_ACTION, ref inXml, out outXml, out status);
        }




        /// <summary>
        /// Read RFID tag data using RSM
        /// </summary>
        /// <param name="tagID">The EPC Tag ID of the tag to be operated upon. (size-encoded binary)</param>
        /// <param name="rfidBank">Desired Tag Bank</param>
        /// <param name="offset">Word offset into tag buffer </param>
        /// <param name="length">Words of data to read from tag buffer. 0 means entire bank</param>
        /// <param name="password">Optional access password</param>
        /// <param name="rfidData">Buffer for read, write, and lock (size-encoded binary)</param>
        /// <returns>Status of the command</returns>
        private int ReadTag(string tagID, RfidBank rfidBank, int offset, int length, string password, out string rfidData)
        {
            int rfidCommandStatus = -1;
            if (String.IsNullOrEmpty(tagID) || length < 0)
            {
                throw new ArgumentException(INCORRECT_ARGUMENT);
            }
            rfidData = string.Empty;

            string attributesXml = RFID_READ_XML_TEMPLATE;
            attributesXml = attributesXml.Replace("TAGID", GetHexDataArrayWithHexLengthPrefix(tagID));
            attributesXml = attributesXml.Replace("BANK", ((int)rfidBank).ToString());
            attributesXml = attributesXml.Replace("OFFSET", offset.ToString());
            attributesXml = attributesXml.Replace("LENGTH", length.ToString());
            attributesXml = attributesXml.Replace("COMMAND", ((int)RfidCommand.Read).ToString());

            SetAttributes(attributesXml);

            rfidCommandStatus = GetRfidCommandStatus();
            if (rfidCommandStatus == STATUS_SUCCESS)
            {
                rfidData = GetRfidData();
            }
            return rfidCommandStatus;
        }

        /// <summary>
        /// Write RFID tag data using RSM
        /// </summary>
        /// <param name="tagID">EPC code of desired tag</param>
        /// <param name="rfidBank">Desired memory bank of tag</param>
        /// <param name="rfidData">Data to write</param>
        /// <param name="offset">Word offset into the memory bank</param>
        /// <param name="password">Optional access password</param>
        /// <returns>Status of the last RFID operation</returns>
        private int WriteTag(string tagID, RfidBank rfidBank, string rfidData, int offset, string password)
        {
            if (String.IsNullOrEmpty(tagID) || String.IsNullOrEmpty(rfidData))
            {
                throw new ArgumentException(INCORRECT_ARGUMENT);
            }

            string attributesXml = RFID_WRITE_XML_TEMPLATE;
            attributesXml = attributesXml.Replace("TAGID", GetHexDataArrayWithHexLengthPrefix(tagID));
            attributesXml = attributesXml.Replace("BANK", ((int)rfidBank).ToString());
            attributesXml = attributesXml.Replace("DATA", GetHexDataArrayWithHexLengthPrefix(rfidData));
            attributesXml = attributesXml.Replace("OFFSET", offset.ToString());
            attributesXml = attributesXml.Replace("COMMAND", ((int)RfidCommand.Write).ToString());

            SetAttributes(attributesXml);

            return GetRfidCommandStatus();
        }

        /// <summary>
        /// Clear the tag cache of the reader.
        /// </summary>
        private void ClearTagCache()
        {
            string attributesXml = "<attribute><id>35010</id><datatype>W</datatype><value>0</value></attribute>";
            SetAttributes(attributesXml);
        }

        /// <summary>
        /// Get status last RFID operation
        /// </summary>
        /// <returns></returns>
        private int GetRfidCommandStatus()
        {
            List<ScannerAttribute> attributes = GetAttributes(new List<int> { ATTRIBUTE_RFID_CMD_STATUS });
            if (attributes != null)
            {
                ScannerAttribute commandStatusAttribute = attributes[0];
                if (commandStatusAttribute.Id == ATTRIBUTE_RFID_CMD_STATUS)
                {
                    return Convert.ToInt32(commandStatusAttribute.Value);
                }
            }
            return -1;
        }

        /// <summary>
        /// Set the parameter attributes for a write operation
        /// </summary>
        /// <param name="attributesXml">Attributes to set in xml format.</param>
        private void SetAttributes(string attributesXml)
        {
            string inXml = "<inArgs>" + Environment.NewLine +
                           "<scannerID>" + GetSelectedScannerID() + "</scannerID>" + Environment.NewLine +
                           "<cmdArgs><arg-xml><attrib_list>" + Environment.NewLine +
                           attributesXml + Environment.NewLine +
                           "</attrib_list></arg-xml></cmdArgs>" + Environment.NewLine +
                           "</inArgs>";

            int status = STATUS_FALSE;
            string outXml = String.Empty;
            m_pCoreScanner.ExecCommand(RSM_ATTR_SET, ref inXml, out outXml, out status);
        }



        private string GetTextboxText(TextBox control)
        {
            string text = string.Empty;
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(delegate
                {
                    text = control.Text;
                }));
            }
            else
            {
                text = control.Text;
            }
            return text.Trim();
        }

        private void SetTextboxText(TextBox control, string value)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(delegate
                {
                    control.Text = value;
                }));
            }
            else
            {
                control.Text = value;
            }
        }

        private string GetLabelText(Label control)
        {
            string text = string.Empty;
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(delegate
                {
                    text = control.Text;
                }));
            }
            else
            {
                text = control.Text;
            }
            return text.Trim();
        }

        private void SetLabelText(Label control, string value)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(delegate
                {
                    control.Text = value;
                }));
            }
            else
            {
                control.Text = value;
            }
        }

        private string GetComboboxItem(ComboBox control)
        {
            string text = string.Empty;
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(delegate
                {
                    text = control.SelectedItem.ToString();
                }));
            }
            else
            {
                text = control.SelectedItem.ToString();
            }
            return text;
        }

        private void SetComboboxItem(ComboBox control, string value)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(delegate
                {
                    control.SelectedItem = value;
                }));
            }
            else
            {
                control.SelectedItem = value;
            }
        }

        /// <summary>
        /// Extract UPCA barcode company prefix and item value.
        /// </summary>
        private void ExtractUpcData()
        {
            SetStatusIcon(CommandStatus.None);

            if (string.IsNullOrEmpty(currentUpca))
            {
                MessageBox.Show("UPCA barcode should not be null or empty.", "Error", MessageBoxButtons.OK);
                return;
            }
            if (currentUpca.Length != UPC_LENGTH)
            {
                MessageBox.Show("UPCA barcode length is not valid.", "Error", MessageBoxButtons.OK);
                return;
            }
            UpdateCompanyItemValues();
        }

        private void UpdateCompanyItemValues()
        {
            string currentPartition = GetComboboxItem(cmbPartition);
            int companyPrefixLength = 12 - Convert.ToInt16(currentPartition);

            CurrentUpcConpanyPrefix = Convert.ToInt64(currentUpca.Substring(0, companyPrefixLength));
            if (companyPrefixLength == 12)
            {
                CurrentUpcItemReference = 0;
            }
            else
            {
                CurrentUpcItemReference = Convert.ToInt64(currentUpca.Substring(companyPrefixLength, 12 - companyPrefixLength));
            }
        }

        /// <summary>
        /// Extract EPC tag id's header, filter, partition and serial number.
        /// </summary>
        private void ExtractEpcData()
        {
            SetStatusIcon(CommandStatus.None);

            if (string.IsNullOrEmpty(currentEpcId))
            {
                MessageBox.Show("EPC Tag id should not be null or empty.", "Error", MessageBoxButtons.OK);
                return;
            }

            var binaryValue = string.Empty;
            foreach (var c in currentEpcId.ToCharArray())
            {
                binaryValue += Convert.ToString(Convert.ToInt64(c.ToString(), 16), 2).PadLeft(4, '0');
            }

            if (binaryValue.Length != SGTIN_LENGTH)
            {
                MessageBox.Show("EPC Tag id length is not valid.", "Error", MessageBoxButtons.OK);
                return;
            }

            if (overrideValues)
            {
                CurrentFilterValue = Convert.ToInt32(binaryValue.Substring(EPC_HEADER_LENGTH, EPC_FILTER_LENGTH), 2);
                CurrentPartitionValue = Convert.ToInt32(binaryValue.Substring((EPC_HEADER_LENGTH + EPC_FILTER_LENGTH), EPC_PARTITION_LENGTH), 2);

                if (CurrentPartitionValue > 6 || CurrentPartitionValue < 0)
                {
                    SetTextboxText(txtEpcId, string.Empty);
                    MessageBox.Show("Invalid EPC Tag. Partition value should be between 0-6." + CurrentPartitionValue + ".", "Error", MessageBoxButtons.OK);
                    return;
                }
                CurrentPartitionRecord = GetPartitionTableRecord(CurrentPartitionValue);

                SetComboboxItem(cmbFilterValue, CurrentFilterValue.ToString());
                SetComboboxItem(cmbPartition, CurrentPartitionValue.ToString());
                overrideValues = true;
            }

            CurrentEPCsUPCPortionInBinary = binaryValue.Substring((EPC_HEADER_LENGTH + EPC_FILTER_LENGTH + EPC_PARTITION_LENGTH), EPC_PREFIX_REFERENCE_LENGTH);
            if (overrideSerial)
            {
                CurrentSerialNumber = Convert.ToInt64(binaryValue.Substring((binaryValue.Length - EPC_SERIAL_NUMBER_LENGTH), EPC_SERIAL_NUMBER_LENGTH), 2);
                SetTextboxText(txtSerialNumber, CurrentSerialNumber.ToString());
                overrideSerial = true;
            }
            else
            {
                SetTextboxText(txtSerialNumber, CurrentSerialNumber.ToString());
            }
        }

        /// <summary>
        /// Create new SGTIN 96 encoded barcode.
        /// </summary>
        private void CreateNewEpcId()
        {
            SetStatusIcon(CommandStatus.None);

            if (string.IsNullOrEmpty(GetTextboxText(txtUpcaBarcode)) || string.IsNullOrEmpty(GetTextboxText(txtEpcId)))
                return;

            string binaryValue = string.Empty;
            UpdateCompanyItemValues();
            binaryValue = PadLeftZero(EPC_HEADER_LENGTH, Convert.ToString(SGTIN_HEADER_VALUE, 2)) +
                          PadLeftZero(EPC_FILTER_LENGTH, Convert.ToString(CurrentFilterValue, 2)) +
                          PadLeftZero(EPC_PARTITION_LENGTH, Convert.ToString(CurrentPartitionValue, 2)) +
                          PadLeftZero(CurrentPartitionRecord.CompanyPrefixBits, Convert.ToString(CurrentUpcConpanyPrefix, 2)) +
                          PadLeftZero(CurrentPartitionRecord.ItemReferenceBits, Convert.ToString(CurrentUpcItemReference, 2)) +
                          PadLeftZero(EPC_SERIAL_NUMBER_LENGTH, Convert.ToString(CurrentSerialNumber, 2));

            var hexValue = BinaryStringToHexString(binaryValue);
            SetTextboxText(txtNewEpcId, hexValue);
        }




        /// <summary>
        /// Get SGTIN Partition table record.
        /// </summary>
        /// <param name="partitionValue">SGTIN partition value.</param>
        /// <returns></returns>
        private SGTINPartitionValue GetPartitionTableRecord(int partitionValue)
        {
            foreach (var item in SGTINPartitionValueTable)
            {
                if (item.PartitionValue == partitionValue)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Convert given binary string to hex string.
        /// </summary>
        /// <param name="binary">Binary string value.</param>
        /// <returns></returns>
        private string BinaryStringToHexString(string binary)
        {
            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);
            int length = binary.Length % 8;
            if (length != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }
            return result.ToString();
        }

        /// <summary>
        /// Get the binary value on specified length.
        /// </summary>
        /// <param name="length">Binary value length</param>
        /// <param name="value">Int value in string format.</param>
        /// <returns></returns>
        private string GetBinaryStringValue(int length, string value)
        {
            int intValue = Convert.ToInt32(value);
            string binaryValue = Convert.ToString(intValue, 2);

            return PadLeftZero(length, binaryValue);
        }

        /// <summary>
        /// Pad left by zero with specified length.
        /// </summary>
        /// <param name="length">Length to pad with zero.</param>
        /// <param name="value">String value.</param>
        /// <returns></returns>
        private string PadLeftZero(int length, string value)
        {
            for (int i = value.Length; i < length; i++)
            {
                value = "0" + value;
            }
            return value;
        }



        private string GetHexDataArrayWithHexLengthPrefix(string hexvalue)
        {
            return GetHexLengthPrefix(hexvalue) + " " + GetHexDataArray(hexvalue);
        }

        private string GetHexLengthPrefix(string invalue)
        {
            invalue = (invalue.Length / 2).ToString("X");
            if (invalue.Length < 16)
                invalue = "0" + invalue;

            var prefix = "0x00 " + GetHexDataArray(invalue);
            return prefix;
        }

        private string GetHexDataArray(string invalue)
        {
            string hexarray = string.Empty;
            for (int i = 0; i < invalue.Length; i = i + 2)
            {
                hexarray += "0x" + invalue.Substring(i, 2) + " ";
            }
            return hexarray.Trim();
        }

        private void ResetFields()
        {
            txtUpcaBarcode.Text = string.Empty;
            txtEpcId.Text = string.Empty;
            txtNewEpcId.Text = string.Empty;

            txtUserBank.Text = string.Empty;
            rdoASCII.Checked = rdoHex.Checked = false;

            cmbFilterValue.SelectedIndex = 0;
            cmbPartition.SelectedIndex = 0;
            txtSerialNumber.Text = string.Empty;
            chkAutoIncrement.Checked = false;

            overrideSerial = true;
            overrideValues = true;
        }

        private void SetStatusIcon(CommandStatus status)
        {
            Bitmap icon = null;
            switch (status)
            {
                case CommandStatus.Failed:
                    icon = Properties.Resources.icon_error;
                    break;
                case CommandStatus.Success:
                    icon = Properties.Resources.icon_success;
                    break;
            }

            if (statusIcon.InvokeRequired)
            {
                statusIcon.Invoke(new MethodInvoker(delegate
                {
                    if (icon == null)
                    {
                        statusIcon.Visible = false;
                    }
                    else
                    {
                        statusIcon.Visible = true;
                        statusIcon.Image = icon;
                    }
                }));
            }
            else
            {
                if (icon == null)
                {
                    statusIcon.Visible = false;
                }
                else
                {
                    statusIcon.Visible = true;
                    statusIcon.Image = icon;
                }
            }
        }



        private void rdoASCII_Binary_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoASCII.Checked)
            {
                SetTextboxText(txtUserBank, userBankASCII);
            }
            if (rdoHex.Checked)
            {
                SetTextboxText(txtUserBank, userBankHex);
            }
        }

        private void tabCtrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GetSelectedTabName().Equals(SSW_TAB_NAME))
            {
                ResetFields();
                if (initSSW)
                {
                    overrideSerial = true;
                    overrideValues = true;
                    initSSW = true;
                }
            }
        }

        private void chkSgtin96_CheckedChanged(object sender, EventArgs e)
        {
            CreateNewEpcId();
        }

        private void cmbFilterValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentFilterValue = Convert.ToInt32(GetComboboxItem(cmbFilterValue));
            overrideValues = false;
            CreateNewEpcId();
        }

        private void cmbPartition_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPartitionValue = Convert.ToInt32(GetComboboxItem(cmbPartition));
            CurrentPartitionRecord = GetPartitionTableRecord(CurrentPartitionValue);
            overrideValues = false;
            CreateNewEpcId();
        }

        private void txtSerialNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtSerialNumber_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSerialNumber.Text))
            {
                overrideSerial = true;
                return;
            }

            var newValue = Convert.ToInt64(GetTextboxText(txtSerialNumber));
            if (newValue > 0 && newValue < SERIAL_MAX_VALUE)
            {
                CurrentSerialNumber = newValue;
                overrideSerial = false;
                CreateNewEpcId();
            }
            else
            {
                SetTextboxText(txtSerialNumber, string.Empty);
                MessageBox.Show("For use in a 96-bit RFID tag, the serial number may be from 1 to 12 digits in length.It must be less than or equal to 274877906943, and the first digit may not be a zero.", "Error", MessageBoxButtons.OK);
            }
        }

        private void btnWriteTag_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(GetTextboxText(txtUpcaBarcode)) || string.IsNullOrEmpty(GetTextboxText(txtEpcId)) || string.IsNullOrEmpty(GetTextboxText(txtNewEpcId)))
                return;

            if (string.IsNullOrEmpty(txtSerialNumber.Text))
            {
                MessageBox.Show("Serial Number should not be empty.", "Error", MessageBoxButtons.OK);
                return;
            }

            SetStatusIcon(CommandStatus.None);
            SetTextboxText(txtUserBank, string.Empty);
            rdoASCII.Checked = rdoHex.Checked = false;

            string epcid = GetTextboxText(txtNewEpcId);
            var status = WriteTag(currentEpcId, RfidBank.Epc, epcid, 2, "00");
            if (status == STATUS_SUCCESS)
            {
                if (chkAutoIncrement.Checked)
                {
                    CurrentSerialNumber++;
                    SetTextboxText(txtSerialNumber, CurrentSerialNumber.ToString());
                }
                SetStatusIcon(CommandStatus.Success);
            }
            else
            {
                SetStatusIcon(CommandStatus.Failed);
            }
            ClearTagCache();
        }

        private void btnVerifyTag_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(GetTextboxText(txtUpcaBarcode)) || string.IsNullOrEmpty(GetTextboxText(txtEpcId)) || string.IsNullOrEmpty(GetTextboxText(txtNewEpcId)))
                return;

            SetStatusIcon(CommandStatus.None);
            SetTextboxText(txtUserBank, string.Empty);
            rdoASCII.Checked = rdoHex.Checked = false;

            bool validation = false;
            if (GetTextboxText(txtEpcId) == GetTextboxText(txtNewEpcId))
            {
                validation = true;
            }
            else
            {
                string conpanyPrefix = CurrentEPCsUPCPortionInBinary.Substring(0, CurrentPartitionRecord.CompanyPrefixBits);
                string itemReference = CurrentEPCsUPCPortionInBinary.Substring(CurrentPartitionRecord.CompanyPrefixBits, CurrentPartitionRecord.ItemReferenceBits);

                var upcPrefix = Convert.ToInt64(conpanyPrefix, 2);
                var upcReference = Convert.ToInt64(itemReference, 2);
                UpdateCompanyItemValues();
                validation = (CurrentUpcConpanyPrefix == upcPrefix) && (CurrentUpcItemReference == upcReference);
            }

            if (validation)
            {
                SetStatusIcon(CommandStatus.Success);

                userBankHex = string.Empty;
                var status = ReadTag(currentEpcId, RfidBank.User, 0, 0, String.Empty, out userBankHex);
                if (status == STATUS_SUCCESS && !string.IsNullOrEmpty(userBankHex))
                {
                    for (int i = userBankHex.Length; i > 0; i = 4)
                    {
                        if (userBankHex.Length == 0)
                            break;

                        var hexval = userBankHex.Substring(userBankHex.Length - 4, 4);
                        if (hexval == "0x00")
                        {
                            userBankHex = userBankHex.Substring(0, userBankHex.Length - 4).Trim();
                        }
                        else
                        {
                            break;
                        }
                    }

                    var nonePrintable = new Regex("0x([0-1])");
                    var ismatch = nonePrintable.IsMatch(userBankHex);
                    try
                    {
                        userBankASCII = ismatch ? "Unable to decode ASCII." : BaseMethods.GetReadableScanDataLabel(userBankHex);
                    }
                    catch
                    {
                        userBankASCII = "Unable to decode ASCII.";
                    }

                    try
                    {
                        userBankHex = userBankHex.Replace("0x", string.Empty).Trim().ToUpper();
                    }
                    catch
                    {
                        userBankHex = "00";
                    }

                    if (ismatch)
                    {
                        SetTextboxText(txtUserBank, userBankHex);
                        rdoHex.Checked = true;
                    }
                    else
                    {
                        SetTextboxText(txtUserBank, userBankASCII);
                        rdoASCII.Checked = true;
                    }
                }
                else
                {
                    MessageBox.Show("Failed to read user bank.", "Warning", MessageBoxButtons.OK);
                }
            }
            else
            {
                SetTextboxText(txtUserBank, string.Empty);
                userBankASCII = userBankHex = string.Empty;
                SetStatusIcon(CommandStatus.Failed);
                SendUif(ERROR_BEEP_UIF_CODE);
            }
            ClearTagCache();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetFields();
        }
    }
}
