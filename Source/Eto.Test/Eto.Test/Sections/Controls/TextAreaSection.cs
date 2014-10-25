using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(TextArea))]
	public class TextAreaSection : Scrollable
	{
		public TextAreaSection()
		{
			Content = Default();
		}

		Control Default()
		{
			var text = new TextArea { Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." };
			LogEvents(text);

			return new TableLayout
			{
				Rows =
				{
					TableLayout.Horizontal(null, ShowSelectedText(text), SetSelectedText(text), ReplaceSelected(text), SelectAll(text), null).With(r => r.Padding = Padding.Empty),
					TableLayout.Horizontal(null, SetAlignment(text), SetCaretButton(text), ChangeColorButton(text), null).With(r => r.Padding = Padding.Empty),
					TableLayout.Horizontal(null, EnabledCheckBox(text), ReadOnlyCheckBox(text), AcceptsTabCheckBox(text), AcceptsReturnCheckBox(text), WrapCheckBox(text), null).With(r => r.Padding = Padding.Empty),
					text
				}
			};
		}

		Control WrapCheckBox(TextArea text)
		{
			var control = new CheckBox { Text = "Wrap" };
			control.CheckedBinding.Bind(text, t => t.Wrap);
			return control;
		}

		Control AcceptsReturnCheckBox(TextArea text)
		{
			var control = new CheckBox { Text = "AcceptsReturn" };
			control.CheckedBinding.Bind(text, t => t.AcceptsReturn);
			return control;
		}

		Control AcceptsTabCheckBox(TextArea text)
		{
			var control = new CheckBox { Text = "AcceptsTab" };
			control.CheckedBinding.Bind(text, t => t.AcceptsTab);
			return control;
		}

		Control SetAlignment(TextArea text)
		{
			var control = new EnumDropDown<HorizontalAlign>();
			control.SelectedValueBinding.Bind(text, t => t.HorizontalAlign);
			return new TableLayout
			{
				Padding = Padding.Empty,
				Spacing = new Size(5, 5),
				Rows = { new TableRow(new Label { Text = "Alignment", VerticalAlign = VerticalAlign.Middle }, control) }
			};
		}

		Control ShowSelectedText(TextArea text)
		{
			var control = new Button { Text = "Show selected text" };
			control.Click += (sender, e) => MessageBox.Show(this, string.Format("Selected Text: {0}", text.SelectedText));
			return control;
		}

		Control SelectAll(TextArea text)
		{
			var control = new Button { Text = "Select All" };
			control.Click += (sender, e) =>
			{
				text.SelectAll();
				text.Focus();
			};
			return control;
		}

		Control SetSelectedText(TextArea textArea)
		{
			var control = new Button { Text = "Set selected text" };
			control.Click += (sender, e) =>
			{
				var text = textArea.Text;
				// select the last half of the text
				textArea.Selection = new Range<int>(text.Length / 2, text.Length / 2 + 1);
				textArea.Focus();
			};
			return control;
		}

		Control ReplaceSelected(TextArea textArea)
		{
			var control = new Button { Text = "Replace selected text" };
			control.Click += (sender, e) =>
			{
				textArea.SelectedText = "Some inserted text!";
				textArea.Focus();
			};
			return control;
		}

		Control SetCaretButton(TextArea textArea)
		{
			var control = new Button { Text = "Set Caret" };
			control.Click += (sender, e) =>
			{
				textArea.CaretIndex = textArea.Text.Length / 2;
				textArea.Focus();
			};
			return control;
		}

		Control ChangeColorButton(TextArea textArea)
		{
			var control = new Button { Text = "Change Color" };
			control.Click += (sender, e) =>
			{
				textArea.BackgroundColor = Colors.Black;
				textArea.TextColor = Colors.Blue;
			};
			return control;
		}

		Control EnabledCheckBox(TextArea textArea)
		{
			var control = new CheckBox { Text = "Enabled" };
			control.CheckedBinding.Bind(textArea, t => t.Enabled);
			return control;
		}

		Control ReadOnlyCheckBox(TextArea textArea)
		{
			var control = new CheckBox { Text = "ReadOnly" };
			control.CheckedBinding.Bind(textArea, t => t.ReadOnly);
			return control;
		}

		void LogEvents(TextArea control)
		{
			control.TextChanged += (sender, e) => Log.Write(control, "TextChanged, Text: {0}", control.Text);

			control.SelectionChanged += (sender, e) => Log.Write(control, "SelectionChanged, Selection: {0}", control.Selection);

			control.CaretIndexChanged += (sender, e) => Log.Write(control, "CaretIndexChanged, CaretIndex: {0}", control.CaretIndex);
		}
	}
}

