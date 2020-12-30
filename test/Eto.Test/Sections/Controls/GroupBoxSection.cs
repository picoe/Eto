using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(GroupBox))]
	public class GroupBoxSection : Panel
	{
		public GroupBoxSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Default" }, Default(), null);

			layout.AddRow(new Label { Text = "With Header" }, Header(), null);

			layout.Add(null, null, true);

			Content = layout;
		}

		GroupBox Default()
		{
			var control = new GroupBox();

			control.Content = new Panel { Size = new Size(100, 100), BackgroundColor = Colors.Blue, Content = "Content" };
			return control;

		}

		Control Header()
		{
			var control = new GroupBox { Text = "Some Header" };

			control.Content = new Panel { Size = new Size(100, 100), Content = "Content" };
			return control;
		}
	}
}

