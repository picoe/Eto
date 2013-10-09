using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ScrollableHandler : WpfDockContainer<swc.Border, Scrollable>, IScrollable
	{
		BorderType borderType;
		bool expandContentWidth = true;
		bool expandContentHeight = true;
		EtoScrollViewer scroller;

		public sw.FrameworkElement ContentControl { get { return scroller; } }

		protected override bool UseContentSize { get { return false; } }

		public class EtoScrollViewer : swc.ScrollViewer
		{
			public swc.Primitives.IScrollInfo GetScrollInfo()
			{
				return base.ScrollInfo;
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

		void HandleSizeChanged(object sender, EventArgs e)
		{
			UpdateSizes();
		}

		void UpdateSizes()
		{
			var info = scroller.GetScrollInfo();
			if (info != null)
			{
				var content = (swc.Border)scroller.Content;
				var viewportSize = new sw.Size(info.ViewportWidth, info.ViewportHeight);
				var prefSize = Content.GetPreferredSize(Conversions.PositiveInfinitySize);

				// hack for when a scrollable is in a group box it expands vertically
				if (Widget.FindParent<GroupBox>() != null)
					viewportSize.Height = Math.Max(0, viewportSize.Height - 1);

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

		public Eto.Drawing.Point ScrollPosition
		{
			get
			{
				EnsureLoaded();
				return new Eto.Drawing.Point((int)scroller.HorizontalOffset, (int)scroller.VerticalOffset);
			}
			set
			{
				scroller.ScrollToVerticalOffset(value.Y);
				scroller.ScrollToHorizontalOffset(value.X);
			}
		}

		public Eto.Drawing.Size ScrollSize
		{
			get
			{
				EnsureLoaded();
				return new Eto.Drawing.Size((int)scroller.ExtentWidth, (int)scroller.ExtentHeight);
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
						Control.BorderThickness = new sw.Thickness(1.0);
						break;
					case BorderType.Line:
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new sw.Thickness(1);
						break;
					case BorderType.None:
						Control.BorderBrush = null;
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
					return base.Size;
				EnsureLoaded();
				var info = scroller.GetScrollInfo();
				if (info != null)
					return new Size((int)info.ViewportWidth, (int)info.ViewportHeight);
				else
					return Size.Empty;
			}
			set
			{
				this.Size = value;
			}
		}

		public Eto.Drawing.Rectangle VisibleRect
		{
			get { return new Eto.Drawing.Rectangle(ScrollPosition, ClientSize); }
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			content.HorizontalAlignment = sw.HorizontalAlignment.Left;
			content.VerticalAlignment = sw.VerticalAlignment.Top;
			content.SizeChanged += HandleSizeChanged;
			scroller.Content = content;
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case Scrollable.ScrollEvent:
					scroller.ScrollChanged += (sender, e) =>
					{
						Widget.OnScroll(new ScrollEventArgs(new Point((int)e.HorizontalOffset, (int)e.VerticalOffset)));
					};
					break;
				default:
					base.AttachEvent(handler);
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
			foreach (var control in Widget.Children)
			{
				control.Invalidate();
			}
		}

		public override void Invalidate(Rectangle rect)
		{
			base.Invalidate(rect);
			foreach (var control in Widget.Children)
			{
				control.Invalidate(rect);
			}
		}
	}
}
