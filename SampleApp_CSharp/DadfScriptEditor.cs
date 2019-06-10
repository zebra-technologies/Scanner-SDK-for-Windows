using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Scanner_SDK_Sample_Application
{
    public partial class DadfScriptEditor : Form
    {
        private string _ScriptSource;
        public string ScriptSource { get { return _ScriptSource; } set { _ScriptSource = value; } }
        
        public DadfScriptEditor()
        {
            InitializeComponent();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ScriptSource = rxtScript.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            rxtScript.Text = ScriptSource;
        }
    }
}