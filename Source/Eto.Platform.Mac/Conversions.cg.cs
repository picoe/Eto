using System;
using Eto.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac
#elif IOS

using MonoTouch.CoreGraphics;

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
	}
}

