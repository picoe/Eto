using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swm = System.Windows.Media;
using swi = System.Windows.Input;

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

		public static T FindChild<T> (this sw.DependencyObject parent, string childName)
		   where T : sw.DependencyObject
		{
			// Confirm parent and childName are valid. 
			if (parent == null) return null;

			T foundChild = null;

			int childrenCount = swm.VisualTreeHelper.GetChildrenCount (parent);
			for (int i = 0; i < childrenCount; i++) {
				var child = swm.VisualTreeHelper.GetChild (parent, i);
				// If the child is not of the request child type child
				T childType = child as T;
				if (childType == null) {
					// recursively drill down the tree
					foundChild = FindChild<T> (child, childName);

					// If the child is found, break so we do not overwrite the found child. 
					if (foundChild != null) break;
				}
				else if (!string.IsNullOrEmpty (childName)) {
					var frameworkElement = child as sw.FrameworkElement;
					// If the child's name is set for search
					if (frameworkElement != null && frameworkElement.Name == childName) {
						// if the child's name is of the request name
						foundChild = (T)child;
						break;
					}
				}
				else {
					// child element found.
					foundChild = (T)child;
					break;
				}
			}

			return foundChild;
		}

		public static bool HasFocus (this sw.DependencyObject control, sw.DependencyObject focusScope, bool checkChildren = true)
		{
			var current = swi.FocusManager.GetFocusedElement (focusScope) as sw.DependencyObject;
			if (!checkChildren)
				return current == control;

			while (current != null)
			{
				if (current == control)
					return true;
				current = swm.VisualTreeHelper.GetParent (current);
			}
			return false;
		}
	}
}
