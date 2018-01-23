using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IFonts"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontsHandler : WidgetHandler<Widget>, Fonts.IHandler
	{
		FontFamily[] availableFontFamilies;
		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get
			{
				if (availableFontFamilies == null)
				{
					var fonts = FontHandler.FontCollection;
					availableFontFamilies = Enumerable.Range(0, fonts.FontFamilyCount)
						.Select(r => fonts.GetFontFamily(r).FamilyNames.GetString(0))
						.Distinct()
						.Select(r => new FontFamily(r))
						.ToArray();
				}
				return availableFontFamilies;
			}
		}

		public bool FontFamilyAvailable(string fontFamily)
		{
			return AvailableFontFamilies.Any(r => string.Equals(r.Name, fontFamily, StringComparison.OrdinalIgnoreCase));
		}
	}
}
