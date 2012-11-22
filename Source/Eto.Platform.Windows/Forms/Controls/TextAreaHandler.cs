using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class TextAreaHandler : WindowsControl<swf.RichTextBox, TextArea>, ITextArea
	{
		int? lastCaretIndex;
		swf.TableLayoutPanel container;
		public class MyTextBox : swf.RichTextBox
		{
		}

		public override Size? DefaultSize
		{
			get { return TextArea.DefaultSize; }
		}

		public override swf.Control ContainerControl
		{
			get { return container; }
		}

		public TextAreaHandler ()
		{
			Control = new MyTextBox {
				Multiline = true,
				AcceptsTab = true,
				Dock = swf.DockStyle.Fill,
				BorderStyle = swf.BorderStyle.None,
				ScrollBars = swf.RichTextBoxScrollBars.Both
			};
			container = new swf.TableLayoutPanel {
				BorderStyle = swf.BorderStyle.FixedSingle
			};
			container.ColumnStyles.Add (new swf.ColumnStyle (swf.SizeType.AutoSize, 1));
			container.RowStyles.Add (new swf.RowStyle (swf.SizeType.AutoSize, 1));
			container.Controls.Add (Control, 0, 0);
		}

		public override void AttachEvent (string handler)
		{
			switch (handler)
			{
			case TextArea.SelectionChangedEvent:
				Control.SelectionChanged += (sender, e) => {
					Widget.OnSelectionChanged (EventArgs.Empty);
				};
				break;
			case TextArea.CaretIndexChangedEvent:
				Control.SelectionChanged += (sender, e) => {
					var caretIndex = CaretIndex;
					if (caretIndex != lastCaretIndex)
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
		
		public bool ReadOnly {
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}
		
		public bool Wrap {
			get { return Control.WordWrap; }
			set { Control.WordWrap = value; }
		}
		
		public void Append (string text, bool scrollToCursor)
		{
			Control.AppendText (text);
			if (scrollToCursor) {
				Control.SelectionStart = Control.Text.Length;
				Control.ScrollToCaret ();
			}
		}

		public string SelectedText
		{
			get { return Control.SelectedText; }
			set {
				var start = Control.SelectionStart;
				Control.SelectedText = value;
				if (value != null)
					Control.Select (start, value.Length);
			}
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
			get { return Control.SelectionStart; }
			set { Control.Select (value, 0); }
		}
	}
}
