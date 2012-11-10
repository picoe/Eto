using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	public static class FontFamilies
	{
		public const string MonospaceFamilyName = "Eto.Monospace";

		public static FontFamily Monospace
		{
			get { return Fonts.GetSystemFontFamily (MonospaceFamilyName); }
		}

		public const string SansFamilyName = "Eto.Sans";

		public static FontFamily Sans
		{
			get { return Fonts.GetSystemFontFamily (SansFamilyName); }
		}

		public const string SerifFamilyName = "Eto.Serif";

		public static FontFamily Serif
		{
			get { return Fonts.GetSystemFontFamily (SerifFamilyName); }
		}
	}
}
