using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
#if WINFORMS
using Eto.Platform.Windows.Drawing;
#endif

namespace Eto.Platform.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IFont"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
    public class FontHandler : WidgetHandler<sw.Font, Font>, IFont
#if WINFORMS
		, IWindowsFontSource
#endif
    {
		sw.TextFormat textFormat;
		public sw.TextFormat TextFormat
		{
			get
			{
				if (textFormat == null)
				{
					textFormat = new sw.TextFormat(
						SDFactory.DirectWriteFactory,
						Control.FontFamily.FamilyNames.GetString(0),
						FontCollection,
						Control.Weight,
						Control.Style,
						Control.Stretch,
						Size * 96.0f / 72.0f // convert from points to pixels. (The documentation says device-independent pixels.)
						);
				}
				return textFormat;
			}
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			this.family = family;
			Size = size;
			FontStyle = style;
			FontDecoration = decoration;

			sw.FontStyle fontStyle;
			sw.FontWeight fontWeight;
			Conversions.Convert(style, out fontStyle, out fontWeight);

			var familyHandler = (FontFamilyHandler)family.Handler;
			Control = familyHandler.Control.GetFirstMatchingFont(fontWeight, sw.FontStretch.Normal, fontStyle);
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
#if WINFORMS
			var sdfont = Eto.Platform.Windows.Conversions.ToSD(systemFont);
			Create(sdfont.Name, size ?? sdfont.SizeInPoints, FontStyle.None, decoration);
#else
			throw new NotImplementedException();
#endif
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			family = typeface.Family;
			this.typeface = typeface;
			var typefaceHandler = (FontTypefaceHandler)typeface.Handler;

			Control = typefaceHandler.Font;
			FontStyle = Control.ToEtoStyle();
			FontDecoration = decoration;
			Size = size;
        }

        void Create(string familyName, float sizeInPoints, FontStyle style, FontDecoration decoration)
        {
			Size = sizeInPoints;
			FontStyle = style;
			FontDecoration = decoration;
			int index;
			if (FontCollection.FindFamilyName(familyName, out index))
			{
				sw.FontStyle fontStyle;
				sw.FontWeight fontWeight;
				Conversions.Convert(style, out fontStyle, out fontWeight);
				Control = FontCollection.GetFontFamily(index).GetFirstMatchingFont(fontWeight, sw.FontStretch.Normal, fontStyle);
			}
        }

        static sw.FontCollection fontCollection;
        public static sw.FontCollection FontCollection
        {
			get
			{
				return fontCollection = fontCollection ?? SDFactory.DirectWriteFactory.GetSystemFontCollection(checkForUpdates: false);
			}
        }

        protected override void Dispose(bool disposing)
        {
            if (textFormat != null)
            {
                textFormat.Dispose();
                textFormat = null;
            }

            base.Dispose(disposing);
        }

		FontFamily family;
        public FontFamily Family
        {
            get { return family ?? (family = new FontFamily(Generator, new FontFamilyHandler(Control.FontFamily))); }
        }

        public string FamilyName
        {
            get { return Family.Name; }
        }

        public FontStyle FontStyle { get; private set; }

		public FontDecoration FontDecoration { get; private set; }

		/// <summary>
		/// The size in points.
		/// </summary>
		public float Size { get; private set; }

		FontTypeface typeface;
        public FontTypeface Typeface
        {
            get { return typeface ?? (typeface = new FontTypeface(Family, new FontTypefaceHandler(Control))); }
        }

		public float XHeight
		{
			get { return Size * Control.Metrics.XHeight / Control.Metrics.DesignUnitsPerEm; }
		}

		public float Ascent
		{
			get { return Size * Control.Metrics.Ascent / Control.Metrics.DesignUnitsPerEm; }
		}

		public float Descent
		{
			get { return Size * Control.Metrics.Descent / Control.Metrics.DesignUnitsPerEm; }
		}

		public float LineHeight
		{
			get { return Ascent + Descent + Size * Control.Metrics.LineGap / Control.Metrics.DesignUnitsPerEm; }
		}

		public float Leading
		{
			get { return LineHeight - (Ascent + Descent);} 
		}

		public float Baseline
		{
			get { return Ascent; }
		}

#if WINFORMS
		public System.Drawing.Font GetFont()
		{
			var familyName = Control.FontFamily.FamilyNames.GetString(0);
			var style = Eto.Platform.Windows.Conversions.ToSD(FontStyle) | Eto.Platform.Windows.Conversions.ToSD(FontDecoration);
			return new System.Drawing.Font(familyName, Size, style);
		}
#endif
	}
}
