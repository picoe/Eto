using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Shared
{
	public class MutableCellEventArgs : CellEventArgs
	{
		public MutableCellEventArgs(Grid grid, Cell cell, int row, int column, object item, CellStates cellState, Control control)
			: base(grid, cell, row, column, item, cellState, control)
		{
		}

		public void SetSelected(bool selected)
		{
			IsSelected = selected;
		}

		public void SetEditable(bool editing)
		{
			IsEditing = editing;
		}

		public void SetTextColor(Color color)
		{
			CellTextColor = color;
		}

		public void SetItem(object item)
		{
			Item = item;
		}

		public void SetControl(Control control)
		{
			Control = control;
		}
	}
	
}
