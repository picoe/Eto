using System;
using Eto.Drawing;

namespace Eto.Mac.Drawing
{
	public class ColorHandler : Color.IHandler
	{
		// this actually just converts to RGB anyway, so don't bother as this is more computationally expensive
		// keeping this here in c
		/*
		public Color? Blend(in Color baseColor, in Color blendColor, float blendFactor)
		{
			if (baseColor.ControlObject is NSColor || blendColor.ControlObject is NSColor)
			{
				// blend colors using native APIs and the effective appearance
				NSAppearance saved = NSAppearance.CurrentAppearance;
				var appearance = NSApplication.SharedApplication.MainWindow?.EffectiveAppearance;
				if (appearance != null)
					NSAppearance.CurrentAppearance = appearance;
				var blendedColor = baseColor.ToNSUI().BlendedColor(blendFactor, blendColor.ToNSUI());
				NSAppearance.CurrentAppearance = saved;
				if (blendedColor != null)
					return blendedColor.ToEto();
			}
			return null;
		}
		*/

		public object ModifyComponent(object controlObject, float? r, float? g, float? b, float? a)
		{
			if (r != null || g != null || b != null)
				return null;
				
			if (controlObject is NSColor color && a != null)
			{
				// we can change the alpha component of system colors
				return color.ColorWithAlphaComponent((nfloat)a.Value);
			}
			
			// nothing is changing, use the same control object
			return controlObject;
		}
	}
}

