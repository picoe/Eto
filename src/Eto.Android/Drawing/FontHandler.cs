using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using au = Android.Util;
using ag = Android.Graphics;
using at = Android.Text;

// Useful: https://stackoverflow.com/questions/19691530/valid-values-for-androidfontfamily-and-what-they-map-to?rq=1

namespace Eto.Android.Drawing
{
	/// <summary>
	/// Handler for <see cref="IFont"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontHandler : WidgetHandler<ag.Typeface, Font>, Font.IHandler
	{
		float size;
		FontFamily family;
		FontStyle style;

		/// <summary>
		/// Used by GraphicsHandler.MeasureText
		/// </summary>
		at.TextPaint paint;
		public at.TextPaint Paint
		{
			get
			{
				if (paint == null)
				{
					paint = new at.TextPaint();
					paint.SetTypeface(this.Control);
					paint.StrikeThruText = decoration.HasFlag(FontDecoration.Strikethrough);
					paint.UnderlineText = decoration.HasFlag(FontDecoration.Underline);
					// Dimensions for TextPaint should be in Px, but when drawing we apply a transform matrix to convert to Dp to Px, so actually set the size in Dp.
					paint.TextSize = Platform.PtToDp(Size);
					}
				return paint;
			}
		}

		ag.Paint.FontMetrics fontMetrics;
		ag.Paint.FontMetrics FontMetrics { get { return fontMetrics = fontMetrics ?? Paint.GetFontMetrics(); } }

		public FontHandler()
		{
		}

		public FontHandler(at.TextPaint paint)
			: this(paint.Typeface, Platform.PxToPt(paint.TextSize))
		{
		}

		public FontHandler(ag.Typeface typeface, float size)
		{
			this.family = Fonts.AvailableFontFamilies.FirstOrDefault();
			this.size = size;
			this.style = typeface.Style.ToEto();
			Control = typeface;
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			this.size = size;
			this.family = family;
			this.style = style;
			this.decoration = decoration;
			this.Control = ag.Typeface.Create(family.ControlObject as ag.Typeface, style.ToAndroid());
		}

		// TODO:
		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			ag.TypefaceStyle style;
			ag.Typeface face;

			switch (systemFont)
			{
			case SystemFont.Bold:
				style = ag.TypefaceStyle.Bold;
				face = ag.Typeface.DefaultBold;
				break;

			case SystemFont.Default:
			case SystemFont.Label:
			case SystemFont.TitleBar:
			case SystemFont.ToolTip:
			case SystemFont.MenuBar:
			case SystemFont.Menu:
			case SystemFont.Message:
			case SystemFont.Palette:
			case SystemFont.StatusBar:
			default:
				style = ag.TypefaceStyle.Normal;
				face = ag.Typeface.Default;
				break;
			}
			
			Control = ag.Typeface.Create(face, style);
			this.size = size ?? Platform.PxToPt(new aw.EditText(Platform.AppContextThemed).TextSize);

			family = new FontFamily("Regular");
			this.decoration = decoration;
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			this.size = size;
			this.family = typeface.Family;
			this.style = typeface.FontStyle;
			this.decoration = decoration;
			this.Control = ag.Typeface.Create(typeface.Family.Name, typeface.FontStyle.ToAndroid());
		}

		public float XHeight
		{
			get { throw new NotImplementedException(); }
		}

		public float Ascent
		{
			get { return Math.Abs(FontMetrics.Ascent); } // TODO: does this need to be scaled?
		}

		public float Descent
		{
			get { return Math.Abs(FontMetrics.Descent); } // TODO: does this need to be scaled?
		}

		public float LineHeight
		{
			get { return Math.Abs(FontMetrics.Top) + Math.Abs(FontMetrics.Bottom); }
		}

		public float Leading
		{
			get { return Math.Abs(FontMetrics.Leading); } // TODO: does this need to be scaled?
		}

		public float Baseline
		{
			get { return 0f; }
		}

		public float Size
		{
			get { return size; }
		}

		public string FamilyName
		{
			get { return Family?.Name; }
		}

		public FontStyle FontStyle
		{
			get { return style; }
		}

		public FontDecoration FontDecoration
		{
			get { return decoration; }
		}

		FontDecoration decoration;

		public FontFamily Family
		{
			get { return family; }
		}

		public FontTypeface Typeface
		{
			get { return new FontTypeface(Family, new FontTypefaceHandler(FontStyle.ToAndroid())); }
		}

		public SizeF MeasureString(string text)
		{
			var w = Paint.MeasureText(text);
			var h = LineHeight;
			return new SizeF(w, h);
		}
	}
}
