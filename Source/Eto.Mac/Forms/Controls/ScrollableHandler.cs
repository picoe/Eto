// can't use flipped right now - bug in monomac/xam.mac that causes crashing since FlippedView gets disposed incorrectly
// has something to do with using layers (background colors) at the same time
//#define USE_FLIPPED

using System;
using Eto.Drawing;
using Eto.Forms;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class ScrollableHandler : MacPanel<NSScrollView, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		bool expandContentWidth = true;
		bool expandContentHeight = true;
		Point scrollPosition;

		public override NSView ContainerControl { get { return Control; } }

		public class EtoScrollView : NSScrollView, IMacControl
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
					AddCursorRect(new CGRect(CGPoint.Empty, Frame.Size), cursor.ControlObject as NSCursor);
			}

			public EtoScrollView(ScrollableHandler handler)
			{
				BackgroundColor = NSColor.Clear;
				BorderType = NSBorderType.BezelBorder;
				DrawsBackground = false;
				HasVerticalScroller = true;
				HasHorizontalScroller = true;
				AutohidesScrollers = true;
				// only draw dirty regions, instead of entire scroll area
				ContentView.CopiesOnScroll = true;
				DocumentView = new EtoDocumentView { Handler = handler };
			}
		}

		public class EtoDocumentView : NSView
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
			public override void SetFrameSize(CGSize newSize)
			{
				base.SetFrameSize(newSize);
				Handler.SetPosition(Handler.scrollPosition, true);
			}
			#endif

		}

		protected override NSScrollView CreateControl()
		{
			return new EtoScrollView(this);
		}

		protected override void Initialize()
		{
			Enabled = true;

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
									handler.scrollPosition = new Point((int)contentBounds.X, (int)Math.Max(0, (view.Frame.Height - contentBounds.Height - contentBounds.Y)));
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
			get { return Control.BorderType.ToEto(); }
			set
			{
				Control.BorderType = value.ToNS();
				LayoutIfNeeded();
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
			var border = Padding.Size + GetBorderSize();
			return SizeF.Min(availableSize, base.GetNaturalSize(availableSize - border) + border);
		}

		protected override CGRect GetContentBounds()
		{
			var contentSize = Content.GetPreferredSize(SizeF.MaxValue);

			if (ExpandContentWidth)
				contentSize.Width = Math.Max(ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = Math.Max(ClientSize.Height, contentSize.Height);
			return new RectangleF(contentSize).ToNS();
		}

		protected override NSViewResizingMask ContentResizingMask()
		{
			return ContentControl.IsFlipped ? base.ContentResizingMask() : (NSViewResizingMask)0;
		}

		void InternalSetFrameSize(CGSize size)
		{
			var view = ContentControl;
			if (!view.IsFlipped)
			{
				var ctl = Content.GetContainerView();
				if (ctl != null)
				{
					var clientHeight = Control.DocumentVisibleRect.Size.Height;
					ctl.Frame = new CGRect(new CGPoint(0, (nfloat)Math.Max(0, clientHeight - size.Height)), size);
					size.Height = (nfloat)Math.Max(clientHeight, size.Height);
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
					Control.ContentView.ScrollToPoint(value.ToNS());
				else if (Control.ContentView.Frame.Height > 0)
					Control.ContentView.ScrollToPoint(new CGPoint(value.X, (nfloat)Math.Max(0, view.Frame.Height - Control.ContentView.Frame.Height - value.Y)));
				Control.ReflectScrolledClipView(Control.ContentView);
			}
			scrollPosition = value;
		}

		public Size ScrollSize
		{			
			get { return ContentControl.Frame.Size.ToEtoSize(); }
			set
			{ 
				InternalSetFrameSize(value.ToNS());
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

		public override void SetContentSize(CGSize contentSize)
		{
			if (MinimumSize != Size.Empty)
			{
				contentSize.Width = (nfloat)Math.Max(contentSize.Width, MinimumSize.Width);
				contentSize.Height = (nfloat)Math.Max(contentSize.Height, MinimumSize.Height);
			}
			if (ExpandContentWidth)
				contentSize.Width = (nfloat)Math.Max(ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = (nfloat)Math.Max(ClientSize.Height, contentSize.Height);
			InternalSetFrameSize(contentSize);
		}

		#if !USE_FLIPPED
		public override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			UpdateScrollSizes();
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
