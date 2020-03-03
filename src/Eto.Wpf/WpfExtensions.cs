using System;
using sw = System.Windows;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using swc = System.Windows.Controls;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Wpf.Forms;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Eto.Wpf
{
	static class WpfExtensions
	{
		public static T GetVisualParent<T>(this sw.DependencyObject control)
			where T : class
		{
			var ce = control as sw.ContentElement;
			while (ce != null)
			{
				control = sw.ContentOperations.GetParent(ce);
				ce = control as sw.ContentElement;
			}

			while (control is swm.Visual || control is swm.Media3D.Visual3D)
			{
				control = swm.VisualTreeHelper.GetParent(control);
				var tmp = control as T;
				if (tmp != null)
					return tmp;
			}
			return null;
		}

		public static IEnumerable<sw.DependencyObject> GetVisualParents(this sw.DependencyObject control)
		{
			while (control != null)
			{
				yield return control;

				control = control.GetVisualParent<sw.DependencyObject>();
			}
		}

		public static T GetParent<T>(this sw.FrameworkElement control)
			where T : class
		{
			while (control != null)
			{
				var tmp = control.Parent as T;
				if (tmp != null)
					return tmp;
				control = control.Parent as sw.FrameworkElement;
			}
			return null;
		}

		public static IEnumerable<sw.DependencyObject> GetParents(this sw.FrameworkElement control)
		{
			while (control != null)
			{
				yield return control;

				control = control.Parent as sw.FrameworkElement;
			}
		}

		public static T FindChild<T>(this sw.DependencyObject parent, string childName = null)
			 where T : sw.DependencyObject
		{
			return FindVisualChildren<T>(parent, childName).FirstOrDefault();
		}

		public static IEnumerable<T> FindVisualChildren<T>(this sw.DependencyObject parent, string childName = null)
			 where T : sw.DependencyObject
		{
			// Confirm parent and childName are valid. 
			if (parent == null) yield break;

			int childrenCount = swm.VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++)
			{
				var child = swm.VisualTreeHelper.GetChild(parent, i);
				// If the child is not of the request child type child
				var childType = child as T;
				if (childType == null)
				{
					// recursively drill down the tree
					foreach (var c in FindVisualChildren<T>(child, childName))
					{
						yield return c;
					}
				}
				else if (childName != null)
				{
					var frameworkElement = child as sw.FrameworkElement;
					// If the child's name is set for search
					if (frameworkElement != null && frameworkElement.Name == childName)
					{
						// if the child's name is of the request name
						yield return childType;
					}
					else
					{
						foreach (var c in FindVisualChildren<T>(child, childName))
						{
							yield return c;
						}
					}
				}
				else
				{
					// child element found.
					yield return childType;
				}
			}
		}

		public static sw.Window GetParentWindow(this sw.FrameworkElement element)
		{
			var window = element.GetParent<sw.Window>();
			if (window == null)
			{
				var app = sw.Application.Current;
				window = app.MainWindow ?? (app.Windows.Count > 0 ? app.Windows[0] : new sw.Window());
			}
			return window;
		}

		public static void RemoveFromParent(this Control control)
		{
			if (control.VisualParent == null)
				return;
			var parent = control.VisualParent.Handler as IWpfContainer;
			if (parent != null)
				parent.Remove(control.GetContainerControl());
		}

		public static bool HasFocus(this sw.DependencyObject control, sw.DependencyObject focusScope, bool checkChildren = true)
		{
			var current = swi.FocusManager.GetFocusedElement(focusScope) as sw.DependencyObject;
			if (!checkChildren)
				return current == control;

			// check content elements
			var ce = current as sw.ContentElement;
			while (ce != null)
			{
				current = sw.ContentOperations.GetParent(ce);
				if (current == control)
					return true;

				ce = control as sw.ContentElement;
			}

			// check visual elements
			while (current is swm.Visual || current is swm.Media3D.Visual3D)
			{
				if (current == control)
					return true;

				current = swm.VisualTreeHelper.GetParent(current);
			}
			return false;
		}

		public static void EnsureLoaded(this sw.FrameworkElement control)
		{
			ApplicationHandler.InvokeIfNecessary(() =>
			{
				if (!control.IsLoaded)
				{
					control.Dispatcher.Invoke(new Action(() => { }), sw.Threading.DispatcherPriority.ApplicationIdle, null);
				}
			});
		}

		public static double Horizontal(this sw.Thickness thickness)
		{
			return thickness.Left + thickness.Right;
		}

		public static double Vertical(this sw.Thickness thickness)
		{
			return thickness.Top + thickness.Bottom;
		}

		public static sw.Size Size(this sw.Thickness thickness)
		{
			return new sw.Size(thickness.Horizontal(), thickness.Vertical());
		}

        public static sw.Size Max(this sw.Size size1, sw.Size size2)
        {
            return new sw.Size(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height));
        }

        public static sw.Size Min(this sw.Size size1, sw.Size size2)
        {
            return new sw.Size(Math.Min(size1.Width, size2.Width), Math.Min(size1.Height, size2.Height));
        }

        public static sw.Size Ceiling(this sw.Size size)
        {
            return new sw.Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
        }

        public static sw.Size Floor(this sw.Size size)
        {
            return new sw.Size(Math.Floor(size.Width), Math.Floor(size.Height));
        }

        public static sw.Rect NormalizedRect(double x, double y, double width, double height)
        {
            if (width < 0)
            {
                x += width;
                width = -width + 1;
            }
            if (height < 0)
            {
                x += height;
                height = -height + 1;
            }
            return new sw.Rect(x, y, width, height);
        }

		public static sw.Size IfInfinity(this sw.Size size1, sw.Size size2)
		{
			if (double.IsInfinity(size1.Width))
				size1.Width = size2.Width;

			if (double.IsInfinity(size1.Height))
				size1.Height = size2.Height;
			return size1;
		}


		public static sw.Size IfNaN(this sw.Size size1, sw.Size size2)
        {
            if (double.IsNaN(size1.Width))
                size1.Width = size2.Width;

            if (double.IsNaN(size1.Height))
                size1.Height = size2.Height;
            return size1;
        }

        public static sw.Size ZeroIfNan(this sw.Size size)
        {
            if (double.IsNaN(size.Width))
                size.Width = 0;
            if (double.IsNaN(size.Height))
                size.Height = 0;
            return size;
        }

		public static sw.Size ZeroIfInfinity(this sw.Size size)
		{
			if (double.IsInfinity(size.Width))
				size.Width = 0;
			if (double.IsInfinity(size.Height))
				size.Height = 0;
			return size;
		}

		public static sw.Size InfinityIfNan(this sw.Size size)
		{
			if (double.IsNaN(size.Width))
				size.Width = double.PositiveInfinity;
			if (double.IsNaN(size.Height))
				size.Height = double.PositiveInfinity;
			return size;
		}

		public static sw.Size Add(this sw.Size size1, sw.Size size2)
		{
			return new sw.Size(size1.Width + size2.Width, size1.Height + size2.Height);
		}

		public static sw.Size Subtract(this sw.Size size1, sw.Size size2)
		{
			return new sw.Size(Math.Max(0, size1.Width - size2.Width), Math.Max(0, size1.Height - size2.Height));
		}

		public static void AddKeyBindings(this swi.InputBindingCollection bindings, swc.ItemCollection items)
		{
			foreach (var item in items.OfType<sw.UIElement>())
			{
				bindings.AddKeyBindings(item);
			}
		}

		public static void AddKeyBindings(this swi.InputBindingCollection bindings, sw.UIElement item)
		{
			if (item == null)
				return;
			bindings.AddRange(item.InputBindings);
			var itemsControl = item as swc.ItemsControl;
			if (itemsControl != null && itemsControl.HasItems)
				AddKeyBindings(bindings, itemsControl.Items);
		}

		public static void RemoveKeyBindings(this swi.InputBindingCollection bindings, sw.UIElement item)
		{
			if (item == null)
				return;
			foreach (var binding in item.InputBindings.OfType<swi.InputBinding>())
				bindings.Remove(binding);

			var itemsControl = item as swc.ItemsControl;
			if (itemsControl != null && itemsControl.HasItems)
				RemoveKeyBindings(bindings, itemsControl.Items);
		}

		public static void RemoveKeyBindings(this swi.InputBindingCollection bindings, swc.ItemCollection items)
		{
			foreach (var item in items.OfType<sw.UIElement>())
			{
				bindings.RemoveKeyBindings(item);
			}
		}

		public static void RenderWithCollect(this swm.Imaging.RenderTargetBitmap bitmap, swm.Visual visual)
		{
			bitmap.Render(visual);
			// fix memory leak with RenderTargetBitmap.  See http://stackoverflow.com/questions/14786490/wpf-memory-leak-using-rendertargetbitmap
			// Reproducible with the 
			// GC.Collect alone seems to fix the issue.  Adding GC.WaitForPendingFinalizers may impact performance.
			GC.Collect();
		}


		static Lazy<bool> s_SpellCheckCanBeEnabled = new Lazy<bool>(CheckIfSpellCheckCanBeEnabled);

		public static bool SpellCheckCanBeEnabled => s_SpellCheckCanBeEnabled.Value;

		private static bool CheckIfSpellCheckCanBeEnabled()
		{
			try
			{
				// check if we can create the spell checker com component
				var spellCheckerFactory = new SpellCheckerFactoryCoClass();

				if (Marshal.IsComObject(spellCheckerFactory))
				{
					Marshal.ReleaseComObject(spellCheckerFactory);
					return true;
				}
				return false;
			}
			catch
			{
				// there was a problem!
				return false;
			}
		}

		[Guid("7AB36653-1796-484B-BDFA-E74F1DB7C1DC")]
		[TypeLibType(TypeLibTypeFlags.FCanCreate)]
		[ClassInterface(ClassInterfaceType.None)]
		[ComImport]
		private class SpellCheckerFactoryCoClass
		{
		}

	}
}
