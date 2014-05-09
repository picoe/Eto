using System;
using Eto.Forms;
using MonoTouch.UIKit;
using NSCell = MonoTouch.UIKit.UITableViewCell;

namespace Eto.iOS.Forms.Cells
{
	public class ComboBoxCellHandler : CellHandler<NSCell, ComboBoxCell>, ComboBoxCell.IHandler
	{
		public ComboBoxCellHandler()
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

		IListStore dataStore;
		public IListStore DataStore
		{
			get { return dataStore; }
			set { dataStore = value; }
		}
	}
}

