using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class CheckBoxCellHandler : CellHandler<swc.DataGridCheckBoxColumn, CheckBoxCell>, ICheckBoxCell
	{
		bool? GetValue (object context)
		{
			var item = context as IGridItem;
			if (item != null) {
				var val = item.GetValue (DataColumn);
				if (val != null) {
					return Convert.ToBoolean (val);
				}
			}
			return null;
		}

		class Column : swc.DataGridCheckBoxColumn
		{
			public CheckBoxCellHandler Handler { get; set; }


			protected override sw.FrameworkElement GenerateElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement (cell, dataItem);
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.CheckBox;
					control.IsChecked = Handler.GetValue (control.DataContext);
				};
				return element;
			}

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateEditingElement (cell, dataItem);
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.CheckBox;
					control.IsChecked = Handler.GetValue (control.DataContext);
				};
				return element;
			}

			protected override bool CommitCellEdit (sw.FrameworkElement editingElement)
			{
				var text = editingElement as swc.CheckBox;
				var item = text.DataContext as IGridItem;
				if (item != null)
					item.SetValue (Handler.DataColumn, text.IsChecked);
				return true;
			}

		}

		public CheckBoxCellHandler ()
		{
			Control = new Column { Handler = this };
		}
	}
}