using System;
using Eto.Drawing;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using sw = System.Windows;
using sd = System.Drawing;

namespace Eto.Wpf.Drawing
{
	public class FontHandler : WidgetHandler<object, Font>, IFont
	{
		FontTypeface typeface;
		FontDecoration decoration;
		sd.Font sdfont;

		public void Apply(swc.Control control, Action<sw.TextDecorationCollection> setDecorations)
		{
			control.FontFamily = WpfFamily;
			control.FontStyle = WpfFontStyle;
			control.FontWeight = WpfFontWeight;
			control.FontSize = PixelSize;
			if (setDecorations != null && WpfTextDecorations != null)
			{
				setDecorations(WpfTextDecorations);
			}
		}

		sd.Font SDFont
		{
			get
			{
				if (sdfont == null)
				{
					var style = sd.FontStyle.Regular;
					if (Widget.Bold) style |= sd.FontStyle.Bold;
					if (Widget.Italic) style |= sd.FontStyle.Italic;
					if (Widget.Underline) style |= sd.FontStyle.Underline;
					if (Widget.Strikethrough) style |= sd.FontStyle.Strikeout;
					sdfont = new sd.Font(WpfFamily.Source, (float)Size, style);
				}
				return sdfont;
			}
		}

		public double PixelSize
		{
			get
			{
				/*if (sw.Application.Current.MainWindow != null) {
					// adjust font size for DPI settings
					var m = sw.PresentationSource.FromVisual (sw.Application.Current.MainWindow).CompositionTarget.TransformToDevice;
					return PointsToPixels (Size * m.M22);
				}
				else*/
				return PointsToPixels(Size);
			}
			set
			{
				Size = PixelsToPoints(value);
			}
		}

		public static double PointsToPixels(double points)
		{
			return points * (96.0 / 72.0);
		}

		public static double PixelsToPoints(double points, swc.Control control = null)
		{
			if (control != null)
			{
				var m = sw.PresentationSource.FromVisual(sw.Application.Current.MainWindow).CompositionTarget.TransformToDevice;
				points /= m.M22;
			}
			return points * (72.0 / 96.0);
		}

		public static Font Apply(swc.Control control, Action<sw.TextDecorationCollection> setDecorations, Font font)
		{
			if (control == null) return font;
			if (font != null)
			{
				((FontHandler)font.Handler).Apply(control, setDecorations);
			}
			else
			{
				control.SetValue(swc.Control.FontFamilyProperty, swc.Control.FontFamilyProperty.DefaultMetadata.DefaultValue);
				control.SetValue(swc.Control.FontStyleProperty, swc.Control.FontStyleProperty.DefaultMetadata.DefaultValue);
				control.SetValue(swc.Control.FontWeightProperty, swc.Control.FontWeightProperty.DefaultMetadata.DefaultValue);
				control.SetValue(swc.Control.FontSizeProperty, swc.Control.FontSizeProperty.DefaultMetadata.DefaultValue);
			}
			return font;

		}

		public sw.FontStyle WpfFontStyle { get; set; }

		public sw.TextDecorationCollection WpfTextDecorations { get; set; }

		public sw.FontWeight WpfFontWeight { get; set; }

		public double Size { get; set; }

		public FontHandler()
		{
		}

		public FontHandler(Eto.Generator generator, swc.Control control)
		{
			this.Family = new FontFamily(generator, new FontFamilyHandler(control.FontFamily));
			this.Size = PixelsToPoints(control.FontSize, control);
			this.WpfFontStyle = control.FontStyle;
			this.WpfFontWeight = control.FontWeight;
		}

		public FontHandler(Eto.Generator generator, swm.FontFamily family, double size, sw.FontStyle style, sw.FontWeight weight)
		{
			Family = new FontFamily(generator, new FontFamilyHandler(family));
			Size = size;
			WpfFontStyle = style;
			WpfFontWeight = weight;
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			Family = family;
			Size = size;
			SetStyle(style);
			SetDecorations(decoration);
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			this.typeface = typeface;
			Family = typeface.Family;
			Size = size;
			WpfFontWeight = WpfTypeface.Weight;
			WpfFontStyle = WpfTypeface.Style;
			SetDecorations(decoration);
		}

		void SetStyle(FontStyle style)
		{
			WpfFontWeight = style.HasFlag(FontStyle.Bold) ? sw.FontWeights.Bold : sw.FontWeights.Normal;

			WpfFontStyle = style.HasFlag(FontStyle.Italic) ? sw.FontStyles.Italic : sw.FontStyles.Normal;
		}

		void SetDecorations(FontDecoration decoration)
		{
			WpfTextDecorations = new sw.TextDecorationCollection();
			if (decoration.HasFlag(FontDecoration.Underline))
				WpfTextDecorations.Add(sw.TextDecorations.Underline);
			if (decoration.HasFlag(FontDecoration.Strikethrough))
				WpfTextDecorations.Add(sw.TextDecorations.Strikethrough);
			this.decoration = decoration;
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			switch (systemFont)
			{
				case SystemFont.Label:
				case SystemFont.Default:
				case SystemFont.Message:
				case SystemFont.Palette:
				case SystemFont.TitleBar:
				case SystemFont.ToolTip:
					Family = new FontFamily(Widget.Platform, new FontFamilyHandler(sw.SystemFonts.MessageFontFamily));
					WpfFontStyle = sw.SystemFonts.MessageFontStyle;
					WpfFontWeight = sw.SystemFonts.MessageFontWeight;
					PixelSize = sw.SystemFonts.MessageFontSize;
					break;
				case SystemFont.Bold:
					Family = new FontFamily(Widget.Platform, new FontFamilyHandler(sw.SystemFonts.MessageFontFamily));
					WpfFontStyle = sw.SystemFonts.MessageFontStyle;
					WpfFontWeight = sw.FontWeights.Bold;
					PixelSize = sw.SystemFonts.MessageFontSize;
					break;
				case SystemFont.MenuBar:
				case SystemFont.Menu:
					Family = new FontFamily(Widget.Platform, new FontFamilyHandler(sw.SystemFonts.MenuFontFamily));
					WpfFontStyle = sw.SystemFonts.MenuFontStyle;
					WpfFontWeight = sw.SystemFonts.MenuFontWeight;
					PixelSize = sw.SystemFonts.MenuFontSize;
					break;
				case SystemFont.StatusBar:
					Family = new FontFamily(Widget.Platform, new FontFamilyHandler(sw.SystemFonts.StatusFontFamily));
					WpfFontStyle = sw.SystemFonts.StatusFontStyle;
					WpfFontWeight = sw.SystemFonts.StatusFontWeight;
					PixelSize = sw.SystemFonts.StatusFontSize;
					break;
				default:
					throw new NotSupportedException();
			}
			if (size != null)
				Size = size.Value;
			SetDecorations(decoration);
		}


		public FontFamily Family
		{
			get;
			set;
		}

		public FontTypeface Typeface
		{
			get
			{
				if (typeface == null)
				{
					typeface = new FontTypeface(Family, new FontTypefaceHandler(new swm.Typeface(WpfFamily, WpfFontStyle, WpfFontWeight, sw.FontStretches.Normal)));
				}
				return typeface;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return Conversions.Convert(WpfFontStyle, WpfFontWeight);
			}
		}

		public FontDecoration FontDecoration
		{
			get { return decoration; }
		}

		public swm.FontFamily WpfFamily
		{
			get { return ((FontFamilyHandler)Family.Handler).Control; }
		}

		public swm.Typeface WpfTypeface
		{
			get { return ((FontTypefaceHandler)Typeface.Handler).Control; }
		}

		float IFont.Size
		{
			get { return (float)Size; }
		}

		public float Ascent
		{
			get
			{
				return (float)(Size * WpfFamily.Baseline);
			}
		}

		float? descent;
		public float Descent
		{
			get
			{
				if (descent == null)
				{
					descent = (float)Size * SDFont.FontFamily.GetCellDescent(SDFont.Style) / SDFont.FontFamily.GetEmHeight(SDFont.Style);
				}
				return descent ?? 0f;
			}
		}

		public float LineHeight
		{
			get { return (float)(Size * WpfFamily.LineSpacing); }
		}

		public float XHeight
		{
			get { return (float)(Size * WpfTypeface.XHeight); }
		}

		public float Baseline
		{
			get { return (float)(Size * WpfFamily.Baseline); }
		}

		public float Leading
		{
			get { return LineHeight - (Ascent + Descent); }
		}

		public string FamilyName
		{
			get { return Family.Name; }
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				sdfont.Dispose();
			}
		}
	}
}
