using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Restart")]
	public class RestartSection : DynamicLayout
	{
		public RestartSection()
		{
			var restartButton = new Button { Text = "Restart Application" };
			restartButton.Click += (sender, e) =>
			{
				Application.Instance.Restart();
			};

			AddCentered(restartButton, verticalCenter: true);
		}
	}
}
