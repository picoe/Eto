using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Platform.Direct2D.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Platform.Direct2D
{
    public static class Conversions
    {
		public static s.Color4 ToDx(this Color color)
        {
			return new s.Color4(color.R, color.G, color.B, color.A);
        }

        public static Color ToEto(this s.Color4 value)
        {
			return new Color { A = value.Alpha, R = value.Red, G = value.Green, B = value.Blue };
        }

#if TODO
        public static Padding ToEto(this s.Thickness value)
        {
            return new Padding((int)value.Left, (int)value.Top, (int)value.Right, (int)value.Bottom);
        }

        public static s.Thickness ToWpf(this Padding value)
        {
            return new s.Thickness(value.Left, value.Top, value.Right, value.Bottom);
        }
#endif
        public static Rectangle ToEto(this s.RectangleF value)
        {
            return new Rectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
        }

        public static RectangleF ToEtoF(this s.RectangleF value)
        {
            return new RectangleF((float)value.X, (float)value.Y, (float)value.Width, (float)value.Height);
        }

		public static s.RectangleF ToDxF(this Rectangle value)
        {
            return new s.RectangleF(value.X, value.Y, value.Width, value.Height);
        }

		public static s.Rectangle ToDx(this Rectangle value)
		{
			return new s.Rectangle(value.X, value.Y, value.Width, value.Height);
		}

		public static s.RectangleF ToDx(this RectangleF value)
        {
            return new s.RectangleF(value.X, value.Y, value.Width, value.Height);
        }

        public static Size ToEto(this s.Size2F value)
        {
            return new Size((int)value.Width, (int)value.Height);
        }

        public static s.Size2F ToDx(this Size value)
        {
            return new s.Size2F(value.Width, value.Height);
        }

		public static s.Size2F ToDx(this SizeF value)
        {
            return new s.Size2F(value.Width, value.Height);
        }

        public static Point ToEto(this s.Vector2 value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        public static s.Vector2 ToDx(this PointF value)
        {
            return new s.Vector2(value.X, value.Y);
        }

        public static string ToWpfMneumonic(this string value)
        {
            if (value == null) return null;
            return value.Replace("_", "__").Replace("&", "_");
        }

		public static sd.BitmapInterpolationMode ToDx(this ImageInterpolation value)
		{
			switch (value)
			{
				case ImageInterpolation.Default:
					return sd.BitmapInterpolationMode.Linear;
				case ImageInterpolation.None:
					return sd.BitmapInterpolationMode.NearestNeighbor;
				case ImageInterpolation.Low:
					return sd.BitmapInterpolationMode.Linear;
				case ImageInterpolation.Medium:
					return sd.BitmapInterpolationMode.Linear;
				case ImageInterpolation.High:
					return sd.BitmapInterpolationMode.Linear;
				default:
					throw new NotSupportedException();
			}
		}

		public static ImageInterpolation ToEto(this sd.BitmapInterpolationMode value)
		{
			switch (value)
			{
				case sd.BitmapInterpolationMode.Linear:
					return ImageInterpolation.High;
				case sd.BitmapInterpolationMode.NearestNeighbor:
					return ImageInterpolation.None;
				default:
					throw new NotSupportedException();
			}
		}

#if TODO
        public static string ConvertMneumonicFromWPF(object obj)
        {
            var value = obj as string;
            if (value == null) return null;
            return Regex.Replace(value, "(?<![_])[_]", (match) => { if (match.Value == "__") return "_"; else return "&"; });
        }

        public static KeyPressEventArgs ToEto(this swi.KeyEventArgs e, KeyType keyType)
        {
            var key = KeyMap.Convert(e.Key, swi.Keyboard.Modifiers);
            return new KeyPressEventArgs(key, keyType)
            {
                Handled = e.Handled
            };
        }

        public static MouseEventArgs ToEto(this swi.MouseEventArgs e, s.IInputElement control)
        {
            var buttons = MouseButtons.None;
            if (e.LeftButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Primary;
            if (e.RightButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Alternate;
            if (e.MiddleButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Middle;
            var modifiers = Key.None;
            var location = e.GetPosition(control).ToEto();

            return new MouseEventArgs(buttons, modifiers, location);
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
            if (value == null) return PageOrientation.Portrait;
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

        public static Size GetSize(s.FrameworkElement element)
        {
            if (!double.IsNaN(element.ActualWidth) && !double.IsNaN(element.ActualHeight))
                return new Size((int)element.ActualWidth, (int)element.ActualHeight);
            else
                return new Size((int)(double.IsNaN(element.Width) ? -1 : element.Width), (int)(double.IsNaN(element.Height) ? -1 : element.Height));
        }

        public static void SetSize(s.FrameworkElement element, Size size)
        {
            element.Width = size.Width == -1 ? double.NaN : size.Width;
            element.Height = size.Height == -1 ? double.NaN : size.Height;
        }
#endif
        public static FontStyle Convert(sw.FontStyle fontStyle, sw.FontWeight fontWeight)
        {
            var style = FontStyle.None;
            if (fontStyle == sw.FontStyle.Italic)
                style |= FontStyle.Italic;
            if (fontStyle == sw.FontStyle.Oblique)
                style |= FontStyle.Italic;

            if (fontWeight == sw.FontWeight.Bold)
                style |= FontStyle.Bold;
            return style;
        }

        public static void Convert(
            FontStyle value, 
            out sw.FontStyle fontStyle,
            out sw.FontWeight fontWeight)
        {
            fontStyle = sw.FontStyle.Normal;
            fontWeight = sw.FontWeight.Normal;

            if ((value & FontStyle.Italic) == FontStyle.Italic)
                fontStyle = sw.FontStyle.Italic;

            if ((value & FontStyle.Bold) == FontStyle.Bold)
                fontWeight = sw.FontWeight.Bold;           
        }

		public static sd.DashStyle ToDx(this DashStyle d)
        {
            if (d == DashStyles.Dash)
                return sd.DashStyle.Dash;
            else if (d == DashStyles.DashDot)
                return sd.DashStyle.DashDot;
            else if (d == DashStyles.DashDotDot)
                return sd.DashStyle.DashDotDot;
            else if (d == DashStyles.Dot)
                return sd.DashStyle.Dot;
            else
                return sd.DashStyle.Solid;
        }

        public static DashStyle ToEto(this sd.DashStyle d)
        {
            if (object.ReferenceEquals(d, sd.DashStyle.Dash))
                return DashStyles.Dash;
            else if (object.ReferenceEquals(d, sd.DashStyle.DashDot))
                return DashStyles.DashDot;
            else if (object.ReferenceEquals(d, sd.DashStyle.DashDotDot))
                return DashStyles.DashDotDot;
            else if (object.ReferenceEquals(d, sd.DashStyle.Dot))
                return DashStyles.Dot;
            else
                return DashStyles.Solid;
        }

        public static FillMode ToEto(this sd.FillMode f)
        {
            return
                f == sd.FillMode.Alternate
                ? FillMode.Alternate
                : FillMode.Winding;
        }

		public static sd.FillMode ToDx(this FillMode f)
        {
            return
                f == FillMode.Alternate
                ? sd.FillMode.Alternate
                : sd.FillMode.Winding;
        }

        public static s.Vector2[] ToDx(this PointF[] points)
        {
            var p = new s.Vector2[points.Length];
            for (var i = 0; i < points.Length; ++i)
                p[i] = points[i].ToDx();
			return p;
        }

		public static s.Matrix3x2 ToDx(this IMatrix m)
		{
			return (s.Matrix3x2)m.ControlObject;
		}

		public static float DegreesToRadians(float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

		public static PenData ToPenData(this Pen pen)
		{
			return (PenData)pen.ControlObject;
		}

		public static sd.Brush ToDx(this Brush brush, sd.RenderTarget target)
		{
			var obj = (BrushData)brush.ControlObject;
			return obj.Get(target);
		}

		public static GraphicsPathHandler ToHandler(this IGraphicsPath path)
		{
			return path as GraphicsPathHandler ?? ((IHandlerSource)path).Handler as GraphicsPathHandler;
		}

		public static sd.Geometry ToGeometry(this IGraphicsPath path)
		{
			var handler = path.ToHandler();
			handler.CloseSink();
			return handler.Control;
		}

	}
}
