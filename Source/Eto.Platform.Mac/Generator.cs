using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using MonoMac.AppKit;
using Eto.Platform.Mac.Drawing;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Eto.Platform.Mac.IO;
using System.Threading;

namespace Eto.Platform.Mac
{
	public class Generator : Eto.Generator
	{ 	
		public override string ID {
			get {
				return "mac";
			}
		}
		
		public static Point GetLocation (NSView view, NSEvent theEvent)
		{
			var loc = view.ConvertPointFromView (theEvent.LocationInWindow, null);
			if (!view.IsFlipped)
				loc.Y = view.Frame.Height - loc.Y;
			return Generator.ConvertF (loc);
		}
		
		public override IDisposable ThreadStart ()
		{
			return new NSAutoreleasePool ();
		}
		
		public static System.Drawing.Size Convert (Size size)
		{
			return new System.Drawing.Size (size.Width, size.Height);
		}

		public static Size Convert (System.Drawing.Size size)
		{
			return new Size (size.Width, size.Height);
		}

		public static System.Drawing.Point Convert (Point point)
		{
			return new System.Drawing.Point (point.X, point.Y);
		}

		public static Point Convert (System.Drawing.Point point)
		{
			return new Point (point.X, point.Y);
		}

		public static System.Drawing.SizeF ConvertF (Size size)
		{
			return new System.Drawing.SizeF (size.Width, size.Height);
		}

		public static Size ConvertF (System.Drawing.SizeF size)
		{
			return new Size ((int)size.Width, (int)size.Height);
		}
		
		public static System.Drawing.RectangleF ConvertF (System.Drawing.RectangleF frame, Size size)
		{
			frame.Size = ConvertF (size);
			return frame;
		}

		public static Rectangle ConvertF (System.Drawing.RectangleF rect)
		{
			return new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		public static System.Drawing.RectangleF ConvertF (Rectangle rect)
		{
			return new System.Drawing.RectangleF ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}
		
		public static Point ConvertF (System.Drawing.PointF point)
		{
			return new Point ((int)point.X, (int)point.Y);
		}

		public static System.Drawing.PointF ConvertF (Point point)
		{
			return new System.Drawing.PointF ((int)point.X, (int)point.Y);
		}
		
		static CGColorSpace deviceRGB;

		static CGColorSpace CreateDeviceRGB ()
		{
			if (deviceRGB != null)
				return deviceRGB;
			deviceRGB = CGColorSpace.CreateDeviceRGB ();
			return deviceRGB;
		}
		
		public static CGColor Convert (Color color)
		{
			return new CGColor (CreateDeviceRGB (), new float[] { color.R, color.G, color.B, color.A });
		}

		public static Color Convert (CGColor color)
		{
			return new Color (color.Components [0], color.Components [1], color.Components [2], color.Alpha);
		}

		public static NSColor ConvertNS (Color color)
		{
			return NSColor.FromDeviceRgba (color.R, color.G, color.B, color.A);
		}

		public static Color Convert (NSColor color)
		{
			if (color == null)
				return Color.Black;
			float red, green, blue, alpha;
			color.GetRgba (out red, out green, out blue, out alpha);
			return new Color (red, green, blue, alpha);
		}
		
		public static MouseEventArgs GetMouseEvent (NSView view, NSEvent theEvent)
		{
			var pt = Generator.GetLocation (view, theEvent);
			Key modifiers = KeyMap.GetModifiers (theEvent);
			MouseButtons buttons = KeyMap.GetMouseButtons (theEvent);
			return new MouseEventArgs (buttons, modifiers, pt);
		}
	}
}
