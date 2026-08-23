using swd = System.Windows.Documents;
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
			ApplyTextFormattingMode(control);
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
			ApplyTextFormattingMode(control);
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
			ApplyTextFormattingMode(control);
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
			// note: TextFormattingMode can't be applied to a range, WPF doesn't consider it a text formatting
			// property.  Set it on the document or element that contains the range instead.
		}

		public void Apply(swm.FormattedText control)
		{
			control.SetFontFamily(WpfFamily);
			control.SetFontStyle(WpfFontStyle);
			control.SetFontStretch(WpfFontStretch);
			control.SetFontWeight(WpfFontWeight);
			control.SetFontSize(WpfSize);
			control.SetTextDecorations(WpfTextDecorationsFrozen);
			// note: the formatting mode of a FormattedText is set when it is created, see CreateFormattedText
		}

		void ApplyTextFormattingMode(sw.DependencyObject element)
		{
			// only set it when specified, otherwise let it keep inheriting from its container as usual
			if (TextFormattingMode != null)
				swm.TextOptions.SetTextFormattingMode(element, TextFormattingMode.Value);
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


		public FontFamily Family { get; set; }

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

		public FontStyle FontStyle => WpfConversions.Convert(WpfFontStyle, WpfFontWeight);

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

		float Font.IHandler.Size => (float)Size;

		public float Ascent => (float)(Size * WpfFamily.Baseline);

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

		public float LineHeight => (float)(Size * WpfFamily.LineSpacing);

		public float XHeight => (float)(Size * WpfTypeface.XHeight);

		public float Baseline => (float)(Size * WpfFamily.Baseline);

		public float Leading => LineHeight - (Ascent + Descent);
		
		public float UnderlinePosition => (float)(Size * -WpfTypeface.UnderlinePosition);

		public float UnderlineThickness => (float)(Size * WpfTypeface.UnderlineThickness);

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

		/// <summary>
		/// Gets or sets the text formatting mode used to lay out text with this font, or null (the default)
		/// to use the mode of the element the text is rendered in.
		/// </summary>
		/// <remarks>
		/// WPF lays out text using the <see cref="swm.TextOptions.TextFormattingModeProperty"/> of the element
		/// it is rendered in, which for <see cref="swm.TextFormattingMode.Display"/> snaps each glyph to whole
		/// pixels and can be noticeably wider than the default <see cref="swm.TextFormattingMode.Ideal"/> layout.
		/// Since text Eto measures and draws itself is not part of any element, set this to the mode your text is
		/// rendered with so that measuring and drawing match, e.g. using a style:
		/// <code>Style.Add&lt;FontHandler&gt;(null, h => h.TextFormattingMode = TextFormattingMode.Display);</code>
		/// When specified, this mode is also set on the controls the font is applied to so they render the same
		/// way, otherwise they keep inheriting the mode from their container.
		/// </remarks>
		public swm.TextFormattingMode? TextFormattingMode { get; set; }

		/// <summary>
		/// Gets or sets the pixels per device independent pixel (dip) used to lay out text with this font when
		/// there is nothing to get it from, which defaults to the scale of the system dpi.
		/// </summary>
		/// <remarks>
		/// This does not scale the text or the size it measures, which are always in dips.  It is the device pixel
		/// grid that <see cref="swm.TextFormattingMode.Display"/> snaps the glyphs to, so it only has an effect with
		/// that mode, where measuring for the wrong dpi reports a width the text isn't rendered with.
		///
		/// Drawing and measuring text on a <see cref="Graphics"/> uses the dpi of what is being drawn on instead of
		/// this value, which is only used when there is no target to get it from, such as <see cref="MeasureString(string)"/>.
		/// </remarks>
		public double PixelsPerDip { get; set; } = SystemPixelsPerDip;

		static double? systemPixelsPerDip;

		static double SystemPixelsPerDip
		{
			get
			{
				if (systemPixelsPerDip == null)
				{
					try
					{
						systemPixelsPerDip = swm.VisualTreeHelper.GetDpi(new swm.DrawingVisual()).PixelsPerDip;
					}
					catch
					{
						systemPixelsPerDip = 1.0;
					}
				}
				return systemPixelsPerDip.Value;
			}
		}

		/// <summary>
		/// Creates a WPF FormattedText for the specified text using this font, honoring its
		/// <see cref="TextFormattingMode"/> and <see cref="PixelsPerDip"/>.
		/// </summary>
		/// <param name="text">Text to lay out</param>
		/// <param name="brush">Brush to draw the text with</param>
		/// <param name="setDecorations">True to apply the decorations of the font, false to leave them out</param>
		/// <param name="pixelsPerDip">Pixels per dip of what the text is drawn on, or null to use <see cref="PixelsPerDip"/></param>
		public swm.FormattedText CreateFormattedText(string text, swm.Brush brush, bool setDecorations = true, double? pixelsPerDip = null)
		{
			text = text ?? string.Empty;
			var formattedText = new swm.FormattedText(
				text,
				CultureInfo.CurrentUICulture,
				sw.FlowDirection.LeftToRight,
				WpfTypeface,
				WpfSize,
				brush,
				null,
				TextFormattingMode ?? swm.TextFormattingMode.Ideal,
				pixelsPerDip ?? PixelsPerDip);

			if (setDecorations && WpfTextDecorationsFrozen != null)
				formattedText.SetTextDecorations(WpfTextDecorationsFrozen, 0, text.Length);

			return formattedText;
		}

		public SizeF MeasureString(string text)
		{
			if (measureBrush == null)
				measureBrush = new swm.SolidColorBrush(swm.Colors.White);
			var formattedText = CreateFormattedText(text, measureBrush, setDecorations: false);
			return new SizeF((float)formattedText.WidthIncludingTrailingWhitespace, (float)formattedText.Height);
		}
	}
}
