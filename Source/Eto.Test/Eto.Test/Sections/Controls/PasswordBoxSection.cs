using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class PasswordBoxSection : Panel
	{
		public PasswordBoxSection()
		{
			var layout = new DynamicLayout();

			layout.AddRow(new Label { Text = "Default" }, Default());
			layout.AddRow(new Label { Text = "Read Only" }, ReadOnly());
			layout.AddRow(new Label { Text = "Disabled" }, Disabled());

			// growing space at end is blank!
			layout.Add(null);

			Content = layout;
		}

		Control Default()
		{
			var control = new PasswordBox { Text = "Some Text" };
			LogEvents(control);
			return control;
		}

		Control ReadOnly()
		{
			var control = new PasswordBox { Text = "Read only text", ReadOnly = true };
			LogEvents(control);
			return control;
		}

		Control Disabled()
		{
			var control = Default();
			control.Enabled = false;
			return control;
		}

		void LogEvents(PasswordBox control)
		{
			control.TextChanged += delegate
			{
				Log.Write(control, "TextChanged, Text: {0}", control.Text);
			};
		}
	}
}

