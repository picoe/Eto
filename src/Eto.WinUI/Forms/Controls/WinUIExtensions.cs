using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Eto.WinUI.Forms.Controls;

static class WinUIExtensions
{
	public static T FindVisualParent<T>(this DependencyObject child) where T : class
	{

		while (child != null)
		{
			if (child is T parent)
				return parent;
			child = VisualTreeHelper.GetParent(child);
		}
		return null;
	}

	public static mux.DependencyObject FindTopVisualParent(this DependencyObject child)
	{
		while (child != null)
		{
			var next = VisualTreeHelper.GetParent(child);
			if (next == null)
				return child;
			child = next;
		}
		return null;
	}

	public static T FindVisualChild<T>(this DependencyObject element) where T : class
	{
		var count = VisualTreeHelper.GetChildrenCount(element);
		for (int i = 0; i < count; i++)
		{
			var child = VisualTreeHelper.GetChild(element, i);
			if (child is T found)
				return found;
			found = child.FindVisualChild<T>();
			if (found != null)
				return found;
		}
		return null;
	}
}
