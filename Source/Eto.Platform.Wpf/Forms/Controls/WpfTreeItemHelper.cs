using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using Eto.Forms;
using System.Collections;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public static class WpfTreeItemHelper
	{
		public static IEnumerable GetChildren (ITreeGridStore<ITreeGridItem> item)
		{
			return item as IEnumerable ?? new DataStoreVirtualCollection<ITreeGridItem>(item);
		}

		public class ChildrenConverter : swd.IValueConverter
		{
			public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var item = value as ITreeGridItem;
				return GetChildren ((ITreeGridStore<ITreeGridItem>)item);
			}

			public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				throw new NotImplementedException ();
			}
		}

		public class IsExpandedConverter : swd.IValueConverter
		{
			public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var item = value as ITreeGridItem;
				return item.Expanded;
			}

			public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return value;
			}
		}
	}
}
