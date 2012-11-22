using System;
using Eto.Forms;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms.Cells
{
	public class TextBoxCellHandler : CellHandler<object, TextBoxCell>, ITextBoxCell
	{
		public TextBoxCellHandler ()
		{
		}

		public override void Configure (object dataItem, UITableViewCell cell)
		{
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

