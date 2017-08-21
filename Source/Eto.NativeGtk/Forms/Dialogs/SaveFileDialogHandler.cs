using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class SaveFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, SaveFileDialog>, SaveFileDialog.IHandler
    {
        public SaveFileDialogHandler()
        {
            Control = new Gtk.FileChooserDialog(GtkWrapper.gtk_file_chooser_dialog_new(string.Empty, IntPtr.Zero, GtkWrapper.GTK_FILE_CHOOSER_ACTION_SAVE));
            GtkWrapper.gtk_file_chooser_set_current_folder(Control.Handle, System.IO.Directory.GetCurrentDirectory());
            GtkWrapper.gtk_file_chooser_set_do_overwrite_confirmation(Control.Handle, true);

            GtkWrapper.gtk_dialog_add_button(Control.Handle, "Save", GtkWrapper.GTK_RESPONSE_OK);
            GtkWrapper.gtk_dialog_add_button(Control.Handle, "Cancel", GtkWrapper.GTK_RESPONSE_CANCEL);
            GtkWrapper.gtk_dialog_set_default_response(Control.Handle, GtkWrapper.GTK_RESPONSE_OK);
        }
    }
}
