using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
	public class FontsHandler : WidgetHandler<Widget>, IFonts
	{
		public IEnumerable<FontFamily> AvailableFontFamilies
		{
			get { return swm.Fonts.SystemFontFamilies.Select (r => new FontFamily(Generator, new FontFamilyHandler(r))); ; }
		}
	}
}
