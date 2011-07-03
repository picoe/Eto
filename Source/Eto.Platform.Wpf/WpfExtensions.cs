using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Platform.Wpf
{
	static class WpfExtensions
	{
		public static T GetParent<T> (this System.Windows.DependencyObject control)
			where T : System.Windows.DependencyObject
		{
			var tmp = System.Windows.Media.VisualTreeHelper.GetParent (control);
			while (tmp != null) {
				tmp = System.Windows.Media.VisualTreeHelper.GetParent (tmp);
				if (tmp is T) return (T)tmp;
			}
			return null;
		}

	}
}
