using System;
using Eto.Drawing;

#if OSX
using MonoMac.CoreGraphics;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac
#elif IOS

using MonoTouch.CoreGraphics;
using Eto.Platform.iOS.Drawing;

namespace Eto.Platform.iOS
#endif
{
	public static partial class Conversions
	{
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

		public static CGAffineTransform ToCG (this IMatrix matrix)
		{
			return (CGAffineTransform)matrix.ControlObject;
		}
		
		public static float DegreesToRadians (float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

		public static CGLineJoin ToCG (this PenLineJoin value)
		{
			switch (value) {
			case PenLineJoin.Bevel:
				return CGLineJoin.Bevel;
			case PenLineJoin.Miter:
				return CGLineJoin.Miter;
			case PenLineJoin.Round:
				return CGLineJoin.Round;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PenLineJoin ToEto (this CGLineJoin value)
		{
			switch (value) {
			case CGLineJoin.Bevel:
				return PenLineJoin.Bevel;
			case CGLineJoin.Miter:
				return PenLineJoin.Miter;
			case CGLineJoin.Round:
				return PenLineJoin.Round;
			default:
				throw new NotSupportedException ();
			}
		}

		public static CGLineCap ToCG (this PenLineCap value)
		{
			switch (value) {
			case PenLineCap.Butt:
				return CGLineCap.Butt;
			case PenLineCap.Round:
				return CGLineCap.Round;
			case PenLineCap.Square:
				return CGLineCap.Square;
			default:
				throw new NotSupportedException ();
			}
		}
		
		public static PenLineCap ToEto (this CGLineCap value)
		{
			switch (value) {
			case CGLineCap.Butt:
				return PenLineCap.Butt;
			case CGLineCap.Round:
				return PenLineCap.Round;
			case CGLineCap.Square:
				return PenLineCap.Square;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PenHandler ToHandler (this IPen pen)
		{
			return (PenHandler)pen.ControlObject;
		}
		
		public static void Apply (this IPen pen, GraphicsHandler graphics)
		{
			pen.ToHandler ().Apply (graphics);
		}
		
		public static BrushHandler ToHandler (this IBrush brush)
		{
			return (BrushHandler)brush.ControlObject;
		}
		
		public static void Apply (this IBrush brush, GraphicsHandler graphics)
		{
			brush.ToHandler ().Apply (graphics);
		}

		public static GraphicsPathHandler ToHandler (this IGraphicsPath path)
		{
			return (GraphicsPathHandler)path.ControlObject;
		}

		public static CGPath ToCG (this IGraphicsPath path)
		{
			return path.ToHandler ().Control;
		}
	}
}

