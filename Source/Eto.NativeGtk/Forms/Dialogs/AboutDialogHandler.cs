using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class AboutDialogHandler : WidgetHandler<IntPtr, AboutDialog, AboutDialog.ICallback>, AboutDialog.IHandler
    {
        private Image _image;

        public AboutDialogHandler()
        {
            Control = GtkWrapper.gtk_about_dialog_new();
            GtkWrapper.gtk_about_dialog_set_wrap_license(Control, true);
        }

        public string Copyright
        {
            get => GtkWrapper.gtk_about_dialog_get_copyright(Control);
            set => GtkWrapper.gtk_about_dialog_set_copyright(Control, value);
        }

        public string[] Designers
        {
            get => GtkWrapper.gtk_about_dialog_get_artists(Control);
            set => GtkWrapper.gtk_about_dialog_set_artists(Control, value);
        }

        public string[] Developers
        {
            get => GtkWrapper.gtk_about_dialog_get_authors(Control);
            set => GtkWrapper.gtk_about_dialog_set_authors(Control, value);
        }

        public string[] Documenters
        {
            get => GtkWrapper.gtk_about_dialog_get_documenters(Control);
            set => GtkWrapper.gtk_about_dialog_set_documenters(Control, value);
        }

        public string License
        {
            get => GtkWrapper.gtk_about_dialog_get_license(Control);
            set => GtkWrapper.gtk_about_dialog_set_license(Control, value);
        }

        public Image Logo
        {
            get => _image;
            set
            {
                _image = value;
                GtkWrapper.gtk_about_dialog_set_logo(Control, _image?.ToGdk().Handle ?? IntPtr.Zero);
            }
        }

        public string ProgramDescription
        {
            get => GtkWrapper.gtk_about_dialog_get_comments(Control);
            set => GtkWrapper.gtk_about_dialog_set_comments(Control, value);
        }

        public string ProgramName
        {
            get => GtkWrapper.gtk_about_dialog_get_program_name(Control);
            set => GtkWrapper.gtk_about_dialog_set_program_name(Control, value);
        }

        public string Title
        {
            get => GtkWrapper.gtk_window_get_title(Control);
            set => GtkWrapper.gtk_window_set_title(Control, value);
        }

        public string Version
        {
            get => GtkWrapper.gtk_about_dialog_get_version(Control);
            set => GtkWrapper.gtk_about_dialog_set_version(Control, value);
        }

        public Uri Website
        {
            get => new Uri(GtkWrapper.gtk_about_dialog_get_website(Control));
            set => GtkWrapper.gtk_about_dialog_set_website(Control, value.ToString());
        }

        public string WebsiteLabel
        {
            get => GtkWrapper.gtk_about_dialog_get_website_label(Control);
            set => GtkWrapper.gtk_about_dialog_set_website_label(Control, value);
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
