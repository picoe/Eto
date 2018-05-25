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
		static readonly object ExpandContentWidth_Key = new object();
		static readonly object ExpandContentHeight_Key = new object();
		static readonly object Enabled_Key = new object();
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

			public override void Layout()
			{
				base.Layout();
				Handler?.PerformScrollLayout();
			}
		}

		public class EtoDocumentView : MacPanelView
		{
		}

		protected override NSScrollView CreateControl() => new EtoScrollView(this);

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
							var contentBounds = handler.Control.ContentView.Bounds;
							if (contentBounds.Height > 0)
								handler.scrollPosition = new Point((int)contentBounds.X, (int)Math.Max(0, (view.Frame.Height - contentBounds.Height - contentBounds.Y)));
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
				if (value != Border)
				{
					Control.BorderType = value.ToNS();
					InvalidateMeasure();
				}
			}
		}

		public override NSView ContentControl => (NSView)Control.DocumentView;

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			Control.NeedsLayout = true;
		}


		Size GetBorderSize() => Border == BorderType.None ? Size.Empty : new Size(2, 2);

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var naturalSize = NaturalSize;
			if (naturalSize != null)
				return naturalSize.Value;
			
			var border = Padding.Size + GetBorderSize();
			naturalSize = SizeF.Min(availableSize, base.GetNaturalSize(availableSize - border) + border);
			NaturalSize = naturalSize;
			return naturalSize.Value;
		}

		public void UpdateScrollSizes()
		{
			InvalidateMeasure();
		}

		void PerformScrollLayout()
		{
			var clientSize = ClientSize;
			var size = SizeF.Empty;
			if (DesiredScrollSize != null)
			{
				// user set desired scroll size
				SizeF availableSize = DesiredScrollSize.Value;
				if (ExpandContentWidth && availableSize.Width >= 0)
					availableSize.Width = size.Width = Math.Max(clientSize.Width, availableSize.Width);
				else if (availableSize.Width < 0)
					availableSize.Width = float.PositiveInfinity;
				if (ExpandContentHeight && availableSize.Height >= 0)
					availableSize.Height = size.Height = Math.Max(clientSize.Height, availableSize.Height);
				else if (availableSize.Height < 0)
					availableSize.Height = float.PositiveInfinity;

				var preferred = Content.GetPreferredSize(availableSize);

				if (availableSize.Width < 0)
					size.Width = preferred.Width;
				if (availableSize.Height < 0)
					size.Height = preferred.Height;
			}
			else
			{
				var preferred = Content.GetPreferredSize(SizeF.PositiveInfinity);
				size = SizeF.Max(clientSize, preferred);
			}
			ContentControl.SetFrameSize(size.ToNS());
			SetPosition(scrollPosition, true);
		}

		public override void PerformContentLayout()
		{
			var ctl = Content.GetContainerView();
			if (ctl == null)
				return;
			
			var size = Content.GetPreferredSize(SizeF.PositiveInfinity).ToNS();

			var clientSize = Control.DocumentVisibleRect.Size;

			if (ExpandContentWidth)
				size.Width = (nfloat)Math.Max(clientSize.Width, size.Width);
			if (ExpandContentHeight)
				size.Height = (nfloat)Math.Max(clientSize.Height, size.Height);

			var clientFrame = new CGRect(new CGPoint(0, (nfloat)Math.Max(0, clientSize.Height - size.Height)), size);

			var padding = Padding;
			padding.Bottom = (int)Math.Max(0, Math.Min(clientSize.Height - size.Height, padding.Bottom));
			ctl.Frame = clientFrame.WithPadding(padding);
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

		static readonly object DesiredScrollSize_Key = new object();

		Size? DesiredScrollSize
		{
			get => Widget.Properties.Get<Size?>(DesiredScrollSize_Key);
			set => Widget.Properties.Set(DesiredScrollSize_Key, value);
		}

		public Size ScrollSize
		{
			get => ContentControl.Frame.Size.ToEtoSize();
			set
			{
				DesiredScrollSize = value.Width >= 0 || value.Height >= 0 ? (Size?)value : null;
				InvalidateMeasure();
			}
		}

		public override Size ClientSize
		{
			get => Control.DocumentVisibleRect.Size.ToEtoSize();
			set
			{
				var size = value;
				var borderSize = GetBorderSize();
				if (size.Width >= 0)
					size.Width += borderSize.Width;
				if (size.Height >= 0)
					size.Height += borderSize.Height;
				base.ClientSize = size;
			}
		}

		public override bool Enabled
		{
			get => Widget.Properties.Get<bool>(Enabled_Key, true);
			set => Widget.Properties.Set(Enabled_Key, value, true);
		}

		public Rectangle VisibleRect => new Rectangle(ScrollPosition, Size.Min(ScrollSize, ClientSize));

		public bool ExpandContentWidth
		{
			get => Widget.Properties.Get<bool>(ExpandContentWidth_Key, true);
			set
			{
				if (ExpandContentWidth != value)
				{
					Widget.Properties.Set(ExpandContentWidth_Key, value, true);
					InvalidateMeasure();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get => Widget.Properties.Get<bool>(ExpandContentHeight_Key, true);
			set
			{
				if (ExpandContentHeight != value)
				{
					Widget.Properties.Set(ExpandContentHeight_Key, value, true);
					InvalidateMeasure();
				}
			}
		}

		public float MinimumZoom { get { return 1f; } set { } }

		public float MaximumZoom { get { return 1f; } set { } }

		public float Zoom { get { return 1f; } set { } }
	}
}
