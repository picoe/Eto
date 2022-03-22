using System;
using Eto.Drawing;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using System.ComponentModel;

namespace Eto.WinForms.Drawing
{
	public interface IWindowsFontSource
	{
		sd.Font GetFont();
	}

	public class FontHandler : WidgetHandler<System.Drawing.Font, Font>, Font.IHandler, IWindowsFontSource
	{
		FontTypeface _typeface;
		FontFamily _family;

		public FontHandler()
		{
			UseCompatibleTextRendering = true;
        }

		public FontHandler(sd.Font font)
			: this()
		{
			Control = font;
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			_family = family;
			var familyHandler = (FontFamilyHandler)family.Handler;
			Control = new sd.Font(familyHandler.Control, size, style.ToSD() | decoration.ToSD());
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			_typeface = typeface;
			var typefaceHandler = (FontTypefaceHandler)typeface.Handler;
			_family = typefaceHandler.Family;
			Control = new sd.Font(typefaceHandler.SDFontFamily, size, typefaceHandler.Control | decoration.ToSD());
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			Control = systemFont.ToSD();
			if (size != null || decoration != FontDecoration.None)
			{
				var newsize = size ?? Control.SizeInPoints;
				Control = new sd.Font(Control.FontFamily, newsize, Control.Style | decoration.ToSD(), sd.GraphicsUnit.Point);
			}
		}

		public string FamilyName => Control.FontFamily.Name;

		public FontStyle FontStyle => Control.Style.ToEtoStyle();

		public FontDecoration FontDecoration => Control.Style.ToEtoDecoration();

		public FontFamily Family => _family ?? (_family = new FontFamily(new FontFamilyHandler(Control.FontFamily)));

		public FontTypeface Typeface => _typeface ?? (_typeface = new FontTypeface(Family, new FontTypefaceHandler(Control.FontFamily, Control.Style)));

		public sd.FontFamily WindowsFamily => Control.FontFamily;

		public float XHeight => Size * 0.5f;

		public float Baseline => Ascent;

		public float Leading => LineHeight - (Ascent + Descent);

		float? ascent;
		public float Ascent
		{
			get
			{
				ascent = ascent ?? Size * Control.FontFamily.GetCellAscent(Control.Style) / Control.FontFamily.GetEmHeight(Control.Style);
				return ascent ?? 0f;
			}
		}

		float? descent;
		/// <summary>
		/// Gets the descent of the font
		/// </summary>
		/// <remarks>
		/// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
		/// </remarks>
		public float Descent
		{
			get
			{
				descent = descent ?? Size * Control.FontFamily.GetCellDescent(Control.Style) / Control.FontFamily.GetEmHeight(Control.Style);
				return descent ?? 0f;
			}
		}

		public float LineHeight => Size * Control.FontFamily.GetLineSpacing(Control.Style) / Control.FontFamily.GetEmHeight(Control.Style);

		public float Size => Control.SizeInPoints;

		public sd.Font GetFont() => Control;

		[DefaultValue(true)]
		public bool UseCompatibleTextRendering { get; set; }

		sd.Graphics measureGraphics;

		public SizeF MeasureString(string text)
		{
			if (UseCompatibleTextRendering)
			{
				if (measureGraphics == null)
					measureGraphics = sd.Graphics.FromImage(new sd.Bitmap(1, 1));

				sd.CharacterRange[] ranges = { new sd.CharacterRange(0, text.Length) };
				var stringFormat = GraphicsHandler.DefaultStringFormat;
				lock (stringFormat)
				{
					stringFormat.SetMeasurableCharacterRanges(ranges);
					var regions = measureGraphics.MeasureCharacterRanges(text, Control, sd.RectangleF.Empty, stringFormat);
					var rect = regions[0].GetBounds(measureGraphics);
					return rect.Size.ToEto();
				}
			}

			var size = swf.TextRenderer.MeasureText(text, Control, sd.Size.Empty, GraphicsHandler.DefaultTextFormat);
			return size.ToEto();
		}
	}
}
