using System;
using Eto.Forms;
using UIKit;
using NSCell = UIKit.UITableViewCell;
using Eto.Drawing;

namespace Eto.iOS.Forms.Cells
{
	public class ImageViewCellHandler : CellHandler<NSCell, ImageViewCell>, ImageViewCell.IHandler
	{
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

		// TODO
		public ImageInterpolation ImageInterpolation { get; set; }
	}
}