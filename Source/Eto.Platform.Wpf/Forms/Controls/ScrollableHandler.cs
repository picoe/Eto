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
		swc.Border content;

		public sw.FrameworkElement ContentControl { get { return content; } }

		protected override bool UseContentSize { get { return false; } }

		public class EtoScrollViewer : swc.ScrollViewer
		{
			public ScrollableHandler Handler { get; set; }

			protected override sw.Size MeasureOverride (sw.Size constraint)
			{
				var size = base.MeasureOverride (constraint);
				var content = (swc.Border)Content;
				var info = this.ScrollInfo;
				var contentHandler = Handler.Content.GetWpfFrameworkElement();
				if (contentHandler != null)
				{
					var preferredSize = contentHandler.GetPreferredSize(null);
					if (Handler.ExpandContentWidth)
						content.Width = Math.Max(0, Math.Max(content.MinWidth, Math.Max(preferredSize.Width, info.ViewportWidth)));
					else
						content.Width = preferredSize.Width;

					if (Handler.ExpandContentHeight)
					{
						var viewportHeight = info.ViewportHeight;
						var extentHeight = info.ExtentHeight;
						// fix issue with GroupBox that makes the control grow continuously
						if (viewportHeight < extentHeight || viewportHeight > extentHeight + 1)
							content.Height = Math.Max(0, Math.Max(content.MinHeight, Math.Max(preferredSize.Height, viewportHeight)));
					}
					else
						content.Height = preferredSize.Height;
				}
				return size;
			}

			public swc.Primitives.IScrollInfo GetScrollInfo()
			{
				return base.ScrollInfo;
			}
		}

		public override Color BackgroundColor
		{
			get { return content.Background.ToEtoColor(); }
			set { content.Background = value.ToWpfBrush(content.Background); }
		}

		public ScrollableHandler ()
		{
			Control = new swc.Border {
				SnapsToDevicePixels = true
			};
			scroller = new EtoScrollViewer {
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				Handler = this,
				CanContentScroll = true,
				SnapsToDevicePixels = true,
				Focusable = false
			};
            scroller.ScrollChanged += HandleChangedSize;
            scroller.Loaded += HandleChangedSize;

			content = new swc.Border {
				SnapsToDevicePixels = true,
				Focusable = false,
				VerticalAlignment = sw.VerticalAlignment.Top,
				HorizontalAlignment = sw.HorizontalAlignment.Left
			};

			scroller.Content = content;
			Control.Child = scroller;
			this.Border = BorderType.Bezel;
		}

        void HandleChangedSize (object sender, EventArgs e)
        {
			UpdateSizes ();
		}

		void UpdateSizes ()
		{
			scroller.InvalidateMeasure ();
		}

        public override void OnLoadComplete (EventArgs e)
        {
            base.OnLoadComplete (e);
			Control.InvalidateMeasure ();
		}

		public void UpdateScrollSizes ()
		{
			Control.InvalidateMeasure ();
			UpdateSizes ();
		}


		public Eto.Drawing.Point ScrollPosition
		{
			get
			{
				EnsureLoaded ();
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
				EnsureLoaded ();
				return new Eto.Drawing.Size ((int)scroller.ExtentWidth, (int)scroller.ExtentHeight);
			}
			set
			{
                content.MinHeight = value.Height;
				content.MinWidth = value.Width;
				UpdateSizes ();
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

		public override Size ClientSize
		{
			get
			{
				if (!Widget.Loaded)
					return base.Size;
				EnsureLoaded ();
				var info = scroller.GetScrollInfo ();
				if (info != null)
					return new Size ((int)info.ViewportWidth, (int)info.ViewportHeight);
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
			get { return new Eto.Drawing.Rectangle (ScrollPosition, ClientSize); }
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			this.content.Child = content;
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Scrollable.ScrollEvent:
				scroller.ScrollChanged += (sender, e) => {
					Widget.OnScroll (new ScrollEventArgs (new Point ((int)e.HorizontalOffset, (int)e.VerticalOffset)));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}


		public bool ExpandContentWidth
		{
			get { return expandContentWidth; }
			set
			{
				if (expandContentWidth != value) {
					expandContentWidth = value;
					UpdateSizes ();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get { return expandContentHeight; }
			set
			{
				if (expandContentHeight != value) {
					expandContentHeight = value;
					UpdateSizes ();
				}
			}
		}

		public override void Invalidate ()
		{
			base.Invalidate ();
			foreach (var control in Widget.Children) {
				control.Invalidate ();
			}
		}

		public override void Invalidate (Rectangle rect)
		{
			base.Invalidate (rect);
			foreach (var control in Widget.Children) {
				control.Invalidate (rect);
			}
		}
	}
}
