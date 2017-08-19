using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
    public class TextBoxHandler : GtkControl<Gtk.Widget, TextBox, TextBox.ICallback>, TextBox.IHandler
    {
        private readonly IntPtr _context, _provider;
        private Color _textcolor;
        private string _prevtext;
        private bool _overridetext, _skipevent;

        public TextBoxHandler()
        {
            Control = new Gtk.Widget(GtkWrapper.gtk_entry_new());
            GtkWrapper.gtk_widget_set_size_request(Control.Handle, 100, -1);

            _context = GtkWrapper.gtk_widget_get_style_context(Control.Handle);
            _provider = GtkWrapper.gtk_css_provider_new();
            GtkWrapper.gtk_style_context_add_provider(_context, _provider, 1000);

            GtkWrapper.gtk_style_context_get_color(_context, 0, out GtkWrapper.RGBA rgba);
            _textcolor = rgba.ToColor();

            Control.AddSignalHandler("changed", (Action<object, EventArgs>)HandleChanged);
        }

        private void HandleChanged(object sender, EventArgs e)
        {
            if (_skipevent)
                return;

            _overridetext = true;
            var args = new TextChangingEventArgs(Text);
            Callback.OnTextChanging(Widget, args);
            _overridetext = false;

            if (args.Cancel)
            {
                _skipevent = true;
                Text = _prevtext;
                _skipevent = false;
            }

            _prevtext = Text;
            Callback.OnTextChanged(Widget, EventArgs.Empty);
        }

        public override void AttachEvent(string id)
        {
            switch (id)
            {
                case TextControl.TextChangedEvent:
                case TextBox.TextChangingEvent:
                    break;
                default:
                    base.AttachEvent(id);
                    break;
            }
        }

        public override Size Size
        {
            get => base.Size;
            set
            {
                GtkWrapper.gtk_entry_set_width_chars(Control.Handle, (value.Width == -1) ? -1 : 0);
                base.Size = value;
            }
        }

        public override string Text
        {
            get => (_overridetext) ? _prevtext : GtkWrapper.gtk_entry_get_text(Control.Handle);
            set => GtkWrapper.gtk_entry_set_text(Control.Handle, value ?? string.Empty);
        }

        public virtual bool ReadOnly
        {
            get => !GtkWrapper.gtk_editable_get_editable(Control.Handle);
            set => GtkWrapper.gtk_editable_set_editable(Control.Handle, !value);
        }

        public int MaxLength
        {
            get => GtkWrapper.gtk_entry_get_max_length(Control.Handle);
            set => GtkWrapper.gtk_entry_set_max_length(Control.Handle, value);
        }

        public string PlaceholderText
        {
            get => GtkWrapper.gtk_entry_get_placeholder_text(Control.Handle);
            set => GtkWrapper.gtk_entry_set_placeholder_text(Control.Handle, value);
        }

        public void SelectAll()
        {
            GtkWrapper.gtk_editable_select_region(Control.Handle, 0, -1);
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

        public int CaretIndex
        {
            get => GtkWrapper.gtk_editable_get_position(Control.Handle);
            set => GtkWrapper.gtk_editable_set_position(Control.Handle, value);
        }

        public Range<int> Selection
        {
            get
            {
                GtkWrapper.gtk_editable_get_selection_bounds(Control.Handle, out int start, out int end);
                return new Range<int>(Math.Min(start, end), Math.Max(start, end));
            }
            set => GtkWrapper.gtk_editable_select_region(Control.Handle, value.Start, value.End);
        }

        public TextAlignment TextAlignment
        {
            get
            {
                var xalign = GtkWrapper.gtk_entry_get_alignment(Control.Handle);

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
                        GtkWrapper.gtk_entry_set_alignment(Control.Handle, 0f);
                        break;
                    case TextAlignment.Center:
                        GtkWrapper.gtk_entry_set_alignment(Control.Handle, 0.5f);
                        break;
                    case TextAlignment.Right:
                        GtkWrapper.gtk_entry_set_alignment(Control.Handle, 1f);
                        break;
                }
            }
        }

        public override bool ShowBorder
        {
            get => GtkWrapper.gtk_entry_get_has_frame(Control.Handle);
            set => GtkWrapper.gtk_entry_set_has_frame(Control.Handle, value);
        }

        public AutoSelectMode AutoSelectMode { get; set; }
    }
}
