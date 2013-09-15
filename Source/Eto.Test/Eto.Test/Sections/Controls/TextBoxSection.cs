using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class TextBoxSection : Scrollable
	{
		public TextBoxSection()
		{
			var layout = new DynamicLayout();

			layout.AddRow(new Label { Text = "Default" }, Default());
			layout.AddRow(new Label { Text = "Different Size" }, DifferentSize());
			layout.AddRow(new Label { Text = "Read Only" }, ReadOnly());
			layout.AddRow(new Label { Text = "Disabled" }, Disabled());
			layout.AddRow(new Label { Text = "Placeholder" }, Placeholder());
			layout.AddRow(new Label { Text = "Limit Length" }, LimitLength());

			// growing space at end is blank!
			layout.Add(null);

			Content = layout;
		}

		Control Default()
		{
			var control = new TextBox { Text = "Some Text" };
			LogEvents(control);

			var selectAll = new Button { Text = "Select All" };
			selectAll.Click += (object sender, EventArgs e) => {
				control.SelectAll();
			};

			var layout = new DynamicLayout(Padding.Empty);
			layout.Add(control);
			layout.AddSeparateRow(null, selectAll, null);
			return layout;
		}

		Control DifferentSize()
		{
			var control = new TextBox { Text = "Some Text", Size = new Size (100, 50) };
			LogEvents(control);
			return control;
		}

		Control ReadOnly()
		{
			var control = new TextBox { Text = "Read only text", ReadOnly = true };
			LogEvents(control);
			return control;
		}

		Control Disabled()
		{
			var control = new TextBox { Text = "Disabled Text", Enabled = false };
			LogEvents(control);
			return control;
		}

		Control LimitLength()
		{
			var control = new TextBox { Text = "Limited to 30 characters", MaxLength = 30 };
			LogEvents(control);
			return control;
		}

		Control Placeholder()
		{
			var control = new TextBox { PlaceholderText = "Some Placeholder" };
			LogEvents(control);
			return control;
		}

		void LogEvents(TextBox control)
		{
			control.TextChanged += delegate
			{
				Log.Write(control, "TextChanged, Text: {0}", control.Text);
			};
		}
	}
}

