using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Badge Label")]
	public class BadgeLabelSection : Scrollable
	{
		public BadgeLabelSection()
		{
			var layout = new DynamicLayout();

			layout.AddRow(null, SetBadgeLabel(), null);
			layout.Add(null);

			Content = layout;
		}

		static Control SetBadgeLabel()
		{
			var layout = new DynamicLayout();

			layout.BeginHorizontal();

			var text = new TextBox();
			var button = new Button { Text = "Set Badge Label" };
			button.Click += (sender, e) => Application.Instance.BadgeLabel = text.Text;
			layout.Add(new Label { Text = "Badge Label Text:", VerticalAlign = VerticalAlign.Middle });
			layout.AddCentered(text);
			layout.Add(button);
			layout.EndHorizontal();

			return layout;
		}
	}
}
