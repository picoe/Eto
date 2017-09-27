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
	public class EtoWatermarkTextBox : mwc.WatermarkTextBox, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class TextBoxHandler : TextBoxHandler<mwc.WatermarkTextBox, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		protected override swc.TextBox TextBox => Control;

		public TextBoxHandler()
		{
			Control = new EtoWatermarkTextBox { Handler = this };
		}

		public override string PlaceholderText
		{
			get { return Control.Watermark as string; }
			set { Control.Watermark = value; }
		}

	}

	public abstract class TextBoxHandler<TControl, TWidget, TCallback> : WpfControl<TControl, TWidget, TCallback>, TextBox.IHandler
		where TControl : swc.Control
		where TWidget : TextBox
		where TCallback: TextBox.ICallback
	{
		protected override sw.Size DefaultSize => new sw.Size(100, double.NaN);

		protected override bool PreventUserResize { get { return true; } }

		protected abstract swc.TextBox TextBox { get; }

		protected virtual swc.Control BorderControl => TextBox;

		public override bool ShowBorder
		{
			get { return BorderControl.ReadLocalValue(swc.Control.BorderThicknessProperty) == sw.DependencyProperty.UnsetValue; }
			set
			{
				if (value)
					BorderControl.ClearValue(swc.Control.BorderThicknessProperty);
				else
					BorderControl.BorderThickness = new sw.Thickness(0);
			}
		}

		public TextAlignment TextAlignment
		{
			get { return TextBox.TextAlignment.ToEto(); }
			set { TextBox.TextAlignment = value.ToWpfTextAlignment(); }
		}

		public TextBoxHandler ()
		{
		}

		protected override void Initialize()
		{
			base.Initialize();
			TextBox.GotKeyboardFocus += Control_GotKeyboardFocus;
		}

		void Control_GotKeyboardFocus(object sender, sw.Input.KeyboardFocusChangedEventArgs e)
		{
			TextBox.SelectAll();
			TextBox.GotKeyboardFocus -= Control_GotKeyboardFocus;
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		static Func<char, bool> testIsNonWord = ch => char.IsWhiteSpace(ch) || char.IsPunctuation(ch);

		static Clipboard clipboard;

		public override void AttachEvent (string id)
		{
			switch (id) {
				case TextControl.TextChangedEvent:
					TextBox.TextChanged += (sender, e) => Callback.OnTextChanged(Widget, EventArgs.Empty);
					break;
				case Eto.Forms.TextBox.TextChangingEvent:
					if (clipboard == null)
						clipboard = new Clipboard();
					TextBox.PreviewTextInput += (sender, e) =>
					{
						var tia = new TextChangingEventArgs(e.Text, Selection);
						Callback.OnTextChanging(Widget, tia);
						e.Handled = tia.Cancel;
					};
					TextBox.AddHandler(swi.CommandManager.PreviewExecutedEvent, new swi.ExecutedRoutedEventHandler((sender, e) =>
					{
						var command = e.Command as swi.RoutedUICommand;
						if (command == null)
							return;
						if (command == swi.ApplicationCommands.Cut || command == swi.ApplicationCommands.Delete)
						{
							var text = TextBox.SelectedText;
							var tia = new TextChangingEventArgs(string.Empty, Selection);
							Callback.OnTextChanging(Widget, tia);
							if (tia.Cancel)
							{
								if (command == swi.ApplicationCommands.Cut)
									clipboard.Text = text;
								e.Handled = true;
							}
						}
						else if (command == swi.ApplicationCommands.Paste)
						{
							var text = clipboard.Text;
							var tia = new TextChangingEventArgs(text, Selection);
							Callback.OnTextChanging(Widget, tia);
							e.Handled = tia.Cancel;
						}
						else if (command == swd.EditingCommands.Delete || command == swd.EditingCommands.Backspace)
						{
							var range = Selection;
							if (range.Length() == 0)
								range = new Range<int>(command == swd.EditingCommands.Delete ? range.Start : range.Start - 1);
							if (range.Start >= 0)
							{
								var tia = new TextChangingEventArgs(string.Empty, range);
								Callback.OnTextChanging(Widget, tia);
								e.Handled = tia.Cancel;
							}
						}
						else if (command == swd.EditingCommands.DeletePreviousWord)
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
						else if (command == swd.EditingCommands.DeleteNextWord)
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
						else if (command.OwnerType == typeof(swd.EditingCommands) && command.Name == "Space")
						{
							// space doesn't trigger TextInput (which you'd expect) as it can be interpreted through IME
							var text = " ";
							var tia = new TextChangingEventArgs(text, Selection);
							Callback.OnTextChanging(Widget, tia);
							e.Handled = tia.Cancel;
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
			get { return TextBox.IsReadOnly; }
			set { TextBox.IsReadOnly = value; }
		}

		public int MaxLength
		{
			get { return TextBox.MaxLength; }
			set { TextBox.MaxLength = value; }
		}

		public string Text
		{
			get { return TextBox.Text; }
			set {
				TextBox.Text = value;
				TextBox.SelectAll();
			}
		}

		public abstract string PlaceholderText { get; set; }

		public int CaretIndex
		{
			get { return TextBox.CaretIndex; }
			set { TextBox.CaretIndex = value; }
		}

		public void SelectAll ()
		{
			TextBox.Focus ();
			TextBox.SelectAll ();
		}

		public Range<int> Selection
		{
			get { return new Range<int>(TextBox.SelectionStart, TextBox.SelectionStart + TextBox.SelectionLength - 1); }
			set
			{
				TextBox.SelectionStart = value.Start;
				TextBox.SelectionLength = value.Length();
			}
		}
	}
}
