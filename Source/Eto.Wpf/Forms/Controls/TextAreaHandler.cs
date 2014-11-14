using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class TextAreaHandler : WpfControl<swc.TextBox, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		int? lastCaretIndex;
		#pragma warning disable 612,618
		readonly Size defaultSize = TextArea.DefaultSize;
		#pragma warning restore 612,618

		protected override Size DefaultSize { get { return defaultSize; } }

		class EtoTextBox : swc.TextBox
		{
			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				if (IsLoaded && IsVisible)
				{
					constraint.Width = !double.IsNaN(constraint.Width) ? Math.Min(constraint.Width, ActualWidth) : ActualWidth;
					constraint.Height = !double.IsNaN(constraint.Height) ? Math.Min(constraint.Height, ActualHeight) : ActualHeight;
				}
				return base.MeasureOverride(constraint);
			}
		}

		public TextAreaHandler ()
		{
			Control = new EtoTextBox
			{
				AcceptsReturn = true,
				AcceptsTab = true,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto
			};
			Wrap = true;
		}

		protected override void SetDecorations(sw.TextDecorationCollection decorations)
		{
			Control.TextDecorations = decorations;
		}

		public override sw.Size GetPreferredSize(sw.Size constraint)
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
				Control.SelectionChanged += (sender, e) => {
					var caretIndex = Control.CaretIndex;
					if (lastCaretIndex != caretIndex)
					{
						Callback.OnCaretIndexChanged(Widget, EventArgs.Empty);
						lastCaretIndex = caretIndex;
					}
				};
				break;
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

		public Range<int> Selection
		{
			get { return new Range<int>(Control.SelectionStart, Control.SelectionStart + Control.SelectionLength - 1); }
			set { Control.Select (value.Start, value.End - value.Start + 1); }
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

		public bool AcceptsTab
		{
			get { return Control.AcceptsTab; }
			set { Control.AcceptsTab = value; }
		}

		public bool AcceptsReturn
		{
			get { return Control.AcceptsReturn; }
			set { Control.AcceptsReturn = value; }
		}

		public HorizontalAlign HorizontalAlign
		{
			get { return Control.HorizontalContentAlignment.ToEto(); }
			set { Control.HorizontalContentAlignment = value.ToWpf(); }
		}

		public VerticalAlign VerticalAlign
		{
			get { return Control.VerticalContentAlignment.ToEto(); }
			set { Control.VerticalContentAlignment = value.ToWpf(); }
		}
	}
}
