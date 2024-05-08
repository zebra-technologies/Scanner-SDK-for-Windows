using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        private void UpdateOutXml(string strOut)
        {
            if (txtOutXml.InvokeRequired)
            {
                txtOutXml.Invoke(new MethodInvoker(delegate
                {
                    txtOutXml.Text = m_xml.IndentXmlString(strOut);
                }));
            }
            else
            {
                txtOutXml.Text = m_xml.IndentXmlString(strOut);
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
    }
}
