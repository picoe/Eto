namespace Eto.GtkSharp.Forms.Controls
{
	public class RadioButtonHandler : GtkControl<Gtk.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		Gtk.EventBox _box;
		Gtk.AccelLabel _label;
		string _text;
		bool _alwaysShowMnemonic;

		protected override Gtk.Widget FontControl => _label;

		public override Gtk.Widget ContainerControl => _box;

		public RadioButtonHandler()
		{
			Control = new Gtk.RadioButton((Gtk.RadioButton)null);
			AddInactiveButton();
			_label = new Gtk.AccelLabel("");
			_label.UseUnderline = true;
			Control.Add(_label);
			_box = new Gtk.EventBox();
			_box.Child = Control;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Realized += Connector.Control_Realized;
			Control.Clicked += Connector.HandleClick;
			Control.Toggled += Connector.HandleCheckedChanged;
		}

		// a hidden checked button so this one can be unchecked like on other platforms
		void AddInactiveButton()
		{
			var inactive = new Gtk.RadioButton(Control);
			inactive.Active = true;
		}

		bool _settingGroup;
		RadioButton _controller;

		public RadioButton Controller
		{
			get => _controller;
			set
			{
				_controller = value;
				var controllerControl = value != null ? GetControl(value) : null;
				if (controllerControl != null && Array.IndexOf(controllerControl.Group, Control) >= 0)
					return;

				var wasChecked = Control.Active;
				// joining a group toggles this button, which isn't a change the user should see
				_settingGroup = true;
				try
				{
					Control.JoinGroup(controllerControl);
					if (controllerControl == null)
						AddInactiveButton();
					Control.Active = wasChecked;
				}
				finally
				{
					_settingGroup = false;
				}
			}
		}

		void UpdateLabel()
		{
			// if Text present show label, otherwise hide it.
			if (_label.Text != null && _label.Text != string.Empty)
			{
				_label.Visible = true;
			}
			else
			{
				_label.Visible = false;
			}
		}

		void Control_Realized(object sender, EventArgs e)
		{
			UpdateLabel();
		}

		protected new RadioButtonConnector Connector { get { return (RadioButtonConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new RadioButtonConnector();
		}

		protected class RadioButtonConnector : GtkControlConnector
		{
			public new RadioButtonHandler Handler { get { return (RadioButtonHandler)base.Handler; } }

			public void HandleCheckedChanged(object sender, EventArgs e)
			{
				if (Handler?._settingGroup == false)
					Handler.Callback.OnCheckedChanged(Handler.Widget, EventArgs.Empty);
			}

			public void HandleClick(object sender, EventArgs e)
			{
				// GTK raises Clicked when other grouped radios are unchecked; only fire for the active (user-clicked) one.
				if (Handler?.Control?.Active == true && !Handler._settingGroup)
					Handler.Callback.OnClick(Handler.Widget, EventArgs.Empty);
			}

			internal void Control_Realized(object sender, EventArgs e) => Handler?.Control_Realized(sender, e);
		}

		public override string Text
		{
			get => _text;
			set
			{
				_text = value;
				if (_label.UseUnderline)
				{
					_label.TextWithMnemonic = _text.ToPlatformMnemonic();
					_label.Pattern = _alwaysShowMnemonic ? GtkMnemonicHelper.ToPatternWithMnemonicUnderline(_text) : null;
				}
				else
				{
					_label.Pattern = null;
					_label.Text = _text;
				}
				UpdateLabel();
			}
		}

		public bool Checked
		{
			get { return Control.Active; }
			set { Control.Active = value; }
		}

		public Color TextColor
		{
			get { return _label.GetForeground(); }
			set
			{
				_label.SetForeground(value, GtkStateFlags.Normal);
				_label.SetForeground(value, GtkStateFlags.Active);
				_label.SetForeground(value, GtkStateFlags.Prelight);
			}
		}

		public bool UseMnemonic
		{
			get => _label.UseUnderline;
			set
			{
				if (value == _label.UseUnderline)
					return; // no change
				var text = Text;
				_label.UseUnderline = value;
				Text = text;
			}
		}

		public bool AlwaysShowMnemonic
		{
			get => _alwaysShowMnemonic;
			set
			{
				if (_alwaysShowMnemonic == value)
					return;
				_alwaysShowMnemonic = value;
				if (_text != null)
					Text = _text;
			}
		}
	}
}
