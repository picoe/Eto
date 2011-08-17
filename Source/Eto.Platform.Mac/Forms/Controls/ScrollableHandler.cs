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
			control = new NSScrollView ();
			control.BackgroundColor = MonoMac.AppKit.NSColor.Control;
			control.BorderType = NSBorderType.BezelBorder;
			control.HasVerticalScroller = true;
			control.HasHorizontalScroller = true;
			control.AutohidesScrollers = true;
			//control.AutoresizingMask = NSViewResizingMask.NotSizable;
			view = new FlippedView ();
			control.DocumentView = view;
			Control = control;
		}
		
		public BorderType Border {
			get {
				switch (control.BorderType)
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
			set {
				switch (value)
				{
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
					throw new NotSupportedException();
				}
			}
		}

		public override object ContainerObject {
			get { return view; }
		}
		
		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			UpdateScrollSizes ();
		}

		public void UpdateScrollSizes ()
		{
			//Control.UpdateTrackingAreas();
			//control.PerformLayout();
			Control.Tile ();
			SD.SizeF size = new SD.SizeF (0, 0);
			foreach (var c in view.Subviews) {
				var frame = c.Frame;
				if (size.Width < frame.Right)
					size.Width = frame.Right;
				if (size.Height < frame.Bottom)
					size.Height = frame.Bottom;
			}
			if (size != view.Frame.Size) {
				//Console.WriteLine ("***Size {0}", size);
				view.SetFrameSize (size);
			}
			//Control.ReflectScrolledClipView (Control.ContentView);
			//Control.UpdateTrackingAreas ();
			//view.ScrollPoint (SD.PointF.Empty);
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
				//var newpt = view.ConvertPointToView (Control.DocumentVisibleRect.Location, Control.ContentView); //..ConvertPointFromView (Control.DocumentVisibleRect.Location, view);
				//Console.WriteLine (" - Pos: {0}, {1}, {2}, {3}", Control.VerticalScroller.FloatValue * (view.Frame.Height - Control.Bounds.Height), Control.DocumentVisibleRect.Location, view.VisibleRect (), Control.ContentView.Bounds);
				
				return Generator.ConvertF (Control.ContentView.Bounds.Location);
				//return Generator.ConvertF (Control.DocumentVisibleRect.Location);
			}
			set { 
				//Control.ScrollsDynamically = false;
				/*CATransaction.Begin ();
				CATransaction.DisableActions = true;
				CATransaction.AnimationDuration = 0;
				*/
				//NSAnimationContext.CurrentContext.Duration = 0;
				//if (value.X == 0 && value.Y == 0)
				{
					Control.ContentView.ScrollToPoint (Generator.ConvertF (value));
					Control.ReflectScrolledClipView (Control.ContentView);
				}
				//else view.ScrollPoint (Generator.ConvertF (value));
				//var pt = Control.ConvertPointToView (Generator.ConvertF (value), view);
				//var pt = view.ConvertPointToView (Generator.ConvertF (value), Control.ContentView);
				//var pt = Generator.ConvertF (value);
				//Control.ContentView.ScrollToPoint (pt);
				//CATransaction.Commit ();
				//Control.ReflectScrolledClipView (Control.ContentView);
				
				//Console.WriteLine ("Set To: {0}, {1}. {2}", value, view.Frame, Control.Frame.Size);
				
				//Control.ScrollsDynamically = true;

				//Control.ReflectScrolledClipView (Control.ContentView);
				//Control.UpdateTrackingAreas ();
			}
		}

		public Size ScrollSize {			
			get { return Generator.ConvertF (view.Frame.Size); }
			set { 
				view.SetFrameSize (Generator.ConvertF (value)); 
				Control.Tile (); 
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
			//base.SetContentSize (contentSize);
			view.SetFrameSize (contentSize);
			if (this.AutoSize) {
				contentSize.Width += 2;
				contentSize.Height += 2;
				Control.SetFrameSize (contentSize);
			}
		}
	}
}
