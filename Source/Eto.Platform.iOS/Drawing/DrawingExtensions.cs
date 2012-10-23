using System;
using Eto.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS
{
	public static class DrawingExtensions
	{
		public static System.Drawing.Size ToSDSize (this Size size)
		{
			return new System.Drawing.Size (size.Width, size.Height);
		}
		
		public static Size ToEtoSize (this System.Drawing.Size size)
		{
			return new Size (size.Width, size.Height);
		}
		
		public static System.Drawing.SizeF ToSDSizeF (this Size size)
		{
			return new System.Drawing.SizeF (size.Width, size.Height);
		}
		
		public static Size ToEtoSize (this System.Drawing.SizeF size)
		{
			return new Size ((int)size.Width, (int)size.Height);
		}
		
		public static System.Drawing.RectangleF SetSize (this System.Drawing.RectangleF frame, Size size)
		{
			frame.Size = size.ToSDSizeF ();
			return frame;
		}
		
		public static Rectangle ToEtoRectangle (this System.Drawing.RectangleF rect)
		{
			return new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}
		
		public static System.Drawing.RectangleF ToSDRectangleF (this Rectangle rect)
		{
			return new System.Drawing.RectangleF ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}
		
		public static Point ToEtoPoint (this System.Drawing.PointF point)
		{
			return new Point ((int)point.X, (int)point.Y);
		}
		
		public static System.Drawing.PointF ToSDPointF (this Point point)
		{
			return new System.Drawing.PointF ((int)point.X, (int)point.Y);
		}
		
		public static System.Drawing.PointF ToSDPointF (this PointF point)
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
		
		public static CGColor ToCGColor (this Color color)
		{
			return new CGColor (CreateDeviceRGB (), new float[] { color.R, color.G, color.B, color.A });
		}
		
		public static Color ToEtoColor (this CGColor color)
		{
			return new Color (color.Components [0], color.Components [1], color.Components [2], color.Alpha);
		}
		
		public static UIColor ToUIColor (this Color color)
		{
			return UIColor.FromRGBA (color.R, color.G, color.B, color.A);
		}
		
		public static Color ToEtoColor (this UIColor color)
		{
			float red, green, blue, alpha;
			color.GetRGBA (out red, out green, out blue, out alpha);
			return new Color (red, green, blue, alpha);
		}
		public static CGInterpolationQuality ToCG (this ImageInterpolation value)
		{
			switch (value) {
			case ImageInterpolation.Default:
				return CGInterpolationQuality.Default;
			case ImageInterpolation.None:
				return CGInterpolationQuality.None;
			case ImageInterpolation.Low:
				return CGInterpolationQuality.Low;
			case ImageInterpolation.Medium:
				return CGInterpolationQuality.Medium;
			case ImageInterpolation.High:
				return CGInterpolationQuality.High;
			default:
				throw new NotSupportedException();
			}
		}
		
		public static ImageInterpolation ToEto (this CGInterpolationQuality value)
		{
			switch (value) {
			case CGInterpolationQuality.Default:
				return ImageInterpolation.Default;
			case CGInterpolationQuality.None:
				return ImageInterpolation.None;
			case CGInterpolationQuality.Low:
				return ImageInterpolation.Low;
			case CGInterpolationQuality.Medium:
				return ImageInterpolation.Medium;
			case CGInterpolationQuality.High:
				return ImageInterpolation.High;
			default:
				throw new NotSupportedException();
			}
		}
	}
}

