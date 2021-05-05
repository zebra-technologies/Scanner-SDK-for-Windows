using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace Scanner_SDK_Sample_Application
{
    partial class TopologyPopupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopologyPopupForm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.close = new System.Windows.Forms.Button();
            this.lblTopologyHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(418, 432);
            this.treeView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "DS6707.ico");
            this.imageList1.Images.SetKeyName(1, "DS9808.ico");
            this.imageList1.Images.SetKeyName(2, "LS1203.ico");
            this.imageList1.Images.SetKeyName(3, "LS2208.ico");
            this.imageList1.Images.SetKeyName(4, "LS3008.ico");
            this.imageList1.Images.SetKeyName(5, "LS3408.ico");
            this.imageList1.Images.SetKeyName(6, "LS3578.ico");
            this.imageList1.Images.SetKeyName(7, "LS4208.ico");
            this.imageList1.Images.SetKeyName(8, "LS4278.ico");
            this.imageList1.Images.SetKeyName(9, "LS9203.ico");
            this.imageList1.Images.SetKeyName(10, "STB_B.ico");
            this.imageList1.Images.SetKeyName(11, "STB_W.ico");
            this.imageList1.Images.SetKeyName(12, "Unknown.ico");
            this.imageList1.Images.SetKeyName(13, "UnknownC.ico");
            this.imageList1.Images.SetKeyName(14, "xroot_node.ico");
            this.imageList1.Images.SetKeyName(15, "DS2278.ico");
            this.imageList1.Images.SetKeyName(16, "MPXXX.ico");
            this.imageList1.Images.SetKeyName(17, "DS9208.ico");
            this.imageList1.Images.SetKeyName(18, "MX101.ico");
            this.imageList1.Images.SetKeyName(19, "DS36X8.ico");
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.close.Location = new System.Drawing.Point(10, 453);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(68, 23);
            this.close.TabIndex = 1;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // lblTopologyHint
            // 
            this.lblTopologyHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTopologyHint.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lblTopologyHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTopologyHint.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTopologyHint.ImageIndex = 12;
            this.lblTopologyHint.ImageList = this.imageList1;
            this.lblTopologyHint.Location = new System.Drawing.Point(90, 455);
            this.lblTopologyHint.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTopologyHint.Name = "lblTopologyHint";
            this.lblTopologyHint.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTopologyHint.Size = new System.Drawing.Size(255, 28);
            this.lblTopologyHint.TabIndex = 2;
            this.lblTopologyHint.Text = "Hint: Unknown/Non-RSM scanner is shown as ";
            this.lblTopologyHint.Click += new System.EventHandler(this.label1_Click);
            // 
            // TopologyPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 494);
            this.Controls.Add(this.lblTopologyHint);
            this.Controls.Add(this.close);
            this.Controls.Add(this.treeView1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(414, 531);
            this.Name = "TopologyPopupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Device Topology";
            this.ResumeLayout(false);

        }

        #endregion


        

        private System.Windows.Forms.TreeView treeView1;
        private ImageList imageList1;
        private Button close;
        private Label lblTopologyHint;

    }
}