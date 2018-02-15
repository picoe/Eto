using System;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swm = Windows.UI.Xaml.Media;
using swi = Windows.UI.Xaml.Input;
using swc = Windows.UI.Xaml.Controls;
using Eto.WinRT.Forms;
using Eto.Forms;

namespace Eto.WinRT
{
	static class WpfExtensions
	{
		public static T GetParent<T> (this sw.DependencyObject control)
			where T : sw.DependencyObject
		{
			var tmp = swm.VisualTreeHelper.GetParent (control);
			while (tmp != null) {
				tmp = swm.VisualTreeHelper.GetParent (tmp);
				var ttmp = tmp as T;
				if (ttmp != null) return ttmp;
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
				var childType = child as T;
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
			if (control.VisualParent == null)
				return;
			var parent = control.VisualParent.Handler as IWpfContainer;
			if (parent != null)
				parent.Remove(control.GetContainerControl());
		}

		public static bool HasFocus (this sw.DependencyObject control, sw.DependencyObject focusScope, bool checkChildren = true)
		{
			var current = swi.FocusManager.GetFocusedElement (
#if TODO_XAML
				focusScope
#endif
				) as sw.DependencyObject;
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
			ApplicationHandler.InvokeIfNecessary(() =>
			{
#if TODO_XAML
				if (!control.IsLoaded)
				{
					control.Dispatcher.Invoke(new Action(() => { }), sw.Threading.DispatcherPriority.ContextIdle, null);
				}
#else
				throw new NotImplementedException();
#endif
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

		public static wf.Size Size(this sw.Thickness thickness)
		{
			return new wf.Size(thickness.Horizontal(), thickness.Vertical());
		}

		public static wf.Size Add(this wf.Size size1, wf.Size size2)
		{
			return new wf.Size(size1.Width + size2.Width, size1.Height + size2.Height);
		}

		public static wf.Size Subtract(this wf.Size size1, wf.Size size2)
		{
			return new wf.Size(Math.Max(0, size1.Width - size2.Width), Math.Max(0, size1.Height - size2.Height));
		}
	}
}
