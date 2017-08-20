using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class ColorDialogHandler : WidgetHandler<Gtk.Widget, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
    {
        public ColorDialogHandler()
        {
            Control = new Gtk.Widget(GtkWrapper.gtk_color_chooser_dialog_new("Choose Color", IntPtr.Zero));
            AllowAlpha = false;
        }

        public bool AllowAlpha
        {
            get => GtkWrapper.gtk_color_chooser_get_use_alpha(Control.Handle);
            set => GtkWrapper.gtk_color_chooser_set_use_alpha(Control.Handle, value);
        }

        public Color Color
        {
            get
            {
                GtkWrapper.gtk_color_chooser_get_rgba(Control.Handle, out GtkWrapper.RGBA rgba);
                return rgba.ToColor();
            }
            set => GtkWrapper.gtk_color_chooser_set_rgba(Control.Handle, value.ToNativeRGBA());
        }

        public bool SupportsAllowAlpha => true;

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
            {
                Callback.OnColorChanged(Widget, EventArgs.Empty);
                return DialogResult.Ok;
            }

            return DialogResult.Cancel;
        }
    }
}
