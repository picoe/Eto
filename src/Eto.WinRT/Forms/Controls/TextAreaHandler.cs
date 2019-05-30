using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Text area handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TextAreaHandler : WpfControl<swc.TextBox, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		int? lastCaretIndex;
		protected override wf.Size DefaultSize => new wf.Size(100, 60);

		public TextAreaHandler()
		{
			Control = new swc.TextBox
			{
				AcceptsReturn = true,
#if TODO_XAML
				AcceptsTab = true,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto
#endif
			};
		}

#if TODO_XAML
		protected override void SetDecorations(sw.TextDecorationCollection decorations)
		{
			Control.TextDecorations = decorations;
		}
#endif
		public override wf.Size GetPreferredSize(wf.Size constraint)
		{
			return base.GetPreferredSize(Conversions.ZeroSize);
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.TextChanged += (sender, e) => Callback.OnTextChanged(Widget, EventArgs.Empty);
					break;
				case TextArea.SelectionChangedEvent:
					Control.SelectionChanged += (sender, e) => Callback.OnSelectionChanged(Widget, EventArgs.Empty);
					break;
				case TextArea.CaretIndexChangedEvent:
					{
						Control.SelectionChanged += (sender, e) =>
						{
							var caretIndex = Control.SelectionStart + Control.SelectionLength;
							if (lastCaretIndex != caretIndex)
							{
								Callback.OnCaretIndexChanged(Widget, EventArgs.Empty);
								lastCaretIndex = caretIndex;
							}
						};
						break;
					}
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set
			{
				Control.IsReadOnly = value;
			}
		}

		public void Append(string text, bool scrollToCursor)
		{
			Control.Text += text;
			if (scrollToCursor)
			{
				var sv = Control.FindChild<swc.ScrollViewer>();
				if (sv != null)
				{
					sv.ChangeView(null, sv.ExtentHeight - sv.ViewportHeight, null, true);
				}
			}
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(); }
		}

		public bool Wrap
		{
			get { return Control.TextWrapping == sw.TextWrapping.Wrap; }
			set
			{
				Control.TextWrapping = value ? sw.TextWrapping.Wrap : sw.TextWrapping.NoWrap;
			}
		}

		public string SelectedText
		{
			get { return Control.SelectedText; }
			set { Control.SelectedText = value; }
		}

		public Range<int> Selection
		{
			get { return new Range<int>(Control.SelectionStart, Control.SelectionStart + Control.SelectionLength - 1); }
			set
			{
				Control.Focus(sw.FocusState.Programmatic);
				Control.Select(value.Start, value.End - value.Start + 1);
			}
		}

		public void SelectAll()
		{
			Control.Focus(sw.FocusState.Programmatic);
			Control.SelectAll();
		}

		public int CaretIndex
		{
			get { return Control.SelectionStart + Control.SelectionLength; }
			set
			{
				Control.Focus(sw.FocusState.Programmatic);
				Control.SelectionStart = value; Control.SelectionLength = 0;
			}
		}

		public bool AcceptsReturn
		{
			get { return Control.AcceptsReturn; }
			set { Control.AcceptsReturn = value; }
		}

		public TextAlignment TextAlignment
		{
			get { return Control.TextAlignment.ToEto(); }
			set { Control.TextAlignment = value.ToWpfTextAlignment(); }
		}

		bool acceptsTab = true;
		public bool AcceptsTab
		{
			get { return acceptsTab; }
			set
			{
				if (value != AcceptsTab)
				{
					if (acceptsTab)
						Control.KeyDown -= Control_KeyDown;
					acceptsTab = value;
					if (acceptsTab)
						Control.KeyDown += Control_KeyDown;
				}
			}
		}

		void Control_KeyDown(object sender, sw.Input.KeyRoutedEventArgs e)
		{
			if (acceptsTab && e.Key == Windows.System.VirtualKey.Tab)
			{
				var selection = Control.SelectionStart + Control.SelectionLength;
				Control.SelectedText = "\t";
				Control.SelectionStart = selection;
				Control.SelectionLength = 0;
				e.Handled = true;
			}
		}


		public bool SpellCheck
		{
			get { return Control.IsSpellCheckEnabled; }
			set { Control.IsSpellCheckEnabled = value; }
		}

		public bool SpellCheckIsSupported { get { return true; } }

		public TextReplacements TextReplacements
		{
			get { return TextReplacements.None; }
			set { }
		}

		public TextReplacements SupportedTextReplacements
		{
			get { return TextReplacements.None; }
		}
	}
}
