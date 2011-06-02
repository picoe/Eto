using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	internal class TableLayoutHandler : WebLayout, ITableLayout
	{
		
		public void Add(Control child, int x, int y)
		{
			while (table.Rows.Count <= y) table.Rows.Add(new SWUC.TableRow());
			SWUC.TableRow row = table.Rows[y];
			while (row.Cells.Count <= x) row.Cells.Add(new SWUC.TableCell());
			SWUC.TableCell cell = row.Cells[x];
			cell.Controls.Add((SWU.Control)child.ControlObject);
		}
		
		public void Move(Control child, int x, int y)
		{
			// TODO: Implement this method
		}
		
		public void CreateControl(int cols, int rows)
		{
			// TODO: Implement this method
		}
		
		public void SetColumnScale(int column, bool scale)
		{
			// TODO: Implement this method
		}
		
		public void SetRowScale(int row, bool scale)
		{
			// TODO: Implement this method
		}
		
		SWUC.Table table;
		
		public TableLayoutHandler(Widget widget, int cols, int rows) : base(widget)
		{
			table = new SWUC.Table();
			//table.Style["WIDTH"] = "100%";
			//table.Style["HEIGHT"] = "100%";
			
		}
		
		public override object ControlObject
		{
			get
			{ return table; }
		}
		
		
		#region IPositionalLayout Members
		
		public void SetLocation(Control child, int x, int y)
		{
		}
		#endregion
		
	}
}

