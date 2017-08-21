using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class SelectFolderDialogHandler : WidgetHandler<IntPtr, SelectFolderDialog>, SelectFolderDialog.IHandler
    {
        public SelectFolderDialogHandler()
        {
            Control = GtkWrapper.gtk_file_chooser_dialog_new(string.Empty, IntPtr.Zero, GtkWrapper.GTK_FILE_CHOOSER_ACTION_SELECT_FOLDER);
            Directory = System.IO.Directory.GetCurrentDirectory();

            GtkWrapper.gtk_dialog_add_button(Control, "Select Folder", GtkWrapper.GTK_RESPONSE_OK);
            GtkWrapper.gtk_dialog_add_button(Control, "Cancel", GtkWrapper.GTK_RESPONSE_CANCEL);
            GtkWrapper.gtk_dialog_set_default_response(Control, GtkWrapper.GTK_RESPONSE_OK);
        }

        public string Title
        {
            get => GtkWrapper.gtk_window_get_title(Control);
            set => GtkWrapper.gtk_window_set_title(Control, value);
        }

        public string Directory
        {
            get => GtkWrapper.gtk_file_chooser_get_current_folder(Control);
            set => GtkWrapper.gtk_file_chooser_set_current_folder(Control, value);
        }

        public DialogResult ShowDialog(Window parent)
        {
            if (parent != null)
            {
                GtkWrapper.gtk_window_set_transient_for(Control, parent.NativeHandle);
                GtkWrapper.gtk_window_set_modal(Control, true);
            }

            GtkWrapper.gtk_widget_show_all(Control);
            var response = GtkWrapper.gtk_dialog_run(Control);
            GtkWrapper.gtk_widget_hide(Control);

            if (response == GtkWrapper.GTK_RESPONSE_OK)
                return DialogResult.Ok;

            return DialogResult.Cancel;
        }
    }
}
