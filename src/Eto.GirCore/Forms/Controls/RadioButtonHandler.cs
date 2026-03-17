namespace Eto.GirCore.Forms.Controls
{
	public class RadioButtonHandler : GirControl<Gtk.CheckButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		string? text;
		bool suppressToggleCallback;

		public void Create(RadioButton controller)
		{
			Control = Gtk.CheckButton.New();
			Control.UseUnderline = true;
			Control.OnToggled += (sender, e) => HandleToggled();

			if (controller?.Handler is RadioButtonHandler controllerHandler)
				Control.SetGroup(controllerHandler.Control);
		}

		void HandleToggled()
		{
			if (suppressToggleCallback)
				return;

			// Only the newly active grouped check button should surface Click.
			if (Control.Active)
				Callback.OnClick(Widget, EventArgs.Empty);
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public string Text
		{
			get => text ?? string.Empty;
			set
			{
				text = value;
				Control.Label = Control.UseUnderline ? value.ToPlatformMnemonic() : value;
			}
		}

		public bool Checked
		{
			get => Control.Active;
			set
			{
				if (Control.Active == value)
					return;
				suppressToggleCallback = true;
				Control.Active = value;
				suppressToggleCallback = false;
				Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			}
		}

		public Color TextColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public bool UseMnemonic
		{
			get => Control.UseUnderline;
			set
			{
				if (value == Control.UseUnderline)
					return;
				var currentText = Text;
				Control.UseUnderline = value;
				Text = currentText;
			}
		}

		public bool AlwaysShowMnemonic
		{
			get => false;
			set { }
		}
	}
}
