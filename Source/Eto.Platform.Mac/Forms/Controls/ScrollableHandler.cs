using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ScrollableHandler : MacDockContainer<NSScrollView, Scrollable>, IScrollable
	{
		bool expandContentWidth = true;
		bool expandContentHeight = true;

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
			public override bool IsFlipped
			{
				get { return true; }
			}
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
				DocumentView = new FlippedView()
			};
			// only draw dirty regions, instead of entire scroll area
			Control.ContentView.CopiesOnScroll = true;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Scrollable.ScrollEvent:
					Control.ContentView.PostsBoundsChangedNotifications = true;
					AddObserver(NSView.BoundsChangedNotification, e =>
					{
						var w = (Scrollable)e.Widget;
						w.OnScroll(new ScrollEventArgs(w.ScrollPosition));
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

		Size GetBorderSize()
		{
			return Border == BorderType.None ? Size.Empty : new Size(2, 2);
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return SizeF.Min(availableSize, base.GetNaturalSize(availableSize) + GetBorderSize());
		}

		void InternalSetFrameSize(SD.SizeF size)
		{
			var view = (NSView)Control.DocumentView;
			if (size != view.Frame.Size)
			{
				view.SetFrameSize(size);
			}
		}

		public void UpdateScrollSizes()
		{
			var contentSize = Content.GetPreferredSize(Size.MaxValue);

			if (ExpandContentWidth)
				contentSize.Width = Math.Max(ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = Math.Max(ClientSize.Height, contentSize.Height);

			InternalSetFrameSize(contentSize.ToSD());
		}

		public override Color BackgroundColor
		{
			get
			{
				return Control.BackgroundColor.ToEto();
			}
			set
			{
				Control.BackgroundColor = value.ToNS();
				Control.DrawsBackground = value.A > 0;
			}
		}

		public Point ScrollPosition
		{
			get
			{ 
				var view = (NSView)Control.DocumentView;
				var loc = Control.ContentView.Bounds.Location;
				if (view.IsFlipped)
					return loc.ToEtoPoint();
				else
					return new Point((int)loc.X, (int)(view.Frame.Height - Control.ContentView.Frame.Height - loc.Y));
			}
			set
			{ 
				var view = (NSView)Control.DocumentView;
				if (view.IsFlipped)
					Control.ContentView.ScrollToPoint(value.ToSDPointF());
				else
					Control.ContentView.ScrollToPoint(new SD.PointF(value.X, view.Frame.Height - Control.ContentView.Frame.Height - value.Y));
				Control.ReflectScrolledClipView(Control.ContentView);
			}
		}

		public Size ScrollSize
		{			
			get { return ((NSView)Control.DocumentView).Frame.Size.ToEtoSize(); }
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
	}
}
