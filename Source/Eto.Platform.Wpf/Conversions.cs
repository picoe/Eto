using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using swi = System.Windows.Input;
using swm = System.Windows.Media;
using sw = System.Windows;
using sp = System.Printing;
using swc = System.Windows.Controls;
using swmi = System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf
{
	public static class Conversions
	{
		public static swm.Color ToWpf (this Color value)
		{
			return swm.Color.FromArgb ((byte)(value.A * byte.MaxValue), (byte)(value.R * byte.MaxValue), (byte)(value.G * byte.MaxValue), (byte)(value.B * byte.MaxValue));
		}

		public static Color ToEto (this swm.Color value)
		{
			return new Color { A = value.A / 255f, R = value.R / 255f, G = value.G / 255f, B = value.B / 255f };
		}

		public static Padding ToEto (this sw.Thickness value)
		{
			return new Padding ((int)value.Left, (int)value.Top, (int)value.Right, (int)value.Bottom);
		}

		public static sw.Thickness ToWpf (this Padding value)
		{
			return new sw.Thickness (value.Left, value.Top, value.Right, value.Bottom);
		}

		public static Rectangle ToEto (this sw.Rect value)
		{
			return new Rectangle ((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
		}

		public static RectangleF ToEtoF (this sw.Rect value)
		{
			return new RectangleF ((float)value.X, (float)value.Y, (float)value.Width, (float)value.Height);
		}

		public static sw.Rect ToWpf (this Rectangle value)
		{
			return new sw.Rect (value.X, value.Y, value.Width, value.Height);
		}

		public static sw.Rect ToWpf (this RectangleF value)
		{
			return new sw.Rect (value.X, value.Y, value.Width, value.Height);
		}

		public static Size ToEto (this sw.Size value)
		{
			return new Size ((int)value.Width, (int)value.Height);
		}

		public static sw.Size ToWpf (this Size value)
		{
			return new sw.Size (value.Width, value.Height);
		}

		public static sw.Size ToWpf (this SizeF value)
		{
			return new sw.Size (value.Width, value.Height);
		}

		public static Point ToEto (this sw.Point value)
		{
			return new Point ((int)value.X, (int)value.Y);
		}

		public static sw.Point ToWpf (this Point value)
		{
			return new sw.Point (value.X, value.Y);
		}

		public static sw.Point ToWpf (this PointF value)
		{
			return new sw.Point (value.X, value.Y);
		}
			
		public static string ToWpfMneumonic (this string value)
		{
			if (value == null)
				return null;
			return value.Replace ("_", "__").Replace ("&", "_");
		}

		public static string ConvertMneumonicFromWPF (object obj)
		{
			var value = obj as string;
			if (value == null)
				return null;
			return Regex.Replace (value, "(?<![_])[_]", (match) => {
				if (match.Value == "__")
					return "_";
				else
					return "&"; });
		}

		public static KeyPressEventArgs ToEto (this swi.KeyEventArgs e)
		{
			var key = KeyMap.Convert (e.Key, swi.Keyboard.Modifiers);
			return new KeyPressEventArgs (key) { Handled = e.Handled };
		}

		public static MouseEventArgs ToEto (this swi.MouseEventArgs e, sw.IInputElement control)
		{
			var buttons = MouseButtons.None;
			if (e.LeftButton == swi.MouseButtonState.Pressed)
				buttons |= MouseButtons.Primary;
			if (e.RightButton == swi.MouseButtonState.Pressed)
				buttons |= MouseButtons.Alternate;
			if (e.MiddleButton == swi.MouseButtonState.Pressed)
				buttons |= MouseButtons.Middle;
			var modifiers = Key.None;
			var location = e.GetPosition (control).ToEto ();

			return new MouseEventArgs (buttons, modifiers, location);
		}

		public static swm.BitmapScalingMode ToWpf (this ImageInterpolation value)
		{
			switch (value) {
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
				throw new NotSupportedException ();
			}
		}

		public static ImageInterpolation ToEto (this swm.BitmapScalingMode value)
		{
			switch (value) {
			case swm.BitmapScalingMode.HighQuality:
				return ImageInterpolation.High;
			case swm.BitmapScalingMode.LowQuality:
				return ImageInterpolation.Low;
			case swm.BitmapScalingMode.NearestNeighbor:
				return ImageInterpolation.None;
			case swm.BitmapScalingMode.Unspecified:
				return ImageInterpolation.Default;
			default:
				throw new NotSupportedException ();
			}
		}

		public static sp.PageOrientation ToSP (this PageOrientation value)
		{
			switch (value) {
			case PageOrientation.Portrait:
				return sp.PageOrientation.Portrait;
			case PageOrientation.Landscape:
				return sp.PageOrientation.Landscape;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PageOrientation ToEto (this sp.PageOrientation? value)
		{
			if (value == null)
				return PageOrientation.Portrait;
			switch (value.Value) {
			case sp.PageOrientation.Landscape:
				return PageOrientation.Landscape;
			case sp.PageOrientation.Portrait:
				return PageOrientation.Portrait;
			default:
				throw new NotSupportedException ();
			}
		}

		public static swc.PageRange ToPageRange (this Range range)
		{
			return new swc.PageRange (range.Start, range.End);
		}

		public static Range ToEto (this swc.PageRange range)
		{
			return new Range (range.PageFrom, range.PageTo - range.PageFrom + 1);
		}

		public static swc.PageRangeSelection ToSWC (this PrintSelection value)
		{
			switch (value) {
			case PrintSelection.AllPages:
				return swc.PageRangeSelection.AllPages;
			case PrintSelection.SelectedPages:
				return swc.PageRangeSelection.UserPages;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PrintSelection ToEto (this swc.PageRangeSelection value)
		{
			switch (value) {
			case swc.PageRangeSelection.AllPages:
				return PrintSelection.AllPages;
			case swc.PageRangeSelection.UserPages:
				return PrintSelection.SelectedPages;
			default:
				throw new NotSupportedException ();
			}
		}

		public static Size GetSize (sw.FrameworkElement element)
		{
			if (!double.IsNaN (element.ActualWidth) && !double.IsNaN (element.ActualHeight))
				return new Size ((int)element.ActualWidth, (int)element.ActualHeight);
			else
				return new Size ((int)(double.IsNaN (element.Width) ? -1 : element.Width), (int)(double.IsNaN (element.Height) ? -1 : element.Height));
		}

		public static void SetSize (sw.FrameworkElement element, Size size)
		{
			element.Width = size.Width == -1 ? double.NaN : size.Width;
			element.Height = size.Height == -1 ? double.NaN : size.Height;
		}

		public static FontStyle Convert (sw.FontStyle fontStyle, sw.FontWeight fontWeight)
		{
			var style = FontStyle.Normal;
			if (fontStyle == sw.FontStyles.Italic)
				style |= FontStyle.Italic;
			if (fontStyle == sw.FontStyles.Oblique)
				style |= FontStyle.Italic;

			if (fontWeight == sw.FontWeights.Bold)
				style |= FontStyle.Bold;
			return style;
		}

		public static swmi.BitmapSource ToWpf (this Image image, int? width = null)
		{
			var imageHandler = image.Handler as IWpfImage;
			if (imageHandler != null)
				return imageHandler.GetImageClosestToSize (width.Value);
			else
				return image.ControlObject as swmi.BitmapSource;
		}

		public static swm.Pen ToWpf (this IPen pen)
		{
			return (swm.Pen)pen.ControlObject;
		}

		public static swm.PenLineJoin ToWpf (this PenLineJoin value)
		{
			switch (value) {
			case PenLineJoin.Miter:
				return swm.PenLineJoin.Miter;
			case PenLineJoin.Bevel:
				return swm.PenLineJoin.Bevel;
			case PenLineJoin.Round:
				return swm.PenLineJoin.Round;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PenLineJoin ToEto (this swm.PenLineJoin value)
		{
			switch (value) {
			case swm.PenLineJoin.Bevel:
				return PenLineJoin.Bevel;
			case swm.PenLineJoin.Miter:
				return PenLineJoin.Miter;
			case swm.PenLineJoin.Round:
				return PenLineJoin.Round;
			default:
				throw new NotSupportedException ();
			}
		}

		public static swm.PenLineCap ToWpf (this PenLineCap value)
		{
			switch (value) {
			case PenLineCap.Butt:
				return swm.PenLineCap.Flat;
			case PenLineCap.Round:
				return swm.PenLineCap.Round;
			case PenLineCap.Square:
				return swm.PenLineCap.Square;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PenLineCap ToEto (this swm.PenLineCap value)
		{
			switch (value) {
			case swm.PenLineCap.Flat:
				return PenLineCap.Butt;
			case swm.PenLineCap.Round:
				return PenLineCap.Round;
			case swm.PenLineCap.Square:
				return PenLineCap.Square;
			default:
				throw new NotSupportedException ();
			}
		}

		public static swm.Brush ToWpf (this IBrush brush)
		{
			return (swm.Brush)brush.ControlObject;
		}
	}
}
