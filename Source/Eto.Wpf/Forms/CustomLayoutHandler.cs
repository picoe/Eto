using System;
using System.Linq;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public class CustomLayoutHandler : WpfLayout<swc.Canvas, CustomLayout, CustomLayout.ICallback>, CustomLayout.IHandler
	{
		public override sw.Size GetPreferredSize(sw.Size constraint)
		{
			var size = new sw.Size();
			foreach (var control in Widget.Controls)
			{
				var container = control.GetContainerControl();
				var preferredSize = control.GetPreferredSize(constraint.ToEtoSize());
				var left = swc.Canvas.GetLeft(container) + preferredSize.Width;
				var top = swc.Canvas.GetTop(container) + preferredSize.Height;
				if (size.Width < left) size.Width = left;
				if (size.Height < top) size.Height = top;
			}
			return size;
		}

		static readonly object Location_Key = new object();

		public class EtoCanvas : swc.Canvas
		{
			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				var availableSize = new sw.Size(double.PositiveInfinity, double.PositiveInfinity);
				foreach (sw.UIElement element in base.InternalChildren)
				{
					if (element != null)
					{
						var handler = ((sw.FrameworkElement)element).Tag as Control.IHandler;
						var location = handler.Widget.Properties.Get<Rectangle>(Location_Key);
						var desiredSize = location.Size.ToWpf();
						element.Measure(desiredSize);
					}
				}
				return default(sw.Size);
			}

			protected override sw.Size ArrangeOverride(sw.Size arrangeSize)
			{
				foreach (sw.UIElement element in InternalChildren)
				{
					if (element != null)
					{
						double x = 0.0;
						double y = 0.0;
						double left = swc.Canvas.GetLeft(element);
						var handler = ((sw.FrameworkElement)element).Tag as Control.IHandler;
						var location = handler.Widget.Properties.Get<Rectangle>(Location_Key);
						var desiredSize = location.Size.ToWpf();

						if (!double.IsNaN(left))
						{
							x = left;
						}
						else
						{
							double right = swc.Canvas.GetRight(element);
							if (!double.IsNaN(right))
							{
								x = arrangeSize.Width - desiredSize.Width - right;
							}
						}
						double top = swc.Canvas.GetTop(element);
						if (!double.IsNaN(top))
						{
							y = top;
						}
						else
						{
							double bottom = swc.Canvas.GetBottom(element);
							if (!double.IsNaN(bottom))
							{
								y = arrangeSize.Height - desiredSize.Height - bottom;
							}
						}
						element.Arrange(new sw.Rect(new sw.Point(x, y), desiredSize));
					}
				}
				return arrangeSize; 
				
				var size = base.ArrangeOverride(arrangeSize);
				var bounds = Rectangle.Empty;

				foreach (var ctl in this.InternalChildren.OfType<sw.FrameworkElement>())
				{
					var handler = ctl.Tag as Control.IHandler;
					var location = handler.Widget.Properties.Get<Rectangle>(Location_Key);

					ctl.Arrange(location.ToWpf());
					bounds.Union(location);
				}

				return size; // bounds.Size.ToWpf();
			}
		}

		public CustomLayoutHandler()
		{
			Control = new EtoCanvas
			{
				SnapsToDevicePixels = true,
				ClipToBounds = true
			};
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public void Add(Control child)
		{
			var element = child.GetContainerControl();
			Control.Children.Add(element);
			UpdatePreferredSize();
		}

		public void Move(Control child, Rectangle location)
		{
			var element = child.GetContainerControl();
			swc.Canvas.SetLeft(element, location.X);
			swc.Canvas.SetTop(element, location.Y);
			//element.Tag = child;
			child.Properties[Location_Key] = location;
			UpdatePreferredSize();
		}

		public void RemoveAll()
		{
			Control.Children.Clear();
		}

		public void Remove(Control child)
		{
			var element = child.GetContainerControl();
			Control.Children.Remove(element);
			UpdatePreferredSize();
		}

		public override void Remove(sw.FrameworkElement child)
		{
			Control.Children.Remove(child);
			UpdatePreferredSize();
		}
	}
}
