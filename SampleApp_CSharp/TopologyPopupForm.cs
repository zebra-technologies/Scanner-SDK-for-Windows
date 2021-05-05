using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Scanner_SDK_Sample_Application
{
    public partial class TopologyPopupForm : Form
    {
        public TopologyPopupForm()
        {
            InitializeComponent();
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void buildTopologyTree(string strXml)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(strXml);
            treeView1.Nodes.Clear();

            XmlNode startNode = xdoc.DocumentElement.ChildNodes[0].ChildNodes[0];
            treeView1.Nodes.Add(new TreeNode("scanners", 14, 14));
            TreeNode tNode = new TreeNode();
            tNode = treeView1.Nodes[0];
            AddNode(startNode, tNode);
        }

        private void getModelAndIcon(XmlNodeList nodeList,ref string scanner_ID, ref string model_name, ref int index)
        {
            XmlNode xNode;
            string prfx_mod_name;
            for (int i = 0; i <= nodeList.Count - 1; i++)
            {
                xNode = nodeList[i];
                if ("modelnumber" == xNode.Name)
                {
                    if ((xNode.ChildNodes.Count > 0) && (null != xNode.ChildNodes[0].Value))
                    {
                        model_name = xNode.ChildNodes[0].Value;
                        prfx_mod_name = xNode.ChildNodes[0].Value.Substring(0, 6);
                    }
                    else
                    {
                        prfx_mod_name = "";
                        model_name = "Unknown";
                    }
                    switch (prfx_mod_name)
                    {
                        case "DS6707":
                            index = 0;
                            break;
                        case "DS9808":
                            index = 1;
                            break;
                        case "LS1203":
                            index = 2;
                            break;
                        case "LS2208":
                            index = 3;
                            break;
                        case "LS3008":
                            index = 4;
                            break;
                        case "LS3408":
                            index = 5;
                            break;
                        case "LS3578":
                            index = 6;
                            break;
                        case "LS4208":
                            index = 7;
                            break;
                        case "LS4278":
                            index = 8;
                            break;
                        case "LS9203":
                            index = 9;
                            break;
                        case "STB4278":
                            index = 11;
                            break;
                        case "DS2278":
                            index = 15;
                            break;
                        case "MP6000":
                        case "MP6200":
                        case "MP7000":
                        case "MP7001":
                            index = 16;
                            break;
                        case "DS9208":
                            index = 17;
                            break;
                        case "MX101":
                            index = 18;
                            break;
                        case "DS3608":
                        case "DS3678":
                            index = 19;
                            break;
                        default:
                            //model_name = "Unknown";
                            index = 12;
                            break;
                    }
                }
                else if ("scannerID" == xNode.Name)
                {
                    scanner_ID = xNode.ChildNodes[0].Value;
                }
                else if ("scanner" == xNode.Name)
                {
                    if (12 == index)
                        index = 13;
                    return; //cradel model name follows child scanners
                    // so if it returns from here, model_name will have cradle model name  
                }
            }
            return;
        }

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i, j;



            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                string model_name = "";
                string scanner_ID = "";
                int index = 0;

                for (i = 0, j = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = nodeList[i];
                    if (xNode.HasChildNodes)
                    {
                        if ("scanner" == xNode.Name)
                        {
                            getModelAndIcon(xNode.ChildNodes,ref scanner_ID, ref model_name, ref index);
                            inTreeNode.Nodes.Add(new TreeNode("ID = "+scanner_ID+" : "+model_name, index, index));
                            tNode = inTreeNode.Nodes[j++];
                            AddNode(xNode, tNode);
                        }
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}