using System;
using System.Diagnostics;
using System.IO;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class OpenWithDialogHandler : WidgetHandler<IntPtr, OpenWithDialog, OpenWithDialog.ICallback>, OpenWithDialog.IHandler
    {
        public string FilePath { get; set; }

        public DialogResult ShowDialog(Window parent)
        {
            var parenthandle = parent == null ? IntPtr.Zero : parent.NativeHandle;
            var file = GioWrapper.g_file_new_for_path(FilePath);
            Control = GtkWrapper.gtk_app_chooser_dialog_new(parenthandle, GtkWrapper.GTK_DIALOG_MODAL | GtkWrapper.GTK_DIALOG_USE_HEADER_BAR, file);
            GtkWrapper.gtk_app_chooser_dialog_set_heading(Control, "Select an app to open \"" + Path.GetFileName(FilePath) + "\" with:");

            GtkWrapper.gtk_widget_show_all(Control);
            var response = GtkWrapper.gtk_dialog_run(Control);
            GtkWrapper.gtk_widget_hide(Control);

            if (response == GtkWrapper.GTK_RESPONSE_OK)
            {
                var appinfo = GtkWrapper.gtk_app_chooser_get_app_info(Control);
                Process.Start(GioWrapper.g_app_info_get_executable(appinfo), "\"" + FilePath + "\"");
                return DialogResult.Ok;
            }

            return DialogResult.Ok;
        }
    }
}
