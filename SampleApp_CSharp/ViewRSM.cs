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
using System.Linq;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        private void GetAllAttributes()
        {
            if (IsScannerConnected())
            {
                string inXML = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                               "</inArgs>";
                string outXML = String.Empty;
                int iStatus = STATUS_FALSE;
                ExecCmd(RSM_ATTR_GETALL, ref inXML, out outXML,out iStatus);
                DisplayResult(iStatus, "ATTR_GETALL");
                UpdateOutXml(outXML);
                

                if(iStatus == STATUS_SUCCESS)
                {
                    List<KeyValuePair<int, string>> lstIDs;
                    m_xml.ReadXmlString_RSMIDList(outXML, out lstIDs);
                    dgvAttributes.Rows.Clear();

                    lstIDs.ForEach(id =>
                    {
                        int index = dgvAttributes.Rows.Add();
                        dgvAttributes.Rows[index].Cells[0].Value = id.Key;
                    });
                }
            }
        }
        
        private void GetAttributes()
        {
            if (IsScannerConnected())
            {
                string strNumbers = GetSelectedAttrNumbers();
                if (String.IsNullOrEmpty(strNumbers))
                {
                    MessageBox.Show("Please select non-empty row(s) to get attributes", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string inXML = GetInXmlForGetSetStore(strNumbers);
                string outXML = "";
                int iStatus = STATUS_FALSE;
                ExecCmd(RSM_ATTR_GET, ref inXML, out outXML, out iStatus);
                UpdateOutXml(outXML);
                DisplayResult(iStatus, "ATTR_GET");

                if(iStatus == STATUS_SUCCESS)
                {
                    List<KeyValuePair<int, string[]>> lstIDProperty;
                    m_xml.ReadXmlString_RSMIDProperty(outXML, out lstIDProperty);
                    if (lstIDProperty.Count == 0)
                    {
                        UpdateResults("ATTR_GET" + " - null return");
                        return;
                    }
                    foreach (DataGridViewRow selectedRow in dgvAttributes.SelectedRows)
                    {
                        object obj = selectedRow.Cells[0].Value;

                        KeyValuePair<int, string[]> foundProperty = lstIDProperty
                            .FirstOrDefault(prop => prop.Key == Convert.ToInt32(obj));

                        if (!Equals(foundProperty, default(KeyValuePair<int, string[]>)))
                        {
                            selectedRow.Cells[1].Value = foundProperty.Value[0];
                            selectedRow.Cells[2].Value = foundProperty.Value[1];
                            selectedRow.Cells[3].Value = foundProperty.Value[2];
                        }
                    }
                }
            }
        }

        private void GetNextAttribute()
        {
            if (IsScannerConnected())
            {
                string strNumber = GetSelectedOneAttrNumber();
                string inXML = GetInXmlForGetSetStore(strNumber);
                string outXML = "";
                int iStatus = STATUS_FALSE;
                ExecCmd(RSM_ATTR_GETNEXT,ref inXML,out outXML,out iStatus);
                UpdateOutXml(outXML);
                DisplayResult(iStatus, "ATTR_GETNEXT");

                if(iStatus == STATUS_SUCCESS)
                {
                    // Last attribute
                    if (!String.IsNullOrEmpty(outXML))
                    {
                        List<KeyValuePair<int, string[]>> lstIDProperty;
                        m_xml.ReadXmlString_RSMIDProperty(outXML, out lstIDProperty);

                        dgvAttributes.Rows[dgvAttributes.CurrentCell.RowIndex + 1].Cells[1].Value = lstIDProperty[0].Value[0];
                        dgvAttributes.Rows[dgvAttributes.CurrentCell.RowIndex + 1].Cells[2].Value = lstIDProperty[0].Value[1];
                        dgvAttributes.Rows[dgvAttributes.CurrentCell.RowIndex + 1].Cells[3].Value = lstIDProperty[0].Value[2];

                        int iSelectedRow = dgvAttributes.CurrentCell.RowIndex;
                        dgvAttributes.CurrentCell = dgvAttributes.Rows[++iSelectedRow].Cells[0];
                    }
                }
            }
        }

        private void SetStoreAttributeValue(int iOpcode)
        {
            if (IsScannerConnected())
            {
                string strNumValPair = GetAttrNumVals();
                if (String.IsNullOrEmpty(strNumValPair))
                {
                    return;
                }
                string inXML = GetInXmlForGetSetStore(strNumValPair); ;
                string strOpCode = (iOpcode == RSM_ATTR_SET) ? "RSM_ATTR_SET" : "RSM_ATTR_STORE";
                string outXML = ExecuteActionCommand(iOpcode, strOpCode, inXML);
                UpdateOutXml(outXML);
            }
        }
        private void SelectAllAttributes()
        {
            for (int index = 0; index < dgvAttributes.Rows.Count; index++)
            {
                dgvAttributes.Rows[index].Selected = true;
            }
        }

        private void ClearAllRsmData()
        {
            dgvAttributes.Rows.Clear();

            foreach (Scanner scanr in m_arScanners)
            {
                m_xml.clear_scanner_attributes(scanr);
            }
        }

        private string GetSelectedAttrNumbers()
        {
            string strNumbers = "";
            if (IsScannerConnected())
            {
                int nSelectedRowCount = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (nSelectedRowCount > 0)
                {
                    var selectedValues = dgvAttributes.SelectedRows.Cast<DataGridViewRow>()
                        .Select(row => row.Cells[0].Value)
                        .Where(value => value != null)
                        .Select(value => value.ToString());
                    strNumbers = string.Join(",", selectedValues);
                }
            }
            return strNumbers;
        }

        private string GetSelectedOneAttrNumber()
        {
            string strNumbers = "";
            if (IsScannerConnected())
            {
                int nSelectedRowCount = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (1 == nSelectedRowCount)
                {
                    if (null != dgvAttributes.SelectedRows[0].Cells[0].Value)
                    {
                        strNumbers = dgvAttributes.SelectedRows[0].Cells[0].Value.ToString();
                    }
                }
            }
            return strNumbers;
        }

        private string GetAttrNumVals()
        {
            StringBuilder sb = new StringBuilder();

            if (dgvAttributes.SelectedRows.Count > 0)
            {
                int iSelectedRows = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Selected);

                for (int index = 0; index < iSelectedRows; index++)
                {
                    sb.Append("<attribute>");

                    // ID
                    string strID = dgvAttributes.SelectedRows[index].Cells[0].Value.ToString();
                    sb.Append("<id>");
                    sb.Append(strID);
                    sb.Append("</id>");

                    // Data type
                    sb.Append("<datatype>");

                    if (strID == "6000" || strID == "6001" || strID == "6003")
                    {
                        sb.Append("X");
                    }
                    else
                    {
                        sb.Append(Convert.ToString(dgvAttributes.SelectedRows[index].Cells[1].Value));
                    }
                    sb.Append("</datatype>");

                    // Value
                    sb.Append("<value>");
                    sb.Append(Convert.ToString(dgvAttributes.SelectedRows[index].Cells[3].Value));
                    sb.Append("</value>");

                    sb.Append("</attribute>");
                }
            }
            else
            {
                MessageBox.Show("Please select row/rows to set/store attributes", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return sb.ToString();
        }

        private void FillRsmList_Numbers(int nAttrCount, Scanner scanr)
        {
            if (null == scanr)
                return;
            m_nRsmAttributeCount = nAttrCount;
            for (int i = 0; i < nAttrCount; i++)
            {
                dgvAttributes.Rows[i].Cells[0].Value = scanr.m_arAttributes.GetValue(i, Scanner.POS_ATTR_ID);
            }
        }

        private void FillRsmList_Attributes(Scanner scanr)
        {
            if (null == scanr)
                return;
            for (int i = 0; i < Scanner.MAX_ATTRIBUTE_COUNT; i++)
            {
                dgvAttributes.Rows[i].Cells[1].Value = scanr.m_arAttributes.GetValue(i, Scanner.POS_ATTR_TYPE);
                dgvAttributes.Rows[i].Cells[2].Value = scanr.m_arAttributes.GetValue(i, Scanner.POS_ATTR_PROPERTY);
                dgvAttributes.Rows[i].Cells[3].Value = scanr.m_arAttributes.GetValue(i, Scanner.POS_ATTR_VALUE);
            }
        }

        private void FillRsmList_Attribute(Scanner scanr, int nIndex)
        {
            if (null == scanr)
                return;

            if (nIndex < Scanner.MAX_ATTRIBUTE_COUNT)
            {
                dgvAttributes.Rows[nIndex].Cells[1].Value = scanr.m_arAttributes.GetValue(nIndex, Scanner.POS_ATTR_TYPE);
                dgvAttributes.Rows[nIndex].Cells[2].Value = scanr.m_arAttributes.GetValue(nIndex, Scanner.POS_ATTR_PROPERTY);
                dgvAttributes.Rows[nIndex].Cells[3].Value = scanr.m_arAttributes.GetValue(nIndex, Scanner.POS_ATTR_VALUE);
            }
        }

    }
}
