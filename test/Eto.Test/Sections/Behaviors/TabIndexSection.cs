using System;
using Eto.Forms;
namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "TabIndex")]
	public class TabIndexSection : Panel
	{
		public TabIndexSection()
		{
			var stack = new StackLayout { TabIndex = 2 };

			stack.Items.Add(new TextBox { Text = "3", TabIndex = 3 });
			stack.Items.Add(new TextBox { Text = "2", TabIndex = 2 });
			stack.Items.Add(new TextBox { Text = "4", TabIndex = 4 });
			stack.Items.Add(new TextBox { Text = "5", TabIndex = 5 });
			stack.Items.Add(new TextBox { Text = "1", TabIndex = 1 });


			var table = new TableLayout { TabIndex = 1 };
			table.Rows.Add(new TextBox { Text = "3", TabIndex = 3 });
			table.Rows.Add(new TextBox { Text = "2", TabIndex = 2 });
			table.Rows.Add(new TextBox { Text = "4", TabIndex = 4 });
			table.Rows.Add(new TextBox { Text = "5", TabIndex = 5 });
			table.Rows.Add(new TextBox { Text = "1", TabIndex = 1 });

			var pixel = new PixelLayout();
			pixel.Add(new TextBox { Text = "3", TabIndex = 3 }, 0, 0);
			pixel.Add(new TextBox { Text = "2", TabIndex = 2 }, 25, 25);
			pixel.Add(new TextBox { Text = "4", TabIndex = 4 }, 50, 50);
			pixel.Add(new TextBox { Text = "5", TabIndex = 5 }, 75, 75);
			pixel.Add(new TextBox { Text = "1", TabIndex = 1 }, 100, 100);

			var dynamic = new DynamicLayout { TabIndex = 3 };
			dynamic.BeginCentered();
			dynamic.Add(new TextBox { Text = "3", TabIndex = 3 });
			dynamic.EndCentered();
			dynamic.BeginVertical();
			dynamic.Add(new TextBox { Text = "2", TabIndex = 2 });
			dynamic.Add(new TextBox { Text = "4", TabIndex = 4 });
			dynamic.EndVertical();
			dynamic.BeginVertical();
			dynamic.BeginHorizontal();
			dynamic.Add(new TextBox { Text = "5", TabIndex = 5 });
			dynamic.Add(new TextBox { Text = "1", TabIndex = 1 });
			dynamic.EndHorizontal();
			dynamic.EndVertical();

			Content = new TableLayout(
				new TableRow("StackLayout-2", stack),
				new TableRow("TableLayout-1", table),
				new TableRow("PixelLayout", pixel),
				new TableRow("DynamicLayout-3", dynamic),
				null);
		}
	}
}
