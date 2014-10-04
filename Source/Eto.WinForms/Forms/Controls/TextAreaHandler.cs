using System;
using System.Linq;
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

		public Color TextColor
		{
			get { return Control.ForeColor.ToEto(); }
			set { Control.ForeColor = value.ToSD(); }
		}

		public void Append(string text, bool scrollToCursor)
		{
			if (scrollToCursor)
			{
				Control.SelectionStart = Control.TextLength;
				Control.SelectedText = text;
				if (EtoEnvironment.Platform.IsMono)
					Control.ScrollToCaret();
				else
					Control.FastScrollToCaret(); // use a faster method for large amounts of text
			}
			else
				Control.AppendText(text);
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

		public Range<int> Selection
		{
			get { return new Range<int>(Control.SelectionStart, Control.SelectionStart + Control.SelectionLength - 1); }
			set { Control.Select(value.Start, value.End - value.Start + 1); }
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

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}