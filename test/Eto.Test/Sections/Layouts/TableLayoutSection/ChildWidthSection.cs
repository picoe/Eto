using Eto.Forms;

namespace Eto.Test.Sections.Layouts.TableLayoutSection
{
	/// <summary>
	/// This tests a TableLayout that contains a control with a width larger than its container
	/// </summary>
	[Section("TableLayout", "Child Width")]
	public class ChildWidthSection : TableLayout
	{
		public ChildWidthSection()
			: base(1, 3)
		{
			var table = new GridView();
			table.Columns.Add(new GridColumn { AutoSize = false, Width = 1000, HeaderText = "Title", DataCell = new TextBoxCell("FormattedTitle"), Editable = false, Sortable = true });

			this.Add(new TextBox { Text = "This is a textbox" }, 0, 0);
			this.Add(new Label { Text = "This is a label" }, 0, 1);
			this.Add(table, 0, 2);
		}
	}
}
