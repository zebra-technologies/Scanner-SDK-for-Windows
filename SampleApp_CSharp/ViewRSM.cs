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
        
        private void GetAllAttributes()
        {
            if (IsScannerConnected())
            {
                string inXML = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                               "</inArgs>";

                string outXML = String.Empty;
                int iOpcode = RSM_ATTR_GETALL;
                int iStatus = STATUS_FALSE;

                ExecCmd(iOpcode, ref inXML, out outXML, out iStatus);
                DisplayResult(iStatus, "ATTR_GETALL");
                UpdateOutXml(outXML);

                if(iStatus == STATUS_SUCCESS)
                {
                    List<KeyValuePair<int, string>> lstIDs;
                    m_xml.ReadXmlString_RSMIDList(outXML, out lstIDs);
                    dgvAttributes.Rows.Clear();

                    for (int index = 0; index < lstIDs.Count; index++)
                    {
                        dgvAttributes.Rows.Add();
                        dgvAttributes.Rows[index].Cells[0].Value = lstIDs[index].Key;
                    }
                }
            }
        }
        private bool performRSMSetStoreAttribute(Scanner.RSMAttribute rsmAttribute, bool isStore)
        {
            if (IsScannerConnected())
            {

                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                    "<arg-xml>" +
                                    "<attrib_list>" +
                                        "<attribute>" +
                                            "<id>" + rsmAttribute.ID + "</id>" +
                                            "<datatype>" + rsmAttribute.Type + "</datatype>" +
                                            "<value>" + rsmAttribute.value + "</value>" +
                                        "</attribute>" +
                                    "</attrib_list>" +
                                    "</arg-xml>" +
                                    "</cmdArgs>" +
                                    "</inArgs>";


                string outXml = "";
                int status = STATUS_FALSE;
                int opCode = isStore ? RSM_ATTR_STORE : RSM_ATTR_SET;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                UpdateOutXml(outXml);
                string strOpCode = "RSM_ATTR_SET";
                if (RSM_ATTR_STORE == opCode)
                {
                    strOpCode = "RSM_ATTR_STORE";
                }
                DisplayResult(status, strOpCode);
            }
            return false;
        }
        private bool PerformRSMGetAttribute( int attributeNo, out Scanner.RSMAttribute attribute )
        {
            value = null;
            attribute = null;
            if (IsScannerConnected())
            {
                string inXml = "<inArgs>" +
                                GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                        "<arg-xml>" +
                                            "<attrib_list>" +
                                                attributeNo +
                                            "</attrib_list>" +
                                        "</arg-xml>" +
                                    "</cmdArgs>" +
                                "</inArgs>";

                int opCode = RSM_ATTR_GET;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
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

                string inXML = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                        "<cmdArgs>" +
                                            "<arg-xml>" +
                                                "<attrib_list>" +
                                                    strNumbers +
                                                "</attrib_list>" +
                                            "</arg-xml>" +
                                        "</cmdArgs>" +
                                    "</inArgs>";

                string outXML = "";
                int iOpcode = RSM_ATTR_GET;
                int iStatus = STATUS_FALSE;

                ExecCmd(iOpcode, ref inXML, out outXML, out iStatus);
                UpdateOutXml(outXML);
                DisplayResult(iStatus, "ATTR_GET");

                if(iStatus == STATUS_SUCCESS)
                {
                    List<KeyValuePair<int, string[]>> lstIDProperty;
                    m_xml.ReadXmlString_RSMIDProperty(outXML, out lstIDProperty);

                    int iRowCountSelected = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Selected);

                    for (int iOuterIndex = iRowCountSelected - 1; iOuterIndex >= 0; iOuterIndex--)
                    {
                        object obj = dgvAttributes.SelectedRows[iOuterIndex].Cells[0].Value;

                        for (int iInnerIndex = 0; iInnerIndex < lstIDProperty.Count; iInnerIndex++)
                        {
                            int val = lstIDProperty[iInnerIndex].Key;
                            if (Convert.ToInt32(obj) == val)
                            {
                                dgvAttributes.SelectedRows[iOuterIndex].Cells[1].Value = lstIDProperty[iInnerIndex].Value[0];
                                dgvAttributes.SelectedRows[iOuterIndex].Cells[2].Value = lstIDProperty[iInnerIndex].Value[1];
                                dgvAttributes.SelectedRows[iOuterIndex].Cells[3].Value = lstIDProperty[iInnerIndex].Value[2];

                                break; // Break the inner iteration
                            }
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
              
                string inXML = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                        "<cmdArgs>" +
                                            "<arg-xml>" +
                                                "<attrib_list>" +
                                                    strNumber +
                                                "</attrib_list>" +
                                            "</arg-xml>" +
                                        "</cmdArgs>" +
                                "</inArgs>";

                int iOpcode = RSM_ATTR_GETNEXT;
                string outXML = "";
                int iStatus = STATUS_FALSE;

                ExecCmd(iOpcode, ref inXML, out outXML, out iStatus);
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

                string inXML = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                        "<cmdArgs>" +
                                            "<arg-xml>" +
                                                "<attrib_list>" +
                                                    strNumValPair +
                                                "</attrib_list>" +
                                            "</arg-xml>" +
                                        "</cmdArgs>" +
                                    "</inArgs>";

                string outXML = "";
                int iStatus = STATUS_FALSE;
                ExecCmd(iOpcode, ref inXML, out outXML, out iStatus);
                UpdateOutXml(outXML);

                string strOpCode = (iOpcode == RSM_ATTR_SET) ? "RSM_ATTR_SET" : "RSM_ATTR_STORE";
                DisplayResult(iStatus, strOpCode);
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

        private void PerformBtnClearAllValuesClick(object sender, EventArgs e)
        {
            //clear "Value" cell of all rows
            int nRowCount = dgvAttributes.RowCount;
            if (0 < nRowCount)
            {
                for (int i = 0; i < nRowCount; i++)
                {
                    dgvAttributes.Rows[i].Cells[3].Value = "";
                }
            }
        }

        private void PerformBtnClearValueClick(object sender, EventArgs e)
        {
            //clear "Value" cell of all selected rows
            int nSelectedRowCount = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (0 < nSelectedRowCount)
            {
                for (int i = 0; i < nSelectedRowCount; i++)
                {
                    dgvAttributes.SelectedRows[i].Cells[3].Value = "";
                }
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
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = nSelectedRowCount - 1; i >= 0; i--)
                    {
                        if (null != dgvAttributes.SelectedRows[i].Cells[0].Value)
                        {
                            if (nSelectedRowCount - 1 != i)
                            {
                                sb.Append(",");
                            }
                            sb.Append(dgvAttributes.SelectedRows[i].Cells[0].Value.ToString());
                        }
                    }
                    strNumbers = sb.ToString();
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
                    sb.Append("<id>");
                    sb.Append(dgvAttributes.SelectedRows[index].Cells[0].Value.ToString());
                    sb.Append("</id>");

                    // Data type
                    sb.Append("<datatype>");
                    string strID = dgvAttributes.SelectedRows[index].Cells[0].Value.ToString();
                    if (strID == "6000" || strID == "6001" || strID == "6003")
                    {
                        sb.Append("X");
                    }
                    else
                    {
                        sb.Append(dgvAttributes.SelectedRows[index].Cells[1].Value.ToString());
                    }
                    sb.Append("</datatype>");

                    // Value
                    sb.Append("<value>");
                    sb.Append(dgvAttributes.SelectedRows[index].Cells[3].Value.ToString());
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

    }
}
