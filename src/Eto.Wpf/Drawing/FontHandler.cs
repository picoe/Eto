using System;
using Eto.Drawing;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using sw = System.Windows;
using sd = System.Drawing;
using swd = System.Windows.Documents;
using System.Globalization;

namespace Eto.Wpf.Drawing
{
	public class FontHandler : WidgetHandler<object, Font>, Font.IHandler
	{
		FontTypeface typeface;
		FontDecoration decoration;
		sd.Font sdfont;

		public static bool ShowSimulatedFonts = false;

		public void Apply(swc.Control control, Action<sw.TextDecorationCollection> setDecorations)
		{
			control.FontFamily = WpfFamily;
			control.FontStyle = WpfFontStyle;
			control.FontStretch = WpfFontStretch;
			control.FontWeight = WpfFontWeight;
			control.FontSize = WpfSize;
			if (setDecorations != null && WpfTextDecorationsFrozen != null)
			{
				setDecorations(WpfTextDecorationsFrozen);
			}
		}

		public void Apply(swc.TextBlock control, Action<sw.TextDecorationCollection> setDecorations)
		{
			control.FontFamily = WpfFamily;
			control.FontStyle = WpfFontStyle;
			control.FontStretch = WpfFontStretch;
			control.FontWeight = WpfFontWeight;
			control.FontSize = WpfSize;
			if (setDecorations != null && WpfTextDecorationsFrozen != null)
			{
				setDecorations(WpfTextDecorationsFrozen);
			}
		}

		public void Apply(swd.TextElement control, Action<sw.TextDecorationCollection> setDecorations)
		{
			control.FontFamily = WpfFamily;
			control.FontStyle = WpfFontStyle;
			control.FontStretch = WpfFontStretch;
			control.FontWeight = WpfFontWeight;
			control.FontSize = WpfSize;
			if (setDecorations != null && WpfTextDecorationsFrozen != null)
			{
				setDecorations(WpfTextDecorationsFrozen);
			}
		}

		public void Apply(swd.TextRange control)
		{
			control.ApplyPropertyValue(swd.TextElement.FontFamilyProperty, WpfFamily);
			control.ApplyPropertyValue(swd.TextElement.FontStyleProperty, WpfFontStyle);
			control.ApplyPropertyValue(swd.TextElement.FontStretchProperty, WpfFontStretch);
			control.ApplyPropertyValue(swd.TextElement.FontWeightProperty, WpfFontWeight);
			control.ApplyPropertyValue(swd.TextElement.FontSizeProperty, WpfSize);
			control.ApplyPropertyValue(swd.Inline.TextDecorationsProperty, WpfTextDecorationsFrozen);
		}

		public void Apply(swm.FormattedText control)
		{
			control.SetFontFamily(WpfFamily);
			control.SetFontStyle(WpfFontStyle);
			control.SetFontStretch(WpfFontStretch);
			control.SetFontWeight(WpfFontWeight);
			control.SetFontSize(WpfSize);
			control.SetTextDecorations(WpfTextDecorationsFrozen);
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

		public double WpfSize
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

		public static double PixelsToPoints(double pixels, sw.FrameworkElement control = null)
		{
			if (control != null)
			{
				var source = sw.PresentationSource.FromVisual(control.GetParentWindow());
				if (source != null)
				{
					var m = source.CompositionTarget.TransformToDevice;
					pixels /= m.M22;
				}
			}
			return pixels * (72.0 / 96.0);
		}

		public sw.FontStyle WpfFontStyle { get; private set; }

		public sw.FontStretch WpfFontStretch { get; private set; }

		sw.TextDecorationCollection WpfTextDecorations { get; set; }

		public sw.TextDecorationCollection WpfTextDecorationsFrozen { get; private set; }

		void SetFrozenDecorations() => WpfTextDecorationsFrozen = (sw.TextDecorationCollection)WpfTextDecorations?.GetAsFrozen();

		public sw.FontWeight WpfFontWeight { get; private set; }

		public double Size { get; private set; }

		public FontHandler()
		{
		}

		public FontHandler(swc.Control control)
		{
			this.Family = new FontFamily(new FontFamilyHandler(control.FontFamily));
			this.Size = PixelsToPoints(control.FontSize);
			this.WpfFontStyle = control.FontStyle;
			this.WpfFontStretch = control.FontStretch;
			this.WpfFontWeight = control.FontWeight;
		}

		public FontHandler(swc.TextBlock control)
		{
			this.Family = new FontFamily(new FontFamilyHandler(control.FontFamily));
			this.Size = PixelsToPoints(control.FontSize);
			this.WpfFontStyle = control.FontStyle;
			this.WpfFontStretch = control.FontStretch;
			this.WpfFontWeight = control.FontWeight;
			var decorations = control.TextDecorations;
			if (decorations != null)
			{
				this.WpfTextDecorations = new sw.TextDecorationCollection(decorations);
				SetFrozenDecorations();
			}
		}

		public FontHandler(swd.TextSelection range, sw.FrameworkElement control)
		{
			var wpfFamily = range.GetPropertyValue(swd.TextElement.FontFamilyProperty) as swm.FontFamily ?? swd.TextElement.GetFontFamily(control);
			this.Family = new FontFamily(new FontFamilyHandler(wpfFamily));
			Size = PixelsToPoints(range.GetPropertyValue(swd.TextElement.FontSizeProperty) as double? ?? swd.TextElement.GetFontSize(control));
			this.WpfFontStyle = range.GetPropertyValue(swd.TextElement.FontStyleProperty) as sw.FontStyle? ?? swd.TextElement.GetFontStyle(control);
			this.WpfFontStretch = range.GetPropertyValue(swd.TextElement.FontStretchProperty) as sw.FontStretch? ?? swd.TextElement.GetFontStretch(control);
			this.WpfFontWeight = range.GetPropertyValue(swd.TextElement.FontWeightProperty) as sw.FontWeight? ?? swd.TextElement.GetFontWeight(control);
			var decorations = range.GetPropertyValue(swd.Inline.TextDecorationsProperty) as sw.TextDecorationCollection;
			if (decorations != null)
			{
				this.WpfTextDecorations = new sw.TextDecorationCollection(decorations);
				SetFrozenDecorations();
			}
		}

		public FontHandler(swm.FontFamily family, double size, sw.FontStyle style, sw.FontWeight weight, sw.FontStretch stretch)
		{
			Family = new FontFamily(new FontFamilyHandler(family));
			Size = size;
			WpfFontStyle = style;
			WpfFontStretch = stretch;
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
			WpfFontStretch = WpfTypeface.Stretch;
			WpfFontStyle = WpfTypeface.Style;
			SetDecorations(decoration);
		}

		void SetStyle(FontStyle style)
		{
			WpfFontWeight = style.HasFlag(FontStyle.Bold) ? sw.FontWeights.Bold : sw.FontWeights.Normal;

			WpfFontStyle = style.HasFlag(FontStyle.Italic) ? sw.FontStyles.Italic : sw.FontStyles.Normal;

			WpfFontStretch = sw.FontStretches.Normal;
		}

		void SetDecorations(FontDecoration decoration)
		{
			WpfTextDecorations = new sw.TextDecorationCollection();
			if (decoration.HasFlag(FontDecoration.Underline))
				WpfTextDecorations.Add(sw.TextDecorations.Underline);
			if (decoration.HasFlag(FontDecoration.Strikethrough))
				WpfTextDecorations.Add(sw.TextDecorations.Strikethrough);
			SetFrozenDecorations();
			this.decoration = decoration;
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			WpfFontStretch = sw.FontStretches.Normal;
			switch (systemFont)
			{
				case SystemFont.Label:
				case SystemFont.User:
				case SystemFont.Default:
				case SystemFont.Message:
				case SystemFont.Palette:
				case SystemFont.TitleBar:
				case SystemFont.ToolTip:
					Family = new FontFamily(new FontFamilyHandler(sw.SystemFonts.MessageFontFamily));
					WpfFontStyle = sw.SystemFonts.MessageFontStyle;
					WpfFontWeight = sw.SystemFonts.MessageFontWeight;
					WpfSize = sw.SystemFonts.MessageFontSize;
					break;
				case SystemFont.Bold:
					Family = new FontFamily(new FontFamilyHandler(sw.SystemFonts.MessageFontFamily));
					WpfFontStyle = sw.SystemFonts.MessageFontStyle;
					WpfFontWeight = sw.FontWeights.Bold;
					WpfSize = sw.SystemFonts.MessageFontSize;
					break;
				case SystemFont.MenuBar:
				case SystemFont.Menu:
					Family = new FontFamily(new FontFamilyHandler(sw.SystemFonts.MenuFontFamily));
					WpfFontStyle = sw.SystemFonts.MenuFontStyle;
					WpfFontWeight = sw.SystemFonts.MenuFontWeight;
					WpfSize = sw.SystemFonts.MenuFontSize;
					break;
				case SystemFont.StatusBar:
					Family = new FontFamily(new FontFamilyHandler(sw.SystemFonts.StatusFontFamily));
					WpfFontStyle = sw.SystemFonts.StatusFontStyle;
					WpfFontWeight = sw.SystemFonts.StatusFontWeight;
					WpfSize = sw.SystemFonts.StatusFontSize;
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
					typeface = new FontTypeface(Family, new FontTypefaceHandler(new swm.Typeface(WpfFamily, WpfFontStyle, WpfFontWeight, WpfFontStretch)));
				}
				return typeface;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return WpfConversions.Convert(WpfFontStyle, WpfFontWeight);
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

		float Font.IHandler.Size
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
				if (sdfont != null)
				{
					sdfont.Dispose();
					sdfont = null;
				}
			}
		}

		static swm.SolidColorBrush measureBrush;

		public SizeF MeasureString(string text)
		{
			if (measureBrush == null)
				measureBrush = new swm.SolidColorBrush(swm.Colors.White);
			var formattedText = new swm.FormattedText(text, CultureInfo.CurrentUICulture, sw.FlowDirection.LeftToRight, WpfTypeface, WpfSize, measureBrush);
			return new SizeF((float)formattedText.WidthIncludingTrailingWhitespace, (float)formattedText.Height);
		}
	}
}
