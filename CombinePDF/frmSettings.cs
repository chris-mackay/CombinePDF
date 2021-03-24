using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Forms;
using System.IO;

namespace CombinePDF
{
    public partial class frmSettings : Form
    {
        

        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            string def = XMLSettings.GetSettingsValue(XMLSettings.ApplicationSettings.DefaultDirectory);

            if (def == "")
            {
                dialog.InitialDirectory = "C:\\";
            }
            else
            {
                dialog.InitialDirectory = def;
            }

            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dir = dialog.FileName;
                txtDirectory.Text = dir;
            }
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            string dir = XMLSettings.GetSettingsValue(XMLSettings.ApplicationSettings.DefaultDirectory);
            txtDirectory.Text = dir;

            bool alwaysOverwrite = bool.Parse(XMLSettings.GetSettingsValue(XMLSettings.ApplicationSettings.AlwaysOverwrite));
            ckbAlwaysOverwrite.Checked = alwaysOverwrite;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string dir = txtDirectory.Text;
            bool alwaysOverwrite = ckbAlwaysOverwrite.Checked;

            if (alwaysOverwrite)
                XMLSettings.SetSettingsValue(XMLSettings.ApplicationSettings.AlwaysOverwrite, "true");
            else
                XMLSettings.SetSettingsValue(XMLSettings.ApplicationSettings.AlwaysOverwrite, "false");

            XMLSettings.SetSettingsValue(XMLSettings.ApplicationSettings.DefaultDirectory, dir);
        }

        private void txtDirectory_TextChanged(object sender, EventArgs e)
        {
            string dir = txtDirectory.Text;

            if (Directory.Exists(dir))
                btnSave.Enabled = true;
            else if(dir == string.Empty)
                btnSave.Enabled = true;
            else
                btnSave.Enabled = false;
        }
    }
}
