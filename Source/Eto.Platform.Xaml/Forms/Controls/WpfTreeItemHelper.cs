#if TODO_XAML
using System;
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;
using swd = Windows.UI.Xaml.Data;
using Eto.Forms;
using System.Collections;

namespace Eto.Platform.Xaml.Forms.Controls
{
	public static class WpfTreeItemHelper
	{
		public static IEnumerable GetChildren (ITreeStore item)
		{
			return item as IEnumerable ?? new DataStoreVirtualCollection<ITreeItem> (item);
		}

		public class ChildrenConverter : swd.IValueConverter
		{
			public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var item = value as ITreeItem;
				return GetChildren(item);
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
				var item = (ITreeItem)value;
				return item.Expanded;
			}

			public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return value;
			}
		}
	}
}
#endif