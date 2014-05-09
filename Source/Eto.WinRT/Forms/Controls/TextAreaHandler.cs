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
		readonly Size defaultSize = TextArea.DefaultSize;

		protected override Size DefaultSize { get { return defaultSize; } }

		public TextAreaHandler ()
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

		public override void AttachEvent (string id)
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
#if TODO_XAML
					Control.SelectionChanged += (sender, e) => {
						var caretIndex = Control.CaretIndex;
						if (lastCaretIndex != caretIndex)
						{
							Widget.OnCaretIndexChanged(EventArgs.Empty);
							lastCaretIndex = caretIndex;
						}
					};
					break;
#else
					throw new NotImplementedException();
#endif
				}
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set {
				Control.IsReadOnly = value;
#if TODO_XAML
				Control.AcceptsTab = !value;
#endif
				Control.AcceptsReturn = !value;
			}
		}

		public void Append (string text, bool scrollToCursor)
		{
#if TODO_XAML
			Control.AppendText (text);
			if (scrollToCursor) Control.ScrollToEnd ();
#else
			throw new NotImplementedException();
#endif
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
#if TODO_XAML			
			get { return Control.CaretIndex; }
			set { Control.CaretIndex = value; }
#else
			get; set;
#endif
		}
	}
}
