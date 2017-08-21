using System;
using System.Collections.Generic;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class OpenFileDialogHandler : GtkFileDialog<Gtk.FileChooserDialog, OpenFileDialog>, OpenFileDialog.IHandler
    {
        public OpenFileDialogHandler()
        {
            Control = new Gtk.FileChooserDialog(GtkWrapper.gtk_file_chooser_dialog_new(string.Empty, IntPtr.Zero, GtkWrapper.GTK_FILE_CHOOSER_ACTION_OPEN));
            GtkWrapper.gtk_file_chooser_set_current_folder(Control.Handle, System.IO.Directory.GetCurrentDirectory());

            GtkWrapper.gtk_dialog_add_button(Control.Handle, "Open", GtkWrapper.GTK_RESPONSE_OK);
            GtkWrapper.gtk_dialog_add_button(Control.Handle, "Cancel", GtkWrapper.GTK_RESPONSE_CANCEL);
            GtkWrapper.gtk_dialog_set_default_response(Control.Handle, GtkWrapper.GTK_RESPONSE_OK);
        }

        public bool MultiSelect
        {
            get => GtkWrapper.gtk_file_chooser_get_select_multiple(Control.Handle);
            set => GtkWrapper.gtk_file_chooser_set_select_multiple(Control.Handle, value);
        }

        public IEnumerable<string> Filenames
        {
            get
            {
                var ret = new List<string>();
                var glist = GtkWrapper.gtk_file_chooser_get_filenames(Control.Handle);

                do
                {
                    ret.Add(glist.GetData());
                }
                while (glist.MoveNext(out glist));

                return ret;
            }
        }

    }
}
