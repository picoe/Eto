using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class ScrollableHandler : WpfPanel<swc.Border, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		bool expandContentWidth = true;
		bool expandContentHeight = true;
		readonly EtoScrollViewer scroller;

		public sw.FrameworkElement ContentControl => scroller;

		public class EtoScrollViewer : swc.ScrollViewer
		{
			public ScrollableHandler Handler { get; set; }

			public swc.Primitives.IScrollInfo GetScrollInfo() => ScrollInfo;

			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				var content = (sw.FrameworkElement)Content;

				// reset to preferred size to calculate scroll sizes initially based on that
				content.Width = Handler.scrollSize.Width;
				content.Height = Handler.scrollSize.Height;
				return base.MeasureOverride(constraint);
			}

			protected override sw.Size ArrangeOverride(sw.Size arrangeSize)
			{
				var content = (sw.FrameworkElement)Content;

				// expand to width or height of viewport, now that we know which scrollbars are mandatory
				var desiredSize = content.DesiredSize;

				if (Handler.ExpandContentWidth)
					content.Width = Math.Max(desiredSize.Width, ScrollInfo.ViewportWidth);
				if (Handler.ExpandContentHeight)
					content.Height = Math.Max(desiredSize.Height, ScrollInfo.ViewportHeight);

				return base.ArrangeOverride(arrangeSize);
			}
		}

		public override Color BackgroundColor
		{
			get { return scroller.Background.ToEtoColor(); }
			set { scroller.Background = value.ToWpfBrush(scroller.Background); }
		}

		public ScrollableHandler()
		{
			Control = new EtoBorder
			{
				Handler = this,
				SnapsToDevicePixels = true,
				Focusable = false,
			};
			scroller = new EtoScrollViewer
			{
				Handler = this,
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				CanContentScroll = true,
				SnapsToDevicePixels = true,
				Focusable = false
			};
			scroller.SizeChanged += HandleSizeChanged;
			scroller.Loaded += HandleSizeChanged;

			Control.Child = scroller;
			Control.SetEtoBorderType(BorderType.Bezel);
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			UpdateSizes();
		}

		void HandleSizeChanged(object sender, EventArgs e)
		{
			if (Widget.Loaded)
				UpdateSizes();
		}

		void UpdateSizes()
		{
			scroller.InvalidateMeasure();
		}

		public override void UpdatePreferredSize()
		{
			UpdateSizes();
			base.UpdatePreferredSize();
		}

		public void UpdateScrollSizes()
		{
			Control.InvalidateMeasure();
			UpdateSizes();
			scroller.UpdateLayout();
		}

		protected override void SetContentScale(bool xscale, bool yscale)
		{
			base.SetContentScale(ExpandContentWidth, ExpandContentHeight);
		}

		public Point ScrollPosition
		{
			get
			{
				EnsureLoaded();
				return new Point((int)scroller.HorizontalOffset, (int)scroller.VerticalOffset);
			}
			set
			{
				scroller.ScrollToVerticalOffset(value.Y);
				scroller.ScrollToHorizontalOffset(value.X);
			}
		}

		sw.Size scrollSize = new sw.Size(double.NaN, double.NaN);
		public Size ScrollSize
		{
			get
			{
				EnsureLoaded();
				return new Size((int)scroller.ExtentWidth, (int)scroller.ExtentHeight);
			}
			set
			{
				//var content = (swc.Border)Control.Child;
				scrollSize = value.ToWpf();
				UpdateSizes();
			}
		}

		static object Border_Key = new object();

		public BorderType Border
		{
			get { return Widget.Properties.Get(Border_Key, BorderType.Bezel); }
			set { Widget.Properties.Set(Border_Key, value, () => Control.SetEtoBorderType(value)); }
		}

		public override Size ClientSize
		{
			get
			{
				if (!Widget.Loaded)
					return Size;
				EnsureLoaded();
				var info = scroller.GetScrollInfo();
				return info != null ? new Size((int)info.ViewportWidth, (int)info.ViewportHeight) : Size.Empty;
			}
			set
			{
				Size = value;
			}
		}

		public Rectangle VisibleRect
		{
			get { return new Rectangle(ScrollPosition, ClientSize); }
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			content.HorizontalAlignment = sw.HorizontalAlignment.Left;
			content.VerticalAlignment = sw.VerticalAlignment.Top;
			content.SizeChanged += HandleSizeChanged;
			scroller.Content = content;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Scrollable.ScrollEvent:
					scroller.ScrollChanged += (sender, e) =>
					{
						Callback.OnScroll(Widget, new ScrollEventArgs(new Point((int)e.HorizontalOffset, (int)e.VerticalOffset)));
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}


		public bool ExpandContentWidth
		{
			get { return expandContentWidth; }
			set
			{
				if (expandContentWidth != value)
				{
					expandContentWidth = value;
					UpdateSizes();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get { return expandContentHeight; }
			set
			{
				if (expandContentHeight != value)
				{
					expandContentHeight = value;
					UpdateSizes();
				}
			}
		}

		public float MaximumZoom { get { return 1f; } set { } }

		public float MinimumZoom { get { return 1f; } set { } }

		public float Zoom { get { return 1f; } set { } }
	}
}
