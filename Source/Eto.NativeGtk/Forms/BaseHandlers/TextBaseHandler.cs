using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
    public class TextBaseHandler<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, TextBox.IHandler
        where TControl : Gtk.Widget
        where TWidget : TextBox
        where TCallback : TextBox.ICallback
    {
        private IntPtr _context, _provider;
        private Color _textcolor;
        private string _prevtext;
        private bool _overridetext, _skipevent;

        protected override void Initialize()
        {
            base.Initialize();

            GtkWrapper.gtk_widget_set_size_request(Handle, 100, -1);

            _context = GtkWrapper.gtk_widget_get_style_context(Handle);
            _provider = GtkWrapper.gtk_css_provider_new();
            GtkWrapper.gtk_style_context_add_provider(_context, _provider, 1000);

            GtkWrapper.gtk_style_context_get_color(_context, 0, out GdkWrapper.RGBA rgba);
            _textcolor = rgba.ToColor();

            ConnectSignal("focus-in-event", (Action<IntPtr, IntPtr, IntPtr>)HandleFocuesInEvent);
            ConnectSignal("changed", (Action<IntPtr, IntPtr>)HandleChanged);
        }

        private static void HandleFocuesInEvent(IntPtr widget, IntPtr evnt, IntPtr user_data)
        {
            var handler = ((GCHandle)user_data).Target as TextBaseHandler<TControl, TWidget, TCallback>;
            if (handler.AutoSelectMode == AutoSelectMode.OnFocus)
                handler.SelectAll();
        }

        private static void HandleChanged(IntPtr editable, IntPtr user_data)
        {
            var handler = ((GCHandle)user_data).Target as TextBaseHandler<TControl, TWidget, TCallback>;
            if (handler._skipevent)
                return;

            handler._overridetext = true;
            var args = new TextChangingEventArgs(handler._prevtext, GtkWrapper.gtk_entry_get_text(handler.Handle));
            handler.Callback.OnTextChanging(handler.Widget, args);
            handler._overridetext = false;

            if (args.Cancel)
            {
                handler._skipevent = true;
                handler.Text = handler._prevtext;
                handler._skipevent = false;
            }

            handler._prevtext = handler.Text;
            handler.Callback.OnTextChanged(handler.Widget, EventArgs.Empty);
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
                GtkWrapper.gtk_entry_set_width_chars(Handle, (value.Width == -1) ? -1 : 0);
                base.Size = value;
            }
        }

        public override string Text
        {
            get => (_overridetext) ? _prevtext : GtkWrapper.gtk_entry_get_text(Handle);
            set => GtkWrapper.gtk_entry_set_text(Handle, value ?? string.Empty);
        }

        public virtual bool ReadOnly
        {
            get => !GtkWrapper.gtk_editable_get_editable(Handle);
            set => GtkWrapper.gtk_editable_set_editable(Handle, !value);
        }

        public int MaxLength
        {
            get => GtkWrapper.gtk_entry_get_max_length(Handle);
            set => GtkWrapper.gtk_entry_set_max_length(Handle, value);
        }

        public string PlaceholderText
        {
            get => GtkWrapper.gtk_entry_get_placeholder_text(Handle);
            set => GtkWrapper.gtk_entry_set_placeholder_text(Handle, value);
        }

        public void SelectAll()
        {
            GtkWrapper.gtk_editable_select_region(Handle, 0, -1);
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
            get => GtkWrapper.gtk_editable_get_position(Handle);
            set => GtkWrapper.gtk_editable_set_position(Handle, value);
        }

        public Range<int> Selection
        {
            get
            {
                GtkWrapper.gtk_editable_get_selection_bounds(Handle, out int start, out int end);
                return new Range<int>(Math.Min(start, end), Math.Max(start, end));
            }
            set => GtkWrapper.gtk_editable_select_region(Handle, value.Start, value.End);
        }

        public TextAlignment TextAlignment
        {
            get
            {
                var xalign = GtkWrapper.gtk_entry_get_alignment(Handle);

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
                        GtkWrapper.gtk_entry_set_alignment(Handle, 0f);
                        break;
                    case TextAlignment.Center:
                        GtkWrapper.gtk_entry_set_alignment(Handle, 0.5f);
                        break;
                    case TextAlignment.Right:
                        GtkWrapper.gtk_entry_set_alignment(Handle, 1f);
                        break;
                }
            }
        }

        public override bool ShowBorder
        {
            get => GtkWrapper.gtk_entry_get_has_frame(Handle);
            set => GtkWrapper.gtk_entry_set_has_frame(Handle, value);
        }

        public AutoSelectMode AutoSelectMode { get; set; }
    }
}
