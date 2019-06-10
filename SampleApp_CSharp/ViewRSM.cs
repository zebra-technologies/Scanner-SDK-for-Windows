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
        
        private void GetAllAttribs()
        {
            if (IsMotoConnectedWithScanners())
            {
                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "</inArgs>";

                int opCode = RSM_ATTR_GETALL;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "ATTR_GETALL");
                UpdateOutXml(outXml);
                if (!chkAsync.Checked)
                {
                    Scanner scanr = null;
                    int nIndex = -1;
                    int nAttrCount = 0;
                    int nOpCode = -1;
                    m_xml.ReadXmlString_RsmAttr(outXml, m_arScanners, out scanr, out nIndex, out nAttrCount, out nOpCode);
                    FillRsmList_Numbers(nAttrCount, scanr);
                }
            }
        }
        private bool performRSMSetStoreAttribute(Scanner.RSMAttribute rsmAttribute, bool isStore)
        {
            if (IsMotoConnectedWithScanners())
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
            if (IsMotoConnectedWithScanners())
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
        private void PerformBtnGetClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                string strNumbers = GetSelectedAttrNumbers();
                if ("" == strNumbers || null == strNumbers)
                {
                    MessageBox.Show("Please select non-empty row(s) to get attributes", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                    "<arg-xml>" +
                                    "<attrib_list>" +
                                    strNumbers +
                                    "</attrib_list>" +
                                    "</arg-xml>" +
                                    "</cmdArgs>" +
                                    "</inArgs>";

                int opCode = RSM_ATTR_GET;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                UpdateOutXml(outXml);
                DisplayResult(status, "ATTR_GET");
                if (!chkAsync.Checked)
                {

                    Scanner scanr = null;
                    int nIndex = -1;
                    int nAttrCount = 0;
                    int nOpCode = -1;

                    m_xml.ReadXmlString_RsmAttr(outXml, m_arScanners, out scanr, out nIndex, out nAttrCount, out nOpCode);
                    int nSelectedRowCount = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Selected);
                    if (nSelectedRowCount > 0)
                    {
                        for (int i = nSelectedRowCount - 1; i >= 0; i--)
                        {
                            if (null != dgvAttributes.SelectedRows[i].Cells[0].Value)
                            {
                                FillRsmList_Attribute(scanr, dgvAttributes.SelectedRows[i].Index);
                            }
                        }
                    }
                }
            }
        }

        private int GetLastNonEmptyRowIndex()
        {
            int lastRowIndex = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Visible);
            int i;
            for (i = lastRowIndex - 1; i >= 0; i--)
            {
                if ((null != dgvAttributes.Rows[i].Cells[0].Value) && ("" != dgvAttributes.Rows[i].Cells[0].Value))
                {
                    break;
                }
            }
            return i;
        }

        private void PerformBtnGetNextClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
               
                string strNumber = GetSelectedOneAttrNumber();
              
                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                    "<arg-xml>" +
                                    "<attrib_list>" +
                                    strNumber +
                                    "</attrib_list>" +
                                    "</arg-xml>" +
                                    "</cmdArgs>" +
                                    "</inArgs>";

                int opCode = RSM_ATTR_GETNEXT;
                string outXml = "";
                int status = STATUS_FALSE;
                int selectedRowCount = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if(1 != selectedRowCount)
                {
                    MessageBox.Show("Please select one row to get next-attributes", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if ("" == strNumber || null == strNumber)
                {
                    MessageBox.Show("Please select a non-empty row to get next-attributes", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (GetLastNonEmptyRowIndex() == dgvAttributes.Rows.GetLastRow(DataGridViewElementStates.Selected))
                {
                    MessageBox.Show("Last row is invalid to select to get next-attributes", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ExecCmd(opCode, ref inXml, out outXml, out status);
                UpdateOutXml(outXml);
                DisplayResult(status, "ATTR_GETNEXT");
               
                if (!chkAsync.Checked)
                {
                    Scanner scanr = null;
                    int nIndex = -1;
                    int nAttrCount = 0;
                    int nOpCode = -1;
                    m_xml.ReadXmlString_RsmAttr(outXml, m_arScanners, out scanr, out nIndex, out nAttrCount, out nOpCode);
                    FillRsmList_Attribute(scanr, nIndex);
                }
            }
        }

        private void SetStoreAttributeValue(int opCode)
        {
            if (IsMotoConnectedWithScanners())
            {
                string strNumValPair = GetAttrNumVals();
                if ("" == strNumValPair)
                    return;
                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                    "<arg-xml>" +
                                    "<attrib_list>" +
                                    strNumValPair +
                                    "</attrib_list>" +
                                    "</arg-xml>" +
                                    "</cmdArgs>" +
                                    "</inArgs>";


                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                UpdateOutXml(outXml);
                string strOpCode = "RSM_ATTR_SET";
                if (RSM_ATTR_STORE == opCode)
                {
                    strOpCode = "RSM_ATTR_STORE";
                }
                DisplayResult(status, strOpCode);
            }
        }

        private void PerformBtnSelectAllClick(object sender, EventArgs e)
        {
            for (int i = 0; i < m_nRsmAttributeCount; i++)
            {
                dgvAttributes.Rows[i].Selected = true;
            }
        }

        private void ClearAllRsmData()
        {
            //clear ALL cells of all rows
            int nRowCount = dgvAttributes.RowCount;
            if (0 < nRowCount)
            {
                for (int i = 0; i < nRowCount; i++)
                {
                    dgvAttributes.Rows[i].Cells[0].Value = "";
                    dgvAttributes.Rows[i].Cells[1].Value = "";
                    dgvAttributes.Rows[i].Cells[2].Value = "";
                    dgvAttributes.Rows[i].Cells[3].Value = "";
                    dgvAttributes.Rows[i].Selected = false;
                }
            }

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
            if (IsMotoConnectedWithScanners())
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
            if (IsMotoConnectedWithScanners())
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
            string strNumValPair = "";
            if (IsMotoConnectedWithScanners())
            {
                int nRowCount = m_nRsmAttributeCount;
                int nSelectedRowCount = dgvAttributes.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if ((0 < nRowCount) && (0 < nSelectedRowCount))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = 0; i < nSelectedRowCount; i++)
                    {
                        if (null != dgvAttributes.SelectedRows[i].Cells[0].Value &&
                            null != dgvAttributes.SelectedRows[i].Cells[3].Value)
                        {
                            sb.Append("<attribute>");
                            sb.Append("<id>");
                            sb.Append(dgvAttributes.SelectedRows[i].Cells[0].Value.ToString());
                            sb.Append("</id>");
                            sb.Append("<datatype>");
                            string AttribID = dgvAttributes.SelectedRows[i].Cells[0].Value.ToString();
                            if (AttribID == "6000" || AttribID == "6001" || AttribID == "6003")
                            {
                                sb.Append("X");
                            }
                            else
                            {
                                sb.Append(dgvAttributes.SelectedRows[i].Cells[1].Value.ToString());
                            }
                            sb.Append("</datatype>");
                            sb.Append("<value>");
                            sb.Append(dgvAttributes.SelectedRows[i].Cells[3].Value.ToString());
                            sb.Append("</value>");
                            sb.Append("</attribute>");
                        }
                    }
                    strNumValPair = sb.ToString();
                }
                else
                    MessageBox.Show("Pl. select row/rows to set/store attributes", APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return strNumValPair;
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
