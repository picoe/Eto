using System;
using Eto.Forms;
using Eto.Drawing;

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
            Control = new Gtk.Widget(GtkWrapper.gtk_check_button_new());

            _tempstate = false;

            _eventbox = GtkWrapper.gtk_event_box_new();
            GtkWrapper.gtk_container_add(_eventbox, Control.Handle);

            _context = GtkWrapper.gtk_widget_get_style_context(Control.Handle);
            _provider = GtkWrapper.gtk_css_provider_new();
            GtkWrapper.gtk_style_context_add_provider(_context, _provider, 1000);

            GtkWrapper.RGBA rgba;
            GtkWrapper.gtk_style_context_get_color(_context, 0, out rgba);
            _textcolor = rgba.ToColor();

            Control.AddSignalHandler("toggled", (Action<object, EventArgs>)HandleToggled);
        }

        private void HandleToggled(object sender, EventArgs e)
        {
            if (_ignoreevents)
                return;

            if (ThreeState)
            {
                _ignoreevents = true;

                if (!_tempstate.HasValue)
                    Checked = true;
                else if (_tempstate.Value)
                    Checked = false;
                else
                    Checked = null;

                _ignoreevents = false;
            }

            _tempstate = Checked;
            Callback.OnCheckedChanged(Widget, EventArgs.Empty);
        }

        public bool? Checked
        {
            get
            {
                if (GtkWrapper.gtk_toggle_button_get_inconsistent(Control.Handle))
                    return null;

                return GtkWrapper.gtk_toggle_button_get_active(Control.Handle);
            }
            set
            {
                _ignoreevents = true;
                if (!value.HasValue)
                {
                    GtkWrapper.gtk_toggle_button_set_inconsistent(Control.Handle, true);
                    GtkWrapper.gtk_toggle_button_set_active(Control.Handle, false);
                }
                else
                {
                    GtkWrapper.gtk_toggle_button_set_inconsistent(Control.Handle, false);
                    GtkWrapper.gtk_toggle_button_set_active(Control.Handle, value.Value);
                }
                _ignoreevents = false;

                if (_tempstate != value)
                {
                    _tempstate = value;
                    Callback.OnCheckedChanged(Widget, EventArgs.Empty);
                }
            }
        }

        public override Gtk.Widget ContainerControl => new Gtk.Widget(_eventbox);

        public override string Text
        {
            get => GtkWrapper.gtk_button_get_label(Control.Handle).ToEtoMnemonic();
            set => GtkWrapper.gtk_button_set_label(Control.Handle, value.ToPlatformMnemonic());
            }


        public Color TextColor
        {
            get => _textcolor;
            set

            {
                _textcolor = value;

                GtkWrapper.gtk_style_context_remove_class(_context, "etocoloring");
                GtkWrapper.gtk_css_provider_load_from_data(_provider,
                    ".etocoloring { color: #" + value.ToString().Substring(3) + "; }", -1, IntPtr.Zero);
                GtkWrapper.gtk_style_context_add_class(_context, "etocoloring");
            }
            }


        public bool ThreeState { get; set; }
    }
}