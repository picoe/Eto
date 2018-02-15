using System;
using Eto.Forms;
using UIKit;
using NSCell = UIKit.UITableViewCell;

namespace Eto.iOS.Forms.Cells
{
	public class CheckBoxCellHandler : CellHandler<NSCell, CheckBoxCell>, CheckBoxCell.IHandler
	{
		public CheckBoxCellHandler()
		{
		}

		public override void Configure(object dataItem, NSCell cell)
		{
			if (Widget.Binding != null)
			{
				var val = Widget.Binding.GetValue(dataItem);
				cell.TextLabel.Text = Convert.ToString(val);
			}
		}

		public override string TitleForSection(object dataItem)
		{
			if (Widget.Binding != null)
			{
				var val = Widget.Binding.GetValue(dataItem);
				return Convert.ToString(val);
			}
			return null;
		}
	}
}

