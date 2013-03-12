using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.Foundation;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ScrollableHandler : MacContainer<NSScrollView, Scrollable>, IScrollable
	{
		NSScrollView control;
		NSView view;
		bool expandContentWidth = true;
		bool expandContentHeight = true;

		class EtoScrollView : NSScrollView, IMacControl
		{
			object IMacControl.Handler { get { return Handler; } }
			public ScrollableHandler Handler { get; set; }
			
			public override void ResetCursorRects ()
			{
				var cursor = Handler.Cursor;
				if (cursor != null)
					this.AddCursorRect (new SD.RectangleF (SD.PointF.Empty, this.Frame.Size), cursor.ControlObject as NSCursor);
			}
			
		}

		class FlippedView : NSView
		{
			public override bool IsFlipped {
				get { return true; }
			}
		}

		public ScrollableHandler ()
		{
			Enabled = true;
			control = new EtoScrollView { Handler = this };
			control.BackgroundColor = MonoMac.AppKit.NSColor.Control;
			control.BorderType = NSBorderType.BezelBorder;
			control.DrawsBackground = false;
			control.HasVerticalScroller = true;
			control.HasHorizontalScroller = true;
			control.AutohidesScrollers = true;
			view = new FlippedView ();
			control.DocumentView = view;
			Control = control;
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Scrollable.ScrollEvent:
				Control.ContentView.PostsBoundsChangedNotifications = true;
				this.AddObserver (NSView.NSViewBoundsDidChangeNotification, e => {
					var w = (Scrollable)e.Widget;
					w.OnScroll (new ScrollEventArgs (w.ScrollPosition));
				}, Control.ContentView);
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public BorderType Border {
			get {
				switch (control.BorderType) {
				case NSBorderType.BezelBorder:
					return BorderType.Bezel;
				case NSBorderType.LineBorder:
					return BorderType.Line;
				case NSBorderType.NoBorder:
					return BorderType.None;
				default:
					throw new NotSupportedException ();
				}
			}
			set {
				switch (value) {
				case BorderType.Bezel:
					control.BorderType = NSBorderType.BezelBorder;
					break;
				case BorderType.Line:
					control.BorderType = NSBorderType.LineBorder;
					break;
				case BorderType.None:
					control.BorderType = NSBorderType.NoBorder;
					break;
				default:
					throw new NotSupportedException ();
				}
			}
		}
		
		public override object ContainerObject {
			get { return view; }
		}
		
		public override void LayoutChildren ()
		{
			base.LayoutChildren ();
			UpdateScrollSizes ();
		}
		
		Size GetBorderSize ()
		{
			return Control.Frame.Size.ToEtoSize () - Control.DocumentVisibleRect.Size.ToEtoSize ();
		}

		public override Size GetPreferredSize (Size availableSize)
		{
			return Size.Min (base.GetPreferredSize (availableSize), availableSize);
		}

		protected override Size GetNaturalSize ()
		{
			return base.GetNaturalSize () + GetBorderSize ();
		}
		
		void InternalSetFrameSize (SD.SizeF size)
		{
			if (size != view.Frame.Size) {
				view.SetFrameSize (size);
			}
		}
		
		public void UpdateScrollSizes ()
		{
			var contentSize = base.GetNaturalSize ();

			if (ExpandContentWidth)
				contentSize.Width = Math.Max (this.ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = Math.Max (this.ClientSize.Height, contentSize.Height);

			InternalSetFrameSize (contentSize.ToSDSizeF ());
		}
		
		public override Color BackgroundColor {
			get {
				return Control.BackgroundColor.ToEto ();
			}
			set {
				Control.BackgroundColor = value.ToNS ();
				Control.DrawsBackground = value.A > 0;
			}
		}
		
		public Point ScrollPosition {
			get { 
				var loc = Control.ContentView.Bounds.Location;
				if (view.IsFlipped)
					return loc.ToEtoPoint ();
				else
					return new Point ((int)loc.X, (int)(view.Frame.Height - Control.ContentView.Frame.Height - loc.Y));
			}
			set { 
				if (view.IsFlipped)
					Control.ContentView.ScrollToPoint (value.ToSDPointF ());
				else
					Control.ContentView.ScrollToPoint (new SD.PointF (value.X, view.Frame.Height - Control.ContentView.Frame.Height - value.Y));
				Control.ReflectScrolledClipView (Control.ContentView);
			}
		}

		public Size ScrollSize {			
			get { return view.Frame.Size.ToEtoSize (); }
			set { 
				InternalSetFrameSize (value.ToSDSizeF ());
			}
		}
		
		public override Size ClientSize {
			get {
				return Control.DocumentVisibleRect.Size.ToEtoSize ();
			}
			set {
				
			}
		}
		
		public override bool Enabled { get; set; }
		
		public override void SetContentSize (SD.SizeF contentSize)
		{
			if (MinimumSize != null) {
				contentSize.Width = Math.Max (contentSize.Width, MinimumSize.Value.Width);
				contentSize.Height = Math.Max (contentSize.Height, MinimumSize.Value.Height);
			}
			if (ExpandContentWidth)
				contentSize.Width = Math.Max (this.ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = Math.Max (this.ClientSize.Height, contentSize.Height);
			InternalSetFrameSize (contentSize);
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			UpdateScrollSizes ();
			this.Widget.SizeChanged += (sender, ee) => {
				UpdateScrollSizes ();
			};
		}
		
		public Rectangle VisibleRect {
			get { return new Rectangle (ScrollPosition, Size.Min (ScrollSize, ClientSize)); }
		}

		public bool ExpandContentWidth
		{
			get { return expandContentWidth; }
			set {
				if (expandContentWidth != value) {
					expandContentWidth = value;
					UpdateScrollSizes ();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get { return expandContentHeight; }
			set {
				if (expandContentHeight != value) {
					expandContentHeight = value;
					UpdateScrollSizes ();
				}
			}
		}
	}
}
