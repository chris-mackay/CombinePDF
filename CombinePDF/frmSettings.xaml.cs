using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace CombinePDF
{
    /// <summary>
    /// Interaction logic for frmSettings.xaml
    /// </summary>
    public partial class frmSettings : Window
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            string def = Properties.Settings.Default.DefaultDirectory;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string dir = Properties.Settings.Default.DefaultDirectory;
            txtDirectory.Text = dir;

            bool alwaysOverwrite = Properties.Settings.Default.AlwaysOverwrite;
            ckbAlwaysOverwrite.IsChecked = alwaysOverwrite;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            string dir = txtDirectory.Text;
            bool alwaysOverwrite = ckbAlwaysOverwrite.IsChecked.Value;

            if (alwaysOverwrite)
                Properties.Settings.Default.AlwaysOverwrite = true;
            else
                Properties.Settings.Default.AlwaysOverwrite = false;

            Properties.Settings.Default.DefaultDirectory = dir;
            Properties.Settings.Default.Save();
        }
    }
}
