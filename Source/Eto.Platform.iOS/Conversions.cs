using System;
using MonoTouch.UIKit;
using Eto.Drawing;
using Eto.Platform.iOS.Drawing;
using Eto.Forms;
using MonoTouch.Foundation;
using System.Linq;
using MonoTouch.CoreGraphics;
using MonoTouch.ImageIO;

namespace Eto.Platform.iOS
{
	public static partial class Conversions
	{
		public static UIColor ToNSUI (this Color color)
		{
			return UIColor.FromRGBA (color.R, color.G, color.B, color.A);
		}
		
		public static Color ToEto (this UIColor color)
		{
			float red, green, blue, alpha;
			color.GetRGBA (out red, out green, out blue, out alpha);
			return new Color (red, green, blue, alpha);
		}

		public static UIFont ToUI (this Font font)
		{
			if (font == null)
				return null;
			return ((FontHandler)font.Handler).Control;
		}

		public static Point GetLocation (UIView view, UIEvent theEvent)
		{
			var touches = theEvent.TouchesForView (view);
			var touch = touches.ToArray<UITouch> ().FirstOrDefault ();
			var loc = touch.LocationInView (view);
			loc.Y = view.Frame.Height - loc.Y;
			return loc.ToEtoPoint ();
		}
		
		public static MouseEventArgs ConvertMouse (UIView view, NSSet touches, UIEvent evt)
		{
			if (touches.Count > 0) {
				UITouch touch = touches.ToArray<UITouch> () [0];
				var location = touch.LocationInView (view);
				return new MouseEventArgs (MouseButtons.Primary, Key.None, location.ToEtoPoint ());
			}
			return new MouseEventArgs (MouseButtons.Primary, Key.None, Point.Empty);
		}

		public static UIImage ToUI (this Image image)
		{
			if (image == null)
				return null;
			var handler = image.Handler as IImageHandler;
			if (handler != null)
				return handler.GetUIImage ();
			else
				return null;
		}

		public static CGImage ToCG (this Image image)
		{
			return image.ToUI ().CGImage;
		}
	}
}

