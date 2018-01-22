using System;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using swi = Windows.UI.Xaml.Input;
using swm = Windows.UI.Xaml.Media;
using wf = Windows.Foundation;
using sw = Windows.UI.Xaml;
//using sp = System.Printing;
using swc = Windows.UI.Xaml.Controls;
using swmi = Windows.UI.Xaml.Media.Imaging;
using wu = Windows.UI;
using wuc = Windows.UI.Core;
using wut = Windows.UI.Text;
using System.Text.RegularExpressions;
using Eto.Direct2D.Drawing;
//using Eto.WinRT.Drawing;

namespace Eto.WinRT
{
	/// <summary>
	/// Xaml conversions.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static partial class Conversions
	{
		public const float WheelDelta = 120f;

		public static readonly wf.Size PositiveInfinitySize = new wf.Size(double.PositiveInfinity, double.PositiveInfinity);
		public static readonly wf.Size ZeroSize = new wf.Size(0, 0);

		public static wu.Color ToWpf(this Color value)
		{

			return wu.Color.FromArgb((byte)(value.A * byte.MaxValue), (byte)(value.R * byte.MaxValue), (byte)(value.G * byte.MaxValue), (byte)(value.B * byte.MaxValue));
		}

		public static swm.Brush ToWpfBrush(this Color value, swm.Brush brush = null)
		{
			var solidBrush = brush as swm.SolidColorBrush;
			if (solidBrush == null
#if TODO_XAML
				|| solidBrush.IsSealed
#endif
				)
			{
				solidBrush = new swm.SolidColorBrush();
			}
			solidBrush.Color = value.ToWpf();
			return solidBrush;
		}

		public static Color ToEto(this wu.Color value)
		{
			return new Color { A = value.A / 255f, R = value.R / 255f, G = value.G / 255f, B = value.B / 255f };
		}

		public static Color ToEtoColor(this swm.Brush brush)
		{
			var solidBrush = brush as swm.SolidColorBrush;
			if (solidBrush != null)
				return solidBrush.Color.ToEto();
			return Colors.Transparent;
		}

		public static Padding ToEto(this sw.Thickness value)
		{
			return new Padding((int)value.Left, (int)value.Top, (int)value.Right, (int)value.Bottom);
		}

		public static sw.Thickness ToWpf(this Padding value)
		{
			return new sw.Thickness(value.Left, value.Top, value.Right, value.Bottom);
		}

		public static Rectangle ToEto(this wf.Rect value)
		{
			return new Rectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
		}

		public static RectangleF ToEtoF(this wf.Rect value)
		{
			return new RectangleF((float)value.X, (float)value.Y, (float)value.Width, (float)value.Height);
		}

		public static wf.Rect ToWpf(this Rectangle value)
		{
			return new wf.Rect(value.X, value.Y, value.Width, value.Height);
		}

#if TODO_XAML
		public static sw.Int32Rect ToWpfInt32(this Rectangle value)
		{
			return new sw.Int32Rect(value.X, value.Y, value.Width, value.Height);
		}
#endif

		public static wf.Rect ToWpf(this RectangleF value)
		{
			return new wf.Rect(value.X, value.Y, value.Width, value.Height);
		}

		public static SizeF ToEto(this wf.Size value)
		{
			return new SizeF((float)value.Width, (float)value.Height);
		}

		public static Size ToEtoSize(this wf.Size value)
		{
			return new Size((int)(double.IsNaN(value.Width) ? -1 : value.Width), (int)(double.IsNaN(value.Height) ? -1 : value.Height));
		}

		public static wf.Size ToWpf(this Size value)
		{
			return new wf.Size(value.Width == -1 ? double.NaN : value.Width, value.Height == -1 ? double.NaN : value.Height);
		}

		public static wf.Size ToWpf(this SizeF value)
		{
			return new wf.Size(value.Width, value.Height);
		}

		public static PointF ToEto(this wf.Point value)
		{
			return new PointF((float)value.X, (float)value.Y);
		}

		public static Point ToEtoPoint(this wf.Point value)
		{
			return new Point((int)value.X, (int)value.Y);
		}

		public static wf.Point ToWpf(this Point value)
		{
			return new wf.Point(value.X, value.Y);
		}

		public static wf.Point ToWpf(this PointF value)
		{
			return new wf.Point(value.X, value.Y);
		}

		public static string ToWpfMneumonic(this string value)
		{
			if (value == null)
				return string.Empty;
			value = value.Replace("_", "__");
			var match = Regex.Match(value, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
			if (match.Success)
			{
				var sb = new StringBuilder(value);
				sb[match.Index] = '_';
				sb.Replace("&&", "&");
				return sb.ToString();
			}
			return value.Replace("&&", "&");
		}

		public static string ToEtoMneumonic(this string value)
		{
			if (value == null)
				return null;
			var match = Regex.Match(value, @"(?<=([^_](?:[_]{2})*)|^)[_](?![_])");
			if (match.Success)
			{
				var sb = new StringBuilder(value);
				sb[match.Index] = '&';
				sb.Replace("__", "_");
				return sb.ToString();
			}
			value = value.Replace("__", "_");
			return value;
		}

#if TODO_XAML
		public static KeyEventArgs ToEto(this wuc.KeyEventArgs e, KeyEventType keyType)
		{
			var key = KeyMap.Convert(e.Key, swi.Keyboard.Modifiers);
			return new KeyEventArgs(key, keyType) { Handled = e.Handled };
		}
#endif

#if TODO_XAML
		public static MouseEventArgs ToEto(this swi.MouseButtonEventArgs e, sw.IInputElement control, swi.MouseButtonState buttonState = swi.MouseButtonState.Pressed)
		{
			var buttons = MouseButtons.None;
			if (e.ChangedButton == swi.MouseButton.Left && e.LeftButton == buttonState)
				buttons |= MouseButtons.Primary;
			if (e.ChangedButton == swi.MouseButton.Right && e.RightButton == buttonState)
				buttons |= MouseButtons.Alternate;
			if (e.ChangedButton == swi.MouseButton.Middle && e.MiddleButton == buttonState)
				buttons |= MouseButtons.Middle;
			var modifiers = Keys.None;
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
			var modifiers = Keys.None;
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
			var modifiers = Keys.None;
			var location = e.GetPosition(control).ToEto();
			var delta = new SizeF(0, (float)e.Delta / WheelDelta);

			return new MouseEventArgs(buttons, modifiers, location, delta);
		}
#endif

#if TODO_XAML
		public static swm.BitmapScalingMode ToWpf(this ImageInterpolation value)
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

		public static swc.PageRange ToPageRange(this Range range)
		{
			return new swc.PageRange(range.Start, range.End);
		}

		public static Range ToEto(this swc.PageRange range)
		{
			return new Range(range.PageFrom, range.PageTo - range.PageFrom + 1);
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
#endif
		public static Size GetSize(this sw.FrameworkElement element)
		{
#if TODO_XAML
			if (element.IsVisible && (!double.IsNaN(element.ActualWidth) && !double.IsNaN(element.ActualHeight)))
				return new Size((int)element.ActualWidth, (int)element.ActualHeight);
			return new Size((int)(double.IsNaN(element.Width) ? -1 : element.Width), (int)(double.IsNaN(element.Height) ? -1 : element.Height));
#else
			if ((!double.IsNaN(element.ActualWidth) && !double.IsNaN(element.ActualHeight)))
				return new Size((int)element.ActualWidth, (int)element.ActualHeight);
			return new Size((int)(double.IsNaN(element.Width) ? -1 : element.Width), (int)(double.IsNaN(element.Height) ? -1 : element.Height));
#endif
		}

		public static void SetSize(this sw.FrameworkElement element, Size size)
		{
			element.Width = size.Width == -1 ? double.NaN : size.Width;
			element.Height = size.Height == -1 ? double.NaN : size.Height;
		}

		public static void SetSize(this sw.FrameworkElement element, wf.Size size)
		{
			element.Width = size.Width;
			element.Height = size.Height;
		}

		public static FontStyle Convert(wut.FontStyle fontStyle, wut.FontWeight fontWeight)
		{
			var style = FontStyle.None;
			if (fontStyle == wut.FontStyle.Italic)
				style |= FontStyle.Italic;
			if (fontStyle == wut.FontStyle.Oblique)
				style |= FontStyle.Italic;
			if (ReferenceEquals(fontWeight, wut.FontWeights.Bold))
				style |= FontStyle.Bold;
			return style;
		}

#if TODO_XAML
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
#endif

		public static swmi.BitmapSource ToWpf(this Image image, int? size = null)
		{
#if TODO_XAML
			if (image == null)
				return null;
			var imageHandler = image.Handler as IWpfImage;
			if (imageHandler != null)
				return imageHandler.GetImageClosestToSize(size);
			return image.ControlObject as swmi.BitmapSource;
#else
			return image.ControlObject as swmi.BitmapSource;
#endif
		}

		public static swc.Image ToWpfImage(this Image image, int? size = null)
		{
			var source = image.ToWpf(size);
			if (source == null)
				return null;
			var swcImage = new swc.Image { Source = source };
			if (size != null)
			{
				swcImage.MaxWidth = size.Value;
				swcImage.MaxHeight = size.Value;
			}
			return swcImage;
		}

#if TODO_XAML
		public static swm.Pen ToWpf(this Pen pen, bool clone = false)
		{
			var p = (swm.Pen)pen.ControlObject;
			if (clone)
				p = p.Clone();
			return p;
		}
#endif

		public static swm.PenLineJoin ToWpf(this PenLineJoin value)
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

		public static swm.PenLineCap ToWpf(this PenLineCap value)
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

		public static swm.Brush ToWpf(this Brush brush, bool clone = false)
		{
			var b = (swm.Brush)brush.ControlObject;
			if (clone)
			{
#if TODO_XAML
				b = b.Clone();
#else
				throw new NotImplementedException();
#endif
			}
			return b;
		}

		public static swm.Matrix ToWpf(this IMatrix matrix)
		{
			return (swm.Matrix)matrix.ControlObject;
		}

		public static swm.Transform ToWpfTransform(this IMatrix matrix)
		{
			return new swm.MatrixTransform { Matrix = matrix.ToWpf() };
		}

#if TODO_XAML
		public static IMatrix ToEtoMatrix(this swm.Transform transform)
		{
			return new MatrixHandler(transform.Value);
		}
#endif

		public static swm.PathGeometry ToWpf(this IGraphicsPath path)
		{
			return (swm.PathGeometry)path.ControlObject;
		}

		public static swm.GradientSpreadMethod ToWpf(this GradientWrapMode wrap)
		{
			switch (wrap)
			{
				case GradientWrapMode.Reflect:
					return swm.GradientSpreadMethod.Reflect;
				case GradientWrapMode.Repeat:
					return swm.GradientSpreadMethod.Repeat;
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
				default:
					throw new NotSupportedException();
			}
		}

		public static sw.TextAlignment ToWpfTextAlignment(this TextAlignment align)
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


#if TODO_XAML
		public static WindowStyle ToEto(this sw.WindowStyle style)
		{
			switch (style)
			{
				case sw.WindowStyle.None:
					return WindowStyle.None;
				case sw.WindowStyle.ThreeDBorderWindow:
					return WindowStyle.Default;
				default:
					throw new NotSupportedException();
			}
		}

		public static sw.WindowStyle ToWpf(this WindowStyle style)
		{
			switch (style)
			{
				case WindowStyle.None:
					return sw.WindowStyle.None;
				case WindowStyle.Default:
					return sw.WindowStyle.ThreeDBorderWindow;
				default:
					throw new NotSupportedException();
			}
		}
#endif
	}
}
