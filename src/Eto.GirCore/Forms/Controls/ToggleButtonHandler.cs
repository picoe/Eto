namespace Eto.GirCore.Forms.Controls
{
	public class ToggleButtonHandler : GirControl<Gtk.ToggleButton, ToggleButton, ToggleButton.ICallback>, ToggleButton.IHandler
	{
		int suppressClick;
		string? text;

		public ToggleButtonHandler()
		{
			Control = Gtk.ToggleButton.New();
			Control.UseUnderline = true;
			Control.OnToggled += (sender, e) => HandleToggled();
		}

		void HandleToggled()
		{
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);

			if (suppressClick == 0)
				Callback.OnClick(Widget, EventArgs.Empty);
		}

		public bool Checked
		{
			get => Control.Active;
			set
			{
				suppressClick++;
				Control.Active = value;
				suppressClick--;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ToggleButton.CheckedChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public string? Text
		{
			get => text;
			set
			{
				text = value;
				Control.Label = Control.UseUnderline ? value?.ToPlatformMnemonic() : value;
			}
		}

		public Image Image { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public ButtonImagePosition ImagePosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public Size MinimumSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public Color TextColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public bool UseMnemonic
		{
			get => Control.UseUnderline;
			set
			{
				if (value == Control.UseUnderline)
					return;
				Control.UseUnderline = value;
				Text = text;
			}
		}

		public bool AlwaysShowMnemonic
		{
			get => false;
			set { }
		}
	}
}
