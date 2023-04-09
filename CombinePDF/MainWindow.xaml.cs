using System.Collections.Generic;
using System.IO;
using System.Windows;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System;

namespace CombinePDF
{
    class FileModel
    {
        public string Filename { get; set; }
        public string Filesize { get; set; }
        public int PageCount { get; set; }
    }

    public partial class MainWindow : Window
    {
        private List<FileModel> filesToCombine = new List<FileModel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            ofd.Title = "Select a file to add";
            ofd.Filter = "Pdf files (*.pdf)|*.pdf";
            ofd.InitialDirectory = Properties.Settings.Default.DefaultDirectory;
            ofd.Multiselect = true;
            bool? result = ofd.ShowDialog();

            if ((bool)result)
            {
                List<string> files = ofd.FileNames.ToList();

                foreach (string file in files)
                {
                    PdfDocument doc = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                    FileModel fileModel = new FileModel();
                    fileModel.Filename = doc.FullPath;
                    fileModel.PageCount = doc.PageCount;
                    fileModel.Filesize = $"{Math.Ceiling(doc.FileSize / 1000.0)} KB";

                    int index = filesToCombine.FindIndex(item => item.Filename == fileModel.Filename);
                    if (index == -1)
                    {
                        filesToCombine.Add(fileModel);
                    }
                }

                dg.ItemsSource = null;
                dg.ItemsSource = filesToCombine;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedItems.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                List<FileModel> files = dg.SelectedItems.Cast<FileModel>().ToList();
                files.ForEach(x => sb.AppendLine(x.Filename));

                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                td.InstructionText = "Remove selected files?";
                td.Text = "Files will not be deleted from your computer";
                td.FooterText = sb.ToString();

                if (td.Show() == TaskDialogResult.Yes)
                {
                    foreach (FileModel file in dg.SelectedItems)
                    {
                        filesToCombine.Remove(file);
                    }

                    dg.ItemsSource = null;
                    dg.ItemsSource = filesToCombine;
                }
            }
            else
            {
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = "No file selected";
                td.Text = "Select a file to remove";
                td.Show();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedItems.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                List<FileModel> files = dg.SelectedItems.Cast<FileModel>().ToList();
                files.ForEach(x => sb.AppendLine(x.Filename));

                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.Icon = TaskDialogStandardIcon.Warning;
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                td.InstructionText = "Delete selected files?";
                td.Text = "Files will be deleted from your computer";
                td.FooterText = sb.ToString();

                if (td.Show() == TaskDialogResult.Yes)
                {
                    foreach (FileModel file in dg.SelectedItems)
                    {
                        try
                        {
                            File.Delete(file.Filename);
                            filesToCombine.Remove(file);
                        }
                        catch (Exception ex)
                        {
                            Error.Show(ex);
                        }
                    }

                    dg.ItemsSource = null;
                    dg.ItemsSource = filesToCombine;
                }
            }
            else
            {
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = "No file selected";
                td.Text = "Select a file to delete";
                td.Show();
            }
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedItems.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                List<FileModel> filesToExtract = dg.SelectedItems.Cast<FileModel>().ToList();
                filesToExtract = filesToExtract.Where(x => x.PageCount > 1).ToList();
                filesToExtract.ForEach(x => sb.AppendLine(x.Filename));

                int totalPageCount = filesToExtract.Sum(x => x.PageCount);

                if (filesToExtract.Count > 0)
                {
                    TaskDialog td = new TaskDialog();
                    td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                    td.Caption = "Combine PDF";
                    td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                    td.InstructionText = "Extract pages?";
                    td.Text = $"({totalPageCount}) pages will be extracted from the following files";
                    td.FooterText = sb.ToString();

                    if (td.Show() == TaskDialogResult.Yes)
                    {
                        CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                        dialog.InitialDirectory = Properties.Settings.Default.DefaultDirectory;
                        dialog.IsFolderPicker = true;

                        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            foreach (FileModel file in filesToExtract)
                            {
                                PdfDocument inputDocument = PdfReader.Open(file.Filename, PdfDocumentOpenMode.Import);

                                string name = Path.GetFileNameWithoutExtension(file.Filename);
                                string dir = dialog.FileName;

                                for (int i = 0; i < inputDocument.PageCount; i++)
                                {
                                    PdfDocument outputDocument = new PdfDocument();
                                    outputDocument.Version = inputDocument.Version;
                                    outputDocument.Info.Title = $"Page {i + 1} of {inputDocument.Info.Title}";

                                    outputDocument.Info.Creator = inputDocument.Info.Creator;

                                    outputDocument.AddPage(inputDocument.Pages[i]);
                                    outputDocument.Save($@"{dir}\{name}-Page{i + 1}.pdf");
                                }
                            }
                        }
                    }
                }
                else
                {
                    TaskDialog msg = new TaskDialog();
                    msg.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                    msg.Caption = "Combine PDF";
                    msg.Icon = TaskDialogStandardIcon.Warning;
                    msg.StandardButtons = TaskDialogStandardButtons.Ok;
                    msg.InstructionText = "No pages to extract";
                    msg.Text = "Select files with more than one page";

                    msg.Show();
                }
            }
            else
            {
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = "No file selected";
                td.Text = "Select a file to extract";
                td.Show();
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedItems.Count > 0)
            {
                var index = 0;

                foreach (FileModel file in dg.SelectedItems)
                {
                    index = dg.Items.IndexOf(file);

                    try
                    {
                        filesToCombine.RemoveAt(index);
                        filesToCombine.Insert(index + 1, file);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        filesToCombine.Insert(index, file);
                    }
                }

                dg.ItemsSource = null;
                dg.ItemsSource = filesToCombine;
                dg.SelectedIndex = index + 1;
            }
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedItems.Count > 0)
            {
                var index = 0;

                foreach (FileModel file in dg.SelectedItems)
                {
                    index = dg.Items.IndexOf(file);

                    try
                    {
                        filesToCombine.RemoveAt(index);
                        filesToCombine.Insert(index - 1, file);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        filesToCombine.Insert(index, file);
                    }
                }

                dg.ItemsSource = null;
                dg.ItemsSource = filesToCombine;
                dg.SelectedIndex = index - 1;
            }
        }

        private void btnCombine_Click(object sender, RoutedEventArgs e)
        {
            if (filesToCombine.Count > 1)
            {
                StringBuilder sb = new StringBuilder();
                filesToCombine.ForEach(x => sb.AppendLine(x.Filename));

                TaskDialog tdConfirm = new TaskDialog();
                tdConfirm.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                tdConfirm.Caption = "Combine PDF";
                tdConfirm.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                tdConfirm.InstructionText = "Combine files?";
                tdConfirm.Text = "Files will be appended to the combined file in " +
                                 "the order that they are presented in the list";
                tdConfirm.FooterText = sb.ToString();

                if (tdConfirm.Show() == TaskDialogResult.Yes)
                {
                    PdfDocument outputDocument = new PdfDocument();

                    foreach (FileModel file in filesToCombine)
                    {
                        PdfDocument inputDocument = PdfReader.Open(file.Filename, PdfDocumentOpenMode.Import);

                        int count = inputDocument.PageCount;
                        for (int i = 0; i < count; i++)
                        {
                            PdfPage page = inputDocument.Pages[i];
                            outputDocument.AddPage(page);
                        }
                    }

                    CommonSaveFileDialog sd = new CommonSaveFileDialog();
                    CommonFileDialogFilter filter = new CommonFileDialogFilter("Pdf files", ".pdf");
                    sd.InitialDirectory = Properties.Settings.Default.DefaultDirectory;
                    sd.Title = "Save combined file";
                    sd.AlwaysAppendDefaultExtension = true;
                    sd.OverwritePrompt = !Properties.Settings.Default.AlwaysOverwrite;
                    sd.DefaultExtension = "pdf";
                    sd.DefaultFileName = "Combined";
                    sd.Filters.Add(filter);

                    if (sd.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        string fullFilePath = sd.FileName;

                        TaskDialog td = new TaskDialog();
                        td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                        td.Caption = "Combine PDF";
                        td.Icon = TaskDialogStandardIcon.Information;
                        td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                        td.InstructionText = "Files have been combined";
                        td.Text = "Open combined file?";
                        td.FooterText = fullFilePath;

                        outputDocument.Save(fullFilePath);

                        if (td.Show() == TaskDialogResult.Yes)
                        {
                            try
                            {
                                Process.Start(fullFilePath);
                            }
                            catch (Exception ex)
                            {
                                Error.Show(ex);
                            }
                        }
                    }
                }
            }
            else
            {
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.Icon = TaskDialogStandardIcon.Warning;
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = "At least two files must be provided before combining";
                td.Text = "Click Add File to add more files";

                td.Show();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            frmSettings settings = new frmSettings();
            settings.Owner = Application.Current.MainWindow;
            settings.ShowDialog();
        }

        #endregion
    }
}
