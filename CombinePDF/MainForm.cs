using System;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using System.Drawing;

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
                td.Caption = "Combine PDF";
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
            if (lstFiles.Items.Count > 1)
            {
                TaskDialog tdConfirm = new TaskDialog();
                tdConfirm.Caption = "Combine PDF";
                tdConfirm.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                tdConfirm.InstructionText = "Are you sure you want to combine the files?";

                if (tdConfirm.Show() == TaskDialogResult.Yes)
                {
                    PdfDocument outputDocument = new PdfDocument();
                    string dir = txtDirectory.Text;

                    foreach (string file in lstFiles.Items)
                    {
                        PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                        int count = inputDocument.PageCount;
                        for (int idx = 0; idx < count; idx++)
                        {
                            PdfPage page = inputDocument.Pages[idx];
                            outputDocument.AddPage(page);
                        }
                    }

                    frmInput input = new frmInput();
                    input.lblPrompt.Text = "Enter the name of the combined file";
                    input.Text = "Combine PDF";

                    if (input.ShowDialog() == DialogResult.OK)
                    {
                        string name = input.txtInput.Text + ".pdf";
                        string filename = Path.Combine(dir, name);

                        TaskDialog tdOpen = new TaskDialog();
                        tdOpen.Caption = "Combine PDF";
                        tdOpen.Icon = TaskDialogStandardIcon.Information;
                        tdOpen.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                        tdOpen.InstructionText = "Files have been combined successfully";
                        tdOpen.Text = "Would you like the open the combined file now?";
                        tdOpen.FooterText = filename;

                        if (!File.Exists(filename))
                        {
                            outputDocument.Save(filename);

                            if (tdOpen.Show() == TaskDialogResult.Yes)
                            {
                                Process.Start(filename);
                            }
                        }
                        else
                        {
                            bool alwaysOverwrite = bool.Parse(XMLSettings.GetSettingsValue(XMLSettings.ApplicationSettings.AlwaysOverwrite));

                            if (alwaysOverwrite)
                            {
                                File.Delete(filename);
                                outputDocument.Save(filename);

                                if (tdOpen.Show() == TaskDialogResult.Yes)
                                {
                                    Process.Start(filename);
                                }
                            }
                            else
                            {
                                TaskDialog tdFileExists = new TaskDialog();
                                tdFileExists.Caption = "Combine PDF";
                                tdFileExists.Icon = TaskDialogStandardIcon.Warning;
                                tdFileExists.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                                tdFileExists.InstructionText = "File already exists in this location";
                                tdFileExists.Text = "Overwrite file?";
                                tdFileExists.FooterText = filename;
                                tdFileExists.FooterCheckBoxText = "Always Overwrite?";
                                tdFileExists.FooterCheckBoxChecked = false;

                                if (tdFileExists.Show() == TaskDialogResult.Yes)
                                {
                                    if (tdFileExists.FooterCheckBoxChecked.Value)
                                    {
                                        XMLSettings.SetSettingsValue(XMLSettings.ApplicationSettings.AlwaysOverwrite, "true");
                                    }
                                    else
                                    {
                                        XMLSettings.SetSettingsValue(XMLSettings.ApplicationSettings.AlwaysOverwrite, "false");
                                    }

                                    File.Delete(filename);
                                    outputDocument.Save(filename);

                                    if (tdOpen.Show() == TaskDialogResult.Yes)
                                    {
                                        Process.Start(filename);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                TaskDialog tdAddFiles = new TaskDialog();
                tdAddFiles.Caption = "Combine PDF";
                tdAddFiles.Icon = TaskDialogStandardIcon.Warning;
                tdAddFiles.StandardButtons = TaskDialogStandardButtons.Ok;
                tdAddFiles.InstructionText = "At least two files must be provided before combining";
                tdAddFiles.Text = "Click Add File to add more files";

                tdAddFiles.Show();
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = ColorTranslator.FromHtml("#729DCE");

            Rectangle rect1 = new Rectangle(lstFiles.Location.X, lstFiles.Location.Y, lstFiles.ClientSize.Width, lstFiles.ClientSize.Height);
            rect1.Inflate(1, 1);
            ControlPaint.DrawBorder(e.Graphics, rect1, borderColor, ButtonBorderStyle.Solid);

            Rectangle rect11 = new Rectangle(lstFiles.Location.X, lstFiles.Location.Y, lstFiles.ClientSize.Width, lstFiles.ClientSize.Height);
            rect11.Inflate(3, 3);
            ControlPaint.DrawBorder(e.Graphics, rect11, borderColor, ButtonBorderStyle.Solid);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                int i = lstFiles.SelectedIndex;
                int j = lstFiles.SelectedIndex - 1;

                string itemToMove = lstFiles.Items[i].ToString();

                lstFiles.Items.RemoveAt(i);
                lstFiles.Items.Insert(j, itemToMove);

            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                int i = lstFiles.SelectedIndex;
                int j = lstFiles.SelectedIndex + 1;

                string itemToMove = lstFiles.Items[i].ToString();

                lstFiles.Items.RemoveAt(i);
                lstFiles.Items.Insert(j, itemToMove);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string dir = string.Empty;
            dir = txtDirectory.Text;

            if (dir != string.Empty)
            {
                lstFiles.Items.Clear();

                string[] files = Files(dir);

                foreach (string file in files)
                {
                    lstFiles.Items.Add(file);
                }
            }
            else
            {
                TaskDialog tdSpecifyDirectory = new TaskDialog();
                tdSpecifyDirectory.Caption = "Combine PDF";
                tdSpecifyDirectory.Icon = TaskDialogStandardIcon.Information;
                tdSpecifyDirectory.StandardButtons = TaskDialogStandardButtons.Ok;
                tdSpecifyDirectory.InstructionText = "No directory specified";
                tdSpecifyDirectory.Text = "Click Browse to specify a directory before refreshing";

                tdSpecifyDirectory.Show();
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void btnClearSettings_Click(object sender, EventArgs e)
        {
            TaskDialog tdDeleteSettings = new TaskDialog();
            tdDeleteSettings.Caption = "Combine PDF";
            tdDeleteSettings.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
            tdDeleteSettings.InstructionText = "Clear all application settings?";

            if (tdDeleteSettings.Show() == TaskDialogResult.Yes)
            {
                File.Delete(XMLSettings.AppSettingsFile);
            }
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                string selectedFile = lstFiles.SelectedItem.ToString();
                int index = lstFiles.SelectedIndex;

                TaskDialog td = new TaskDialog();
                td.Caption = "Combine PDF";
                td.Icon = TaskDialogStandardIcon.Warning;
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                td.InstructionText = "Are you sure you want to delete the selected file?";
                td.Text = "THE FILE WILL BE PERMANENTLY DELETED FROM YOUR COMPUTER";
                td.FooterText = selectedFile;

                if (td.Show() == TaskDialogResult.Yes)
                {
                    File.Delete(selectedFile);
                    lstFiles.Items.RemoveAt(index);
                }
            }
        }
    }
}
