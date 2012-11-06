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
	public static class FontExtensions
	{
		public static sd.Font ToSD(this Font font)
		{
			if (font == null)
				return null;
			var handler = font.Handler as FontHandler;
			return new sd.Font (handler.Family.Source, (float)handler.Size, ToSD (handler.FontStyle, handler.FontWeight), sd.GraphicsUnit.Point);
		}

		public static Font ToEto (this sd.Font font, Eto.Generator generator)
		{
			if (font == null)
				return null;
			return new Font (generator, new FontHandler (font));
		}

		public static sd.FontStyle ToSD(sw.FontStyle style, sw.FontWeight weight)
		{
			var val = sd.FontStyle.Regular;
			if (style == sw.FontStyles.Italic)
				val |= sd.FontStyle.Italic;
			if (weight == sw.FontWeights.Bold)
				val |= sd.FontStyle.Bold;
			return val;
		}
	}

	public class FontHandler : WidgetHandler<object, Font>, IFont
	{

		public void Apply (swc.Control control)
		{
			control.FontFamily = Family;

			control.FontStyle = FontStyle;
			control.FontWeight = FontWeight;

			control.FontSize = this.PixelSize;
		}

		public double PixelSize
		{
			get
			{
				if (sw.Application.Current.MainWindow != null)
				{
					// adjust font size for DPI settings
					var m = sw.PresentationSource.FromVisual (sw.Application.Current.MainWindow).CompositionTarget.TransformToDevice;
					return PointsToPixels (Size * m.M22);
				}
				else
					return PointsToPixels (Size);
			}
		}


		public static double PointsToPixels (double points)
		{
			return points * (96.0 / 72.0);
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

		string Convert (FontFamily family)
		{
			switch (family) {
				case FontFamily.Monospace:
					return "Courier New";
				case FontFamily.Sans:
					return "Verdana";
				case FontFamily.Serif:
					return "Times New Roman";
				default:
					throw new NotSupportedException ();
			}
		}

		public swm.Typeface Typeface
		{
			get
			{
				return new swm.Typeface (Family, FontStyle, FontWeight, sw.FontStretches.Normal);
			}
		}

		public System.Windows.Media.FontFamily Family
		{
			get; set;
		}

		public System.Windows.FontStyle FontStyle
		{
			get; set;
		}

		public System.Windows.FontWeight FontWeight
		{
			get;
			set;
		}

		public double Size
		{
			get; set; 
		}

		public FontHandler ()
		{
		}

		public FontHandler (sd.Font font)
		{
			this.Size = font.SizeInPoints;
			this.FontWeight = font.Bold ? sw.FontWeights.Bold : sw.FontWeights.Normal;
			this.FontStyle = font.Italic ? sw.FontStyles.Italic : sw.FontStyles.Normal;
			this.Family = new swm.FontFamily(font.FontFamily.Name);
		}

		public void Create (FontFamily family, float size, FontStyle style)
		{
			this.Family = new System.Windows.Media.FontFamily (Convert (family));
			this.Size = size;
			if ((style & Eto.Drawing.FontStyle.Bold) != 0) this.FontWeight = System.Windows.FontWeights.Bold;
			else this.FontWeight = System.Windows.FontWeights.Normal;

			if ((style & Eto.Drawing.FontStyle.Italic) != 0) this.FontStyle = System.Windows.FontStyles.Italic;
			else this.FontStyle = System.Windows.FontStyles.Normal;
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
					Family = System.Windows.SystemFonts.MessageFontFamily;
					FontStyle = System.Windows.SystemFonts.MessageFontStyle;
					FontWeight = System.Windows.SystemFonts.MessageFontWeight;
					Size = System.Windows.SystemFonts.MessageFontSize;
					break;
				case SystemFont.Bold:
					Family = System.Windows.SystemFonts.MessageFontFamily;
					FontStyle = System.Windows.SystemFonts.MessageFontStyle;
					FontWeight = System.Windows.FontWeights.Bold;
					Size = System.Windows.SystemFonts.MessageFontSize;
					break;
				case SystemFont.MenuBar:
				case SystemFont.Menu:
					Family = System.Windows.SystemFonts.MenuFontFamily;
					FontStyle = System.Windows.SystemFonts.MenuFontStyle;
					FontWeight = System.Windows.SystemFonts.MenuFontWeight;
					Size = System.Windows.SystemFonts.MenuFontSize;
					break;
				case SystemFont.StatusBar:
					Family = System.Windows.SystemFonts.StatusFontFamily;
					FontStyle = System.Windows.SystemFonts.StatusFontStyle;
					FontWeight = System.Windows.SystemFonts.StatusFontWeight;
					Size = System.Windows.SystemFonts.StatusFontSize;
					break;
				default:
					throw new NotSupportedException ();
			}
			if (size != null) Size = size.Value;
		}


		public bool Bold
		{
			get { return this.FontWeight == System.Windows.FontWeights.Bold; }
		}

		public bool Italic
		{
			get { return this.FontStyle == System.Windows.FontStyles.Italic; }
		}

		float IFont.Size
		{
			get { return (float)this.Size; }
		}

		public string FontName
		{
			get { return Family.Source; }
		}
	}
}
