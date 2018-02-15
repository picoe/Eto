using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", typeof(Cursor))]
	public class CursorSection : Panel
	{
		public CursorSection()
		{
			var layout = new TableLayout();
			layout.Spacing = new Size(20, 20);

			TableRow row;

			layout.Rows.Add(row = new TableRow());

			foreach (var type in Enum.GetValues(typeof(CursorType)).OfType<CursorType?>())
			{
				var label = new Label
				{ 
					Size = new Size(100, 50), 
					Text = type.ToString(),
					VerticalAlignment = VerticalAlignment.Center,
					TextAlignment = TextAlignment.Center,
					BackgroundColor = Colors.Silver
				};
				if (type == null)
					label.Cursor = null;
				else
					label.Cursor = new Cursor(type.Value);
				row.Cells.Add(label);

				if (row.Cells.Count > 3)
					layout.Rows.Add(row = new TableRow());
			}

			Content = TableLayout.AutoSized(layout, centered: true);

		}
	}
}

