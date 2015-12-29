using Eto.Forms;
using Eto.Drawing;
using System;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(GridView), "GridView Formatting")]
	public class GridCellFormattingSection : GridViewSection
	{

		protected override void LogEvents(GridView control)
		{
			control.RowHeight = 36;
			var font = Fonts.Serif(18, FontStyle.Italic);
			control.CellFormatting += (sender, e) =>
			{
				// Log.Write (control, "Formatting Row: {1}, Column: {2}, Item: {0}", e.Item, e.Row, control.Columns.IndexOf (e.Column));
				e.Font = font;
				e.BackgroundColor = e.Row % 2 == 0 ? Colors.Blue : Colors.LightBlue;
				e.ForegroundColor = e.Row % 2 == 0 ? Colors.Lime : Colors.Yellow;
			};
		}

	}
}

