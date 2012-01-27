using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ScrollableHandler : WpfFrameworkElement<swc.Border, Scrollable>, IScrollable
	{
		BorderType borderType;
		swc.ScrollViewer scroller;

		public override Eto.Drawing.Color BackgroundColor
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public override Size Size
		{
			get
			{
				return new Size ((int)Control.Width, (int)Control.Height);
			}
			set
			{
				Control.Width = value.Width; Control.Height = value.Height;
			}
		}

		public ScrollableHandler ()
		{
			Control = new swc.Border {
				SnapsToDevicePixels = true
			};
			scroller = new swc.ScrollViewer {
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Visible,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Visible,
				CanContentScroll = true,
				SnapsToDevicePixels = true
			};
			Control.Child = scroller;
			this.Border = BorderType.Bezel;
		}

		public void UpdateScrollSizes ()
		{
			var layout = Widget.Layout.Handler as IWpfLayout;
			if (layout != null) layout.AutoSize ();
		}

		public Eto.Drawing.Point ScrollPosition
		{
			get
			{
				return new Eto.Drawing.Point ((int)scroller.HorizontalOffset, (int)scroller.VerticalOffset);
			}
			set
			{
				scroller.ScrollToVerticalOffset (value.Y);
				scroller.ScrollToHorizontalOffset (value.X);
			}
		}

		public Eto.Drawing.Size ScrollSize
		{
			get
			{
				return new Eto.Drawing.Size ((int)scroller.ScrollableWidth, (int)scroller.ScrollableHeight);
			}
			set
			{
				
			}
		}

		public BorderType Border
		{
			get { return borderType; }
			set
			{
				borderType = value;
				switch (value) {
					case BorderType.Bezel:
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new sw.Thickness (1.0);
						break;
					case BorderType.Line:
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new sw.Thickness (1);
						break;
					case BorderType.None:
						Control.BorderBrush = null;
						//Control.BorderThickness = new W.Thickness(0);
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}

		public Eto.Drawing.Size ClientSize
		{
			get
			{
				return new Eto.Drawing.Size ((int)scroller.ViewportWidth, (int)scroller.ViewportHeight);
			}
			set
			{
			}
		}

		public Eto.Drawing.Rectangle VisibleRect
		{
			get { return new Eto.Drawing.Rectangle (ScrollPosition, ScrollSize); }
		}

		public object ContainerObject
		{
			get { return scroller; }
		}

		public void SetLayout (Layout layout)
		{
			scroller.Content = layout.ControlObject;
		}

		public Eto.Drawing.Size? MinimumSize
		{
			get
			{
				if (Control.MinWidth == 0 && Control.MinHeight == 0)
					return new Eto.Drawing.Size ((int)Control.MinWidth, (int)Control.MinHeight);
				else
					return null;
			}
			set
			{
				if (value != null) {
					Control.MinWidth = value.Value.Width;
					Control.MinHeight = value.Value.Height;
				}
				else {
					Control.MinHeight = 0;
					Control.MinWidth = 0;
				}
			}
		}
	}
}
