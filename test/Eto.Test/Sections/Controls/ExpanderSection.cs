using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Expander))]
	public class ExpanderSection : Panel
	{
		public ExpanderSection()
		{
			var expandedCheckBox = new CheckBox { Text = "Expanded" };
			var enabledCheckBox = new CheckBox { Text = "Enabled" };

			var expander = new Expander
			{
				Header = "Test Header",
				Content = new Panel {  Size = new Size(200, 200), BackgroundColor = Colors.Blue }
			};

			expandedCheckBox.CheckedBinding.Bind(expander, e => e.Expanded);
			enabledCheckBox.CheckedBinding.Bind(expander, e => e.Enabled);

			LogEvents(expander);

			var expander2 = new Expander
			{
				Header = new StackLayout
				{ 
					Orientation = Orientation.Horizontal, 
					Items = { "Test Expanded with custom header", new TextBox() }
				},
				Expanded = true,
				Content = new Panel { Size = new Size(300, 200), BackgroundColor = Colors.Blue }
			};

			LogEvents(expander2);

			Content = new StackLayout
			{
				Padding = new Padding(10),
				Items =
				{
					new StackLayout {
						Orientation = Orientation.Horizontal,
						Items = {
							expandedCheckBox,
							enabledCheckBox
						}
					},
					expander,
					expander2
				}
			};
		}

		void LogEvents(Expander button)
		{
			button.ExpandedChanged += (sender, e) => Log.Write(button, "ExpandedChanged: {0}", button.Expanded);
		}
	}
}

