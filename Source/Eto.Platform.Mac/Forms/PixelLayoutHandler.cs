using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms
{
	public class PixelLayoutHandler : MacContainer<NSView, PixelLayout>, IPixelLayout
	{
		bool loaded;
		Dictionary<Control, Point> points = new Dictionary<Control, Point> ();

		public override NSView ContainerControl { get { return Control; } }

		public PixelLayoutHandler()
		{
			Control = new NSView();
		}
		
		public SD.RectangleF GetPosition (Control control)
		{
			Point point;
			if (points.TryGetValue (control, out point)) {
				var frameSize = ((NSView)control.ControlObject).Frame.Size;
				return new SD.RectangleF (point.ToSDPointF (), frameSize);
			}
			return control.GetContainerView().Frame;
		}
		
		public override Size GetPreferredSize (Size availableSize)
		{
			Size size = Size.Empty;
			foreach (var item in points) {
				var frameSize = item.Key.GetPreferredSize (availableSize);
				size = Size.Max (size, frameSize + new Size (item.Value));
			}
			return size;
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			Control.PostsFrameChangedNotifications = true;
			this.AddObserver (NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs oa) { 
				var handler = oa.Widget.Handler as PixelLayoutHandler;
				handler.LayoutChildren ();
			}
			);
			loaded = true;
		}
		
		void SetPosition (Control control, Point point, float frameHeight, bool flipped)
		{
			var offset = ((IMacViewHandler)control.Handler).PositionOffset;
			var childView = control.GetContainerView ();
			
			var preferredSize = control.GetPreferredSize (Control.Frame.Size.ToEtoSize ());

			SD.PointF origin;
			if (flipped)
				origin = new System.Drawing.PointF (
					point.X + offset.Width,
					point.Y + offset.Height
				);
			else
				origin = new System.Drawing.PointF (
					point.X + offset.Width,
					frameHeight - (preferredSize.Height + point.Y + offset.Height)
				);
			
			var frame = new SD.RectangleF (origin, preferredSize.ToSDSizeF ());
			if (frame != childView.Frame) {
				childView.Frame = frame;
			}
		}

		public override void LayoutChildren ()
		{
			var controlPoints = points.ToArray ();
			var frameHeight = Control.Frame.Height;
			var flipped = Control.IsFlipped;
			foreach (var item in controlPoints) {
				SetPosition (item.Key, item.Value, frameHeight, flipped);
			}
		}
		
		public void Add (Control child, int x, int y)
		{
			var location = new Point (x, y);
			points [child] = location;
			var childView = child.GetContainerView ();
			if (loaded) {
				var frameHeight = Control.Frame.Height;
				SetPosition (child, location, frameHeight, Control.IsFlipped);
			}
			Control.AddSubview (childView);
			if (loaded)
				LayoutParent ();
		}

		public void Move (Control child, int x, int y)
		{
			var location = new Point (x, y);
			if (points [child] != location) {
				points [child] = location;
				if (loaded) {
					var frameHeight = Control.Frame.Height;
					SetPosition (child, location, frameHeight, Control.IsFlipped);
					LayoutParent ();
				}
			}
		}
		
		public void Remove (Control child)
		{
			var childView = child.GetContainerView ();
			points.Remove (child);
			childView.RemoveFromSuperview ();
			if (loaded)
				LayoutParent ();
		}
	}
}
