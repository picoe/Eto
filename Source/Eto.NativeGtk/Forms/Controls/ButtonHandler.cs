using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
    public class ButtonHandler : GtkControl<Gtk.Widget, Button, Button.ICallback>, Button.IHandler
    {
        private readonly IntPtr _context, _provider;
        private Size _minimumSize;
        private Image _image;
        private Color _textcolor;

        public ButtonHandler()
        {
            Handle = GtkWrapper.gtk_button_new();
            GtkWrapper.gtk_button_set_always_show_image(Handle, true);
            GtkWrapper.gtk_widget_set_size_request(Handle, 80, -1);

            _minimumSize = new Size(80, 0);

            _context = GtkWrapper.gtk_widget_get_style_context(Handle);
            _provider = GtkWrapper.gtk_css_provider_new();
            GtkWrapper.gtk_style_context_add_provider(_context, _provider, 1000);

            GtkWrapper.gtk_style_context_get_color(_context, 0, out GdkWrapper.RGBA rgba);
            _textcolor = rgba.ToColor();

            ConnectSignal("clicked", (Action<IntPtr, IntPtr>)HandleClicked);
            ConnectSignal("size-allocate", (Action<IntPtr, IntPtr, IntPtr>)HandleSizeAllocate);
        }

        private static void HandleClicked(IntPtr button, IntPtr user_data)
        {
            var handler = ((GCHandle)user_data).Target as ButtonHandler;
            handler.Callback.OnClick(handler.Widget, EventArgs.Empty);
        }

        private static void HandleSizeAllocate(IntPtr widget, IntPtr allocation, IntPtr user_data)
        {
            var handler = ((GCHandle)user_data).Target as ButtonHandler;
            var alloc = (GdkWrapper.Rectangle)Marshal.PtrToStructure(allocation, typeof(GdkWrapper.Rectangle));
            var width = Math.Max(alloc.Width, handler._minimumSize.Width);
            var height = Math.Max(alloc.Height, handler._minimumSize.Height);

            if (alloc.Width != width || alloc.Height != height)
                GtkWrapper.gtk_widget_set_size_request(handler.Handle, width, height);
        }

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                GtkWrapper.gtk_button_set_image(Handle, _image.ToGtk().Handle);
            }
        }

        public ButtonImagePosition ImagePosition
        {
            get
            {
                switch (GtkWrapper.gtk_button_get_image_position(Handle))
                {
                    case GtkWrapper.GTK_POS_LEFT:
                        return ButtonImagePosition.Left;
                    case GtkWrapper.GTK_POS_RIGHT:
                        return ButtonImagePosition.Right;
                    case GtkWrapper.GTK_POS_TOP:
                        return ButtonImagePosition.Above;
                    case GtkWrapper.GTK_POS_BOTTOM:
                        return ButtonImagePosition.Below;
                }

                return ButtonImagePosition.Overlay;
            }
            set
            {
                switch (value)
                {
                    case ButtonImagePosition.Left:
                        GtkWrapper.gtk_button_set_image_position(Handle, GtkWrapper.GTK_POS_LEFT);
                        break;
                    case ButtonImagePosition.Right:
                        GtkWrapper.gtk_button_set_image_position(Handle, GtkWrapper.GTK_POS_RIGHT);
                        break;
                    case ButtonImagePosition.Below:
                        GtkWrapper.gtk_button_set_image_position(Handle, GtkWrapper.GTK_POS_BOTTOM);
                        break;
                    default:
                        GtkWrapper.gtk_button_set_image_position(Handle, GtkWrapper.GTK_POS_TOP);
                        break;
                }
            }
        }

        public Size MinimumSize
        {
            get => _minimumSize;
            set
            {
                if (_minimumSize != value)
                {
                    _minimumSize = value;
                    GtkWrapper.gtk_widget_queue_resize(Handle);
                }
            }
        }

        public override string Text
        {
            get => GtkWrapper.gtk_button_get_label(Handle).ToEtoMnemonic();
            set => GtkWrapper.gtk_button_set_label(Handle, value.ToPlatformMnemonic());
        }

        public Color TextColor
        {
            get => _textcolor;
            set
            {
                _textcolor = value;

                GtkWrapper.gtk_style_context_remove_class(_context, "etocoloring");
                GtkWrapper.gtk_css_provider_load_from_data(_provider, ".etocoloring { color: #" + value.ToString().Substring(3) + "; }", -1, IntPtr.Zero);
                GtkWrapper.gtk_style_context_add_class(_context, "etocoloring");
            }
        }
    }
}