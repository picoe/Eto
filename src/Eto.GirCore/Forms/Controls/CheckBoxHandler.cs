namespace Eto.GirCore.Forms.Controls
{
	public class CheckBoxHandler : GirControl<Gtk.CheckButton, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		bool toggling;
		string? text;

		public CheckBoxHandler()
		{
			Control = Gtk.CheckButton.New();
			Control.UseUnderline = true;
			Control.OnToggled += (sender, e) => HandleToggled();
		}

		void HandleToggled()
		{
			if (toggling)
				return;

			toggling = true;
			if (ThreeState)
			{
				if (!Control.Inconsistent && Control.Active)
					Control.Inconsistent = true;
				else if (Control.Inconsistent)
				{
					Control.Inconsistent = false;
					Control.Active = true;
				}
			}

			Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			toggling = false;
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

		public bool? Checked
		{
			get => Control.Inconsistent ? null : Control.Active;
			set
			{
				if (value == null)
				{
					Control.Inconsistent = true;
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
				else
				{
					var hasChanged = Control.Inconsistent && Control.Active == value.Value;
					Control.Inconsistent = false;
					Control.Active = value.Value;
					if (hasChanged)
						Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public bool ThreeState { get; set; }

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
