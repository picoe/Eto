namespace Eto.Mac.Forms.Controls
{
	public abstract class MacButton<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, TextControl.IHandler
		where TControl : NSButton
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		MacMnemonicString _str;

		MacMnemonicString MnemonicString => _str ??= new();

		public virtual string Text
		{
			get { return _str?.Text ?? string.Empty; }
			set
			{
				MnemonicString.Text = value;
				SetAttributes();
				InvalidateMeasure();
			}
		}

		public virtual Color TextColor
		{
			get { return _str?.TextColor ?? NSColor.ControlText.ToEto(); }
			set
			{
				if (value != TextColor)
				{
					MnemonicString.TextColor = value;
					SetAttributes();
				}
			}
		}
		
		public bool UseMnemonic
		{
			get => _str.UseMnemonic;
			set
			{
				MnemonicString.UseMnemonic = value;
				SetAttributes();
			}
		}
		
		public bool AlwaysShowMnemonic
		{
			get => _str.AlwaysShowMnemonic;
			set
			{
				MnemonicString.AlwaysShowMnemonic = value;
				SetAttributes();
			}
		}
		
		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetAttributes(true);
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (!Widget.Loaded)
				SetAttributes(true);
			return base.GetNaturalSize(availableSize);
		}

		void SetAttributes(bool force = false)
		{
			if (Widget.Loaded || force)
			{
				Control.AttributedTitle = _str?.AttributedString ?? new NSAttributedString();
			}
		}
	}
}

