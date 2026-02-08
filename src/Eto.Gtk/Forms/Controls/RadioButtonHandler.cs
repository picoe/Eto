namespace Eto.GtkSharp.Forms.Controls
{
	public class RadioButtonHandler : GtkControl<Gtk.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		Gtk.EventBox _box;
		Gtk.AccelLabel _label;
		string _text;

		protected override Gtk.Widget FontControl => _label;

		public override Gtk.Widget ContainerControl => _box;

		public void Create(RadioButton controller)
		{
			if (controller != null)
				Control = new Gtk.RadioButton(RadioButtonHandler.GetControl(controller));
			else
			{
				Control = new Gtk.RadioButton((Gtk.RadioButton)null);
				// make gtk work like others in that no radio button is initially selected
				var inactive = new Gtk.RadioButton(Control);
				inactive.Active = true;
			}
			_label = new Gtk.AccelLabel("");
			_label.UseUnderline = true;
			Control.Add(_label);
			Control.Realized += Connector.Control_Realized;
			Control.Clicked += Connector.HandleClick;
			Control.Toggled += Connector.HandleCheckedChanged;
			_box = new Gtk.EventBox();
			_box.Child = Control;
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

			public void HandleCheckedChanged(object sender, EventArgs e) => Handler?.Callback.OnCheckedChanged(Handler.Widget, EventArgs.Empty);

			public void HandleClick(object sender, EventArgs e)
			{
				// GTK raises Clicked when other grouped radios are unchecked; only fire for the active (user-clicked) one.
				if (Handler?.Control?.Active == true)
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
					_label.TextWithMnemonic = _text.ToPlatformMnemonic();
				else
					_label.Text = _text;
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
			get => false;
			set { /* not supported in GTK */ }
		}
	}
}
