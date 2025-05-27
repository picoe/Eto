namespace Eto.WinUI;

public static class WinUIConversions
{
	public const float WheelDelta = 120f;

	public static readonly sw.Size PositiveInfinitySize = new sw.Size(double.PositiveInfinity, double.PositiveInfinity);
	public static readonly sw.Size ZeroSize = new sw.Size(0, 0);
	public static readonly sw.Size NaNSize = new sw.Size(double.NaN, double.NaN);

	public static wu.Color ToWinUI(this Color value)
	{
		return wu.Color.FromArgb((byte)(value.A * byte.MaxValue), (byte)(value.R * byte.MaxValue), (byte)(value.G * byte.MaxValue), (byte)(value.B * byte.MaxValue));
	}

	public static muxm.Brush ToWinUIBrush(this Color value)
	{
		var solidBrush = new muxm.SolidColorBrush();
		solidBrush.Color = value.ToWinUI();
		return solidBrush;
	}

	public static Color ToEto(this wu.Color value)
	{
		return new Color { A = value.A / 255f, R = value.R / 255f, G = value.G / 255f, B = value.B / 255f };
	}

	public static Color ToEtoColor(this muxm.Brush brush)
	{
		var solidBrush = brush as muxm.SolidColorBrush;
		if (solidBrush != null)
			return solidBrush.Color.ToEto();
		return Colors.Transparent;
	}

	public static Padding ToEto(this mux.Thickness value)
	{
		return new Padding((int)value.Left, (int)value.Top, (int)value.Right, (int)value.Bottom);
	}

	public static mux.Thickness ToWinUI(this Padding value)
	{
		return new mux.Thickness(value.Left, value.Top, value.Right, value.Bottom);
	}

	public static Rectangle ToEto(this sw.Rect value)
	{
		if (value.IsEmpty)
			return Rectangle.Empty;
		return new Rectangle((int)value.X, (int)value.Y, (int)Math.Ceiling(value.Width), (int)Math.Ceiling(value.Height));
	}

	public static RectangleF ToEtoF(this sw.Rect value)
	{
		if (value.IsEmpty)
			return RectangleF.Empty;
		return new RectangleF((float)value.X, (float)value.Y, (float)value.Width, (float)value.Height);
	}

	public static sw.Rect ToWinUI(this Rectangle value)
	{
		value.Normalize();
		return new sw.Rect(value.X, value.Y, value.Width, value.Height);
	}

	public static sw.Rect ToWinUI(this RectangleF value)
	{
		value.Normalize();
		return new sw.Rect(value.X, value.Y, value.Width, value.Height);
	}

	public static SizeF ToEto(this sw.Size value)
	{
		return new SizeF((float)value.Width, (float)value.Height);
	}

	public static Size ToEtoSize(this sw.Size value)
	{
		return new Size((int)(double.IsNaN(value.Width) ? -1 : Math.Ceiling(value.Width)), (int)(double.IsNaN(value.Height) ? -1 : Math.Ceiling(value.Height)));
	}

	public static sw.Size ToWinUI(this Size value)
	{
		return new sw.Size(value.Width == -1 ? double.NaN : value.Width, value.Height == -1 ? double.NaN : value.Height);
	}

	public static sw.Size ToWinUI(this SizeF value)
	{
		return new sw.Size(value.Width, value.Height);
	}

	public static PointF ToEto(this sw.Point value)
	{
		return new PointF((float)value.X, (float)value.Y);
	}

	public static Point ToEtoPoint(this sw.Point value)
	{
		return new Point((int)value.X, (int)value.Y);
	}

	public static sw.Point ToWinUI(this Point value)
	{
		return new sw.Point(value.X, value.Y);
	}

	public static sw.Point ToWinUI(this PointF value)
	{
		return new sw.Point(value.X, value.Y);
	}

	/*
	public static KeyEventArgs ToEto(this swi.KeyEventArgs e, KeyEventType keyType)
	{
		var swkey = e.Key;
		if (swkey == swi.Key.System)
			swkey = e.SystemKey;
		var key = swkey.ToEtoWithModifier(swi.Keyboard.Modifiers);
		return new KeyEventArgs(key, keyType) { Handled = e.Handled };
	}

	public static MouseButtons GetEtoButtons(this swi.MouseButtonEventArgs e, swi.MouseButtonState buttonState = swi.MouseButtonState.Pressed)
	{
		var buttons = MouseButtons.None;
		if (e.ChangedButton == swi.MouseButton.Left && e.LeftButton == buttonState)
			buttons |= MouseButtons.Primary;
		if (e.ChangedButton == swi.MouseButton.Right && e.RightButton == buttonState)
			buttons |= MouseButtons.Alternate;
		if (e.ChangedButton == swi.MouseButton.Middle && e.MiddleButton == buttonState)
			buttons |= MouseButtons.Middle;
		return buttons;
	}

	public static MouseEventArgs ToEto(this swi.MouseButtonEventArgs e, sw.IInputElement control, swi.MouseButtonState buttonState = swi.MouseButtonState.Pressed)
	{
		var buttons = e.GetEtoButtons(buttonState);
		var modifiers = swi.Keyboard.Modifiers.ToEto();
		var location = e.GetPosition(control).ToEto();

		return new MouseEventArgs(buttons, modifiers, location);
	}

	public static MouseEventArgs ToEto(this swi.MouseEventArgs e, sw.IInputElement control, swi.MouseButtonState buttonState = swi.MouseButtonState.Pressed)
	{
		var buttons = MouseButtons.None;
		if (e.LeftButton == buttonState)
			buttons |= MouseButtons.Primary;
		if (e.RightButton == buttonState)
			buttons |= MouseButtons.Alternate;
		if (e.MiddleButton == buttonState)
			buttons |= MouseButtons.Middle;
		var modifiers = swi.Keyboard.Modifiers.ToEto();
		var location = e.GetPosition(control).ToEto();

		return new MouseEventArgs(buttons, modifiers, location);
	}

	public static MouseEventArgs ToEto(this swi.MouseWheelEventArgs e, sw.IInputElement control, swi.MouseButtonState buttonState = swi.MouseButtonState.Pressed)
	{
		var buttons = MouseButtons.None;
		if (e.LeftButton == buttonState)
			buttons |= MouseButtons.Primary;
		if (e.RightButton == buttonState)
			buttons |= MouseButtons.Alternate;
		if (e.MiddleButton == buttonState)
			buttons |= MouseButtons.Middle;
		var modifiers = swi.Keyboard.Modifiers.ToEto();
		var location = e.GetPosition(control).ToEto();
		var delta = new SizeF(0, (float)e.Delta / WheelDelta);

		return new MouseEventArgs(buttons, modifiers, location, delta);
	}

	public static swm.BitmapScalingMode ToWinUI(this ImageInterpolation value)
	{
		switch (value)
		{
			case ImageInterpolation.Default:
				return swm.BitmapScalingMode.Unspecified;
			case ImageInterpolation.None:
				return swm.BitmapScalingMode.NearestNeighbor;
			case ImageInterpolation.Low:
				return swm.BitmapScalingMode.LowQuality;
			case ImageInterpolation.Medium:
				return swm.BitmapScalingMode.HighQuality;
			case ImageInterpolation.High:
				return swm.BitmapScalingMode.HighQuality;
			default:
				throw new NotSupportedException();
		}
	}

	public static ImageInterpolation ToEto(this swm.BitmapScalingMode value)
	{
		switch (value)
		{
			case swm.BitmapScalingMode.HighQuality:
				return ImageInterpolation.High;
			case swm.BitmapScalingMode.LowQuality:
				return ImageInterpolation.Low;
			case swm.BitmapScalingMode.NearestNeighbor:
				return ImageInterpolation.None;
			case swm.BitmapScalingMode.Unspecified:
				return ImageInterpolation.Default;
			default:
				throw new NotSupportedException();
		}
	}

	public static sp.PageOrientation ToSP(this PageOrientation value)
	{
		switch (value)
		{
			case PageOrientation.Portrait:
				return sp.PageOrientation.Portrait;
			case PageOrientation.Landscape:
				return sp.PageOrientation.Landscape;
			default:
				throw new NotSupportedException();
		}
	}

	public static PageOrientation ToEto(this sp.PageOrientation? value)
	{
		if (value == null)
			return PageOrientation.Portrait;
		switch (value.Value)
		{
			case sp.PageOrientation.Landscape:
				return PageOrientation.Landscape;
			case sp.PageOrientation.Portrait:
				return PageOrientation.Portrait;
			default:
				throw new NotSupportedException();
		}
	}

	public static swc.PageRange ToPageRange(this Range<int> range)
	{
		return new swc.PageRange(range.Start, range.End);
	}

	public static Range<int> ToEto(this swc.PageRange range)
	{
		return new Range<int>(range.PageFrom, range.PageTo);
	}

	public static swc.PageRangeSelection ToSWC(this PrintSelection value)
	{
		switch (value)
		{
			case PrintSelection.AllPages:
				return swc.PageRangeSelection.AllPages;
			case PrintSelection.SelectedPages:
				return swc.PageRangeSelection.UserPages;
			default:
				throw new NotSupportedException();
		}
	}

	public static PrintSelection ToEto(this swc.PageRangeSelection value)
	{
		switch (value)
		{
			case swc.PageRangeSelection.AllPages:
				return PrintSelection.AllPages;
			case swc.PageRangeSelection.UserPages:
				return PrintSelection.SelectedPages;
			default:
				throw new NotSupportedException();
		}
	}

	public static Size GetSize(this sw.FrameworkElement element)
	{
		if (!double.IsNaN(element.ActualWidth) && !double.IsNaN(element.ActualHeight))
			return new Size((int)Math.Ceiling(element.ActualWidth), (int)Math.Ceiling(element.ActualHeight));
		return new Size((int)(double.IsNaN(element.Width) ? -1 : Math.Ceiling(element.Width)), (int)(double.IsNaN(element.Height) ? -1 : Math.Ceiling(element.Height)));
	}

	public static sw.Size GetMinSize(this sw.FrameworkElement element)
	{
		return new sw.Size(element.MinWidth, element.MinHeight);
	}

	public static void SetSize(this sw.FrameworkElement element, Size size)
	{
		element.Width = size.Width == -1 ? double.NaN : size.Width;
		element.Height = size.Height == -1 ? double.NaN : size.Height;
	}

	public static void SetMaxSize(this sw.FrameworkElement element, Size size)
	{
		element.MaxWidth = size.Width == -1 ? double.NaN : size.Width;
		element.MaxHeight = size.Height == -1 ? double.NaN : size.Height;
	}

	public static sw.Size GetMaxSize(this sw.FrameworkElement element)
	{
		return new sw.Size(element.MaxWidth, element.MaxHeight);
	}

	public static void SetSize(this sw.FrameworkElement element, sw.Size size)
	{
		element.Width = size.Width;
		element.Height = size.Height;
	}

	public static FontStyle Convert(sw.FontStyle fontStyle, sw.FontWeight fontWeight)
	{
		var style = FontStyle.None;
		if (fontStyle == sw.FontStyles.Italic)
			style |= FontStyle.Italic;
		if (fontStyle == sw.FontStyles.Oblique)
			style |= FontStyle.Italic;
		if (fontWeight == sw.FontWeights.Bold)
			style |= FontStyle.Bold;
		return style;
	}

	public static FontDecoration Convert(sw.TextDecorationCollection decorations)
	{
		var decoration = FontDecoration.None;
		if (decorations != null)
		{
			if (sw.TextDecorations.Underline.All(decorations.Contains))
				decoration |= FontDecoration.Underline;
			if (sw.TextDecorations.Strikethrough.All(decorations.Contains))
				decoration |= FontDecoration.Strikethrough;
		}
		return decoration;
	}

	public static Bitmap ToEto(this swmi.BitmapSource bitmap)
	{
		if (bitmap == null)
			return null;
		return new Bitmap(new BitmapHandler(bitmap));
	}

	public static swmi.BitmapSource ToWinUI(this Image image, float? scale = null, Size? fittingSize = null)
	{
		if (image == null)
			return null;
		var imageHandler = image.Handler as IWinUIImage;
		if (imageHandler != null)
			return imageHandler.GetImageClosestToSize(scale ?? Screen.PrimaryScreen.LogicalPixelSize, fittingSize);
		return image.ControlObject as swmi.BitmapSource;
	}

	public static swmi.BitmapSource ToWinUI(this IconFrame image, float? scale = null, Size? fittingSize = null)
	{
		return ((Bitmap)image?.ControlObject).ToWinUI(scale, fittingSize);
	}

	public static swc.Image ToWinUIImage(this Image image, float? scale = null, Size? fittingSize = null)
	{
		var source = image.ToWinUI(scale, fittingSize);
		if (source == null)
			return null;
		var swcImage = new swc.Image { Source = source };
		if (fittingSize != null)
		{
			swcImage.SetMaxSize(fittingSize.Value);
		}
		return swcImage;
	}

	public static swm.Pen ToWinUI(this Pen pen, bool clone = false)
	{
		var p = (swm.Pen)pen.ControlObject;
		if (clone)
			p = p.Clone();
		return p;
	}

	public static swm.PenLineJoin ToWinUI(this PenLineJoin value)
	{
		switch (value)
		{
			case PenLineJoin.Miter:
				return swm.PenLineJoin.Miter;
			case PenLineJoin.Bevel:
				return swm.PenLineJoin.Bevel;
			case PenLineJoin.Round:
				return swm.PenLineJoin.Round;
			default:
				throw new NotSupportedException();
		}
	}

	public static PenLineJoin ToEto(this swm.PenLineJoin value)
	{
		switch (value)
		{
			case swm.PenLineJoin.Bevel:
				return PenLineJoin.Bevel;
			case swm.PenLineJoin.Miter:
				return PenLineJoin.Miter;
			case swm.PenLineJoin.Round:
				return PenLineJoin.Round;
			default:
				throw new NotSupportedException();
		}
	}

	public static swm.PenLineCap ToWinUI(this PenLineCap value)
	{
		switch (value)
		{
			case PenLineCap.Butt:
				return swm.PenLineCap.Flat;
			case PenLineCap.Round:
				return swm.PenLineCap.Round;
			case PenLineCap.Square:
				return swm.PenLineCap.Square;
			default:
				throw new NotSupportedException();
		}
	}

	public static PenLineCap ToEto(this swm.PenLineCap value)
	{
		switch (value)
		{
			case swm.PenLineCap.Flat:
				return PenLineCap.Butt;
			case swm.PenLineCap.Round:
				return PenLineCap.Round;
			case swm.PenLineCap.Square:
				return PenLineCap.Square;
			default:
				throw new NotSupportedException();
		}
	}

	public static muxm.Brush ToWinUI(this Brush brush)
	{
		return ((FrozenBrushWrapper)brush.ControlObject).FrozenBrush;
	}

	public static swm.Matrix ToWinUI(this IMatrix matrix)
	{
		return (swm.Matrix)matrix.ControlObject;
	}

	public static swm.Transform ToWinUITransform(this IMatrix matrix)
	{
		return new swm.MatrixTransform(matrix.ToWinUI());
	}

	public static IMatrix ToEtoMatrix(this swm.Transform transform)
	{
		return new MatrixHandler(transform.Value);
	}

	public static swm.PathGeometry ToWinUI(this IGraphicsPath path)
	{
		return (swm.PathGeometry)path.ControlObject;
	}

	public static swm.GradientSpreadMethod ToWinUI(this GradientWrapMode wrap)
	{
		switch (wrap)
		{
			case GradientWrapMode.Reflect:
				return swm.GradientSpreadMethod.Reflect;
			case GradientWrapMode.Repeat:
				return swm.GradientSpreadMethod.Repeat;
			case GradientWrapMode.Pad:
				return swm.GradientSpreadMethod.Pad;
			default:
				throw new NotSupportedException();
		}
	}

	public static GradientWrapMode ToEto(this swm.GradientSpreadMethod spread)
	{
		switch (spread)
		{
			case swm.GradientSpreadMethod.Reflect:
				return GradientWrapMode.Reflect;
			case swm.GradientSpreadMethod.Repeat:
				return GradientWrapMode.Repeat;
			case swm.GradientSpreadMethod.Pad:
				return GradientWrapMode.Pad;
			default:
				throw new NotSupportedException();
		}
	}

	public static WindowStyle ToEto(this sw.WindowStyle style)
	{
		switch (style)
		{
			case sw.WindowStyle.None:
				return WindowStyle.None;
			case sw.WindowStyle.ThreeDBorderWindow:
			case sw.WindowStyle.SingleBorderWindow:
				return WindowStyle.Default;
			case sw.WindowStyle.ToolWindow:
				return WindowStyle.Utility;
			default:
				throw new NotSupportedException();
		}
	}

	public static sw.WindowStyle ToWinUI(this WindowStyle style)
	{
		switch (style)
		{
			case WindowStyle.None:
				return sw.WindowStyle.None;
			case WindowStyle.Default:
				return sw.WindowStyle.ThreeDBorderWindow;
			case WindowStyle.Utility:
				return sw.WindowStyle.ToolWindow;
			default:
				throw new NotSupportedException();
		}
	}

	public static CalendarMode ToEto(this swc.CalendarSelectionMode mode)
	{
		switch (mode)
		{
			case System.Windows.Controls.CalendarSelectionMode.SingleDate:
				return CalendarMode.Single;
			case System.Windows.Controls.CalendarSelectionMode.SingleRange:
				return CalendarMode.Range;
			case System.Windows.Controls.CalendarSelectionMode.MultipleRange:
			case System.Windows.Controls.CalendarSelectionMode.None:
			default:
				throw new NotSupportedException();
		}
	}

	public static swc.CalendarSelectionMode ToWinUI(this CalendarMode mode)
	{
		switch (mode)
		{
			case CalendarMode.Single:
				return swc.CalendarSelectionMode.SingleDate;
			case CalendarMode.Range:
				return swc.CalendarSelectionMode.SingleRange;
			default:
				throw new NotSupportedException();
		}
	}

	public static TextAlignment ToEto(this sw.HorizontalAlignment align)
	{
		switch (align)
		{
			case sw.HorizontalAlignment.Left:
				return TextAlignment.Left;
			case sw.HorizontalAlignment.Right:
				return TextAlignment.Right;
			case sw.HorizontalAlignment.Center:
				return TextAlignment.Center;
			default:
				throw new NotSupportedException();
		}
	}

	public static sw.HorizontalAlignment ToWinUI(this TextAlignment align)
	{
		switch (align)
		{
			case TextAlignment.Center:
				return sw.HorizontalAlignment.Center;
			case TextAlignment.Left:
				return sw.HorizontalAlignment.Left;
			case TextAlignment.Right:
				return sw.HorizontalAlignment.Right;
			default:
				throw new NotSupportedException();
		}
	}

	public static TextAlignment ToEto(this sw.TextAlignment align)
	{
		switch (align)
		{
			case sw.TextAlignment.Left:
				return TextAlignment.Left;
			case sw.TextAlignment.Right:
				return TextAlignment.Right;
			case sw.TextAlignment.Center:
				return TextAlignment.Center;
			case sw.TextAlignment.Justify:
				return TextAlignment.Left;
			default:
				throw new NotSupportedException();
		}
	}

	public static sw.TextAlignment ToWinUITextAlignment(this TextAlignment align)
	{
		switch (align)
		{
			case TextAlignment.Center:
				return sw.TextAlignment.Center;
			case TextAlignment.Left:
				return sw.TextAlignment.Left;
			case TextAlignment.Right:
				return sw.TextAlignment.Right;
			default:
				throw new NotSupportedException();
		}
	}

	public static VerticalAlignment ToEto(this sw.VerticalAlignment align)
	{
		switch (align)
		{
			case sw.VerticalAlignment.Top:
				return VerticalAlignment.Top;
			case sw.VerticalAlignment.Bottom:
				return VerticalAlignment.Bottom;
			case sw.VerticalAlignment.Center:
				return VerticalAlignment.Center;
			case sw.VerticalAlignment.Stretch:
				return VerticalAlignment.Stretch;
			default:
				throw new NotSupportedException();
		}
	}

	public static sw.VerticalAlignment ToWinUI(this VerticalAlignment align)
	{
		switch (align)
		{
			case VerticalAlignment.Top:
				return sw.VerticalAlignment.Top;
			case VerticalAlignment.Bottom:
				return sw.VerticalAlignment.Bottom;
			case VerticalAlignment.Center:
				return sw.VerticalAlignment.Center;
			case VerticalAlignment.Stretch:
				return sw.VerticalAlignment.Stretch;
			default:
				throw new NotSupportedException();
		}
	}

	public static Font GetEtoFont(this swc.Control control)
	{
		return new Font(new FontHandler(control));
	}

	public static Font SetEtoFont(this swc.Control control, Font font, Action<sw.TextDecorationCollection> setDecorations = null)
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

	public static Font SetEtoFont(this swd.TextRange control, Font font)
	{
		if (control == null) return font;
		if (font != null)
		{
			((FontHandler)font.Handler).Apply(control);
		}
		else
		{
			control.ApplyPropertyValue(swd.TextElement.FontFamilyProperty, swc.Control.FontFamilyProperty.DefaultMetadata.DefaultValue);
			control.ApplyPropertyValue(swd.TextElement.FontStyleProperty, swc.Control.FontStyleProperty.DefaultMetadata.DefaultValue);
			control.ApplyPropertyValue(swd.TextElement.FontWeightProperty, swc.Control.FontWeightProperty.DefaultMetadata.DefaultValue);
			control.ApplyPropertyValue(swd.TextElement.FontSizeProperty, swc.Control.FontSizeProperty.DefaultMetadata.DefaultValue);
			control.ApplyPropertyValue(swd.Inline.TextDecorationsProperty, new sw.TextDecorationCollection());
		}
		return font;
	}

	public static Font SetEtoFont(this swm.FormattedText control, Font font)
	{
		if (control == null) return font;
		if (font != null)
		{
			((FontHandler)font.Handler).Apply(control);
		}
		return font;
	}

	public static FontFamily SetEtoFamily(this swd.TextRange control, FontFamily fontFamily)
	{
		if (control == null) return fontFamily;
		if (fontFamily != null)
		{
			((FontFamilyHandler)fontFamily.Handler).Apply(control);
		}
		else
		{
			control.ApplyPropertyValue(swd.TextElement.FontFamilyProperty, swc.Control.FontFamilyProperty.DefaultMetadata.DefaultValue);
		}
		return fontFamily;
	}

	public static Font SetEtoFont(this swc.TextBlock control, Font font, Action<sw.TextDecorationCollection> setDecorations = null)
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

	public static swm.Typeface ToWinUITypeface(this Font font)
	{
		var handler = (FontHandler)font.Handler;
		return handler.WinUITypeface;
	}

	public static swc.Dock ToWinUI(this DockPosition position)
	{
		switch (position)
		{
			case DockPosition.Top:
				return swc.Dock.Top;
			case DockPosition.Left:
				return swc.Dock.Left;
			case DockPosition.Right:
				return swc.Dock.Right;
			case DockPosition.Bottom:
				return swc.Dock.Bottom;
			default:
				throw new NotSupportedException();
		}
	}

	public static DockPosition ToEtoTabPosition(this swc.Dock dock)
	{
		switch (dock)
		{
			case swc.Dock.Left:
				return DockPosition.Left;
			case swc.Dock.Top:
				return DockPosition.Top;
			case swc.Dock.Right:
				return DockPosition.Right;
			case swc.Dock.Bottom:
				return DockPosition.Bottom;
			default:
				throw new NotSupportedException();
		}
	}

	public static StepperValidDirections ToEto(this xwt.ValidSpinDirections direction)
	{
		var dir = StepperValidDirections.None;
		if (direction.HasFlag(xwt.ValidSpinDirections.Increase))
			dir |= StepperValidDirections.Up;
		if (direction.HasFlag(xwt.ValidSpinDirections.Decrease))
			dir |= StepperValidDirections.Down;
		return dir;
	}

	public static xwt.ValidSpinDirections ToWinUI(this StepperValidDirections direction)
	{
		var dir = xwt.ValidSpinDirections.None;
		if (direction.HasFlag(StepperValidDirections.Up))
			dir |= xwt.ValidSpinDirections.Increase;
		if (direction.HasFlag(StepperValidDirections.Down))
			dir |= xwt.ValidSpinDirections.Decrease;
		return dir;
	}

	public static void SetEtoBorderType(this swc.Border control, BorderType value, Func<muxm.Brush> getBezelBrush = null)
	{
		switch (value)
		{
			case BorderType.Bezel:
				control.BorderBrush = getBezelBrush?.Invoke() ?? sw.SystemColors.ControlDarkBrush;
				control.BorderThickness = new mux.Thickness(1);
				break;
			case BorderType.Line:
				control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
				control.BorderThickness = new mux.Thickness(1);
				break;
			case BorderType.None:
				control.BorderBrush = null;
				control.BorderThickness = new mux.Thickness(0);
				break;
			default:
				throw new NotSupportedException();

		}
	}

	public static void SetEtoBorderType(this swc.Control control, BorderType value, Func<muxm.Brush> getBezelBrush = null)
	{
		switch (value)
		{
			case BorderType.Bezel:
				control.BorderBrush = getBezelBrush?.Invoke() ?? sw.SystemColors.ControlDarkBrush;
				control.BorderThickness = new mux.Thickness(1);
				break;
			case BorderType.Line:
				control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
				control.BorderThickness = new mux.Thickness(1);
				break;
			case BorderType.None:
				control.BorderBrush = null;
				control.BorderThickness = new mux.Thickness(0);
				break;
			default:
				throw new NotSupportedException();

		}
	}

	public static Icon ToEto(this System.Drawing.Icon icon)
	{
		return new Icon(new IconHandler(icon));
	}

	public static Icon ToEtoIcon(this swm.ImageSource bitmap)
	{
		if (bitmap == null)
			return null;

		return null;
	}
	public static System.Drawing.Bitmap ToSD(this Image image)
	{
		if (image == null)
			return null;
		var icon = image as Icon;
		if (icon != null)
		{
			image = icon.GetFrame(1)?.Bitmap;
		}

		var bmp = image as Bitmap;
		if (bmp != null)
		{
			using (var ms = new MemoryStream())
			{
				bmp.Save(ms, ImageFormat.Png);
				ms.Position = 0;
				return new System.Drawing.Bitmap(ms);
			}
		}

		return null;
	}

	public static System.Drawing.Icon ToSDIcon(this Image image)
	{
		if (image == null)
			return null;

		// TODO: Test if image is an icon and convert with all frames intact

		return System.Drawing.Icon.FromHandle(image.ToSD().GetHicon());
	}

	public static string GetEnglishName(this swm.LanguageSpecificStringDictionary nameDictionary)
	{
		return CustomControls.FontDialog.NameDictionaryExtensions.GetEnglishName(nameDictionary);
	}

	public static string GetDisplayName(this swm.LanguageSpecificStringDictionary nameDictionary)
	{
		return CustomControls.FontDialog.NameDictionaryExtensions.GetDisplayName(nameDictionary);
	}

	public static string GetName(this swm.LanguageSpecificStringDictionary nameDictionary, string ietfLanguageTag)
	{
		return CustomControls.FontDialog.NameDictionaryExtensions.GetName(nameDictionary, ietfLanguageTag);
	}

	public static swi.Cursor ToWinUI(this Cursor cursor) => cursor?.ControlObject as swi.Cursor;

	public static bool HasAlpha(this swm.PixelFormat format)
	{
		return format == swm.PixelFormats.Bgra32
			|| format == swm.PixelFormats.Pbgra32
			|| format == swm.PixelFormats.Prgba128Float
			|| format == swm.PixelFormats.Prgba64
			|| format == swm.PixelFormats.Rgba64
			|| format == swm.PixelFormats.Rgba128Float;
	}
	public static bool IsPremultiplied(this swm.PixelFormat format)
	{
		return format == swm.PixelFormats.Pbgra32
			|| format == swm.PixelFormats.Prgba128Float
			|| format == swm.PixelFormats.Prgba64;
	}
	public static swm.PixelFormat GetNonPremultipliedFormat(this swm.PixelFormat format)
	{
		if (format == swm.PixelFormats.Pbgra32)
			return swm.PixelFormats.Bgra32;
		if (format == swm.PixelFormats.Prgba128Float)
			return swm.PixelFormats.Rgba128Float;
		if (format == swm.PixelFormats.Prgba64)
			return swm.PixelFormats.Rgba64;
		return format;
	}

	public static Color? GetResourceColor(this sw.FrameworkElement cell, sw.ResourceKey key)
	{
		var res = cell.TryFindResource(key);
		if (res is muxm.SolidColorBrush brush)
			return brush.ToEtoColor();
		if (res is wu.Color color)
			return color.ToEto();
		return null;
	}

	public static Color? GetResourceColor(this sw.FrameworkElement cell, params sw.ResourceKey[] keys)
	{
		for (int i = 0; i < keys.Length; i++)
		{
			sw.ResourceKey key = keys[i];
			var value = GetResourceColor(cell, key);
			if (value != null)
				return value;
		}
		return null;
	}
	*/
}
