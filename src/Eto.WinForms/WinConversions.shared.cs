using System;
using System.Globalization;
using Eto.Drawing;
using Eto.Forms;
using sd = System.Drawing;
using sdp = System.Drawing.Printing;
using sd2 = System.Drawing.Drawing2D;
using swf = System.Windows.Forms;
using sdi = System.Drawing.Imaging;

#if WPF
namespace Eto.Wpf
#else
namespace Eto.WinForms
#endif
{
	public static partial class WinConversions
	{
		public const float WheelDelta = 120f;

		public static MouseEventArgs ToEto(this swf.MouseEventArgs e, swf.Control control)
		{
			var point = control.PointToClient(swf.Control.MousePosition).ToEto();
			var buttons = ToEto(e.Button);
			var modifiers = swf.Control.ModifierKeys.ToEto();

			return new MouseEventArgs(buttons, modifiers, point, new SizeF(0, (float)e.Delta / WheelDelta));
		}

		public static SizeF ToEtoDelta(this swf.MouseEventArgs e)
		{
			return new SizeF(0, (float)e.Delta / WheelDelta);
		}

		public static MouseButtons ToEto(this swf.MouseButtons button)
		{
			MouseButtons buttons = MouseButtons.None;

			if ((button & swf.MouseButtons.Left) != 0)
				buttons |= MouseButtons.Primary;

			if ((button & swf.MouseButtons.Right) != 0)
				buttons |= MouseButtons.Alternate;

			if ((button & swf.MouseButtons.Middle) != 0)
				buttons |= MouseButtons.Middle;

			return buttons;
		}

		public static Padding ToEto(this swf.Padding padding)
		{
			return new Padding(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}

		public static swf.Padding ToSWF(this Padding padding)
		{
			return new swf.Padding(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}

		public static Color ToEto(this sd.Color color)
		{
			return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
		}

		public static sd.Color ToSD(this Color color)
		{
			return sd.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
		}

		public static Point ToEto(this sd.Point point)
		{
			return new Point(point.X, point.Y);
		}

		public static PointF ToEto(this sd.PointF point)
		{
			return new PointF(point.X, point.Y);
		}

		public static sd.PointF ToSD(this PointF point)
		{
			return new sd.PointF(point.X, point.Y);
		}

		public static sd.Point ToSDPoint(this PointF point)
		{
			return new sd.Point((int)point.X, (int)point.Y);
		}

		public static Size ToEto(this sd.Size size)
		{
			return new Size(size.Width, size.Height);
		}

		public static sd.Size ToSD(this Size size)
		{
			return new sd.Size(size.Width, size.Height);
		}

		public static Size ToEtoF(this sd.SizeF size)
		{
			return new Size((int)size.Width, (int)size.Height);
		}

		public static SizeF ToEto(this sd.SizeF size)
		{
			return new SizeF(size.Width, size.Height);
		}

		public static sd.SizeF ToSD(this SizeF size)
		{
			return new sd.SizeF(size.Width, size.Height);
		}

		public static Rectangle ToEto(this sd.Rectangle rect)
		{
			return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToEto(this sd.RectangleF rect)
		{
			return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.Rectangle ToSD(this Rectangle rect)
		{
			return new sd.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.RectangleF ToSD(this RectangleF rect)
		{
			return new sd.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.Rectangle ToSDRectangle(this RectangleF rect)
		{
			return new sd.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		internal static sd.Point[] ToSD(this Point[] points)
		{
			var result =
				new sd.Point[points.Length];

			for (var i = 0;
				i < points.Length;
				++i)
			{
				var p = points[i];
				result[i] =
					new sd.Point(p.X, p.Y);
			}

			return result;
		}

		internal static sd.PointF[] ToSD(this PointF[] points)
		{
			var result =
				new sd.PointF[points.Length];

			for (var i = 0;
				i < points.Length;
				++i)
			{
				var p = points[i];
				result[i] =
					new sd.PointF(p.X, p.Y);
			}

			return result;
		}

		internal static PointF[] ToEto(this sd.PointF[] points)
		{
			var result =
				new PointF[points.Length];

			for (var i = 0;
				i < points.Length;
				++i)
			{
				var p = points[i];
				result[i] =
					new PointF(p.X, p.Y);
			}

			return result;
		}
	}
}
