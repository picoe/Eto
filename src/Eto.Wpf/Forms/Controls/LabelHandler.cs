namespace Eto.Wpf.Forms.Controls
{
	public class EtoLabel : swc.Label
	{
		public LabelHandler Handler { get; set; }
		protected override void OnAccessKey(swi.AccessKeyEventArgs e)
		{
			// move focus to the next control after the label
			var tRequest = new swi.TraversalRequest(swi.FocusNavigationDirection.Next);
			MoveFocus(tRequest);
		}

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			var size = Handler.MeasureOverride(constraint, base.MeasureOverride);
			size.Width += 1;
			return size;
		}
	}

	public class LabelHandler : WpfControl<swc.Label, Label, Label.ICallback>, Label.IHandler
	{
		readonly swc.AccessText accessText;
		double? previousDesiredHeight;
		string text;

		protected override void SetDecorations(sw.TextDecorationCollection decorations)
		{
			accessText.TextDecorations = decorations;
		}

		public LabelHandler()
		{
			accessText = new swc.AccessText();
			Control = new EtoLabel
			{
				Handler = this,
				Padding = new sw.Thickness(0),
				Content = accessText
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
			get { return Control.HorizontalContentAlignment.ToEto(); }
			set
			{
				Control.HorizontalContentAlignment = value.ToWpf();
				accessText.TextAlignment = value.ToWpfTextAlignment();
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return Control.VerticalContentAlignment.ToEto(); }
			set { Control.VerticalContentAlignment = value.ToWpf(); }
		}

		public WrapMode Wrap
		{
			get
			{
				switch (accessText.TextWrapping)
				{
					case sw.TextWrapping.NoWrap:
						return WrapMode.None;
					case sw.TextWrapping.Wrap:
						return WrapMode.Character;
					case sw.TextWrapping.WrapWithOverflow:
						return WrapMode.Word;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				if (value != Wrap)
				{
					switch (value)
					{
						case WrapMode.Word:
							accessText.TextWrapping = sw.TextWrapping.WrapWithOverflow;
							break;
						case WrapMode.Character:
							accessText.TextWrapping = sw.TextWrapping.Wrap;
							break;
						case WrapMode.None:
							accessText.TextWrapping = sw.TextWrapping.NoWrap;
							break;
						default:
							throw new NotSupportedException();
					}
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
			get { return accessText.Foreground.ToEtoColor(); }
			set { accessText.Foreground = value.ToWpfBrush(accessText.Foreground); }
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

		void SetText()
		{
			var newText = text;
			if (Wrap == WrapMode.Character && text != null)
			{
				// wpf will always word wrap, so we replace spaces with nbsp
				// so that it is forced to wrap at the character level
				newText = newText.Replace(' ', (char)0xa0); // no break space
			}

			accessText.Text = newText.ToPlatformMnemonic();
		}
	}
}
