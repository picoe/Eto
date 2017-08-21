using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class SaveFileDialogHandler : FileDialogBaseHandler<SaveFileDialog>, SaveFileDialog.IHandler
    {
        public SaveFileDialogHandler()
        {
            Control = GtkWrapper.gtk_file_chooser_dialog_new(string.Empty, IntPtr.Zero, GtkWrapper.GTK_FILE_CHOOSER_ACTION_SAVE);
            GtkWrapper.gtk_file_chooser_set_current_folder(Control, System.IO.Directory.GetCurrentDirectory());
            GtkWrapper.gtk_file_chooser_set_do_overwrite_confirmation(Control, true);

            GtkWrapper.gtk_dialog_add_button(Control, "Save", GtkWrapper.GTK_RESPONSE_OK);
            GtkWrapper.gtk_dialog_add_button(Control, "Cancel", GtkWrapper.GTK_RESPONSE_CANCEL);
            GtkWrapper.gtk_dialog_set_default_response(Control, GtkWrapper.GTK_RESPONSE_OK);
        }
    }
}
