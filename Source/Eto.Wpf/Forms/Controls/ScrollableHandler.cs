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
		BorderType borderType;
		bool expandContentWidth = true;
		bool expandContentHeight = true;
		readonly EtoScrollViewer scroller;

		public sw.FrameworkElement ContentControl { get { return scroller; } }

		protected override bool UseContentSize { get { return false; } }

		public class EtoScrollViewer : swc.ScrollViewer
		{
			public swc.Primitives.IScrollInfo GetScrollInfo()
			{
				return ScrollInfo;
			}
		}

		public override Color BackgroundColor
		{
			get { return scroller.Background.ToEtoColor(); }
			set { scroller.Background = value.ToWpfBrush(scroller.Background); }
		}

		public ScrollableHandler()
		{
			Control = new swc.Border
			{
				SnapsToDevicePixels = true,
				Focusable = false,
			};
			scroller = new EtoScrollViewer
			{
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				CanContentScroll = true,
				SnapsToDevicePixels = true,
				Focusable = false
			};
			scroller.SizeChanged += HandleSizeChanged;
			scroller.Loaded += HandleSizeChanged;

			Control.Child = scroller;
			this.Border = BorderType.Bezel;
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
			var info = scroller.GetScrollInfo();
			if (info != null)
			{
				var content = (swc.Border)scroller.Content;
				var viewportSize = new sw.Size(info.ViewportWidth, info.ViewportHeight);
				var prefSize = Content.GetPreferredSize(new sw.Size(double.MaxValue, double.MaxValue));

				// hack for when a scrollable is in a group box it expands vertically indefinitely
				// -2 since when you resize the scrollable it grows slowly
				if (Widget.FindParent<GroupBox>() != null)
					viewportSize.Height = Math.Max(0, viewportSize.Height - 2);

				if (ExpandContentWidth)
					content.Width = Math.Max(0, Math.Max(prefSize.Width, viewportSize.Width));
				else
					content.Width = prefSize.Width;

				if (ExpandContentHeight)
					content.Height = Math.Max(0, Math.Max(prefSize.Height, viewportSize.Height));
				else
					content.Height = prefSize.Height;

				scroller.InvalidateMeasure();
			}
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

		public Size ScrollSize
		{
			get
			{
				EnsureLoaded();
				return new Size((int)scroller.ExtentWidth, (int)scroller.ExtentHeight);
			}
			set
			{
				var content = (swc.Border)Control.Child;
				content.MinHeight = value.Height;
				content.MinWidth = value.Width;
				UpdateSizes();
			}
		}

		public BorderType Border
		{
			get { return borderType; }
			set
			{
				borderType = value;
				switch (value)
				{
					case BorderType.Bezel:
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new sw.Thickness(1);
						break;
					case BorderType.Line:
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new sw.Thickness(1);
						break;
					case BorderType.None:
						Control.BorderBrush = null;
                        Control.BorderThickness = new sw.Thickness(0);
                        break;
					default:
						throw new NotSupportedException();
				}
			}
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

		public override void Invalidate()
		{
			base.Invalidate();
			foreach (var control in Widget.VisualChildren)
			{
				control.Invalidate();
			}
		}

		public override void Invalidate(Rectangle rect)
		{
			base.Invalidate(rect);
			foreach (var control in Widget.VisualChildren)
			{
				control.Invalidate(rect);
			}
		}

		public float MaximumZoom { get { return 1f; } set { } }

		public float MinimumZoom { get { return 1f; } set { } }

		public float Zoom { get { return 1f; } set { } }
	}
}
