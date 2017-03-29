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
		public EtoTextBox()
		{
			MaxLength = 0;
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

	public class TextBoxHandler : TextBoxHandler<EtoTextBox, TextBox, TextBox.ICallback>
	{
		public override swf.TextBox SwfTextBox => Control;

		public override EtoTextBox EtoTextBox => Control;

		public TextBoxHandler()
		{
			Control = new EtoTextBox();
		}
	}

	public abstract class TextBoxHandler<TControl, TWidget, TCallback> : WindowsControl<TControl, TWidget, TCallback>, TextBox.IHandler
		where TControl: swf.Control
		where TWidget: TextBox
		where TCallback: TextBox.ICallback
	{
		public abstract swf.TextBox SwfTextBox { get; }

		public abstract EtoTextBox EtoTextBox { get; }

		public virtual bool ShowBorder
		{
			get { return SwfTextBox.BorderStyle != swf.BorderStyle.None; }
			set
			{
				SwfTextBox.BorderStyle = value ? swf.BorderStyle.Fixed3D : swf.BorderStyle.None;
				SetPlaceholderText(); // setting border clears this out for some reason
			}
		}

		public TextAlignment TextAlignment
		{
			get { return SwfTextBox.TextAlign.ToEto(); }
			set
			{
				SwfTextBox.TextAlign = value.ToSWF();
				SetPlaceholderText(); // setting text alignment clears this out for some reason
			}
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
								if (selection.Start >= 0 && selection.End < SwfTextBox.TextLength)
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
					Widget.TextInput += (sender, e) =>
					{
						var tia = new TextChangingEventArgs(e.Text, Selection);
						Callback.OnTextChanging(Widget, tia);
						e.Cancel = tia.Cancel;
					};
					if (EtoTextBox != null)
					{
						EtoTextBox.Cutting += (sender, e) =>
						{
							clipboard.Clear();
							clipboard.Text = SwfTextBox.SelectedText;
							var tia = new TextChangingEventArgs(string.Empty, Selection);
							Callback.OnTextChanging(Widget, tia);
							e.Cancel = tia.Cancel;
						};
						EtoTextBox.Pasting += (sender, e) =>
						{
							var tia = new TextChangingEventArgs(clipboard.Text, Selection);
							Callback.OnTextChanging(Widget, tia);
							e.Cancel = tia.Cancel;
						};
					}
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public virtual bool ReadOnly
		{
			get { return SwfTextBox.ReadOnly; }
			set
			{
				SwfTextBox.ReadOnly = value;
				SetPlaceholderText();
			}
		}

		public int MaxLength
		{
			get { return SwfTextBox.MaxLength; }
			set { SwfTextBox.MaxLength = value; }
		}

		static object PlaceholderText_Key = new object();

		public string PlaceholderText
		{
			get { return Widget.Properties.Get<string>(PlaceholderText_Key); }
			set
			{
				Widget.Properties.Set(PlaceholderText_Key, value, SetPlaceholderText);
			}
		}
		
		void SetPlaceholderText()
		{
			Win32.SendMessage(SwfTextBox.Handle, Win32.WM.EM_SETCUEBANNER, IntPtr.Zero, PlaceholderText);
		}

		public void SelectAll()
		{
			SwfTextBox.SelectAll();
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
			get { return SwfTextBox.SelectionStart; }
			set
			{
				SwfTextBox.SelectionStart = value;
				SwfTextBox.SelectionLength = 0;
			}
		}

		public Range<int> Selection
		{
			get { return new Range<int>(SwfTextBox.SelectionStart, SwfTextBox.SelectionStart + SwfTextBox.SelectionLength - 1); }
			set
			{
				SwfTextBox.SelectionStart = value.Start;
				SwfTextBox.SelectionLength = value.Length();
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

		protected override void Initialize()
		{
			base.Initialize();
			Widget.GotFocus += Widget_GotFocus;
		}

		void Widget_GotFocus(object sender, EventArgs e)
		{
			if (AutoSelectMode == AutoSelectMode.OnFocus)
				SelectAll();
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				if (AutoSelectMode == AutoSelectMode.Never)
					Selection = new Range<int>(value.Length, value.Length - 1);
			}
		}

		public AutoSelectMode AutoSelectMode { get; set; }
	}
}
