namespace Eto.Wpf.Forms.Controls
{
	
	public class EtoLabel : EtoAccessLabel
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class LabelHandler : WpfControl<EtoAccessLabel, Label, Label.ICallback>, Label.IHandler
	{
		double? previousDesiredHeight;
		string text;

		protected override void SetDecorations(sw.TextDecorationCollection decorations)
		{
			Control.TextDecorations = decorations;
		}

		public LabelHandler()
		{
			// accessText = new swc.AccessText();
			Control = new EtoLabel
			{
				Handler = this
			};
			Control.Target = Control;
			Control.SizeChanged += Control_SizeChanged;
		}

		void Control_SizeChanged(object sender, sw.SizeChangedEventArgs e)
		{
			// not loaded? don't worry about it.
			if (!Control.IsLoaded)
				return;

			// if we have a set height or no wrapping, let's skip this
			if (Wrap == WrapMode.None || !double.IsNaN(UserPreferredSize.Height))
				return;

			var newDesiredHeight = Control.DesiredSize.Height;
			if (previousDesiredHeight == null)
			{
				// don't update preferred sizes when called the first time.
				// when there's many labels this causes a major slowdown
				// the initial size should already have been taken care of by 
				// the initial layout pass.
				previousDesiredHeight = newDesiredHeight;
				return;
			}

			// Ignore any change that is less than half the line height of the current font
			// as WPF will return inconsistent results for its DesiredSize.Height in
			// odd scales to position on pixel boundaries (e.g. 150%, 175%), 
			// causing an endless update cycle in some cases.
			if (Math.Abs(previousDesiredHeight.Value - newDesiredHeight) < Control.FontSize / 2)
				return;

			// update parents when the actual desired height has changed
			// otherwise parent containers won't shrink vertically when it gets wider when wrapped
			previousDesiredHeight = newDesiredHeight;
			UpdatePreferredSize();
		}

		protected override void Initialize()
		{
			base.Initialize();
			TextAlignment = TextAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
			Wrap = WrapMode.Word;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					// do nothing, label doesn't get updated by the user
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		public TextAlignment TextAlignment
		{
			get { return Control.TextAlignment.ToEto(); }
			set
			{
				Control.TextAlignment = value.ToWpfTextAlignment();
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return Control.VerticalContentAlignment.ToEto(); }
			set { Control.VerticalContentAlignment = value.ToWpf(); }
		}

		public WrapMode Wrap
		{
			get => Control.TextWrapping.ToEto();
			set
			{
				if (value != Wrap)
				{
					Control.TextWrapping = value.ToWpf();
					SetText();
					UpdatePreferredSize();
				}
			}
		}


		public override void UpdatePreferredSize()
		{
			ParentMinimumSize = WpfConversions.ZeroSize;
			base.UpdatePreferredSize();
		}

		public override Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				SetText();
			}
		}

		public bool UseMnemonic
		{
			get => Control.UseMnemonic;
			set => Control.UseMnemonic = value;
		}

		public bool AlwaysShowMnemonic
		{
			get => Control.AlwaysShowMnemonic;
			set => Control.AlwaysShowMnemonic = value;
		}

		public bool EnableMnemonic
		{
			get => Control.EnableMnemonic;
			set => Control.EnableMnemonic = value;
		}

		void SetText()
		{
			var newText = text;
			if (Wrap == WrapMode.Character && text != null)
			{
				// wpf will always word wrap, so we replace spaces with nbsp
				// so that it is forced to wrap at the character level
				newText = newText.Replace(' ', (char)0xa0); // no break space
			}

			Control.Text = newText;
		}
	}
}
