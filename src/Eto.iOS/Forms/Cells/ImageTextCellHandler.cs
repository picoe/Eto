using System;
using Eto.Forms;
using UIKit;
using NSCell = UIKit.UITableViewCell;
using Eto.Drawing;

namespace Eto.iOS.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<NSCell, ImageTextCell>, ImageTextCell.IHandler
	{
		public override void Configure(object dataItem, NSCell cell)
		{
			cell.TextLabel.TextAlignment = TextAlignment.ToUI();
			if (Widget.TextBinding != null)
			{
				var val = Widget.TextBinding.GetValue(dataItem);
				cell.TextLabel.Text = Convert.ToString(val);
			}
		}

		public override string TitleForSection(object dataItem)
		{
			if (Widget.TextBinding != null)
			{
				var val = Widget.TextBinding.GetValue(dataItem);
				return Convert.ToString(val);
			}
			return null;
		}

		// TODO
		public ImageInterpolation ImageInterpolation { get; set; }

		public TextAlignment TextAlignment { get; set; }

		public VerticalAlignment VerticalAlignment { get; set; }
	}
}