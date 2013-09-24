using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfDockContainer<T, W> : WpfContainer<T, W>, IDockContainer
		where T : sw.FrameworkElement
		where W : DockContainer
	{
		Control content;
		swc.Border border;
		Size? clientSize;

		protected virtual bool UseContentSize { get { return true; } }

		public override Size ClientSize
		{
			get
			{
				if (!Control.IsLoaded && clientSize != null) return clientSize.Value;
				else return Conversions.GetSize(border);
			}
			set
			{
				clientSize = value;
				Conversions.SetSize(border, value);
			}
		}

		public override sw.Size GetPreferredSize(sw.Size? constraint)
		{
			var size = PreferredSize;
			if (double.IsNaN(size.Width) || double.IsNaN(size.Height))
			{
				var contentSize = constraint ?? new sw.Size(double.PositiveInfinity, double.PositiveInfinity);
				contentSize = new sw.Size(Math.Max(0, contentSize.Width - Padding.Horizontal), Math.Max(0, contentSize.Height - Padding.Vertical));
				var baseSize = sw.Size.Empty;
				if (UseContentSize)
				{
					var preferredSize = content.GetPreferredSize(contentSize);
					baseSize = new sw.Size(Math.Max(0, Math.Max(baseSize.Width, preferredSize.Width + Padding.Horizontal)), Math.Max(0, Math.Max(baseSize.Height, preferredSize.Height + Padding.Vertical)));
				}
				else
					baseSize = base.GetPreferredSize(contentSize);
				if (double.IsNaN(size.Width))
					size.Width = baseSize.Width;
				if (double.IsNaN(size.Height))
					size.Height = baseSize.Height;
			}
			return new sw.Size(Math.Max(0, size.Width), Math.Max(0, size.Height));
		}

		public class EtoBorder : swc.Border
		{

			// Override the default Measure method of Panel 
			protected override sw.Size MeasureOverride(sw.Size availableSize)
			{
				sw.Size panelDesiredSize = new sw.Size();

				// In our example, we just have one child.  
				// Report that our panel requires just the size of its only child. 
				var child = Child as sw.FrameworkElement;
				if (child != null)
				{
					child.Measure(availableSize);
					panelDesiredSize = child.DesiredSize;
					if (!double.IsPositiveInfinity(availableSize.Width))
					{
						panelDesiredSize.Width = availableSize.Width;
						child.Width = availableSize.Width;
					}
					if (!double.IsPositiveInfinity(availableSize.Height))
					{
						panelDesiredSize.Height = availableSize.Height;
						child.Height = availableSize.Height;
					}
				}

				return panelDesiredSize;
			}

			protected override sw.Size ArrangeOverride(sw.Size finalSize)
			{
				var child = Child;
				if (child != null)
				{
					child.Arrange(new sw.Rect(new sw.Point(), finalSize));
				}
				return finalSize; // Returns the final Arranged size
			}
		}


		public WpfDockContainer()
		{
			border = new swc.Border
			{
				SnapsToDevicePixels = true,
				Focusable = false,
			};
			border.SizeChanged += (sender, e) =>
			{
				var element = content.GetContainerControl();
				if (element != null)
				{
					if (!double.IsNaN(element.Width))
						element.Width = Math.Max(0, e.NewSize.Width - Padding.Horizontal);
					if (!double.IsNaN(element.Height))
						element.Height = Math.Max(0, e.NewSize.Height - Padding.Vertical);
				}
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetContainerContent(border);
		}

		public Padding Padding
		{
			get { return border.Padding.ToEto(); }
			set { border.Padding = value.ToWpf(); }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				content = value;
				if (content != null)
				{
					var element = content.GetContainerControl();
					element.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					element.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					border.Child = element;
				}
				else
					border.Child = null;
				SetContent();
			}
		}

		public abstract void SetContainerContent(sw.FrameworkElement content);

		public virtual void SetContent()
		{
		}

		public override void Remove(sw.FrameworkElement child)
		{
			if (border.Child == child)
			{
				content = null;
				border.Child = null;
			}
		}
	}
}
