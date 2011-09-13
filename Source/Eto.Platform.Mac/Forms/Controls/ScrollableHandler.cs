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
		
		public ScrollableHandler ()
		{
			control = new NSScrollView();
			control.BackgroundColor = MonoMac.AppKit.NSColor.Control;
			control.BorderType = NSBorderType.BezelBorder;
			control.HasVerticalScroller = true;
			control.HasHorizontalScroller = true;
			control.AutohidesScrollers = false;
			view = new NSView ();
			control.DocumentView = view;
			Control = control;
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
				var oldpos = ScrollPosition;
				view.SetFrameSize (size);
				ScrollPosition = oldpos;
			}
		}
		
		public void UpdateScrollSizes ()
		{
			Control.Tile ();
			SD.SizeF size = SD.SizeF.Empty;
			var layout = Widget.Layout.Handler as IMacLayout;
			if (layout != null) {
				foreach (var c in Widget.Controls) {
					var frame = layout.GetPosition (c);
					if (size.Width < frame.Right)
						size.Width = frame.Right;
					if (size.Height < frame.Bottom)
						size.Height = frame.Bottom;
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
				return new Point ((int)loc.X, (int)Math.Max (0, (view.Frame.Height - Control.ContentView.Frame.Height - loc.Y)));
			}
			set { 
				Control.ContentView.ScrollToPoint (new SD.PointF (value.X, Math.Max (0, view.Frame.Height - Control.ContentView.Frame.Height - value.Y)));
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
				if ((Control.AutoresizingMask & (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable)) == 0) {
					Control.SetFrameSize (contentSize);
				}
			}
		}
	}
}
