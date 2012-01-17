using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WC = System.Windows.Controls;
using W = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ScrollableHandler : WpfFrameworkElement<WC.Border, Scrollable>, IScrollable
	{
		BorderType borderType;
		WC.ScrollViewer scroller;

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
				return new Size ((int)Control.MinWidth, (int)Control.MinHeight);
			}
			set
			{
				Control.MinWidth = value.Width; Control.MinHeight = value.Height;
			}
		}

		public ScrollableHandler ()
		{
			Control = new WC.Border ();
			scroller = new WC.ScrollViewer {
				VerticalScrollBarVisibility = WC.ScrollBarVisibility.Auto,
				HorizontalScrollBarVisibility = WC.ScrollBarVisibility.Auto
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
			get
			{
				return borderType;
			}
			set
			{
				borderType = value;
				switch (value) {
					case BorderType.Bezel:
						Control.BorderBrush = W.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new W.Thickness (1.0);
						break;
					case BorderType.Line:
						Control.BorderBrush = W.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new W.Thickness (1);
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
			get { return Control; }
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
