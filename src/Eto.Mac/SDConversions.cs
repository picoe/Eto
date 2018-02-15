#if !UNIFIED
using System;
using sd = System.Drawing;
using Eto.Drawing;

namespace Eto
{
	public static partial class Conversions
	{
		public static sd.Size ToSD (this Size size)
		{
			return new sd.Size (size.Width, size.Height);
		}
		
		public static Size ToEto (this sd.Size size)
		{
			return new Size (size.Width, size.Height);
		}
		
		public static sd.SizeF ToSDSizeF (this Size size)
		{
			return new sd.SizeF (size.Width, size.Height);
		}
		
		public static Size ToEtoSize (this sd.SizeF size)
		{
			return new Size ((int)size.Width, (int)size.Height);
		}
		
		public static SizeF ToEto (this sd.SizeF size)
		{
			return new SizeF (size.Width, size.Height);
		}
		
		public static sd.SizeF ToSD (this SizeF size)
		{
			return new sd.SizeF (size.Width, size.Height);
		}
		
		public static sd.RectangleF SetSize (this sd.RectangleF frame, Size size)
		{
			frame.Size = size.ToSDSizeF ();
			return frame;
		}
		
		public static sd.Rectangle ToSD (this Rectangle rect)
		{
			return new sd.Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public static sd.RectangleF ToSDRectangleF (this Rectangle rect)
		{
			return new sd.RectangleF (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public static Rectangle ToEto (this sd.Rectangle rect)
		{
			return new Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToEto (this sd.RectangleF rect)
		{
			return new RectangleF (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.RectangleF ToSD (this RectangleF rect)
		{
			return new sd.RectangleF (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Rectangle ToEtoRectangle (this sd.RectangleF rect)
		{
			return new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		public static Point ToEtoPoint (this sd.PointF point)
		{
			return new Point ((int)point.X, (int)point.Y);
		}

		public static PointF ToEto (this sd.PointF point)
		{
			return new PointF (point.X, point.Y);
		}

		public static sd.PointF ToSDPointF (this Point point)
		{
			return new sd.PointF ((int)point.X, (int)point.Y);
		}

		public static Point ToEto (this sd.Point point)
		{
			return new Point (point.X, point.Y);
		}

		public static sd.Point ToSD (this Point point)
		{
			return new sd.Point (point.X, point.Y);
		}
		
		public static sd.PointF ToSD (this PointF point)
		{
			return new sd.PointF (point.X, point.Y);
		}

		internal static sd.PointF[] ToSD (this PointF[] points)
		{
			var result = new sd.PointF[points.Length];
			
			for (var i = 0; i < points.Length; ++i) {
				var p = points [i];
				result [i] = new sd.PointF (p.X, p.Y);
			}
			
			return result;
		}

	}
}
#endif