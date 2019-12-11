using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
#if WINFORMS
using Eto.WinForms.Drawing;
#else
using Windows.Globalization.Fonts;
#endif

namespace Eto.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IFont"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontHandler : WidgetHandler<sw.Font, Font>, Font.IHandler
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

#if !WINFORMS
		static LanguageFontGroup languageFontGroup = new LanguageFontGroup("en"); // TODO: add Eto support for other locales.
#endif

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
#if WINFORMS
			var sdfont = Eto.WinForms.WinConversions.ToSD(systemFont);
			Create(sdfont.Name, size ?? sdfont.SizeInPoints, FontStyle.None, decoration);
#else
			var familyName = "Segoe UI";
			var sizeInPoints = size ?? 12f;
			var fontStyle = FontStyle.None;
			
			// TODO: map the systemfont and default size to a family name and font size.

			// See: http://msdn.microsoft.com/en-us/library/windows/apps/hh700394.aspx
			switch (systemFont)
			{
				case SystemFont.Default:
				case SystemFont.Bold:
				case SystemFont.TitleBar:
				case SystemFont.ToolTip:
				case SystemFont.Label:
				case SystemFont.MenuBar:
				case SystemFont.Menu:
				case SystemFont.Message:
				case SystemFont.Palette:
				case SystemFont.StatusBar:
				default:
					break;
			}

			Create(familyName, sizeInPoints, fontStyle, decoration);
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
				var dxFontFamily = FontCollection.GetFontFamily(index);
				Control = dxFontFamily.GetFirstMatchingFont(fontWeight, sw.FontStretch.Normal, fontStyle);
				if (Control == null)
					Control = dxFontFamily.GetFont(0);
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

		public static sw.FontFamily GetFontFamily(string familyName)
		{
			sw.FontFamily result = null;
			int index;
			if (FontHandler.FontCollection.FindFamilyName(familyName, out index))
				result = FontHandler.FontCollection.GetFontFamily(index);
			return result;
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
            get { return family ?? (family = new FontFamily(new FontFamilyHandler(Control.FontFamily))); }
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
			var style = Eto.WinForms.WinConversions.ToSD(FontStyle) | Eto.WinForms.WinConversions.ToSD(FontDecoration);
			return new System.Drawing.Font(familyName, Size, style);
		}
#endif

		public SizeF MeasureString(string text)
		{
			using (var textLayout = GraphicsHandler.GetTextLayout(Widget, text))
			{
				var metrics = textLayout.Metrics;
				return new SizeF(metrics.WidthIncludingTrailingWhitespace, metrics.Height);
			}
		}
	}
}
