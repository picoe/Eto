using System;
using Eto.Drawing;
using System.Runtime.InteropServices;
#if IOS
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#elif XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if IOS
namespace Eto.iOS
#else
namespace Eto.Mac
#endif
{

	#if !UNIFIED
	public static class NSStringAttributeKey
	{
		public static NSString ForegroundColor = NSAttributedString.ForegroundColorAttributeName;
		public static NSString Shadow = NSAttributedString.ShadowAttributeName;
		public static NSString Font = NSAttributedString.FontAttributeName;
		public static NSString BaselineOffset = NSAttributedString.BaselineOffsetAttributeName;
		public static NSString UnderlineStyle = NSAttributedString.UnderlineStyleAttributeName;
		public static NSString BackgroundColor = NSAttributedString.BackgroundColorAttributeName;
		public static NSString StrikethroughStyle = NSAttributedString.StrikethroughStyleAttributeName;
		public static NSString ParagraphStyle = NSAttributedString.ParagraphStyleAttributeName;
	}
	#endif

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

		public static CGSize ToNS(this SizeF size)
		{
			return new CGSize(size.Width, size.Height);
		}

		public static SizeF ToEto(this CGSize point)
		{
			return new SizeF((float)point.Width, (float)point.Height);
		}

		public static CGSize ToNS(this Size size)
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

		public static CGRect ToNS(this Rectangle rect)
		{
			return new CGRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToEto(this CGRect rect)
		{
			return new RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}

		public static Rectangle ToEtoRectangle(this CGRect rect)
		{
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		#if IOS

		public static CGPoint ToSDPointF (this Point point)
		{
			return new CGPoint ((nfloat)point.X, (nfloat)point.Y);
		}

		public static CGPoint ToSD (this PointF point)
		{
			return new CGPoint ((nfloat)point.X, (nfloat)point.Y);
		}

		#endif

#if !UNIFIED

		public static CGPoint ToNS(this System.Drawing.PointF point)
		{
			return new CGPoint(point.X, point.Y);
		}

		public static System.Drawing.SizeF ToSD(this CGSize size)
		{
			return new System.Drawing.SizeF((float)size.Width, (float)size.Height);
		}

		public static CGSize ToNS(this System.Drawing.Size size)
		{
			return new CGSize((float)size.Width, (float)size.Height);
		}

		public static CGSize ToNS(this System.Drawing.SizeF size)
		{
			return new CGSize((float)size.Width, (float)size.Height);
		}

		public static CGRect ToNS(this System.Drawing.RectangleF rect)
		{
			return new CGRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static System.Drawing.RectangleF ToSD(this CGRect rect)
		{
			return new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}
#endif

		public static CGRect SetSize(this CGRect frame, SizeF size)
		{
			frame.Size = size.ToNS();
			return frame;
		}

		public static CGRect WithPadding(this CGRect frame, Padding padding)
		{
			frame.X += padding.Left;
			frame.Width = (nfloat)Math.Max(0, frame.Width - padding.Horizontal);

			// assumes view is not flipped.
			frame.Y += padding.Bottom;
			frame.Height = (nfloat)Math.Max(0, frame.Height - padding.Vertical);
			return frame;
		}

		public static Size TruncateInfinity(this SizeF size)
		{
			var result = new Size();
			if (float.IsPositiveInfinity(size.Width))
				result.Width = int.MaxValue;
			else
				result.Width = (int)Math.Truncate(size.Width);
			if (float.IsPositiveInfinity(size.Height))
				result.Height = int.MaxValue;
			else
				result.Height = (int)Math.Truncate(size.Height);
			return result;
		}
	}
}

