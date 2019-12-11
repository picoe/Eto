using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Shared
{
	public class MutableCellEventArgs : CellEventArgs
	{
		public MutableCellEventArgs(Grid grid, Cell cell, int row, object item, CellStates cellState)
			: base(grid, cell, row, item, cellState)
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
	}
	
}
