using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public class ScrollableHandler : MacContainer<NSScrollView, Scrollable>, IScrollable
	{
		NSScrollView control;
		NSView view;
		
		class MyScrollView : NSScrollView
		{
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
			control = new MyScrollView { Handler = this };
			control.BackgroundColor = MonoMac.AppKit.NSColor.Control;
			control.BorderType = NSBorderType.BezelBorder;
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
				this.AddObserver (NSView.NSViewBoundsDidChangeNotification, delegate(ObserverActionArgs e) {
					e.Widget.OnScroll (new ScrollEventArgs (e.Widget.ScrollPosition));
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
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			UpdateScrollSizes ();
		}
		
		void InternalSetFrameSize (SD.SizeF size)
		{
			if (size != view.Frame.Size) {
				view.SetFrameSize (size);
			}
		}
		
		public void UpdateScrollSizes ()
		{
			Control.Tile ();
			SD.SizeF size = SD.SizeF.Empty;
			if (Widget.Layout != null) {
				var layout = Widget.Layout.InnerLayout.Handler as IMacLayout;
				if (layout != null) {
					foreach (var c in Widget.Controls) {
						var frame = layout.GetPosition (c);
						if (size.Width < frame.Right)
							size.Width = frame.Right;
						if (size.Height < frame.Bottom)
							size.Height = frame.Bottom;
					}
				}
			}
			InternalSetFrameSize (size);
		}
		
		public override Color BackgroundColor {
			get {
				return Generator.Convert (Control.BackgroundColor);
			}
			set {
				Control.BackgroundColor = Generator.ConvertNS (value);
			}
		}
		
		public Point ScrollPosition {
			get { 
				var loc = Control.ContentView.Bounds.Location;
				if (view.IsFlipped)
					return Generator.ConvertF (loc);
				else
					return new Point ((int)loc.X, (int)(view.Frame.Height - Control.ContentView.Frame.Height - loc.Y));
			}
			set { 
				if (view.IsFlipped)
					Control.ContentView.ScrollToPoint (Generator.ConvertF (value));
				else
					Control.ContentView.ScrollToPoint (new SD.PointF (value.X, view.Frame.Height - Control.ContentView.Frame.Height - value.Y));
				Control.ReflectScrolledClipView (Control.ContentView);
			}
		}

		public Size ScrollSize {			
			get { return Generator.ConvertF (view.Frame.Size); }
			set { 
				InternalSetFrameSize (Generator.ConvertF (value));
			}
		}
		
		public override Size ClientSize {
			get {
				return Generator.ConvertF (Control.DocumentVisibleRect.Size);
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
			InternalSetFrameSize (contentSize);
			if (this.AutoSize) {
				contentSize.Width += 2;
				contentSize.Height += 2;
				
				if ((Control.AutoresizingMask & (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable)) == (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable)) {
					if (Widget.ParentLayout != null) {
						var layout = Widget.ParentLayout.InnerLayout.Handler as IMacLayout;
						if (layout != null)
							layout.SetContainerSize (contentSize);
					}
				} else
					Control.SetFrameSize (contentSize);
			}
		}
		
		public Rectangle VisibleRect {
			get { return new Rectangle (ScrollPosition, Size.Min (ScrollSize, ClientSize)); }
		}
	}
}
