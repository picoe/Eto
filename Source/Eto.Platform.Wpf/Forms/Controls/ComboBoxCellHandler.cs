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
		class Converter : swd.IValueConverter
		{
			public CheckBoxCellHandler Handler { get; set; }

			public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var item = value as IGridItem;
				if (item == null) return null;
				return item.GetValue (Handler.Control.DisplayIndex);
			}

			public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return value;
			}
		}

		public CheckBoxCellHandler ()
		{
			Control = new swc.DataGridCheckBoxColumn { 
			};
		}

		public override void Bind (int column)
		{
			Control.Binding = new swd.Binding {
				Mode = swd.BindingMode.TwoWay,
				TargetNullValue = false,
				Path = new sw.PropertyPath (string.Format (".[{0}]", column))
			};
		}
	}
}