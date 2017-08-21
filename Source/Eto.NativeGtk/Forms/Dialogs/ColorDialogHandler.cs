using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class ColorDialogHandler : WidgetHandler<IntPtr, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
    {
        public ColorDialogHandler()
        {
            Control = GtkWrapper.gtk_color_chooser_dialog_new("Choose Color", IntPtr.Zero);
            AllowAlpha = false;
        }

        public bool AllowAlpha
        {
            get => GtkWrapper.gtk_color_chooser_get_use_alpha(Control);
            set => GtkWrapper.gtk_color_chooser_set_use_alpha(Control, value);
        }

        public Color Color
        {
            get
            {
                GtkWrapper.gtk_color_chooser_get_rgba(Control, out GdkWrapper.RGBA rgba);
                return rgba.ToColor();
            }
            set => GtkWrapper.gtk_color_chooser_set_rgba(Control, value.ToNativeRGBA());
        }

        public bool SupportsAllowAlpha => true;

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
            {
                Callback.OnColorChanged(Widget, EventArgs.Empty);
                return DialogResult.Ok;
            }

            return DialogResult.Cancel;
        }
    }
}
