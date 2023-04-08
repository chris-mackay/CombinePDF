using System;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

public static class Error
{
    public static void Show(Exception exception)
    {
        TaskDialog td = new TaskDialog();

        TaskDialogButton tdbCopyError = new TaskDialogButton("tdbCopyError", "Copy error message");
        tdbCopyError.Click += new EventHandler(tdbCopyError_Click);

        TaskDialogButton tdbClose = new TaskDialogButton("tdbClose", "Close");
        tdbClose.Click += new EventHandler(tdbClose_Click);

        td.Controls.Add(tdbCopyError);
        td.Controls.Add(tdbClose);
        td.Caption = "Error";
        td.InstructionText = "An unexpected error has occurred in the application";
        td.Text = "Click 'Copy error message' and send to your administrator";
        td.DetailsExpandedText = exception.ToString();
        td.DetailsCollapsedLabel = "Show detailed error message";
        td.Icon = TaskDialogStandardIcon.Error;

        td.Show();
    }

    private static void tdbClose_Click(object sender, EventArgs e)
    {
        TaskDialogButton tdb = sender as TaskDialogButton;
        ((TaskDialog)tdb.HostingDialog).Close();
    }

    private static void tdbCopyError_Click(object sender, EventArgs e)
    {
        TaskDialogButton tdb = sender as TaskDialogButton;
        ((TaskDialog)tdb.HostingDialog).Close(TaskDialogResult.Ok);

        Clipboard.Clear();
        Clipboard.SetText(((TaskDialog)tdb.HostingDialog).DetailsExpandedText);

        TaskDialog td = new TaskDialog();
        td.Caption = "Combine PDF";
        td.Icon = TaskDialogStandardIcon.Information;
        td.StandardButtons = TaskDialogStandardButtons.Close;
        td.InstructionText = "Error message copied";
        td.Text = "Please send to your administrator";

        td.Show();
    }
}
