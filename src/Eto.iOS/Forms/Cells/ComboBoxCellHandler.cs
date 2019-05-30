using System;
using Eto.Forms;
using UIKit;
using NSCell = UIKit.UITableViewCell;
using System.Collections.Generic;

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

		IEnumerable<object> dataStore;
		public IEnumerable<object> DataStore
		{
			get { return dataStore; }
			set { dataStore = value; }
		}
	}
}

