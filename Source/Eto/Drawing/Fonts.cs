using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	public interface IFonts : IWidget
	{
		IEnumerable<FontFamily> AvailableFontFamilies { get; }

		FontFamily GetSystemFontFamily (string systemFontFamily);
	}

	public static class Fonts
	{
		public static IEnumerable<FontFamily> AvailableFontFamilies(Generator generator = null)
		{
			generator = generator ?? Generator.Current;
			var fonts = generator.CreateHandler<IFonts>();
			return fonts.AvailableFontFamilies;
		}

		public static FontFamily GetSystemFontFamily (string systemFontFamily, Generator generator = null)
		{
			generator = generator ?? Generator.Current;
			var fonts = generator.CreateHandler<IFonts>();
			return fonts.GetSystemFontFamily(systemFontFamily);
		}
	}
}

