namespace Eto.Wpf.Forms.Controls
{
	public class EtoTextBox : swc.TextBox, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class TextAreaHandler : TextAreaHandler<EtoTextBox, TextArea, TextArea.ICallback>
	{
		public override string Text
		{
			get { return Control.Text; }
			set
			{
				SuppressSelectionChanged++;
				Control.Text = value;
				SuppressSelectionChanged--;
				Control.Select(value?.Length ?? 0, 0);
			}
		}

		public override bool Wrap
		{
			get { return Control.TextWrapping == sw.TextWrapping.Wrap; }
			set
			{
				Control.TextWrapping = value ? sw.TextWrapping.Wrap : sw.TextWrapping.NoWrap;
			}
		}

		public override string SelectedText
		{
			get { return Control.SelectedText; }
			set { Control.SelectedText = value ?? string.Empty; }
		}

		public override Range<int> Selection
		{
			get { return new Range<int>(Control.SelectionStart, Control.SelectionStart + Control.SelectionLength - 1); }
			set { Control.Select(value.Start, value.End - value.Start + 1); }
		}

		public override int CaretIndex
		{
			get { return Control.CaretIndex; }
			set { Control.CaretIndex = value; }
		}


		protected override void SetDecorations(sw.TextDecorationCollection decorations)
		{
			Control.TextDecorations = decorations;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextArea.CaretIndexChangedEvent:
					int? lastCaretIndex = null;
					Control.SelectionChanged += (sender, e) =>
					{
						var caretIndex = Control.CaretIndex;
						if (lastCaretIndex != caretIndex)
						{
							Callback.OnCaretIndexChanged(Widget, EventArgs.Empty);
							lastCaretIndex = caretIndex;
						}
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override void ScrollTo(Range<int> range)
		{
			var rect = Control.GetRectFromCharacterIndex(range.End);
			Control.ScrollToVerticalOffset(rect.Top);
			Control.ScrollToHorizontalOffset(rect.Left);
		}

		public override int TextLength => Control.Text.Length;
	}

	public abstract class TextAreaHandler<TControl, TWidget, TCallback> : WpfControl<TControl, TWidget, TCallback>, TextArea.IHandler
		where TControl : swc.Primitives.TextBoxBase, IEtoWpfControl, new()
		where TWidget : TextArea
		where TCallback : TextArea.ICallback
	{
		protected int SuppressSelectionChanged { get; set; }
		protected int SuppressTextChanged { get; set; }
		protected override sw.Size DefaultSize => new sw.Size(100, 60);

		protected override bool PreventUserResize { get { return true; } }

		public TextAreaHandler()
		{
			Control = new TControl
			{
				AcceptsReturn = true,
				AcceptsTab = true,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				Handler = this
			};
			Wrap = true;
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			UserPreferredSize = DefaultSize; // otherwise it grows to the constraint, which we don't want.
		}

		public override sw.Size MeasureOverride(sw.Size constraint, Func<sw.Size, sw.Size> measure)
		{
			var size = base.MeasureOverride(constraint, measure);
			return size;
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.TextChanged += Control_TextChanged;
					break;
				case TextArea.SelectionChangedEvent:
					Control.SelectionChanged += Control_SelectionChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void Control_TextChanged(object sender, swc.TextChangedEventArgs e)
		{
			if (SuppressTextChanged == 0)
				Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		void Control_SelectionChanged(object sender, sw.RoutedEventArgs e)
		{
			if (SuppressSelectionChanged == 0)
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
		}

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set
			{
				Control.IsReadOnly = value;
				Control.AcceptsTab = value ? false : AcceptsTab;
			}
		}

		public void Append(string text, bool scrollToCursor)
		{
			Control.AppendText(text);
			if (scrollToCursor) Control.ScrollToEnd();
		}

		public abstract string Text { get; set; }

		public abstract bool Wrap { get; set; }

		public abstract string SelectedText { get; set; }

		public abstract Range<int> Selection { get; set; }

		public abstract int CaretIndex { get; set; }

		public void SelectAll() => Control.SelectAll();

		static readonly object AcceptsTabKey = new object();

		public bool AcceptsTab
		{
			get { return Widget.Properties.Get<bool?>(AcceptsTabKey) ?? true; }
			set
			{
				Widget.Properties[AcceptsTabKey] = value;
				if (!Control.IsReadOnly)
					Control.AcceptsTab = value;
			}
		}

		public bool AcceptsReturn
		{
			get { return Control.AcceptsReturn; }
			set { Control.AcceptsReturn = value; }
		}

		public virtual TextAlignment TextAlignment
		{
			get { return Control.HorizontalContentAlignment.ToEto(); }
			set { Control.HorizontalContentAlignment = value.ToWpf(); }
		}

		public virtual VerticalAlignment VerticalAlign
		{
			get { return Control.VerticalContentAlignment.ToEto(); }
			set { Control.VerticalContentAlignment = value.ToWpf(); }
		}

		public bool SpellCheck
		{
			get { return Control.SpellCheck.IsEnabled; }
			set
			{
				// if it can't be enabled, just ignore instead of crashing..
				// see: https://support.microsoft.com/en-us/help/3088234/spelling-checker-isn-t-supported-in-the-net-4-6-1-in-some-conditions
				if (!WpfExtensions.SpellCheckCanBeEnabled)
					return;

				try
				{
					Control.SpellCheck.IsEnabled = value;
				}
				catch
				{
					// can sometimes fail if the spell checker is still initializing, so just disable.
					Control.SpellCheck.IsEnabled = false;
				}
			}
		}

		public bool SpellCheckIsSupported => WpfExtensions.SpellCheckCanBeEnabled;

		public TextReplacements TextReplacements
		{
			get => TextReplacements.None;
			set { }
		}

		public TextReplacements SupportedTextReplacements => TextReplacements.None;
		
		static readonly object Border_Key = new object();

		public virtual BorderType Border
		{
			get { return Widget.Properties.Get(Border_Key, BorderType.Bezel); }
			set { Widget.Properties.Set(Border_Key, value, () => Control.SetEtoBorderType(value)); }
		}

		public abstract void ScrollTo(Range<int> range);

		public virtual void ScrollToEnd() => Control.ScrollToEnd();

		public virtual void ScrollToStart() => Control.ScrollToHome();

		public abstract int TextLength { get; }

	}
}
