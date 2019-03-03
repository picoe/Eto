using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ScrollableHandler : GtkPanel<Gtk.ScrolledWindow, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		readonly Gtk.Viewport vp;
		readonly Gtk.HBox hbox;
		readonly Gtk.VBox vbox;
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

		public class EtoScrolledWindow : Gtk.ScrolledWindow
		{
#if GTK3
			// does this always work?
			int GetBorderSize() => Math.Max(0, AllocatedHeight - Child.AllocatedHeight);

			protected override void OnAdjustSizeRequest(Gtk.Orientation orientation, out int minimum_size, out int natural_size)
			{
				base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);

				// the natural size of the scrolled window should be the size of the child viewport
				if (Child != null)
				{
					Child.GetPreferredSize(out var ms, out var ns);
					var child_size = orientation == Gtk.Orientation.Horizontal ? ns.Width : ns.Height;
					natural_size = Math.Max(natural_size, child_size + GetBorderSize());
				}
			}
#endif
		}

		public class EtoVBox : Gtk.VBox
		{
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
			Control = new EtoScrolledWindow();
#if GTK3
			// for some reason on mac it doesn't shrink past 47 pixels otherwise
			// also, what is an appropriate size?
			if (EtoEnvironment.Platform.IsMac)
				Control.SetSizeRequest(10, 10);
#endif
			// ensure things are top-left and not centered
			hbox = new Gtk.HBox();

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
				if (Handler.autoSize)
				{
					args.Requisition = Handler.vp.SizeRequest();
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
				Handler.Callback.OnSizeChanged(Handler.Widget, EventArgs.Empty);
			}

			public void HandleScrollableScrollEvent(object sender, EventArgs e)
			{
				Handler.Callback.OnScroll(Handler.Widget, new ScrollEventArgs(Handler.ScrollPosition));
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
				Gdk.Rectangle rect = vp.Allocation;
				int spacing = Convert.ToInt32(Control.StyleGetProperty("scrollbar-spacing")) + 1;
				return new Size(rect.Width - spacing, rect.Height - spacing);
			}
			set
			{
				int spacing = Convert.ToInt32(Control.StyleGetProperty("scrollbar-spacing")) + 1;
				vp.SetSizeRequest(value.Width + spacing, value.Height + spacing);
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

		public Size ScrollSize
		{
			get
			{
				//return scrollSize;
				return new Size((int)(vp.Hadjustment.Upper), (int)(vp.Vadjustment.Upper));
			}
			set
			{
				//scrollSize = value;
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
