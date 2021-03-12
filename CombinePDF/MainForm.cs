using System;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;

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
                // Load all PDF files from directory
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

                // Load all PDF files from directory
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
            ofd.Filter = "PDF files (*.pdf)|*.pdf";
            ofd.InitialDirectory = XMLSettings.GetSettingsValue(XMLSettings.ApplicationSettings.DefaultDirectory);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lstFiles.Items.Add(ofd.FileName);
            }
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                string selectedFile = lstFiles.SelectedItem.ToString();
                int index = lstFiles.SelectedIndex;

                TaskDialog td = new TaskDialog();
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                td.InstructionText = "Are you sure you want to remove the selected file from the list?";
                td.Text = selectedFile;
                td.FooterText = "This will not delete the actual file";

                if (td.Show() == TaskDialogResult.Yes)
                {
                    lstFiles.Items.RemoveAt(index);
                }
            }
        }

        private void btnCombine_Click(object sender, EventArgs e)
        {
            TaskDialog tdConfirm = new TaskDialog();
            tdConfirm.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
            tdConfirm.InstructionText = "Are you sure you want to combine the files?";

            if (tdConfirm.Show() == TaskDialogResult.Yes)
            {
                PdfDocument outputDocument = new PdfDocument();
                string dir = txtDirectory.Text;

                string[] files = Files(dir);

                foreach (string file in files)
                {
                    // Open the document to import pages from it.
                    PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                    int count = inputDocument.PageCount;
                    for (int idx = 0; idx < count; idx++)
                    {
                        PdfPage page = inputDocument.Pages[idx];
                        outputDocument.AddPage(page);
                    }
                }

                string filename = dir + @"\Combined.pdf";
                outputDocument.Save(filename);

                TaskDialog tdOpen = new TaskDialog();
                tdOpen.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                tdOpen.InstructionText = "Files have been combined successfully";
                tdOpen.Text = "Would you like the open the combined file now?";
                tdOpen.FooterText = filename;

                if (tdOpen.Show() == TaskDialogResult.Yes)
                {
                    Process.Start(filename);
                }
            }
        }
    }
}
