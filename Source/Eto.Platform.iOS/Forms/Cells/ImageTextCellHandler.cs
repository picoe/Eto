using System;
using Eto.Forms;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<object, ImageTextCell>, IImageTextCell
	{
		public ImageTextCellHandler ()
		{
		}

		public override void Configure (object dataItem, UITableViewCell cell)
		{
			if (Widget.TextBinding != null) {
				var val = Widget.TextBinding.GetValue (dataItem);
				cell.TextLabel.Text = Convert.ToString (val);
			}
		}

		public override string TitleForSection (object dataItem)
		{
			if (Widget.TextBinding != null) {
				var val = Widget.TextBinding.GetValue (dataItem);
				return Convert.ToString (val);
			}
			return null;
		}
	}
}

