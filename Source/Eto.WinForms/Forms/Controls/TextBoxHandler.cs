using System;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Eto.WinForms.Forms.Controls
{
	[DesignerCategory("Code")]
	public class EtoTextBox : swf.TextBox
	{
		string watermarkText;
		public string WatermarkText
		{
			get { return watermarkText; }
			set
			{
				watermarkText = value;
				Win32.SendMessage(Handle, Win32.WM.EM_SETCUEBANNER, IntPtr.Zero, watermarkText);
			}
		}

		public event EventHandler<CancelEventArgs> Copying;

		protected virtual void OnCopying(CancelEventArgs e)
		{
			if (Copying != null)
				Copying(this, e);
		}
		public event EventHandler<CancelEventArgs> Cutting;

		protected virtual void OnCutting(CancelEventArgs e)
		{
			if (Cutting != null)
				Cutting(this, e);
		}
		public event EventHandler<CancelEventArgs> Pasting;

		protected virtual void OnPasting(CancelEventArgs e)
		{
			if (Pasting != null)
				Pasting(this, e);
		}

		protected override void WndProc(ref swf.Message m)
		{
			var e = new CancelEventArgs();
			if (m.Msg == (int)Win32.WM.CUT)
				OnCutting(e);
			if (m.Msg == (int)Win32.WM.COPY)
				OnCopying(e);
			if (m.Msg == (int)Win32.WM.PASTE)
				OnPasting(e);
			if (e.Cancel)
				return;

			base.WndProc(ref m);
		}
	}

	public class TextBoxHandler : WindowsControl<EtoTextBox, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		public TextBoxHandler()
		{
			Control = new EtoTextBox();
		}

		public bool ShowBorder
		{
			get { return Control.BorderStyle != swf.BorderStyle.None; }
			set { Control.BorderStyle = value ? swf.BorderStyle.Fixed3D : swf.BorderStyle.None; }
		}

		static Func<char, bool> testIsNonWord = ch => char.IsWhiteSpace(ch) || char.IsPunctuation(ch);

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextBox.TextChangingEvent:
					var clipboard = new Clipboard();
					Widget.KeyDown += (sender, e) =>
					{
						switch (e.KeyData)
						{
							case Keys.Delete:
							case Keys.Backspace:
							case Keys.Shift | Keys.Delete:
							case Keys.Shift | Keys.Backspace:
								var selection = Selection;
								if (selection.Length() == 0)
									selection = new Range<int>(e.KeyData == Keys.Delete ? CaretIndex : CaretIndex - 1);
								if (selection.Start >= 0 && selection.End < Control.TextLength)
								{
									var tia = new TextChangingEventArgs(string.Empty, selection);
									Callback.OnTextChanging(Widget, tia);
									e.Handled = tia.Cancel;
								}
								break;
							case Keys.Control | Keys.Delete:
								{
									// delete next word
									string text = Text;
									int start = CaretIndex;
									int end = start;
									int length = text.Length;

									// find end of next word
									while (end < length && !testIsNonWord(text[end]))
										end++;
									while (end < length && testIsNonWord(text[end]))
										end++;

									if (end > start)
									{
										var tia = new TextChangingEventArgs(string.Empty, new Range<int>(start, end - 1));
										Callback.OnTextChanging(Widget, tia);
										e.Handled = tia.Cancel;
									}
								}
								break;
							case Keys.Control | Keys.Backspace:
								{
									// delete previous word
									string text = Text;
									int end = CaretIndex;
									int start = end;

									// find start of previous word
									while (start > 0 && testIsNonWord(text[start - 1]))
										start--;
									while (start > 0 && !testIsNonWord(text[start - 1]))
										start--;

									if (end > start)
									{
										var tia = new TextChangingEventArgs(string.Empty, new Range<int>(start, end - 1));
										Callback.OnTextChanging(Widget, tia);
										e.Handled = tia.Cancel;
									}
								}
								break;
						}
					};
					Control.Cutting += (sender, e) =>
					{
						clipboard.Clear();
						clipboard.Text = Control.SelectedText;
						var tia = new TextChangingEventArgs(string.Empty, Selection);
						Callback.OnTextChanging(Widget, tia);
						e.Cancel = tia.Cancel;
					};
					Control.Pasting += (sender, e) =>
					{
						var tia = new TextChangingEventArgs(clipboard.Text, Selection);
						Callback.OnTextChanging(Widget, tia);
						e.Cancel = tia.Cancel;
					};
					Widget.TextInput += (sender, e) =>
					{
						var tia = new TextChangingEventArgs(e.Text, Selection);
						Callback.OnTextChanging(Widget, tia);
						e.Cancel = tia.Cancel;
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

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string PlaceholderText
		{
			get { return Control.WatermarkText; }
			set { Control.WatermarkText = value; }
		}

		public void SelectAll()
		{
			Control.Focus();
			Control.SelectAll();
		}

		static readonly Win32.WM[] intrinsicEvents = {
														 Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK,
														 Win32.WM.RBUTTONDOWN, Win32.WM.RBUTTONUP, Win32.WM.RBUTTONDBLCLK
													 };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}

		public int CaretIndex
		{
			get { return Control.SelectionStart; }
			set
			{
				Control.SelectionStart = value;
				Control.SelectionLength = 0;
			}
		}

		public Range<int> Selection
		{
			get { return new Range<int>(Control.SelectionStart, Control.SelectionStart + Control.SelectionLength - 1); }
			set
			{
				Control.SelectionStart = value.Start;
				Control.SelectionLength = value.Length();
			}
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			SetAutoSize();
		}

		protected override void SetAutoSize()
		{
			base.SetAutoSize();
			if (XScale && YScale)
				Control.AutoSize = true;
		}
	}
}
