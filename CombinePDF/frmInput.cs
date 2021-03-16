using System;
using System.IO;
using System.Windows.Forms;

namespace CombinePDF
{
    public partial class frmInput : Form
    {
        public frmInput()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //Remove the illegal characters
            txtInput.Text = GetSafeFilename(txtInput.Text);
        }

        private static string GetSafeFilename(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            if (txtInput.Text.Length > 0)
            {
                btnOK.Enabled = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        private void frmInput_Load(object sender, EventArgs e)
        {
            btnOK.Enabled = false;
        }
    }
}
