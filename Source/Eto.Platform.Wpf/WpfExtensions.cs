using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using swc = System.Windows.Controls;
using Eto.Platform.Wpf.Forms;
using Eto.Forms;

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

		public static T FindChild<T> (this sw.DependencyObject parent, string childName = null)
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

		public static void RemoveFromParent(this Control control)
		{
			if (control.Parent == null)
				return;
			var parent = control.Parent.Handler as IWpfContainer;
			if (parent != null)
				parent.Remove(control.GetContainerControl());
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

		public static void EnsureLoaded (this sw.FrameworkElement control)
		{
			ApplicationHandler.InvokeIfNecessary (() => {
				if (!control.IsLoaded)
					control.Dispatcher.Invoke (new Action (() => { }), sw.Threading.DispatcherPriority.ContextIdle, null);
			});
		}

		public static double Horizontal (this sw.Thickness thickness)
		{
			return thickness.Left + thickness.Right;
		}

		public static double Vertical (this sw.Thickness thickness)
		{
			return thickness.Top + thickness.Bottom;
		}

		public static sw.Size Size (this sw.Thickness thickness)
		{
			return new sw.Size (thickness.Horizontal (), thickness.Vertical ());
		}
	}
}
