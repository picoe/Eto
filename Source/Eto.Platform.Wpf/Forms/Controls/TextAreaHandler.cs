using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TextAreaHandler : WpfControl<swc.TextBox, TextArea>, ITextArea
	{
		int? lastCaretIndex;

		public TextAreaHandler ()
		{
			Control = new swc.TextBox {
				Width = TextArea.DefaultSize.Width,
				Height = TextArea.DefaultSize.Height,
				AcceptsReturn = true,
				AcceptsTab = true,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto
			};
		}

		public override void AttachEvent (string handler)
		{
			switch (handler)
			{
			case TextArea.TextChangedEvent:
				Control.TextChanged += (sender, e) => {
					Widget.OnTextChanged (EventArgs.Empty);
				};
				break;
			case TextArea.SelectionChangedEvent:
				Control.SelectionChanged += (sender, e) => {
					Widget.OnSelectionChanged (EventArgs.Empty);
				};
				break;
			case TextArea.CaretIndexChangedEvent:
				Control.SelectionChanged += (sender, e) => {
					var caretIndex = Control.CaretIndex;
					if (lastCaretIndex != caretIndex)
					{
						Widget.OnCaretIndexChanged (EventArgs.Empty);
						lastCaretIndex = caretIndex;
					}
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set {
				Control.IsReadOnly = value;
				Control.AcceptsTab = !value;
				Control.AcceptsReturn = !value;
			}
		}

		public void Append (string text, bool scrollToCursor)
		{
			Control.AppendText (text);
			if (scrollToCursor) Control.ScrollToEnd ();
		}

		public string Text
		{
			get	{ return Control.Text; }
			set	{ Control.Text = value;	}
		}

		public bool Wrap
		{
			get { return Control.TextWrapping == sw.TextWrapping.Wrap; }
			set	{
				Control.TextWrapping = value ? sw.TextWrapping.Wrap : sw.TextWrapping.NoWrap;
			}
		}

		public string SelectedText
		{
			get { return Control.SelectedText; }
			set { Control.SelectedText = value; }
		}

		public Range Selection
		{
			get { return new Range (Control.SelectionStart, Control.SelectionLength); }
			set { Control.Select (value.Start, value.Length); }
		}

		public void SelectAll ()
		{
			Control.SelectAll ();
		}

		public int CaretIndex
		{
			get { return Control.CaretIndex; }
			set { Control.CaretIndex = value; }
		}
	}
}
