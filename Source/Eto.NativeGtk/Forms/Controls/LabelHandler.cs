using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
    public class LabelHandler : GtkControl<Gtk.Widget, Label, Label.ICallback>, Label.IHandler
    {
        private readonly IntPtr _eventbox, _context, _provider;
        private Color _textcolor;

        public LabelHandler()
        {
            _eventbox = GtkWrapper.gtk_event_box_new();
            Control = new Gtk.Widget(GtkWrapper.gtk_label_new(string.Empty));
            GtkWrapper.gtk_container_add(_eventbox, Control.Handle);

            _context = GtkWrapper.gtk_widget_get_style_context(Control.Handle);
            _provider = GtkWrapper.gtk_css_provider_new();
            GtkWrapper.gtk_style_context_add_provider(_context, _provider, 1000);

            GtkWrapper.gtk_style_context_get_color(_context, 0, out GtkWrapper.RGBA rgba);
            _textcolor = rgba.ToColor();

            TextAlignment = TextAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public override Gtk.Widget ContainerControl => new Gtk.Widget(_eventbox);

        public override Gtk.Widget EventControl => new Gtk.Widget(_eventbox);

        public WrapMode Wrap
        {
            get
            {
                if (!GtkWrapper.gtk_label_get_line_wrap(Control.Handle))
                    return WrapMode.None;
                if (GtkWrapper.gtk_label_get_line_wrap_mode(Control.Handle) == GtkWrapper.PANGO_WRAP_CHAR)
                    return WrapMode.Character;
                return WrapMode.Word;
            }
            set
            {
                switch (value)
                {
                    case WrapMode.None:
                        GtkWrapper.gtk_label_set_line_wrap(Control.Handle, false);
                        break;
                    case WrapMode.Word:
                        GtkWrapper.gtk_label_set_line_wrap(Control.Handle, true);
                        GtkWrapper.gtk_label_set_line_wrap_mode(Control.Handle, GtkWrapper.PANGO_WRAP_WORD);
                        break;
                    case WrapMode.Character:
                        GtkWrapper.gtk_label_set_line_wrap(Control.Handle, true);
                        GtkWrapper.gtk_label_set_line_wrap_mode(Control.Handle, GtkWrapper.PANGO_WRAP_CHAR);
                        break;
                }
            }
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

        public override string Text
        {
            get => GtkWrapper.gtk_label_get_text(Control.Handle).ToEtoMnemonic();
            set => GtkWrapper.gtk_label_set_text(Control.Handle, value.ToPlatformMnemonic());
        }

        public TextAlignment TextAlignment
        {
            get
            {
                var xalign = GtkWrapper.gtk_label_get_yalign(Control.Handle);

                if (xalign < 0.4f)
                    return TextAlignment.Left;
                if (xalign > 0.6f)
                    return TextAlignment.Right;
                return TextAlignment.Center;
            }
            set
            {
                switch (value)
                {
                    case TextAlignment.Left:
                        GtkWrapper.gtk_label_set_xalign(Control.Handle, 0f);
                        break;
                    case TextAlignment.Center:
                        GtkWrapper.gtk_label_set_xalign(Control.Handle, 0.5f);
                        break;
                    case TextAlignment.Right:
                        GtkWrapper.gtk_label_set_xalign(Control.Handle, 1f);
                        break;
                }
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get
            {
                var yalign = GtkWrapper.gtk_label_get_yalign(Control.Handle);

                if (yalign < 0.4f)
                    return VerticalAlignment.Top;
                if (yalign > 0.6f)
                    return VerticalAlignment.Bottom;
                return VerticalAlignment.Center;
            }
            set
            {
                switch (value)
                {
                    case VerticalAlignment.Stretch:
                    case VerticalAlignment.Top:
                        GtkWrapper.gtk_label_set_yalign(Control.Handle, 0f);
                        break;
                    case VerticalAlignment.Center:
                        GtkWrapper.gtk_label_set_yalign(Control.Handle, 0.5f);
                        break;
                    case VerticalAlignment.Bottom:
                        GtkWrapper.gtk_label_set_yalign(Control.Handle, 1f);
                        break;
                }
            }
        }
    }
}
