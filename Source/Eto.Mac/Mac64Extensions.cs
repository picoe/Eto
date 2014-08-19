using System;
using Eto.Drawing;
using System.Runtime.InteropServices;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac
{
	public static class Mac64Extensions
	{

		public static CGPoint ToNS(this Point point)
		{
			return new CGPoint(point.X, point.Y);
		}

		public static PointF ToEto(this CGPoint point)
		{
			return new PointF((float)point.X, (float)point.Y);
		}

		public static Point ToEtoPoint(this CGPoint point)
		{
			return new Point((int)point.X, (int)point.Y);
		}

		public static CGPoint ToNS(this PointF point)
		{
			return new CGPoint(point.X, point.Y);
		}

		public static CGPoint ToNS(this System.Drawing.PointF point)
		{
			return new CGPoint(point.X, point.Y);
		}

		public static CGSize ToNS(this SizeF size)
		{
			return new CGSize(size.Width, size.Height);
		}

		public static SizeF ToEto(this CGSize point)
		{
			return new SizeF((float)point.Width, (float)point.Height);
		}

		public static System.Drawing.SizeF ToSD(this CGSize size)
		{
			return new System.Drawing.SizeF((float)size.Width, (float)size.Height);
		}

		public static CGSize ToNS(this Size size)
		{
			return new CGSize((float)size.Width, (float)size.Height);
		}

		public static CGSize ToNS(this System.Drawing.Size size)
		{
			return new CGSize((float)size.Width, (float)size.Height);
		}

		public static CGSize ToNS(this System.Drawing.SizeF size)
		{
			return new CGSize((float)size.Width, (float)size.Height);
		}

		public static Size ToEtoSize(this CGSize size)
		{
			return new Size((int)size.Width, (int)size.Height);
		}

		public static CGRect ToNS(this RectangleF rect)
		{
			return new CGRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static CGRect ToNS(this System.Drawing.RectangleF rect)
		{
			return new CGRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToEto(this CGRect rect)
		{
			return new RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}

		public static System.Drawing.RectangleF ToSD(this CGRect rect)
		{
			return new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}

		public static CGRect SetSize(this CGRect frame, SizeF size)
		{
			frame.Size = size.ToNS();
			return frame;
		}
	}
}

