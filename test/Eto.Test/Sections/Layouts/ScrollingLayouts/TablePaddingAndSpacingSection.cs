using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.Sections.Layouts.ScrollingLayouts
{
	[Section("Scrollable", typeof(Scrollable), "Table Padding & Spacing")]
	public class TablePaddingAndSpacingSection : Scrollable
	{
		public TablePaddingAndSpacingSection()
		{
			var layout = new TableLayout
			{
				Padding = 10,
				Spacing = new Size(10, 10)
			};

			layout.Rows.Add(new Panel
			{
				Content = "You should be able to see a green label at the bottom"
			});

			for (int i = 0; i < 20; i++)
			{
				layout.Rows.Add(CreateChild());
			}

			layout.Rows.Add(new Panel
			{
				BackgroundColor = Colors.LightGreen,
				Content = "End control. Should be 10px padding below."
			});

			Content = layout;
		}

		TableLayout CreateChild()
		{
			return new TableLayout
			{
				BackgroundColor = Colors.Silver,
				Padding = 10,
				Spacing = new Size(10, 10),
				Rows =
				{
					new TableRow(),
					new TableRow("Text", new TextBox()),
					new TableRow(new TableCell(), new CheckBox { Text = "Check Box" })
				}
			};
		}
	}
}
