namespace Eto.GtkSharp.Forms.Controls
{
	public class ScrollableHandler : GtkPanel<Gtk.ScrolledWindow, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		readonly Gtk.Viewport vp;
		readonly Gtk.Box hbox;
		readonly Gtk.Box vbox;
		BorderType border;
		bool expandWidth = true;
		bool expandHeight = true;
		Gtk.Widget layoutWidget;

		public BorderType Border
		{
			get => border;
			set
			{
				border = value;
				switch (border)
				{
					case BorderType.Bezel:
						Control.ShadowType = Gtk.ShadowType.In;
						break;
					case BorderType.Line:
						Control.ShadowType = Gtk.ShadowType.In;
						break;
					case BorderType.None:
						Control.ShadowType = Gtk.ShadowType.None;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public partial class EtoScrolledWindow : Eto.GtkSharp.Forms.EtoScrolledWindow
		{
#if GTK3
			// The frame drawn when there's a border is part of this widget's allocation but not the
			// viewport's, so it has to be added when measuring the content. Get it from the style as GTK
			// itself does rather than from the difference in allocations, which is zero until the first
			// size allocation -- an auto-sized window measures before that, so it would come up short by
			// the size of the border.
			public Size BorderSize
			{
				get
				{
					if (ShadowType == Gtk.ShadowType.None)
						return Size.Empty;
					var border = StyleContext.GetBorder(StateFlags);
					return new Size(border.Left + border.Right, border.Top + border.Bottom);
				}
			}

			// Size the content is being measured for during a height-for-width/width-for-height request,
			// or -1 for an unconstrained request. GtkScrolledWindow ignores the supplied size for those
			// requests (it can scroll, so it has no need for them), which would leave the content measured
			// at its own natural width -- reporting the taller height that wrapping content (labels,
			// checkboxes, etc) has when narrow, even though it is displayed expanded to the width of the
			// viewport.
			// GTK calls OnAdjustSizeRequest after the size request itself returns, so these can't simply be
			// cleared when the request is done; each is instead reset by the next unconstrained request for
			// the same orientation. Note GtkScrolledWindow answers a height-for-width request by calling
			// its own unconstrained height request, so ignore that nested call.
			int measureForWidth = -1;
			int measureForHeight = -1;
			bool inMeasureForWidth;
			bool inMeasureForHeight;

			protected override void OnGetPreferredHeightForWidth(int width, out int minimum_height, out int natural_height)
			{
				measureForWidth = width;
				inMeasureForWidth = true;
				try
				{
					base.OnGetPreferredHeightForWidth(width, out minimum_height, out natural_height);
				}
				finally
				{
					inMeasureForWidth = false;
				}
			}

			protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
			{
				if (!inMeasureForWidth)
					measureForWidth = -1;
				base.OnGetPreferredHeight(out minimum_height, out natural_height);
			}

			protected override void OnGetPreferredWidthForHeight(int height, out int minimum_width, out int natural_width)
			{
				measureForHeight = height;
				inMeasureForHeight = true;
				try
				{
					base.OnGetPreferredWidthForHeight(height, out minimum_width, out natural_width);
				}
				finally
				{
					inMeasureForHeight = false;
				}
			}

			protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
			{
				if (!inMeasureForHeight)
					measureForHeight = -1;
				base.OnGetPreferredWidth(out minimum_width, out natural_width);
			}

			protected override void OnAdjustSizeRequest(Gtk.Orientation orientation, out int minimum_size, out int natural_size)
			{
				base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
				// the natural size of the scrolled window should be the size of the child viewport

				var h = Handler;
				if (h != null)
				{
					var preferredSize = orientation == Gtk.Orientation.Horizontal ? h.UserPreferredSize.Width : h.UserPreferredSize.Height;

					if (preferredSize > 0)
						natural_size = preferredSize;
					else if (Child != null)
					{
						var borderSize = BorderSize;
						int child_size;
						if (orientation == Gtk.Orientation.Horizontal)
						{
							if (measureForHeight >= 0)
								Child.GetPreferredWidthForHeight(Math.Max(0, measureForHeight - borderSize.Height), out _, out child_size);
							else
							{
								Child.GetPreferredSize(out _, out var ns);
								child_size = ns.Width;
							}
							natural_size = Math.Max(natural_size, child_size + borderSize.Width);
						}
						else
						{
							if (measureForWidth >= 0)
								Child.GetPreferredHeightForWidth(Math.Max(0, measureForWidth - borderSize.Width), out _, out child_size);
							else
							{
								Child.GetPreferredSize(out _, out var ns);
								child_size = ns.Height;
							}
							natural_size = Math.Max(natural_size, child_size + borderSize.Height);
						}
					}

					minimum_size = Math.Min(natural_size, minimum_size);
				}
			}
#endif
		}

		public class EtoVBox : Gtk.Box
		{
			public EtoVBox() : base(Gtk.Orientation.Vertical, 0)
			{
			}
#if GTK3
			protected override void OnAdjustSizeRequest(Gtk.Orientation orientation, out int minimum_size, out int natural_size)
			{
				base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
				// scrolled size should always be the natural size, not minimum size
				minimum_size = natural_size;
			}
#endif
		}

		public ScrollableHandler()
		{
			Control = new EtoScrolledWindow { Handler = this };
#if GTK3
			// for some reason on mac it doesn't shrink past 47 pixels otherwise
			// also, what is an appropriate size?
			if (EtoEnvironment.Platform.IsMac)
				Control.SetSizeRequest(10, 10);
#endif
			// ensure things are top-left and not centered
			hbox = new Gtk.Box(Gtk.Orientation.Horizontal, 0);

			vbox = new EtoVBox();
			vbox.PackStart(hbox, true, true, 0);

			// use viewport to autosize the scrolled window to the size of the content
			vp = new Gtk.Viewport
			{
				ShadowType = Gtk.ShadowType.None,
				Child = vbox
			};

			Control.Add(vp);
			Border = BorderType.Bezel;
		}

		protected override void Initialize()
		{
			base.Initialize();
#if GTK2
			Control.SizeRequested += Connector.HandleControlSizeRequested;
			vp.SizeRequested += Connector.HandleViewportSizeRequested;
#endif
			Control.VScrollbar.VisibilityNotifyEvent += Connector.HandleScrollbarVisibilityChanged;
			Control.HScrollbar.VisibilityNotifyEvent += Connector.HandleScrollbarVisibilityChanged;
		}

		protected new ScrollableConnector Connector => (ScrollableConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new ScrollableConnector();

		protected class ScrollableConnector : GtkPanelEventConnector
		{
			public new ScrollableHandler Handler { get { return (ScrollableHandler)base.Handler; } }
#if GTK2
			public void HandleControlSizeRequested(object o, Gtk.SizeRequestedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				if (handler.autoSize)
				{
					args.Requisition = handler.vp.SizeRequest();
				}
			}

			public void HandleViewportSizeRequested(object o, Gtk.SizeRequestedArgs args)
			{
				var handler = Handler;
				if (handler != null)
				{
					var viewport = (Gtk.Viewport)o;
					if (handler.autoSize)
					{
						var size = viewport.SizeRequest();
						//Console.WriteLine ("Autosizing to {0}x{1}", size.Width, size.Height);
						args.Requisition = size;
					}
				}
			}
#endif
			public void HandleScrollbarVisibilityChanged(object sender, EventArgs e)
			{
				Handler?.Callback.OnSizeChanged(Handler.Widget, EventArgs.Empty);
			}

			public void HandleScrollableScrollEvent(object sender, EventArgs e)
			{
				Handler?.Callback.OnScroll(Handler.Widget, new ScrollEventArgs(Handler.ScrollPosition));
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Scrollable.ScrollEvent:
					Control.Events |= Gdk.EventMask.ScrollMask;
					Control.Vadjustment.ValueChanged += Connector.HandleScrollableScrollEvent;
					Control.Hadjustment.ValueChanged += Connector.HandleScrollableScrollEvent;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

#if GTK2
		bool autoSize = true;
		public override Size Size
		{
			get { return base.Size; }
			set
			{
				base.Size = value;
				autoSize = false;
			}
		}
#endif

		protected override void SetContainerContent(Gtk.Widget content)
		{
			layoutWidget = content;
			hbox.PackStart(content, false, true, 0);
			SetPacking();
		}

		void SetPacking()
		{
			if (layoutWidget != null)
				hbox.SetChildPacking(layoutWidget, expandWidth, expandWidth, 0, Gtk.PackType.Start);
			vbox.SetChildPacking(hbox, expandHeight, expandHeight, 0, Gtk.PackType.Start);
		}

		protected override void SetBackgroundColor(Color? color)
		{
			if (color != null)
				vp.SetBackground(color.Value);
			else
				vp.ClearBackground();
		}

		public override Size ClientSize
		{
			get
			{
				// the viewport's allocation is the visible area of the content, with the border and any
				// scrollbars (and their spacing) already taken out of it.
				Gdk.Rectangle rect = vp.Allocation;
				return new Size(rect.Width, rect.Height);
			}
			set
			{
				vp.SetSizeRequest(value.Width, value.Height);
			}
		}

		public void UpdateScrollSizes()
		{
			Control.CheckResize();
			vp.CheckResize();
		}

		public Point ScrollPosition
		{
			get { return new Point((int)vp.Hadjustment.Value, (int)vp.Vadjustment.Value); }
			set
			{
				Size clientSize = ClientSize;
				Size scrollSize = ScrollSize;
				vp.Hadjustment.Value = Math.Min(value.X, scrollSize.Width - clientSize.Width);
				vp.Vadjustment.Value = Math.Min(value.Y, scrollSize.Height - clientSize.Height);
			}
		}

		Size? desiredScrollSize;

		public Size ScrollSize
		{
			get
			{
#if GTK2
				return new Size((int)(vp.Hadjustment.Upper), (int)(vp.Vadjustment.Upper));
#else
				// Don't use the scroll adjustments here: GTK clamps their upper bound to at least the
				// visible page size, whereas ScrollSize is the extent of the content, which is smaller than
				// the viewport when the content isn't expanded to fill it (as on the other platforms).
				// Measure the content at the size it is displayed at, so content expanded to the viewport
				// reports the height it actually has at that width rather than at its natural width.
				var visible = vp.Allocation.Size;
				vbox.GetPreferredWidth(out _, out var naturalWidth);
				var width = expandWidth ? Math.Max(naturalWidth, visible.Width) : naturalWidth;
				vbox.GetPreferredHeightForWidth(width, out _, out var naturalHeight);
				var height = expandHeight ? Math.Max(naturalHeight, visible.Height) : naturalHeight;

				// a scroll size set explicitly acts as a minimum, same as on macOS
				if (desiredScrollSize != null)
				{
					width = Math.Max(width, desiredScrollSize.Value.Width);
					height = Math.Max(height, desiredScrollSize.Value.Height);
				}
				return new Size(width, height);
#endif
			}
			set
			{
				desiredScrollSize = value.Width >= 0 || value.Height >= 0 ? (Size?)value : null;
				vp.Hadjustment.Upper = value.Width;
				vp.Vadjustment.Upper = value.Height;
			}
		}

		public Rectangle VisibleRect
		{
			get { return new Rectangle(ScrollPosition, Size.Min(ScrollSize, ClientSize)); }
		}

		public bool ExpandContentWidth
		{
			get { return expandWidth; }
			set
			{
				if (expandWidth != value)
				{
					expandWidth = value;
					SetPacking();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get { return expandHeight; }
			set
			{
				if (expandHeight != value)
				{
					expandHeight = value;
					SetPacking();
				}
			}
		}

		public float MinimumZoom { get { return 1f; } set { } }

		public float MaximumZoom { get { return 1f; } set { } }

		public float Zoom { get { return 1f; } set { } }
	}
}
