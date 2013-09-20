using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.DynamicLayoutSection
{
	public class RowsSection : DynamicLayout
	{
		public RowsSection()
		{
			var table = new GridView();
			table.Columns.Add(new GridColumn { AutoSize = false, Width = 1000, HeaderText = "Title", DataCell = new TextBoxCell("FormattedTitle"), Editable = false, Sortable = true });

			this.AddRow(new TextBox { Text = "This is a textbox" });
			this.AddRow(new Label { Text = "This is a label" });
			this.AddRow(table);
		}
	}
}
