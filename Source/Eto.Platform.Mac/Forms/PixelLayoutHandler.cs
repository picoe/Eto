using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Mac
{
	public class PixelLayoutHandler : MacLayout<NSView, PixelLayout>, IPixelLayout
	{
		Dictionary<Control, Point> points = new Dictionary<Control, Point> ();
		
		public override NSView Control {
			get {
				return (NSView)Widget.Container.ContainerObject;
			}
			protected set {
				base.Control = value;
			}
		}
		
		public override SD.RectangleF GetPosition (Control control)
		{
			Point point;
			if (points.TryGetValue (control, out point))
			{
				var frameSize = ((NSView)control.ControlObject).Frame.Size;
				return new SD.RectangleF(Generator.ConvertF(point), frameSize);
			}
			return base.GetPosition (control);
		}
		
		public override void SizeToFit ()
		{
			SD.SizeF size = SD.SizeF.Empty;
			foreach (var item in points) {
				AutoSize (item.Key);
				var frameSize = ((NSView)item.Key.ControlObject).Frame.Size;
				if (size.Width < frameSize.Width + item.Value.X)
					size.Width = frameSize.Width + item.Value.X;
				if (size.Height < frameSize.Height + item.Value.Y)
					size.Height = frameSize.Height + item.Value.Y;
			}
			
			if (size != Control.Frame.Size) {
				SetContainerSize (size);
			}
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			Control.PostsFrameChangedNotifications = true;
			this.AddObserver (NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs e) { 
				var handler = e.Widget.Handler as PixelLayoutHandler;
				handler.Layout ();
			});

		}
		
		public override void OnLoadComplete ()
		{
			base.OnLoadComplete ();
			Layout ();
		}
		
		void SetPosition (Control control, Point point, float frameHeight, bool flipped)
		{
			var offset = ((IMacView)control.Handler).PositionOffset;
			var childView = control.ControlObject as NSView;
			/*if (frameHeight < requiredHeight) {
				frameHeight = requiredHeight;
				Control.SetFrameSize (new SD.SizeF(Control.Frame.Width, frameHeight));
			}*/
			//frameHeight = Math.Max (frameHeight, point.Y + offset.Height + childView.Frame.Height);
			
			SD.PointF origin;
			if (flipped) origin = new System.Drawing.PointF (point.X + offset.Width, point.Y + offset.Height);
			else origin = new System.Drawing.PointF (point.X + offset.Width, frameHeight - (childView.Frame.Height + point.Y + offset.Height));
			//var origin = new System.Drawing.PointF (point.X + offset.Width, point.Y + offset.Height);
			
			//origin.Y = Math.Max (0, origin.Y);
			//Console.WriteLine ("Setting {0} to {1}, translated: {2}", control, point, origin);
			childView.SetFrameOrigin (origin);
		}
		
		void Layout ()
		{
			var controlPoints = points.ToArray ();
			/*var size = SD.SizeF.Empty;
			foreach (var item in controlPoints) {
				var childView = item.Key.ControlObject as NSView;
				var point = item.Value;
				var frameSize = childView.Frame.Size;
				point.X += (int)frameSize.Width;
				point.Y += (int)frameSize.Height;
				if (size.Width < point.X) size.Width = point.X;
				if (size.Height < point.Y) size.Height = point.Y;
			}
			Control.SetFrameSize (size);*/
			var frameHeight = Control.Frame.Height;
			//var frameHeight = Math.Max (Widget.Container.Size.Height, Control.Frame.Height);
			var flipped = Control.IsFlipped;
			foreach (var item in controlPoints) {
				SetPosition (item.Key, item.Value, frameHeight, flipped);
			}
		}
		
		public void Add (Control child, int x, int y)
		{
			var location = new Point (x, y);
			points [child] = location;
			var childView = child.ControlObject as NSView;
			//childView.AutoresizingMask = NSViewResizingMask.MinYMargin;
			if (Widget.Loaded) {
				var frameHeight = Control.Frame.Height;
				//var frameHeight = Math.Max (Widget.Container.Size.Height, Control.Frame.Height);
				SetPosition (child, location, frameHeight, Control.IsFlipped);
			}
			Control.AddSubview (childView);
		}

		public void Move (Control child, int x, int y)
		{
			var location = new Point (x, y);
			points [child] = location;
			if (Widget.Loaded) {
				var frameHeight = Control.Frame.Height;
				//var frameHeight = Math.Max (Widget.Container.Size.Height, Control.Frame.Height);
				SetPosition (child, location, frameHeight, Control.IsFlipped);
			}
		}
		
		public void Remove (Control child)
		{
			var childView = child.ControlObject as NSView;
			points.Remove (child);
			childView.RemoveFromSuperview ();
		}
	}
}
