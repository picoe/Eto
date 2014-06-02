using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;

namespace Eto.WinForms
{
	public class TextAreaHandler : WindowsControl<swf.RichTextBox, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		int? lastCaretIndex;
		swf.TableLayoutPanel container;

		public static Size DefaultMinimumSize = new Size(100, 60);

		public override Size? DefaultSize
		{
			get { return DefaultMinimumSize; }
		}

		public override swf.Control ContainerControl
		{
			get { return container; }
		}

		public TextAreaHandler()
		{
			Control = new swf.RichTextBox
			{
				Size = sd.Size.Empty,
				Multiline = true,
				AcceptsTab = true,
				Dock = swf.DockStyle.Fill,
				BorderStyle = swf.BorderStyle.None,
				ScrollBars = swf.RichTextBoxScrollBars.Both,
			};
			container = new swf.TableLayoutPanel
			{
				MinimumSize = sd.Size.Empty,
				BorderStyle = swf.BorderStyle.FixedSingle,
				Size = DefaultMinimumSize.ToSD()
			};
			container.ColumnStyles.Add(new swf.ColumnStyle(swf.SizeType.AutoSize, 1));
			container.RowStyles.Add(new swf.RowStyle(swf.SizeType.AutoSize, 1));
			container.Controls.Add(Control, 0, 0);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextArea.SelectionChangedEvent:
					Control.SelectionChanged += (sender, e) => Callback.OnSelectionChanged(Widget, EventArgs.Empty);
					break;
				case TextArea.CaretIndexChangedEvent:
					Control.SelectionChanged += (sender, e) =>
					{
						var caretIndex = CaretIndex;
						if (caretIndex != lastCaretIndex)
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

		public bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		public bool Wrap
		{
			get { return Control.WordWrap; }
			set { Control.WordWrap = value; }
		}

		public void Append(string text, bool scrollToCursor)
		{
			Control.AppendText(text);
			if (scrollToCursor)
			{
				Control.SelectionStart = Control.Text.Length;
				Control.ScrollToCaret();
			}
		}

		public string SelectedText
		{
			get { return Control.SelectedText; }
			set
			{
				var start = Control.SelectionStart;
				Control.SelectedText = value;
				if (value != null)
					Control.Select(start, value.Length);
			}
		}

		public Range Selection
		{
			get { return new Range(Control.SelectionStart, Control.SelectionLength); }
			set { Control.Select(value.Start, value.Length); }
		}

		public void SelectAll()
		{
			Control.SelectAll();
		}

		public int CaretIndex
		{
			get { return Control.SelectionStart; }
			set { Control.Select(value, 0); }
		}
	}
}
