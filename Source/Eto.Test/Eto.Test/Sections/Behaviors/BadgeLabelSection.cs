using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test.Sections.Behaviors
{
	public class BadgeLabelSection : Scrollable
	{
		public BadgeLabelSection()
		{
			var layout = new DynamicLayout();

			layout.AddRow(null, SetBadgeLabel(), null);
			layout.Add(null);

			Content = layout;
		}

		Control SetBadgeLabel()
		{
			var layout = new DynamicLayout();

			layout.BeginHorizontal();

			TextBox text = new TextBox();
			Button button = new Button { Text = "Set Badge Label" };
			button.Click += (sender, e) => {
				Application.Instance.BadgeLabel = text.Text;
			};
			layout.Add(new Label { Text = "Badge Label Text:", VerticalAlign = VerticalAlign.Middle });
			layout.AddCentered(text);
			layout.Add(button);
			layout.EndHorizontal();

			return layout;
		}
	}
}
