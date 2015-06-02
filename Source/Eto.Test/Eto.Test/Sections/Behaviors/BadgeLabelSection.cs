using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Badge Label")]
	public class BadgeLabelSection : Scrollable
	{
		public BadgeLabelSection()
		{
			var layout = new DynamicLayout { Spacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(null, SetBadgeLabel(), null);
			layout.Add(null);

			Content = layout;
		}

		static Control SetBadgeLabel()
		{
			var layout = new DynamicLayout { Spacing = new Size(5, 5) };

			layout.BeginHorizontal();

			var text = new TextBox();
			var button = new Button { Text = "Set Badge Label" };
			button.Click += (sender, e) => Application.Instance.BadgeLabel = text.Text;
			layout.Add(new Label { Text = "Badge Label Text:", VerticalAlignment = VerticalAlignment.Center });
			layout.AddCentered(text);
			layout.Add(button);
			layout.EndHorizontal();

			return layout;
		}
	}
}
