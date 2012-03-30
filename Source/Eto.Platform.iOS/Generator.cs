using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Linq;
using Eto.Drawing;
using MonoTouch.CoreGraphics;
using Eto.Forms;

namespace Eto.Platform.iOS
{
	public class Generator : Eto.Generator
	{
		public const string GeneratorID = "ios";

		public override string ID {
			get { return GeneratorID; }
		}
		
		public override bool Supports<T> ()
		{
			var type = typeof(T);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad) {
				// all iPad-only stuff is not supported on other idioms..
				if (type == typeof(ISplitter))
					return false;
				
			}
			return base.Supports<T> ();
		}
		
		public static Point GetLocation (UIView view, UIEvent theEvent)
		{
			var touches = theEvent.TouchesForView (view);
			var touch = touches.ToArray<UITouch> ().FirstOrDefault ();
			var loc = touch.LocationInView (view);
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

		public static System.Drawing.PointF ConvertF (PointF point)
		{
			return new System.Drawing.PointF (point.X, point.Y);
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

		public static UIColor ConvertUI (Color color)
		{
			return UIColor.FromRGBA (color.R, color.G, color.B, color.A);
		}

		public static Color Convert (UIColor color)
		{
			float red, green, blue, alpha;
			color.GetRGBA (out red, out green, out blue, out alpha);
			return new Color (red, green, blue, alpha);
		}
		
		public static MouseEventArgs ConvertMouse (UIView view, NSSet touches, UIEvent evt)
		{
			if (touches.Count > 0) {
				UITouch touch = touches.ToArray<UITouch> () [0];
				var location = touch.LocationInView (view);
				return new MouseEventArgs (MouseButtons.Primary, Key.None, ConvertF (location));
			}
			return new MouseEventArgs (MouseButtons.Primary, Key.None, Point.Empty);
		}
	}
}

