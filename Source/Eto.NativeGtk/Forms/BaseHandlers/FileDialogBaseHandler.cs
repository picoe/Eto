using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public abstract class FileDialogBaseHandler<TWidget> : WidgetHandler<IntPtr, TWidget>, FileDialog.IHandler
        where TWidget : FileDialog
    {
        public string FileName
        {
            get => GtkWrapper.gtk_file_chooser_get_filename(Control);
            set => GtkWrapper.gtk_file_chooser_set_filename(Control, value);
        }

        public Uri Directory
        {
            get => new Uri(GtkWrapper.gtk_file_chooser_get_current_folder(Control));
            set => GtkWrapper.gtk_file_chooser_set_current_folder(Control, value.AbsolutePath);
        }

        public int CurrentFilterIndex { get; set; }

        public bool CheckFileExists
        {
            get { return false; }
            set { }
        }

        public string Title
        {
            get => GtkWrapper.gtk_window_get_title(Control);
            set => GtkWrapper.gtk_window_set_title(Control, value);
        }

        public void InsertFilter(int index, FileFilter filter)
        {
            
        }

        public void RemoveFilter(int index)
        {
            
        }

        public void ClearFilters()
        {

        }

        public DialogResult ShowDialog(Window parent)
        {
            var glist = GtkWrapper.gtk_file_chooser_list_filters(Control);

            do
            {
                if (glist.Data != IntPtr.Zero)
                    GtkWrapper.gtk_file_chooser_remove_filter(Control, glist.Data);
            }
            while (glist.MoveNext(out glist));

            for (int i = 0; i < Widget.Filters.Count; i++)
            {
                var val = Widget.Filters[i];
                var filter = GtkWrapper.gtk_file_filter_new();
                GtkWrapper.gtk_file_filter_set_name(filter, val.Name);
                foreach (var pattern in val.Extensions)
                    GtkWrapper.gtk_file_filter_add_pattern(filter, "*" + pattern);
                GtkWrapper.gtk_file_chooser_add_filter(Control, filter);

                if (CurrentFilterIndex == i)
                    GtkWrapper.gtk_file_chooser_set_filter(Control, filter);
            }

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
