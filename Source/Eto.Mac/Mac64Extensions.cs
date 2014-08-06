using System;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using System.Runtime.InteropServices;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac
{
	public static class Mac64Extensions
	{

		public static NSPoint ToNS(this Point point)
		{
			return new NSPoint(point.X, point.Y);
		}

		public static PointF ToEto(this NSPoint point)
		{
			return new PointF((float)point.X, (float)point.Y);
		}

		public static Point ToEtoPoint(this NSPoint point)
		{
			return new Point((int)point.X, (int)point.Y);
		}

		public static NSPoint ToNS(this PointF point)
		{
			return new NSPoint(point.X, point.Y);
		}

		public static NSPoint ToNS(this System.Drawing.PointF point)
		{
			return new NSPoint(point.X, point.Y);
		}

		public static NSSize ToNS(this SizeF size)
		{
			return new NSSize(size.Width, size.Height);
		}

		public static SizeF ToEto(this NSSize point)
		{
			return new SizeF((float)point.Width, (float)point.Height);
		}

		public static System.Drawing.SizeF ToSD(this NSSize size)
		{
			return new System.Drawing.SizeF((float)size.Width, (float)size.Height);
		}

		public static NSSize ToNS(this Size size)
		{
			return new NSSize((float)size.Width, (float)size.Height);
		}

		public static NSSize ToNS(this System.Drawing.Size size)
		{
			return new NSSize((float)size.Width, (float)size.Height);
		}

		public static NSSize ToNS(this System.Drawing.SizeF size)
		{
			return new NSSize((float)size.Width, (float)size.Height);
		}

		public static Size ToEtoSize(this NSSize size)
		{
			return new Size((int)size.Width, (int)size.Height);
		}

		public static NSRect ToNS(this RectangleF rect)
		{
			return new NSRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static NSRect ToNS(this System.Drawing.RectangleF rect)
		{
			return new NSRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToEto(this NSRect rect)
		{
			return new RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}

		public static System.Drawing.RectangleF ToSD(this NSRect rect)
		{
			return new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}

		public static NSRect SetSize(this NSRect frame, SizeF size)
		{
			frame.Size = size.ToNS();
			return frame;
		}
	}
}

