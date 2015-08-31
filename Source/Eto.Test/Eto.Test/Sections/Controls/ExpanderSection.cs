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
			var expander = new Expander
			{
				Header = "Hello",
				//Size = new Size(200, 200),
				Content = new Panel {  Size = new Size(200, 200), BackgroundColor = Colors.Blue }
			};

			LogEvents(expander);

			//Content = expander;
			//return;
			Content = new StackLayout
			{
				Items =
				{
					expander
				}
			};
		}

		void LogEvents(Expander button)
		{
			button.ExpandedChanged += (sender, e) => Log.Write(button, "ExpandedChanged: {0}", button.Expanded);
		}
	}
}

