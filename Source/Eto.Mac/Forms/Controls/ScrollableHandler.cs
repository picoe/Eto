// can't use flipped right now - bug in monomac/xam.mac that causes crashing since FlippedView gets disposed incorrectly
// has something to do with using layers (background colors) at the same time
//#define USE_FLIPPED

using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac.Forms.Controls
{
	public class ScrollableHandler : MacPanel<NSScrollView, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		bool expandContentWidth = true;
		bool expandContentHeight = true;
		Point scrollPosition;

		public override NSView ContainerControl { get { return Control; } }

		class EtoScrollView : NSScrollView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public ScrollableHandler Handler
			{ 
				get { return (ScrollableHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public override void ResetCursorRects()
			{
				var cursor = Handler.Cursor;
				if (cursor != null)
					AddCursorRect(new SD.RectangleF(SD.PointF.Empty, Frame.Size), cursor.ControlObject as NSCursor);
			}
		}

		class FlippedView : NSView
		{
			public WeakReference WeakHandler { get; set; }

			public ScrollableHandler Handler
			{ 
				get { return (ScrollableHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
			#if USE_FLIPPED
			public override bool IsFlipped
			{
				get { return true; }
			}
			#endif
			#if !USE_FLIPPED
			public override void SetFrameSize(SD.SizeF newSize)
			{
				base.SetFrameSize(newSize);
				Handler.SetPosition(Handler.scrollPosition, true);
			}
			#endif
		}

		public ScrollableHandler()
		{
			Enabled = true;
			Control = new EtoScrollView
			{
				Handler = this, 
				BackgroundColor = NSColor.Control,
				BorderType = NSBorderType.BezelBorder,
				DrawsBackground = false,
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				DocumentView = new FlippedView { Handler = this }
			};

			// only draw dirty regions, instead of entire scroll area
			Control.ContentView.CopiesOnScroll = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			if (!ContentControl.IsFlipped)
				// need to keep the scroll position as it scrolls instead of calculating
				HandleEvent(Scrollable.ScrollEvent);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Scrollable.ScrollEvent:
					Control.ContentView.PostsBoundsChangedNotifications = true;
					AddObserver(NSView.BoundsChangedNotification, e =>
					{
						var handler = e.Handler as ScrollableHandler;
						if (handler != null)
						{
							var view = handler.ContentControl;
							if (!view.IsFlipped)
							{
								var contentBounds = handler.Control.ContentView.Bounds;
								if (contentBounds.Height > 0)
									handler.scrollPosition = new Point((int)contentBounds.X, (int)(view.Frame.Height - contentBounds.Height - contentBounds.Y));
							}
							handler.Callback.OnScroll(handler.Widget, new ScrollEventArgs(handler.ScrollPosition));
						}
					}, Control.ContentView);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public BorderType Border
		{
			get
			{
				switch (Control.BorderType)
				{
					case NSBorderType.BezelBorder:
						return BorderType.Bezel;
					case NSBorderType.LineBorder:
						return BorderType.Line;
					case NSBorderType.NoBorder:
						return BorderType.None;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case BorderType.Bezel:
						Control.BorderType = NSBorderType.BezelBorder;
						break;
					case BorderType.Line:
						Control.BorderType = NSBorderType.LineBorder;
						break;
					case BorderType.None:
						Control.BorderType = NSBorderType.NoBorder;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public override NSView ContentControl
		{
			get { return (NSView)Control.DocumentView; }
		}

		public override void LayoutChildren()
		{
			base.LayoutChildren();
			UpdateScrollSizes();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			UpdateScrollSizes();
		}

		Size GetBorderSize()
		{
			return Border == BorderType.None ? Size.Empty : new Size(2, 2);
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return SizeF.Min(availableSize, base.GetNaturalSize(availableSize) + GetBorderSize());
		}

		protected override SD.RectangleF GetContentBounds()
		{
			var contentSize = Content.GetPreferredSize(SizeF.MaxValue);

			if (ExpandContentWidth)
				contentSize.Width = Math.Max(ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = Math.Max(ClientSize.Height, contentSize.Height);
			return new RectangleF(contentSize).ToSD();
		}

		protected override NSViewResizingMask ContentResizingMask()
		{
			return ContentControl.IsFlipped ? base.ContentResizingMask() : (NSViewResizingMask)0;
		}

		void InternalSetFrameSize(SD.SizeF size)
		{
			var view = ContentControl;
			if (!view.IsFlipped)
			{
				var ctl = Content.GetContainerView();
				if (ctl != null)
				{
					var clientHeight = Control.DocumentVisibleRect.Size.Height;
					ctl.Frame = new SD.RectangleF(new SD.PointF(0, Math.Max(0, clientHeight - size.Height)), size);
					size.Height = Math.Max(clientHeight, size.Height);
				}
			}
			if (size != view.Frame.Size)
			{
				view.SetFrameSize(size);
			}
		}

		public void UpdateScrollSizes()
		{
			InternalSetFrameSize(GetContentBounds().Size);
		}

		public override Color BackgroundColor
		{
			get
			{
				return Control.BackgroundColor.ToEto();
			}
			set
			{
				Control.BackgroundColor = value.ToNSUI();
				Control.DrawsBackground = value.A > 0;
			}
		}

		public Point ScrollPosition
		{
			get
			{ 
				var view = ContentControl;
				if (Widget.Loaded && view.IsFlipped)
				{
					return Control.ContentView.Bounds.Location.ToEtoPoint();
				}
				return scrollPosition;
			}
			set
			{ 
				SetPosition(value, false);
			}
		}

		void SetPosition(Point value, bool force)
		{
			if (Widget.Loaded || force)
			{
				var view = ContentControl;
				if (view.IsFlipped)
					Control.ContentView.ScrollToPoint(value.ToSDPointF());
				else if (Control.ContentView.Frame.Height > 0)
					Control.ContentView.ScrollToPoint(new SD.PointF(value.X, Math.Max(0, view.Frame.Height - Control.ContentView.Frame.Height - value.Y)));
				Control.ReflectScrolledClipView(Control.ContentView);
			}
			scrollPosition = value;
		}

		public Size ScrollSize
		{			
			get { return ContentControl.Frame.Size.ToEtoSize(); }
			set
			{ 
				InternalSetFrameSize(value.ToSDSizeF());
			}
		}

		public override Size ClientSize
		{
			get
			{
				return Control.DocumentVisibleRect.Size.ToEtoSize();
			}
			set
			{
				
			}
		}

		public override bool Enabled { get; set; }

		public override void SetContentSize(SD.SizeF contentSize)
		{
			if (MinimumSize != Size.Empty)
			{
				contentSize.Width = Math.Max(contentSize.Width, MinimumSize.Width);
				contentSize.Height = Math.Max(contentSize.Height, MinimumSize.Height);
			}
			if (ExpandContentWidth)
				contentSize.Width = Math.Max(ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = Math.Max(ClientSize.Height, contentSize.Height);
			InternalSetFrameSize(contentSize);
		}

		#if !USE_FLIPPED
		public override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			SetPosition(scrollPosition, true);
		}
		#endif

		public Rectangle VisibleRect
		{
			get { return new Rectangle(ScrollPosition, Size.Min(ScrollSize, ClientSize)); }
		}

		public bool ExpandContentWidth
		{
			get { return expandContentWidth; }
			set
			{
				if (expandContentWidth != value)
				{
					expandContentWidth = value;
					UpdateScrollSizes();
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
					UpdateScrollSizes();
				}
			}
		}

		public float MinimumZoom { get { return 1f; } set { } }

		public float MaximumZoom { get { return 1f; } set { } }

		public float Zoom { get { return 1f; } set { } }
	}
}
