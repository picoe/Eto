namespace Eto.Wpf.Forms.Controls
{
	public class EtoScrollViewer : swc.ScrollViewer
	{
		public ScrollableHandler Handler { get; set; }

		// real viewport width from the last arrange (scrollbar- and screen-safe), used to measure
		// expanded content at the width it will actually be displayed at.
		double _lastViewportWidth = double.NaN;

		public swc.Primitives.IScrollInfo GetScrollInfo() => ScrollInfo;

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			var content = (sw.FrameworkElement)Content;

			// reset to preferred size to calculate scroll sizes initially based on that
			content.Width = Handler.scrollSize.Width;
			content.Height = Handler.scrollSize.Height;

			// When content is expanded to fill the viewport width, measure it at that width so its
			// extent (and especially its height) reflects the width it is actually displayed at.
			// Otherwise wrapping content (labels, checkboxes, etc.) reports the taller height it would
			// have at its narrower preferred width, inflating the vertical scroll extent even though it
			// is shown wider and shorter.
			if (Handler.ExpandContentWidth && double.IsNaN(content.Width))
				ConstrainExpandedWidth(content, _lastViewportWidth);

			return base.MeasureOverride(constraint);
		}

		// Constrain the content to the viewport width when it either fits within it (so it just
		// expands to fill) or reflows to fit it — i.e. measuring at the viewport width makes the
		// content taller, as happens with wrapping labels/checkboxes. Content that is wider and does
		// NOT reflow (a rigid, too-wide control) is left unconstrained so it still scrolls
		// horizontally, per ExpandContentWidth's documented behavior.
		static void ConstrainExpandedWidth(sw.FrameworkElement content, double viewport)
		{
			if (double.IsNaN(viewport) || double.IsInfinity(viewport) || viewport <= 0)
				return;

			content.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity));
			var naturalWidth = content.DesiredSize.Width;
			var naturalHeight = content.DesiredSize.Height;

			var constrain = naturalWidth <= viewport + 0.5;
			if (!constrain)
			{
				content.Measure(new sw.Size(viewport, double.PositiveInfinity));
				constrain = content.DesiredSize.Height > naturalHeight + 0.5;
			}
			if (constrain)
				content.Width = viewport;
		}

		protected override sw.Size ArrangeOverride(sw.Size arrangeSize)
		{
			var content = (sw.FrameworkElement)Content;

			// expand to width or height of viewport, now that we know which scrollbars are mandatory
			var desiredSize = content.DesiredSize;

			// ScrollInfo.ViewportWidth/Height give the visible area with scrollbar thickness
			// already removed, which is what we want to expand the content to. However, when this
			// scrollable lives in a SizeToContent window, the measure pass runs with the monitor
			// work-area as its constraint, leaving ViewportWidth/Height equal to the screen size.
			// Clamp to the actual arrange size so the content isn't expanded to fill the screen.
			// In a normally constrained layout ViewportWidth <= arrangeSize.Width, so this is a no-op.
			var viewportWidth = Math.Min(ScrollInfo.ViewportWidth, arrangeSize.Width);
			var viewportHeight = Math.Min(ScrollInfo.ViewportHeight, arrangeSize.Height);

			// remember the real viewport width so the next measure can size expanded content to it
			_lastViewportWidth = viewportWidth;

			if (Handler.ExpandContentWidth)
				content.Width = Math.Max(desiredSize.Width, viewportWidth);
			if (Handler.ExpandContentHeight)
				content.Height = Math.Max(desiredSize.Height, viewportHeight);

			return base.ArrangeOverride(arrangeSize);
		}
	}

	public class ScrollableHandler : WpfPanel<swc.Border, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		bool expandContentWidth = true;
		bool expandContentHeight = true;
		readonly EtoScrollViewer scroller;
		Size? _lastSize;

		public sw.FrameworkElement ContentControl => scroller;

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
			if (!Widget.Loaded)
				return;

			// Avoid updating sizes if the size didn't actually change, as this can cause a loop when the content is set to expand
			var newSize = Size;
			if (_lastSize == newSize)
				return;

			UpdateSizes();

			_lastSize = Size;
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
				return new Point((int)scroller.HorizontalOffset, (int)scroller.VerticalOffset);
			}
			set
			{
				scroller.ScrollToVerticalOffset(value.Y);
				scroller.ScrollToHorizontalOffset(value.X);
			}
		}

		internal sw.Size scrollSize = new sw.Size(double.NaN, double.NaN);
		public Size ScrollSize
		{
			get
			{
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

		public override void OnChildPreferredSizeUpdated()
		{
			base.OnChildPreferredSizeUpdated();
			if (ExpandContentWidth || ExpandContentHeight)
				UpdateSizes();
		}
	}
}
