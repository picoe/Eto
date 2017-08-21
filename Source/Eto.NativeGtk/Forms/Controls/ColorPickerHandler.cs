using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
    public class ColorPickerHandler : GtkControl<Gtk.Widget, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
    {
        public ColorPickerHandler()
        {
            Handle = GtkWrapper.gtk_color_button_new();
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
                GtkWrapper.gtk_color_chooser_get_rgba(Control.Handle, out GdkWrapper.RGBA rgba);
                return rgba.ToColor();
            }
            set => GtkWrapper.gtk_color_chooser_set_rgba(Control.Handle, value.ToNativeRGBA());
        }

        public bool SupportsAllowAlpha => true;

        public override void AttachEvent(string id)
        {
            switch (id)
            {
                case ColorPicker.ColorChangedEvent:
                    Control.AddSignalHandler("color-set", (Action<object, EventArgs>)HandleColorSet);
                    break;
                default:
                    base.AttachEvent(id);
                    break;
            }
        }

        private void HandleColorSet(object sender, EventArgs e)
        {
            Callback.OnColorChanged(Widget, EventArgs.Empty);
        }
    }
}