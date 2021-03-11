using System;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;

namespace CombinePDF
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private string[] Files(string dir)
        {
            string[] fInfos = Directory.GetFiles(dir, "*.pdf", SearchOption.TopDirectoryOnly);

            return fInfos;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            XMLSettings.CreateAppSettings_SetDefaults();

            string dir = XMLSettings.GetSettingsValue(XMLSettings.ApplicationSettings.DefaultDirectory);
            txtDirectory.Text = dir;
            txtDirectory.Select(dir.Length + 1, 0);

            bool isChecked = DrawingDirectoryIsDefault(dir);
            ckbDefault.Checked = isChecked;

            if (isChecked)
            {
                // Load all PDF files in directory
                lstFiles.Items.Clear();

                string[] files = Files(dir);
                foreach (string file in files)
                {
                    lstFiles.Items.Add(file);
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dir = dialog.FileName;
                txtDirectory.Text = dir;

                // Load all PDF files in directory
                lstFiles.Items.Clear();

                string[] files = Files(dir);
                foreach (string file in files)
                {
                    lstFiles.Items.Add(file);
                }
            }
        }

        private void txtDirectory_TextChanged(object sender, EventArgs e)
        {
            string dir = txtDirectory.Text;

            if (!DrawingDirectoryIsDefault(dir))
            {
                ckbDefault.Checked = false;
                ckbDefault.Enabled = true;
            }
            else
            {
                ckbDefault.Checked = true;
                ckbDefault.Enabled = false;
            }
        }

        private bool DrawingDirectoryIsDefault(string dir)
        {
            bool flag = false;
            dir = txtDirectory.Text;

            string savedDir = XMLSettings.GetSettingsValue(XMLSettings.ApplicationSettings.DefaultDirectory);

            if (dir != string.Empty && System.IO.Directory.Exists(dir))
            {
                if (dir == savedDir)
                    flag = true;
                else
                    flag = false;
            }

            return flag;
        }

        private void ckbDefault_CheckedChanged(object sender, EventArgs e)
        {
            string dir = txtDirectory.Text;
            bool isChecked = ckbDefault.Checked;

            if (isChecked)
                if (dir != string.Empty && System.IO.Directory.Exists(dir))
                {
                    XMLSettings.SetSettingsValue(XMLSettings.ApplicationSettings.DefaultDirectory, dir);
                    ckbDefault.Enabled = false;
                }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a file to add";
            ofd.InitialDirectory = XMLSettings.GetSettingsValue(XMLSettings.ApplicationSettings.DefaultDirectory);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lstFiles.Items.Add(ofd.FileName);
            }
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {

        }
    }
}
