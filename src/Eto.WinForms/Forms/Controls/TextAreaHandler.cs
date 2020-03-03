using System;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class TextAreaHandler : TextAreaHandler<TextArea, TextArea.ICallback>
	{

	}

	public class EtoRichTextBox : swf.RichTextBox
	{
		public bool AcceptsReturn { get; set; }

		protected override bool IsInputKey(swf.Keys keyData)
		{
			if (!AcceptsTab &&
				(keyData & ~swf.Keys.Modifiers) == swf.Keys.Tab &&
				(keyData & (swf.Keys.Control | swf.Keys.Alt)) == 0
			)
				return false;

			if (!AcceptsReturn && keyData == swf.Keys.Return)
				return false;

			return base.IsInputKey(keyData);
		}

		protected override void OnKeyDown(swf.KeyEventArgs e)
		{
			if (!AcceptsReturn && e.KeyData == swf.Keys.Return)
			{
				e.Handled = true;
				return;
			}

			base.OnKeyDown(e);
		}
	}

	public class TextAreaHandler<TWidget, TCallback> : WindowsControl<EtoRichTextBox, TWidget, TCallback>, TextArea.IHandler
		where TWidget : TextArea
		where TCallback : TextArea.ICallback
	{
		int? lastCaretIndex;
		swf.TableLayoutPanel container;

		public static Size DefaultMinimumSize = new Size(100, 60);

		public override Size? GetDefaultSize(Size availableSize)
		{
			return DefaultMinimumSize;
		}

		public override swf.Control ContainerControl
		{
			get { return container; }
		}

		public TextAreaHandler()
		{
			Control = new EtoRichTextBox
			{
				Size = sd.Size.Empty,
				Multiline = true,
				AcceptsTab = true,
				AcceptsReturn = true,
				Dock = swf.DockStyle.Fill,
				BorderStyle = swf.BorderStyle.None,
				ScrollBars = swf.RichTextBoxScrollBars.Both,
				LanguageOption = swf.RichTextBoxLanguageOptions.DualFont
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

		/// <summary>
		/// Supresses the selection/caret changed events if greater than zero
		/// </summary>
		protected int SuppressSelectionChanged { get; set; }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextArea.SelectionChangedEvent:
					Control.SelectionChanged += (sender, e) =>
					{
						if (SuppressSelectionChanged <= 0)
							Callback.OnSelectionChanged(Widget, EventArgs.Empty);
					};
					break;
				case TextArea.CaretIndexChangedEvent:
					Control.SelectionChanged += (sender, e) =>
					{
						var caretIndex = CaretIndex;
						if (SuppressSelectionChanged <= 0 && caretIndex != lastCaretIndex)
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

		public override Color TextColor
		{
			get { return Control.ForeColor.ToEto(); }
			set { Control.ForeColor = value.ToSD(); }
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				SuppressSelectionChanged++;
				var val = value ?? string.Empty;
				base.Text = val;
				if (!Control.IsHandleCreated) // correct??
					Callback.OnTextChanged(Widget, EventArgs.Empty);
				Selection = Eto.Forms.Range.FromLength(val.Length, 0); // Fully qualified because System.Range was introduced in .NET Core 3.0
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				SuppressSelectionChanged--;
			}
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
				SuppressSelectionChanged++;
				var val = value ?? string.Empty;
				Control.SelectedText = val;
				Control.Select(start, val.Length);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				SuppressSelectionChanged--;
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

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}

		public TextAlignment TextAlignment
		{
			get { return Control.SelectionAlignment.ToEto(); }
			set
			{
				if (value == TextAlignment) return;
				var sel = Selection;
				Control.SelectAll();
				Control.SelectionAlignment = value.ToSWF();
				Selection = sel;
			}
		}


		public bool SpellCheck
		{
			get { return false; }
			set { }
		}

		public bool SpellCheckIsSupported { get { return false; } }

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