using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        private void PerformReadWeightClick(object sender, EventArgs e)
        {

            string outXml = ExecuteActionCommandOffline(SCALE_READ_WEIGHT, "SCALE_READ_WEIGHT");
            UpdateOutXml(outXml);

            string weight;
            string weightMode;
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

        }

    }
}
