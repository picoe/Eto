using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class AboutDialogHandler : WidgetHandler<Gtk.Widget, AboutDialog, AboutDialog.ICallback>, AboutDialog.IHandler
    {
        private Image _image;

        public AboutDialogHandler()
        {
            Control = new Gtk.Widget(GtkWrapper.gtk_about_dialog_new());
            GtkWrapper.gtk_about_dialog_set_wrap_license(Control.Handle, true);
        }

        public string Copyright
        {
            get => GtkWrapper.gtk_about_dialog_get_copyright(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_copyright(Control.Handle, value);
        }

        public string[] Designers
        {
            get => GtkWrapper.gtk_about_dialog_get_artists(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_artists(Control.Handle, value);
        }

        public string[] Developers
        {
            get => GtkWrapper.gtk_about_dialog_get_authors(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_authors(Control.Handle, value);
        }

        public string[] Documenters
        {
            get => GtkWrapper.gtk_about_dialog_get_documenters(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_documenters(Control.Handle, value);
        }

        public string License
        {
            get => GtkWrapper.gtk_about_dialog_get_license(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_license(Control.Handle, value);
        }

        public Image Logo
        {
            get => _image;
            set
            {
                _image = value;
                GtkWrapper.gtk_about_dialog_set_logo(Control.Handle, _image?.ToGdk().Handle ?? IntPtr.Zero);
            }
        }

        public string ProgramDescription
        {
            get => GtkWrapper.gtk_about_dialog_get_comments(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_comments(Control.Handle, value);
        }

        public string ProgramName
        {
            get => GtkWrapper.gtk_about_dialog_get_program_name(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_program_name(Control.Handle, value);
        }

        public string Title
        {
            get => GtkWrapper.gtk_window_get_title(Control.Handle);
            set => GtkWrapper.gtk_window_set_title(Control.Handle, value);
        }

        public string Version
        {
            get => GtkWrapper.gtk_about_dialog_get_version(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_version(Control.Handle, value);
        }

        public Uri Website
        {
            get => new Uri(GtkWrapper.gtk_about_dialog_get_website(Control.Handle));
            set => GtkWrapper.gtk_about_dialog_set_website(Control.Handle, value.ToString());
        }

        public string WebsiteLabel
        {
            get => GtkWrapper.gtk_about_dialog_get_website_label(Control.Handle);
            set => GtkWrapper.gtk_about_dialog_set_website_label(Control.Handle, value);
        }

        public DialogResult ShowDialog(Window parent)
        {
            if (parent != null)
            {
                GtkWrapper.gtk_window_set_transient_for(Control.Handle, parent.NativeHandle);
                GtkWrapper.gtk_window_set_modal(Control.Handle, true);
            }

            GtkWrapper.gtk_widget_show_all(Control.Handle);
            var response = GtkWrapper.gtk_dialog_run(Control.Handle);
            GtkWrapper.gtk_widget_hide(Control.Handle);

            if (response == GtkWrapper.GTK_RESPONSE_OK)
                return DialogResult.Ok;

            return DialogResult.Cancel;
        }
    }
}
