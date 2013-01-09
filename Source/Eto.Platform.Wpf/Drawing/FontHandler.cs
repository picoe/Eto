using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using sw = System.Windows;
using sd = System.Drawing;

namespace Eto.Platform.Wpf.Drawing
{
	public class FontHandler : WidgetHandler<object, Font>, IFont
	{
		FontTypeface typeface;
		sd.Font sdfont;

		public void Apply (swc.Control control)
		{
			control.FontFamily = WpfFamily;
			control.FontStyle = WpfFontStyle;
			control.FontWeight = WpfFontWeight;

			control.FontSize = this.PixelSize;
		}

		sd.Font SDFont
		{
			get
			{
				if (sdfont == null) {
					var style = sd.FontStyle.Regular;
					if (Widget.Bold) style |= sd.FontStyle.Bold;
					if (Widget.Italic) style |= sd.FontStyle.Italic;
					sdfont = new sd.Font (WpfFamily.Source, (float)Size, style);
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
					return PointsToPixels (Size);
			}
		}

		public static double PointsToPixels (double points)
		{
			return points * (96.0 / 72.0);
		}

		public static double PixelsToPoints (double points, swc.Control control = null)
		{
			if (control != null) {
				var m = sw.PresentationSource.FromVisual (sw.Application.Current.MainWindow).CompositionTarget.TransformToDevice;
				points /= m.M22;
			}
			return points * (72.0 / 96.0);
		}

		public static void Apply (swc.Control control, Font font)
		{
			if (control == null) return;
			if (font != null) {
				((FontHandler)font.Handler).Apply (control);
			}
			else {
				control.SetValue (swc.Control.FontFamilyProperty, swc.Control.FontFamilyProperty.DefaultMetadata.DefaultValue);
				control.SetValue (swc.Control.FontStyleProperty, swc.Control.FontStyleProperty.DefaultMetadata.DefaultValue);
				control.SetValue (swc.Control.FontWeightProperty, swc.Control.FontWeightProperty.DefaultMetadata.DefaultValue);
				control.SetValue (swc.Control.FontSizeProperty, swc.Control.FontSizeProperty.DefaultMetadata.DefaultValue);
			}

		}

		public sw.FontStyle WpfFontStyle
		{
			get;
			set;
		}

		public sw.FontWeight WpfFontWeight
		{
			get;
			set;
		}

		public double Size
		{
			get;
			set;
		}

		public FontHandler ()
		{
		}

		public FontHandler (Eto.Generator generator, swc.Control control)
		{
			this.Family = new FontFamily (generator, new FontFamilyHandler (control.FontFamily));
			this.Size = PixelsToPoints (control.FontSize, control);
			this.WpfFontStyle = control.FontStyle;
			this.WpfFontWeight = control.FontWeight;
		}

		public FontHandler (Eto.Generator generator, swm.FontFamily family, double size, sw.FontStyle style, sw.FontWeight weight)
		{
			this.Family = new FontFamily(generator, new FontFamilyHandler (family));
			this.Size = size;
			this.WpfFontStyle = style;
			this.WpfFontWeight = weight;
		}

		public void Create (FontFamily family, float size, FontStyle style)
		{
			this.Family = family;
			this.Size = size;
			SetStyle (style);
		}

		public void Create (FontTypeface typeface, float size)
		{
			this.typeface = typeface;
			this.Family = typeface.Family;
			this.Size = size;
			this.WpfFontWeight = WpfTypeface.Weight;
			this.WpfFontStyle = WpfTypeface.Style;
		}

		void SetStyle (FontStyle style)
		{
			if ((style & Eto.Drawing.FontStyle.Bold) != 0) this.WpfFontWeight = System.Windows.FontWeights.Bold;
			else this.WpfFontWeight = System.Windows.FontWeights.Normal;

			if ((style & Eto.Drawing.FontStyle.Italic) != 0) this.WpfFontStyle = System.Windows.FontStyles.Italic;
			else this.WpfFontStyle = System.Windows.FontStyles.Normal;
		}

		public void Create (SystemFont systemFont, float? size)
		{

			switch (systemFont) {
			case SystemFont.Label:
			case SystemFont.Default:
			case SystemFont.Message:
			case SystemFont.Palette:
			case SystemFont.TitleBar:
			case SystemFont.ToolTip:
				Family = new FontFamily(Widget.Generator, new FontFamilyHandler (sw.SystemFonts.MessageFontFamily));
				WpfFontStyle = System.Windows.SystemFonts.MessageFontStyle;
				WpfFontWeight = System.Windows.SystemFonts.MessageFontWeight;
				Size = System.Windows.SystemFonts.MessageFontSize;
				break;
			case SystemFont.Bold:
				Family = new FontFamily(Widget.Generator, new FontFamilyHandler (sw.SystemFonts.MessageFontFamily));
				WpfFontStyle = System.Windows.SystemFonts.MessageFontStyle;
				WpfFontWeight = System.Windows.FontWeights.Bold;
				Size = System.Windows.SystemFonts.MessageFontSize;
				break;
			case SystemFont.MenuBar:
			case SystemFont.Menu:
				Family = new FontFamily(Widget.Generator, new FontFamilyHandler (sw.SystemFonts.MenuFontFamily));
				WpfFontStyle = System.Windows.SystemFonts.MenuFontStyle;
				WpfFontWeight = System.Windows.SystemFonts.MenuFontWeight;
				Size = System.Windows.SystemFonts.MenuFontSize;
				break;
			case SystemFont.StatusBar:
				Family = new FontFamily(Widget.Generator, new FontFamilyHandler (sw.SystemFonts.StatusFontFamily));
				WpfFontStyle = System.Windows.SystemFonts.StatusFontStyle;
				WpfFontWeight = System.Windows.SystemFonts.StatusFontWeight;
				Size = System.Windows.SystemFonts.StatusFontSize;
				break;
			default:
				throw new NotSupportedException ();
			}
			if (size != null) Size = size.Value;
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
				if (typeface == null) {
					typeface = new FontTypeface(Family, new FontTypefaceHandler(new swm.Typeface (WpfFamily, WpfFontStyle, WpfFontWeight, sw.FontStretches.Normal)));
				}
				return typeface;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return Conversions.Convert (WpfFontStyle, WpfFontWeight);
			}
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
				if (descent == null) {
					descent = (float)Size * SDFont.FontFamily.GetCellDescent (SDFont.Style) / SDFont.FontFamily.GetEmHeight (SDFont.Style);
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

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing) {
				sdfont.Dispose ();
			}
		}
    }
}
