using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using mwc = Xceed.Wpf.Toolkit;
using swi = System.Windows.Input;
using swd = System.Windows.Documents;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class TextBoxHandler : WpfControl<mwc.WatermarkTextBox, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		protected override Size DefaultSize { get { return new Size(100, -1); } }

		protected override bool PreventUserResize { get { return true; } }

		static sw.Thickness DefaultBorderThickness = new mwc.DateTimePicker().BorderThickness;

		public override bool ShowBorder
		{
			get { return !Control.BorderThickness.ToEto().IsZero; }
			set { Control.BorderThickness = value ? DefaultBorderThickness : new sw.Thickness(0); }
		}

		public TextBoxHandler ()
		{
			Control = new mwc.WatermarkTextBox();
			Control.GotKeyboardFocus += Control_GotKeyboardFocus;
		}

		void Control_GotKeyboardFocus(object sender, sw.Input.KeyboardFocusChangedEventArgs e)
		{
			Control.SelectAll();
			Control.GotKeyboardFocus -= Control_GotKeyboardFocus;
		}

		public override sw.Size GetPreferredSize(sw.Size constraint)
		{
			return base.GetPreferredSize(WpfConversions.ZeroSize);
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		static Func<char, bool> testIsNonWord = ch => char.IsWhiteSpace(ch) || char.IsPunctuation(ch);

		public override void AttachEvent (string id)
		{
			switch (id) {
				case TextControl.TextChangedEvent:
					Control.TextChanged += (sender, e) => Callback.OnTextChanged(Widget, EventArgs.Empty);
					break;
				case TextBox.TextChangingEvent:
					var clipboard = new Clipboard();
					Control.PreviewTextInput += (sender, e) =>
					{
						var tia = new TextChangingEventArgs(e.Text, Selection);
						Callback.OnTextChanging(Widget, tia);
						e.Handled = tia.Cancel;
					};
					Control.AddHandler(swi.CommandManager.PreviewExecutedEvent, new swi.ExecutedRoutedEventHandler((sender, e) =>
					{
						if (e.Command == swi.ApplicationCommands.Cut || e.Command == swi.ApplicationCommands.Delete)
						{
							var text = Control.SelectedText;
							var tia = new TextChangingEventArgs(string.Empty, Selection);
							Callback.OnTextChanging(Widget, tia);
							if (tia.Cancel)
							{
								if (e.Command == swi.ApplicationCommands.Cut)
									clipboard.Text = text;
								e.Handled = true;
							}
						}
						else if (e.Command == swi.ApplicationCommands.Paste)
						{
							var text = clipboard.Text;
							var tia = new TextChangingEventArgs(text, Selection);
							Callback.OnTextChanging(Widget, tia);
							e.Handled = tia.Cancel;
						}
						else if (e.Command == swd.EditingCommands.Delete || e.Command == swd.EditingCommands.Backspace)
						{
							var range = Selection;
							if (range.Length() == 0)
								range = new Range<int>(e.Command == swd.EditingCommands.Delete ? range.Start : range.Start - 1);
							if (range.Start >= 0)
							{
								var tia = new TextChangingEventArgs(string.Empty, range);
								Callback.OnTextChanging(Widget, tia);
								e.Handled = tia.Cancel;
							}
						}
						else if (e.Command == swd.EditingCommands.DeletePreviousWord)
						{
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
						else if (e.Command == swd.EditingCommands.DeleteNextWord)
						{
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
					}));
					break;
				default:
					base.AttachEvent (id);
					break;
			}
		}

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set { Control.IsReadOnly = value; }
		}

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string Text
		{
			get { return Control.Text; }
			set {
				Control.Text = value;
				Control.SelectAll();
			}
		}

		public string PlaceholderText
		{
			get { return Control.Watermark as string; }
			set { Control.Watermark = value; }
		}

		public int CaretIndex
		{
			get { return Control.CaretIndex; }
			set { Control.CaretIndex = value; }
		}

		public void SelectAll ()
		{
			Control.Focus ();
			Control.SelectAll ();
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
	}
}
