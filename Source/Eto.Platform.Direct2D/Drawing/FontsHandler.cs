using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Platform.Direct2D.Drawing
{
    public class FontsHandler : WidgetHandler<Widget>, IFonts
    {
        private List<FontFamily> availableFontFamilies;
        public IEnumerable<FontFamily> AvailableFontFamilies
        {            
            get 
            {
                if (availableFontFamilies != null)
                {
                    sw.FontCollection Control = null; // BUGBUG: TODO

                    for (var i = 0;
                        i < Control.FontFamilyCount;
                        ++i)
                    {
                        availableFontFamilies.Add(
                            new FontFamily(
                                this.Generator,
                                Control.GetFontFamily(
                                    i)
                                .FamilyNames
                                // BUGBUG: TODO: uses the first locale
                                .GetLocaleName(0))); 
                    }
                }
                return availableFontFamilies;
            }
        }

        public IFontFamily GetSystemFontFamily(string systemFontFamily)
        {
            throw new NotImplementedException();
        }

		public bool FontFamilyAvailable(string fontFamily)
		{
			throw new NotImplementedException();
		}
	}
}
