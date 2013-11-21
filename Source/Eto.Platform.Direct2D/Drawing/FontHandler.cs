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
    public class FontHandler : WidgetHandler<sw.Font, Font>, IFont
    {        
        private float sizeInPoints = 0f;
        string familyName;
        FontStyle style;
        sw.FontStyle fontStyle;
        sw.FontWeight fontWeight;

        // These need to be disposed
        sw.FontFace fontFace = null;
        sw.TextFormat textFormat = null;


        public void Create(FontTypeface typeface, float sizeInPoints)
        {
            this.sizeInPoints = sizeInPoints;
            Create(typeface.Name, sizeInPoints, typeface.FontStyle);            
        }

        public void Create(SystemFont systemFont, float? sizeInPoints)
        {
            this.sizeInPoints = sizeInPoints ?? 0f;
            throw new NotImplementedException();            
        }

        public void Create(FontFamily family, float sizeInPoints, FontStyle style)
        {
            this.sizeInPoints = sizeInPoints;
            var familyName = family.Name;

            Create(familyName, sizeInPoints, style);
        }

        private void Create(string familyName, float sizeInPoints, FontStyle style)
        {
            // family name
            this.familyName = familyName;

            // font style
            this.style = style;

            sw.FontStyle s;
            sw.FontWeight w;
            Conversions.Convert(style, out s, out w);

            this.textFormat =
                new sw.TextFormat(
					SDFactory.DirectWriteFactory,
                    familyName,
                    null, // font collection
                    w,
                    s,
                    sw.FontStretch.Normal,
                    sizeInPoints, // TODO: should this be in pixels? The documentation says device-independent pixels.
                    "en-us");
            
            // Create a font collection
            var collection = FontCollection();

            int index = 0;
            if (collection.FindFamilyName(
                familyName: familyName,
                index: out index))
            {
                // font family
                var fontFamily =
                    collection.GetFontFamily(index);

                Conversions.Convert(
                    style,
                    out fontStyle,
                    out fontWeight);

                // font
                this.Control = 
                    fontFamily.GetFirstMatchingFont(
                        fontWeight,
                        sw.FontStretch.Normal,
                        fontStyle);

                // finally, the font face
                this.fontFace = new sw.FontFace(Control);
            }
        }

        private static sw.FontCollection fontCollection;
        private static sw.FontCollection FontCollection()
        {
            if (fontCollection == null)
                fontCollection =
                SDFactory.DirectWriteFactory.GetSystemFontCollection(
                    checkForUpdates: false);

            return fontCollection;
        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.textFormat != null)
            {
                this.textFormat.Dispose();
                this.textFormat = null;
            }

            if (this.fontFace != null)
            {
                this.fontFace.Dispose();
                this.fontFace = null;
            }

            base.Dispose(disposing);
        }

        private sw.FontMetrics? gdiCompatibleMetrics;
        private sw.FontMetrics GdiCompatibleMetrics
        {
            get
            {
                if(gdiCompatibleMetrics == null)
                    gdiCompatibleMetrics =
                        this.fontFace.GetGdiCompatibleMetrics(
                            emSize: this.Widget.Size, // TODO: should this be in pixels?
                            pixelsPerDip: 1,
                            transform: null);

                return gdiCompatibleMetrics.Value;
            }
        }

        public float AscentInPixels
        {
            get
            {
                return this.GdiCompatibleMetrics.Ascent;
            }
        }

        public float DescentInPixels
        {
            get { return this.GdiCompatibleMetrics.Descent; }
        }

        public float XHeightInPixels
        {
            get { return this.GdiCompatibleMetrics.XHeight; }
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

        private float? lineHeightInPixels = null;
        public float LineHeightInPixels
        {
            get 
            {
                return AscentInPixels +
                    DescentInPixels +
                    GdiCompatibleMetrics.LineGap;
            }
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
			get { throw new NotImplementedException(); }
		}

		public float Ascent
		{
			get { throw new NotImplementedException(); }
		}

		public float Descent
		{
			get { throw new NotImplementedException(); }
		}

		public float LineHeight
		{
			get { throw new NotImplementedException(); }
		}

		public float Leading
		{
			get { throw new NotImplementedException(); }
		}

		public float Baseline
		{
			get { throw new NotImplementedException(); }
		}


		public object ControlObject
		{
			get { throw new NotImplementedException(); }
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			throw new NotImplementedException();
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			throw new NotImplementedException();
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			throw new NotImplementedException();
		}

		public FontDecoration FontDecoration
		{
			get { throw new NotImplementedException(); }
		}
	}
}
