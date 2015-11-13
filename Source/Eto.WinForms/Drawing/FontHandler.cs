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
		FontTypeface typeface;
		FontFamily family;

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
			this.family = family;
			var familyHandler = (FontFamilyHandler)family.Handler;
			Control = new sd.Font(familyHandler.Control, size, style.ToSD() | decoration.ToSD());
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			this.typeface = typeface;

			var familyHandler = (FontFamilyHandler)typeface.Family.Handler;
			Control = new sd.Font(familyHandler.Control, size, typeface.FontStyle.ToSD() | decoration.ToSD());
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

		public string FamilyName
		{
			get { return Control.FontFamily.Name; }
		}

		public FontStyle FontStyle
		{
			get { return Control.Style.ToEtoStyle(); }
		}

		public FontDecoration FontDecoration
		{
			get { return Control.Style.ToEtoDecoration(); }
		}

		public FontFamily Family
		{
			get { return family = family ?? new FontFamily(new FontFamilyHandler(Control.FontFamily)); }
		}

		public FontTypeface Typeface
		{
			get { return typeface = typeface ?? new FontTypeface(Family, new FontTypefaceHandler(Control.Style)); }
		}

		public sd.FontFamily WindowsFamily
		{
			get { return Control.FontFamily; }
		}

		public float XHeight
		{
			get { return Size * 0.5f; }
		}

		public float Baseline
		{
			get { return Ascent; }
		}

		public float Leading
		{
			get { return LineHeight - (Ascent + Descent); }
		}

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

		public float LineHeight { get { return Size * Control.FontFamily.GetLineSpacing(Control.Style) / Control.FontFamily.GetEmHeight(Control.Style); } }

		public float Size { get { return Control.SizeInPoints; } }

		public sd.Font GetFont()
		{
			return Control;
		}

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
