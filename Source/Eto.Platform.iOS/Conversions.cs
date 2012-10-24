using System;
using MonoTouch.UIKit;
using Eto.Drawing;

namespace Eto.Platform.iOS
{
	public static partial class Conversions
	{
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
	}
}

