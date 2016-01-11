using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

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
				Padding = new Padding(10),
				Spacing = new Size(5, 5),
				Rows =
				{
					TextAreaOptions(text),
					TextAreaOptions2(text),
					TextAreaOptions3(text),
					TextAreaOptions4(text),
					text
				}
			};
		}

		public static Control TextAreaOptions(TextArea text)
		{
			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items = { null, ShowSelectedText(text), SetSelectedText(text), ReplaceSelected(text), SelectAll(text), null }
			};
		}

		public static Control TextAreaOptions2(TextArea text)
		{
			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items = { null, SetAlignment(text), SetCaretButton(text), ChangeColorButton(text), null }
			};
		}

		public static Control TextAreaOptions3(TextArea text)
		{
			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items =
				{
					null,
					EnabledCheckBox(text),
					ReadOnlyCheckBox(text),
					AcceptsTabCheckBox(text),
					AcceptsReturnCheckBox(text),
					WrapCheckBox(text),
					SpellCheckCheckBox(text),
					null
				}
			};
		}

		public static Control TextAreaOptions4(TextArea text)
		{
			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items =
				{
					null,
					TextReplacementsDropDown(text),
					null
				}
				};
		}

		static Control WrapCheckBox(TextArea text)
		{
			var control = new CheckBox { Text = "Wrap" };
			control.CheckedBinding.Bind(text, t => t.Wrap);
			return control;
		}

		static Control AcceptsReturnCheckBox(TextArea text)
		{
			var control = new CheckBox { Text = "AcceptsReturn" };
			control.CheckedBinding.Bind(text, t => t.AcceptsReturn);
			return control;
		}

		static Control AcceptsTabCheckBox(TextArea text)
		{
			var control = new CheckBox { Text = "AcceptsTab" };
			control.CheckedBinding.Bind(text, t => t.AcceptsTab);
			return control;
		}

		static Control SpellCheckCheckBox(TextArea text)
		{
			var control = new CheckBox { Text = "SpellCheck", Enabled = text.SpellCheckIsSupported };
			control.CheckedBinding.Bind(text, t => t.SpellCheck);
			return control;
		}

		static Control SetAlignment(TextArea text)
		{
			var control = new EnumDropDown<TextAlignment>();
			control.SelectedValueBinding.Bind(text, t => t.TextAlignment);
			return new TableLayout
			{
				Padding = Padding.Empty,
				Spacing = new Size(5, 5),
				Rows = { new TableRow(new Label { Text = "Alignment", VerticalAlignment = VerticalAlignment.Center }, control) }
			};
		}

		static Control TextReplacementsDropDown(TextArea text)
		{
			var control = new Button { Text = "TextReplacements" };
			control.Click += (sender, e) => {
				var replacements = text.TextReplacements;
				var supported = text.SupportedTextReplacements;
				var contextMenu = new ContextMenu();
				foreach (var val in Enum.GetValues(typeof(TextReplacements)).OfType<TextReplacements>())
				{
					if (val == TextReplacements.All || val == TextReplacements.None)
					{
						var item = new ButtonMenuItem { Text = val.ToString() };
						item.Click += (sender2, e2) => text.TextReplacements = val;
						contextMenu.Items.Add(item);
						continue;
					}
					if (!supported.HasFlag(val))
						continue;
					var checkItem = new CheckMenuItem { Text = val.ToString(), Checked = replacements.HasFlag(val) };
					checkItem.CheckedChanged += (sender2, e2) => {
						var rep = text.TextReplacements;
						if (checkItem.Checked)
							rep |= val;
						else
							rep &= ~val;
						text.TextReplacements = rep;
					};
					contextMenu.Items.Add(checkItem);
				}
				contextMenu.Show(control);
			};
			return control;
		}

		static Control ShowSelectedText(TextArea text)
		{
			var control = new Button { Text = "Show selected text" };
			control.Click += (sender, e) => MessageBox.Show(text, string.Format("Selected Text: {0}", text.SelectedText));
			return control;
		}

		static Control SelectAll(TextArea text)
		{
			var control = new Button { Text = "Select All" };
			control.Click += (sender, e) =>
			{
				text.SelectAll();
				text.Focus();
			};
			return control;
		}

		static Control SetSelectedText(TextArea textArea)
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

		static Control ReplaceSelected(TextArea textArea)
		{
			var control = new Button { Text = "Replace selected text" };
			control.Click += (sender, e) =>
			{
				textArea.SelectedText = "Some inserted text!";
				textArea.Focus();
			};
			return control;
		}

		static Control SetCaretButton(TextArea textArea)
		{
			var control = new Button { Text = "Set Caret" };
			control.Click += (sender, e) =>
			{
				textArea.CaretIndex = textArea.Text.Length / 2;
				textArea.Focus();
			};
			return control;
		}

		static Control ChangeColorButton(TextArea textArea)
		{
			var control = new Button { Text = "Change Color" };
			control.Click += (sender, e) =>
			{
				textArea.BackgroundColor = Colors.Black;
				textArea.TextColor = Colors.Blue;
			};
			return control;
		}

		static Control EnabledCheckBox(TextArea textArea)
		{
			var control = new CheckBox { Text = "Enabled" };
			control.CheckedBinding.Bind(textArea, t => t.Enabled);
			return control;
		}

		static Control ReadOnlyCheckBox(TextArea textArea)
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

