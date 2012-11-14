using System;
using MonoTouch.UIKit;
using Eto.Drawing;
using Eto.Platform.iOS.Drawing;

namespace Eto.Platform.iOS
{
	public static partial class Conversions
	{
		public static UIColor ToUI (this Color color)
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

	}
}

