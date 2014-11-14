using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(GroupBox))]
	public class GroupBoxSection : Panel
	{
		public GroupBoxSection()
		{
			var layout = new DynamicLayout();
			
			
			layout.AddRow(new Label { Text = "Default" }, Default());
			
			layout.AddRow(new Label { Text = "With Header" }, Header());
			
			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new GroupBox();

			control.Content = new CheckBoxSection { Border = BorderType.None };
			return control;
			
		}

		Control Header()
		{
			var control = new GroupBox { Text = "Some Header" };
			
			control.Content = new LabelSection();
			return control;
		}
	}
}

