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
	/// <summary>
	/// Handler for <see cref="IFont"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
    public class FontHandler : WidgetHandler<sw.Font, Font>, IFont
    {
		public sw.TextFormat TextFormat { get; private set; }

		private float sizeInPoints;
        string familyName;
        FontStyle style;
		FontDecoration decoration;
        sw.FontStyle fontStyle;
        sw.FontWeight fontWeight;
		
        // These need to be disposed
        sw.FontFace fontFace = null;

		private string GetTranslatedName(FontFamily family)
		{
			var f = (FontFamilyHandler)family.Handler;
			return f.TranslatedName;
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			Create(GetTranslatedName(family), size, style, decoration);
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			Create("Consolas", size ?? 12, FontStyle.None, FontDecoration.None); // BUGBUG: Fix
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			throw new NotImplementedException();
		}

        public void Create(FontTypeface typeface, float sizeInPoints)
        {
            Create(typeface.Name, sizeInPoints, typeface.FontStyle, FontDecoration.None);
        }

        public void Create(SystemFont systemFont, float? sizeInPoints)
        {
            throw new NotImplementedException();            
        }

        public void Create(FontFamily family, float sizeInPoints, FontStyle style)
        {
            Create(GetTranslatedName(family), sizeInPoints, style, FontDecoration.None);
        }

        private void Create(string familyName, float sizeInPoints, FontStyle style, FontDecoration decoration)
        {
            this.familyName = familyName;
			this.sizeInPoints = sizeInPoints;
            this.style = style;
			this.decoration = decoration;

            sw.FontStyle s;
            sw.FontWeight w;
            Conversions.Convert(style, out s, out w);

            this.TextFormat = new sw.TextFormat(
				SDFactory.DirectWriteFactory,
                familyName,
                null, // font collection
                w,
                s,
                sw.FontStretch.Normal,
                sizeInPoints * 96.0f / 72.0f, // convert from points to pixels. (The documentation says device-independent pixels.)
                "en-us");
            
            int index = 0;
            if (FontCollection.FindFamilyName(
                familyName: familyName,
                index: out index))
            {
                var fontFamily = FontCollection.GetFontFamily(index);
				Conversions.Convert(style, out fontStyle, out fontWeight);
				this.Control = fontFamily.GetFirstMatchingFont(fontWeight, sw.FontStretch.Normal, fontStyle);
            }
        }

        private static sw.FontCollection fontCollection;
        private static sw.FontCollection FontCollection
        {
			get
			{
				return fontCollection = fontCollection ??
					SDFactory.DirectWriteFactory.GetSystemFontCollection(checkForUpdates: false);
			}
        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.TextFormat != null)
            {
                this.TextFormat.Dispose();
                this.TextFormat = null;
            }

            if (this.fontFace != null)
            {
                this.fontFace.Dispose();
                this.fontFace = null;
            }

            base.Dispose(disposing);
        }

        public FontFamily Family
        {
            get { throw new NotImplementedException(); }
        }

        public string FamilyName
        {
            get { return familyName; }
        }

        public FontStyle FontStyle
        {
            get { return style; }
        }

		public FontDecoration FontDecoration
		{
			get { return this.decoration; }
		}

        public float Size
        {
            get { return sizeInPoints; }
        }

        public FontTypeface Typeface
        {
            get { return null; }
        }

		public float XHeight
		{
			get { return Size * this.Control.Metrics.XHeight / this.Control.Metrics.DesignUnitsPerEm; }
		}

		public float Ascent
		{
			get { return Size * this.Control.Metrics.Ascent / this.Control.Metrics.DesignUnitsPerEm; }
		}

		public float Descent
		{
			get { return Size * this.Control.Metrics.Descent / this.Control.Metrics.DesignUnitsPerEm; }
		}

		public float LineHeight
		{
			get { return Ascent + Descent + Size * this.Control.Metrics.LineGap / this.Control.Metrics.DesignUnitsPerEm; }
		}

		public float Leading
		{
			get { return LineHeight - (Ascent + Descent);} 
		}

		public float Baseline
		{
			get { return Ascent; }
		}

		public object ControlObject
		{
			get { return this.Control; }
		}
	}
}
