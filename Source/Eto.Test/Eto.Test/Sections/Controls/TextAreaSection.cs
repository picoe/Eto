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
			var layout = new DynamicLayout();

			layout.AddRow(new Label { Text = "Default" }, Default());
			layout.AddRow(new Label { Text = "Different Size" }, DifferentSize());
			layout.AddRow(new Label { Text = "Read Only" }, ReadOnly());
			layout.AddRow(new Label { Text = "Disabled" }, Disabled());
			layout.AddRow(new Label { Text = "No Wrap" }, NoWrap());
			layout.AddRow(new Label { Text = "Wrap" }, Wrap());
			layout.AddRow(new Label { Text = "Pixel Layout" }, PixelLayout());
			
			// growing space at end is blank!
			layout.Add(null);

			Content = layout;
		}

		Control Default()
		{
			var text = new TextArea { Text = "Some Text" };
			LogEvents(text);

			var layout = new DynamicLayout();

			layout.Add(text);
			layout.BeginVertical(Padding.Empty, Size.Empty);
			layout.AddRow(null, ShowSelectedText(text), SetSelectedText(text), ReplaceSelected(text), SelectAll(text), SetCaret(text),ChangeColor(text),Disable(text), null);
			layout.EndVertical();

			return layout;
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
			control.Click += (sender, e) => {
				text.SelectAll();
				text.Focus();
			};
			return control;
		}

		Control SetSelectedText(TextArea textArea)
		{
			var control = new Button { Text = "Set selected text" };
			control.Click += (sender, e) => {
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
			control.Click += (sender, e) => {
				textArea.SelectedText = "Some inserted text!";
				textArea.Focus();
			};
			return control;
		}

		Control SetCaret(TextArea textArea)
		{
			var control = new Button { Text = "Set Caret" };
			control.Click += (sender, e) => {
				textArea.CaretIndex = textArea.Text.Length / 2;
				textArea.Focus();
			};
			return control;
		}

		Control ChangeColor(TextArea textArea)
		{
			var control = new Button { Text = "Change Color" };
			control.Click += (sender, e) =>
			{
				textArea.BackgroundColor = Colors.Black;
				textArea.TextColor = Colors.Blue;
			};
			return control;
		}

		Control Disable(TextArea textArea)
		{
			var control = new CheckBox {Text = "Enable/Disable", Checked = true};
			//control.Bindings.Add("Checked",)
			textArea.Bind<bool>("Enabled", control, "Checked",DualBindingMode.OneWay);
			return control;
		}

		Control DifferentSize()
		{
			var control = new TextArea { Text = "Some Text", Size = new Size (100, 20) };
			LogEvents(control);
			return control;
		}

		Control ReadOnly()
		{
			var control = new TextArea { Text = "Read only text", ReadOnly = true };
			LogEvents(control);
			return control;
		}

		Control Disabled()
		{
			var control = new TextArea { Text = "Disabled text that you cannot select", Enabled = false };
			return control;
		}

		Control Wrap()
		{
			var control = new TextArea
			{
				Text = "Some very long text that should wrap. Some very long text that should wrap. Some very long text that should wrap. Some very long text that should wrap." + System.Environment.NewLine + "Second Line",
				Wrap = true
			};
			LogEvents(control);
			return control;
		}

		Control NoWrap()
		{
			var control = new TextArea
			{
				Text = "Some very long text that should not wrap. Some very long text that should not wrap. Some very long text that should not wrap. Some very long text that should not wrap." + System.Environment.NewLine + "Second Line",
				Wrap = false
			};
			LogEvents(control);
			return control;
		}

		Control PixelLayout()
		{
			var control = new PixelLayout();
			control.Add(new TextArea
			{
				Text = "Some text that is contained in a pixel layout."
			}, Point.Empty);
			return control;
		}

		void LogEvents(TextArea control)
		{
			control.TextChanged += delegate
			{
				Log.Write(control, "TextChanged, Text: {0}", control.Text);
			};

			control.SelectionChanged += (sender, e) => {
				Log.Write(control, "SelectionChanged, Selection: {0}", control.Selection);
			};

			control.CaretIndexChanged += (sender, e) => {
				Log.Write(control, "CaretIndexChanged, CaretIndex: {0}", control.CaretIndex);
			};
		}
	}
}

