using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
    public class CheckBoxHandler : GtkControl<Gtk.Widget, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
    {
        private readonly IntPtr _eventbox, _context, _provider;
        private bool? _tempstate;
        private bool _ignoreevents;
        private Color _textcolor;

        public CheckBoxHandler()
        {
            Handle = GtkWrapper.gtk_check_button_new();

            _tempstate = false;

            _eventbox = GtkWrapper.gtk_event_box_new();
            GtkWrapper.gtk_container_add(_eventbox, Handle);

            _context = GtkWrapper.gtk_widget_get_style_context(Handle);
            _provider = GtkWrapper.gtk_css_provider_new();
            GtkWrapper.gtk_style_context_add_provider(_context, _provider, 1000);

            GtkWrapper.gtk_style_context_get_color(_context, 0, out GdkWrapper.RGBA rgba);
            _textcolor = rgba.ToColor();

            ConnectSignal("toggled", (Action<IntPtr, IntPtr>)HandleToggled);
        }

        private static void HandleToggled(IntPtr togglebutton, IntPtr user_data)
        {
            var handler = ((GCHandle)user_data).Target as CheckBoxHandler;
            if (handler._ignoreevents)
                return;

            if (handler.ThreeState)
            {
                handler._ignoreevents = true;

                if (!handler._tempstate.HasValue)
                    handler.Checked = true;
                else if (handler._tempstate.Value)
                    handler.Checked = false;
                else
                    handler.Checked = null;

                handler._ignoreevents = false;
            }

            handler._tempstate = handler.Checked;
            handler.Callback.OnCheckedChanged(handler.Widget, EventArgs.Empty);
        }

        public bool? Checked
        {
            get
            {
                if (GtkWrapper.gtk_toggle_button_get_inconsistent(Handle))
                    return null;

                return GtkWrapper.gtk_toggle_button_get_active(Handle);
            }
            set
            {
                var triggerevent = !_ignoreevents && _tempstate != value;

                _ignoreevents = true;
                if (!value.HasValue)
                {
                    GtkWrapper.gtk_toggle_button_set_inconsistent(Handle, true);
                    GtkWrapper.gtk_toggle_button_set_active(Handle, false);
                }
                else
                {
                    GtkWrapper.gtk_toggle_button_set_inconsistent(Handle, false);
                    GtkWrapper.gtk_toggle_button_set_active(Handle, value.Value);
                }
                _ignoreevents = false;

                if (triggerevent)
                {
                    _tempstate = value;
                    Callback.OnCheckedChanged(Widget, EventArgs.Empty);
                }
            }
        }

        public override Gtk.Widget ContainerControl => new Gtk.Widget(_eventbox);

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

        public bool ThreeState { get; set; }
    }
}