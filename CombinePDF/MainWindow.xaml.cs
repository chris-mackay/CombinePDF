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
            // Create a new instance of the OpenFileDialog class
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            // Set the properties of the OpenFileDialog instance
            ofd.Title = "Select a file to add"; // Set the title of the dialog box
            ofd.Filter = "Pdf files (*.pdf)|*.pdf"; // Set the filter to only display PDF files
            ofd.InitialDirectory = Properties.Settings.Default.DefaultDirectory; // Set the initial directory to the user's default directory
            ofd.Multiselect = true; // Allow the user to select multiple files
            bool? result = ofd.ShowDialog(); // Display the dialog box and store the result

            // If the user clicked the "OK" button
            if ((bool)result)
            {
                // Retrieve the selected file names and store them in a list
                List<string> files = ofd.FileNames.ToList();

                // For each selected file
                foreach (string file in files)
                {
                    // Open the file as a PDF document
                    PdfDocument doc = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                    // Create a new FileModel instance
                    FileModel fileModel = new FileModel();

                    // Set the properties of the FileModel instance based on the PDF document metadata
                    fileModel.Filename = doc.FullPath;
                    fileModel.PageCount = doc.PageCount;
                    fileModel.Filesize = $"{Math.Ceiling(doc.FileSize / 1000.0)} KB";

                    // Check if the FileModel instance already exists in the list filesToCombine
                    int index = filesToCombine.FindIndex(item => item.Filename == fileModel.Filename);
                    if (index == -1) // If the instance does not exist
                    {
                        // Add the FileModel instance to the list
                        filesToCombine.Add(fileModel);
                    }
                }

                // Reset the ItemsSource property of the DataGrid to display the updated list of files
                dg.ItemsSource = null;
                dg.ItemsSource = filesToCombine;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Check if any files are selected
            if (dg.SelectedItems.Count > 0)
            {
                // Create a StringBuilder to store the names of the files to be removed
                StringBuilder sb = new StringBuilder();
                // Cast the selected items to a list of FileModel objects
                List<FileModel> files = dg.SelectedItems.Cast<FileModel>().ToList();
                // Append each file name to the StringBuilder
                files.ForEach(x => sb.AppendLine(x.Filename));

                // Create a TaskDialog to confirm the file removal
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                td.InstructionText = "Remove selected files?";
                td.Text = "Files will not be deleted from your computer";
                td.FooterText = sb.ToString();

                // If the user clicks "Yes", remove the selected files from the filesToCombine list
                if (td.Show() == TaskDialogResult.Yes)
                {
                    foreach (FileModel file in dg.SelectedItems)
                    {
                        filesToCombine.Remove(file);
                    }

                    // Refresh the ItemsSource of the DataGrid to reflect the changes
                    dg.ItemsSource = null;
                    dg.ItemsSource = filesToCombine;
                }
            }
            else
            {
                // If no files are selected, display an error message using a TaskDialog
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
            // Check if any items are selected in the dg (datagrid) control
            if (dg.SelectedItems.Count > 0)
            {
                // Create a StringBuilder object to store the filenames of the selected files
                StringBuilder sb = new StringBuilder();

                // Create a List<FileModel> object containing the selected files, and add their filenames to the StringBuilder
                List<FileModel> files = dg.SelectedItems.Cast<FileModel>().ToList();
                files.ForEach(x => sb.AppendLine(x.Filename));

                // Create a TaskDialog to confirm the deletion with the user
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.Icon = TaskDialogStandardIcon.Warning;
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                td.InstructionText = "Delete selected files?";
                td.Text = "Files will be deleted from your computer";
                td.FooterText = sb.ToString();

                // If the user confirms the deletion, delete the files and remove them from the filesToCombine list
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

                    // Refresh the datagrid with the updated filesToCombine list
                    dg.ItemsSource = null;
                    dg.ItemsSource = filesToCombine;
                }
            }
            // If no items are selected in the datagrid, show an error message
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
            // Check if any files are selected in the UI's datagrid
            if (dg.SelectedItems.Count > 0)
            {
                // Create a StringBuilder to hold the filenames of the selected files
                StringBuilder sb = new StringBuilder();

                // Convert the selected items in the datagrid to a list of FileModel objects
                List<FileModel> filesToExtract = dg.SelectedItems.Cast<FileModel>().ToList();

                // Filter the list of FileModel objects to only include those with more than one page
                filesToExtract = filesToExtract.Where(x => x.PageCount > 1).ToList();

                // Add the filenames of the filtered list of files to the StringBuilder
                filesToExtract.ForEach(x => sb.AppendLine(x.Filename));

                // Calculate the total number of pages to be extracted from the selected files
                int totalPageCount = filesToExtract.Sum(x => x.PageCount);

                // If there are files to extract pages from, prompt the user with a TaskDialog
                if (filesToExtract.Count > 0)
                {
                    TaskDialog td = new TaskDialog();
                    td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                    td.Caption = "Combine PDF";
                    td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                    td.InstructionText = "Extract pages?";
                    td.Text = $"({totalPageCount}) pages will be extracted from the following files";
                    td.FooterText = sb.ToString();

                    // If the user clicks "Yes" on the TaskDialog, open a CommonOpenFileDialog
                    if (td.Show() == TaskDialogResult.Yes)
                    {
                        CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                        dialog.InitialDirectory = Properties.Settings.Default.DefaultDirectory;
                        dialog.IsFolderPicker = true;

                        // If the user selects a folder in the CommonOpenFileDialog, extract pages from each selected file
                        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            foreach (FileModel file in filesToExtract)
                            {
                                // Open the input PDF file
                                PdfDocument inputDocument = PdfReader.Open(file.Filename, PdfDocumentOpenMode.Import);

                                // Get the filename and directory path for the output files
                                string name = Path.GetFileNameWithoutExtension(file.Filename);
                                string dir = dialog.FileName;

                                // Iterate through each page in the input PDF file and save it as a separate PDF file
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
                    // If there are no files to extract pages from, show a warning message with a TaskDialog
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
                // If no files are selected in the datagrid
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = "No file selected";
                td.Text = "Select a file to extract";
                td.Show();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            List<FileModel> missingFiles = new List<FileModel>();

            // Iterate over each file in the list of files to combine
            foreach (FileModel file in filesToCombine)
            {
                try
                {
                    // Try to open the PDF document for this file and get its page count and file size
                    PdfDocument doc = PdfReader.Open(file.Filename, PdfDocumentOpenMode.Import);
                    file.PageCount = doc.PageCount;
                    file.Filesize = $"{Math.Ceiling(doc.FileSize / 1000.0)} KB";
                }
                catch (FileNotFoundException)
                {
                    // If the file is not found, add it to the list of missing files
                    missingFiles.Add(file);
                }
                catch (Exception ex)
                {
                    // If any other exception is caught, show an error message
                    Error.Show(ex);
                }
            }

            if (missingFiles.Count > 0)
            {
                // If there are any missing files, create a message to display them to the user
                missingFiles.ForEach(x => sb.AppendLine(x.Filename));
                missingFiles.ForEach(x => filesToCombine.Remove(x));

                // Show a dialog informing the user that some files were not found and will be removed
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = $"({missingFiles.Count}) files not found";
                td.Text = "Files will be removed from the list";
                td.FooterText = sb.ToString();
                td.Show();
            }

            // Refresh the list of files in the data grid
            dg.ItemsSource = null;
            dg.ItemsSource = filesToCombine;
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            // Check if there are at least two files to combine
            if (filesToCombine.Count > 1)
            {
                // Create a new PDF document for the combined output
                PdfDocument outputDocument = new PdfDocument();

                // Specify the path for the temporary file
                string temp = Path.Combine(Path.GetTempPath(), "temp.pdf");

                // Iterate through each file and add its pages to the output document
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

                // Save the output document to a temporary file
                outputDocument.Save(temp);

                // Create a new preview window
                frmPreview preview = new frmPreview();
                preview.Owner = Application.Current.MainWindow;

                // Set the source of the PDF viewer to the temporary file
                UriBuilder uriBuilder = new UriBuilder(temp);
                preview.pdfViewer.Source = uriBuilder.Uri;

                // Show the preview window
                preview.Show();
            }
            else
            {
                // Show an error message if there are not enough files to preview
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.Icon = TaskDialogStandardIcon.Warning;
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = "At least two files must be provided before previewing";
                td.Text = "Click Add File to add more files";
                td.Show();
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            // Check if only one item is selected in the data grid view
            if (dg.SelectedItems.Count == 1)
            {
                var index = 0;

                // Get the index of the selected file
                foreach (FileModel file in dg.SelectedItems)
                {
                    index = dg.Items.IndexOf(file);

                    try
                    {
                        // Remove the file from its current position and insert it to the next position
                        filesToCombine.RemoveAt(index);
                        filesToCombine.Insert(index + 1, file);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // If the file cannot be inserted to the next position, insert it to the current position
                        filesToCombine.Insert(index, file);
                    }
                }

                // Refresh the data grid view and select the moved file
                dg.ItemsSource = null;
                dg.ItemsSource = filesToCombine;
                dg.SelectedIndex = index + 1;
            }
            else
            {
                // Show a warning message if multiple items are selected in the data grid view
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = "Select a single file";
                td.Text = "Files can only be moved one at a time";
                td.Show();
            }
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            // Check if exactly one item is selected in the data grid
            if (dg.SelectedItems.Count == 1)
            {
                // Declare and initialize variables
                var index = 0;

                // Loop through each selected item (should be only one)
                foreach (FileModel file in dg.SelectedItems)
                {
                    // Get the index of the selected item
                    index = dg.Items.IndexOf(file);

                    try
                    {
                        // Remove the selected item from its current position
                        // and insert it one position higher
                        filesToCombine.RemoveAt(index);
                        filesToCombine.Insert(index - 1, file);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // If the selected item is already at the top of the list,
                        // insert it back at the original position
                        filesToCombine.Insert(index, file);
                    }
                }

                // Refresh the data grid with the updated list and select the item
                // that was just moved up
                dg.ItemsSource = null;
                dg.ItemsSource = filesToCombine;
                dg.SelectedIndex = index - 1;
            }
            else
            {
                // If zero or multiple items are selected, show an error message
                TaskDialog td = new TaskDialog();
                td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                td.Caption = "Combine PDF";
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.InstructionText = "Select a single file";
                td.Text = "Files can only be moved one at a time";
                td.Show();
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
                    List<FileModel> missingFiles = new List<FileModel>();
                    PdfDocument outputDocument = new PdfDocument();
                    StringBuilder sb2 = new StringBuilder();

                    foreach (FileModel file in filesToCombine)
                    {
                        try
                        {
                            PdfDocument inputDocument = PdfReader.Open(file.Filename, PdfDocumentOpenMode.Import);

                            int count = inputDocument.PageCount;
                            for (int i = 0; i < count; i++)
                            {
                                PdfPage page = inputDocument.Pages[i];
                                outputDocument.AddPage(page);
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            missingFiles.Add(file);
                        }
                        catch (Exception ex)
                        {
                            Error.Show(ex);
                        }
                    }

                    if (missingFiles.Count > 0)
                    {
                        missingFiles.ForEach(x => sb2.AppendLine(x.Filename));
                        missingFiles.ForEach(x => filesToCombine.Remove(x));

                        TaskDialog tdMissing = new TaskDialog();
                        tdMissing.StartupLocation = TaskDialogStartupLocation.CenterOwner;
                        tdMissing.Caption = "Combine PDF";
                        tdMissing.StandardButtons = TaskDialogStandardButtons.Ok;
                        tdMissing.InstructionText = $"({missingFiles.Count}) files not found";
                        tdMissing.Text = "Files will be removed from the list";
                        tdMissing.FooterText = sb2.ToString();
                        tdMissing.Show();

                        dg.ItemsSource = null;
                        dg.ItemsSource = filesToCombine; 
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
