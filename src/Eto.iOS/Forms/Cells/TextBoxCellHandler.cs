using System;
using Eto.Forms;
using UIKit;
using NSCell = UIKit.UITableViewCell;

namespace Eto.iOS.Forms.Cells
{
	public class TextBoxCellHandler : CellHandler<NSCell, TextBoxCell>, TextBoxCell.IHandler
	{
		public TextBoxCellHandler ()
		{
		}

		public TextAlignment TextAlignment { get; set; }

		public VerticalAlignment VerticalAlignment { get; set; }

		public override void Configure (object dataItem, NSCell cell)
		{
			cell.TextLabel.TextAlignment = TextAlignment.ToUI();
			if (Widget.Binding != null) {
				var val = Widget.Binding.GetValue (dataItem);
				cell.TextLabel.Text = Convert.ToString (val);
			}
		}
		
		public override string TitleForSection (object dataItem)
		{
			if (Widget.Binding != null) {
				var val = Widget.Binding.GetValue (dataItem);
				return Convert.ToString (val);
			}
			return null;
		}
	}
}

