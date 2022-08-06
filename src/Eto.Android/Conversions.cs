using System;
using Eto.Forms;
using Eto.Drawing;
using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using Eto.Android.Drawing;

namespace Eto.Android
{
	public static class Conversions
	{
		public static Padding GetPadding(this av.View view)
		{
			return new Padding(view.PaddingLeft, view.PaddingTop, view.PaddingRight, view.PaddingBottom);
		}

		public static void SetPadding(this av.View view, Padding padding)
		{
			view.SetPadding(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}
		
		public static void SetPadding(this av.View view, Int32 padding)
		{
			view.SetPadding(padding, padding, padding, padding);
		}

		public static Color ToEto(this global::Android.Content.Res.ColorStateList colors)
		{
			return Color.FromArgb(colors.DefaultColor);
		}

		public static Color ToEto(this ag.Color color)
		{
			return Color.FromArgb(color.R, color.G, color.B, color.A);
		}

		public static ag.Color ToAndroid(this Color color)
		{
			return new ag.Color((int)color.ToArgb());
		}

		public static ag.Paint ToAndroid(this Pen pen)
		{
			return (ag.Paint)pen.ControlObject;
		}

		public static ag.Paint ToAndroid(this Brush brush)
		{
			return ((BrushHandler)brush.Handler).GetPaint(brush);
		}

		public static ag.Paint.Join ToAndroid(this PenLineJoin value)
		{
			switch (value)
			{
				case PenLineJoin.Miter:
					return ag.Paint.Join.Miter;
				case PenLineJoin.Bevel:
					return ag.Paint.Join.Bevel;
				case PenLineJoin.Round:
					return ag.Paint.Join.Round;
				default:
					throw new NotSupportedException();
			}
		}

		public static PenLineJoin ToEto(this ag.Paint.Join value)
		{
			if (object.ReferenceEquals(value, ag.Paint.Join.Bevel))
				return PenLineJoin.Bevel;
			if (object.ReferenceEquals(value, ag.Paint.Join.Miter))
				return PenLineJoin.Miter;
			if (object.ReferenceEquals(value, ag.Paint.Join.Round))
				return PenLineJoin.Round;
			throw new NotSupportedException();
		}

		public static ag.Paint.Cap ToSD(this PenLineCap value)
		{
			switch (value)
			{
				case PenLineCap.Butt:
					return ag.Paint.Cap.Butt;
				case PenLineCap.Round:
					return ag.Paint.Cap.Round;
				case PenLineCap.Square:
					return ag.Paint.Cap.Square;
				default:
					throw new NotSupportedException();
			}
		}

		public static PenLineCap ToEto(this ag.Paint.Cap value)
		{
			if (object.ReferenceEquals(value, ag.Paint.Cap.Butt))
				return PenLineCap.Butt;
			if (object.ReferenceEquals(value, ag.Paint.Cap.Round))
				return PenLineCap.Round;
			if (object.ReferenceEquals(value, ag.Paint.Cap.Square))
				return PenLineCap.Square;
			throw new NotSupportedException();
		}

		public static ag.Matrix ToAndroid(this IMatrix m)
		{
			return (ag.Matrix)m.ControlObject;
		}

		public static IMatrix ToEto(this ag.Matrix m)
		{
			return new MatrixHandler(m);
		}

		public static Point ToEto(this ag.Point point)
		{
			return new Point(point.X, point.Y);
		}

		public static PointF ToEto(this ag.PointF point)
		{
			return new PointF(point.X, point.Y);
		}

		public static ag.PointF ToAndroid(this PointF point)
		{
			return new ag.PointF(point.X, point.Y);
		}

		public static ag.Point ToAndroidPoint(this PointF point)
		{
			return new ag.Point((int)point.X, (int)point.Y);
		}

		public static Rectangle ToEto(this ag.Rect rect)
		{
			return new Rectangle(rect.Left, rect.Top, rect.Width(), rect.Height());
		}

		public static RectangleF ToEto(this ag.RectF rect)
		{
			return new RectangleF(rect.Left, rect.Top, rect.Width(), rect.Height());
		}

		public static ag.Rect ToAndroid(this Rectangle rect)
		{
			return new ag.Rect(rect.X, rect.Y, rect.Right, rect.Bottom);
		}

		public static ag.RectF ToAndroid(this RectangleF rect)
		{
			return new ag.RectF(rect.X, rect.Y, rect.Right, rect.Bottom);
		}

		public static ag.Rect ToAndroidRectangle(this RectangleF rect)
		{
			return new ag.Rect((int)rect.X, (int)rect.Y, (int)rect.Right, (int)rect.Bottom);
		}

		internal static ag.Point[] ToAndroid(this Point[] points)
		{
			var result =
				new ag.Point[points.Length];

			for (var i = 0;
				i < points.Length;
				++i)
			{
				var p = points[i];
				result[i] =
					new ag.Point(p.X, p.Y);
			}

			return result;
		}

		internal static ag.PointF[] ToAndroid(this PointF[] points)
		{
			var result =
				new ag.PointF[points.Length];

			for (var i = 0;
				i < points.Length;
				++i)
			{
				var p = points[i];
				result[i] =
					new ag.PointF(p.X, p.Y);
			}

			return result;
		}

		internal static PointF[] ToEto(this ag.PointF[] points)
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

		public static GraphicsPathHandler ToHandler(this IGraphicsPath path)
		{
			return ((GraphicsPathHandler)path.ControlObject);
		}

		public static ag.Path ToAndroid(this IGraphicsPath path)
		{
			return ((GraphicsPathHandler)path.ControlObject).Control;
		}

		public static ag.TypefaceStyle ToAndroid(this FontStyle style)
		{
			var ret = ag.TypefaceStyle.Normal;
			if ((style & FontStyle.Bold) != 0)
				ret = ag.TypefaceStyle.Bold;
			if ((style & FontStyle.Italic) != 0)
				ret = ag.TypefaceStyle.Italic;
			return ret;
		}

		public static FontStyle ToEto(this ag.TypefaceStyle style)
		{
			var ret = FontStyle.None;
			if (style == ag.TypefaceStyle.Normal)
				ret = FontStyle.None;
			else if (style == ag.TypefaceStyle.Bold)
				ret = FontStyle.Bold;
			else if (style == ag.TypefaceStyle.Italic)
				ret = FontStyle.Italic;
			return ret;
		}

		public static TextAlignment ToEtoHorizontal(this av.GravityFlags gravity)
		{
			switch (gravity & av.GravityFlags.HorizontalGravityMask)
			{
				case av.GravityFlags.CenterHorizontal:
					return TextAlignment.Center;
				case av.GravityFlags.Left:
					return TextAlignment.Left;
				case av.GravityFlags.Right:
					return TextAlignment.Right;
				default:
					throw new NotSupportedException();
			}
		}

		public static av.GravityFlags ToAndroidGravity(this TextAlignment value)
		{
			switch (value)
			{
				case TextAlignment.Center:
					return av.GravityFlags.CenterHorizontal;
				case TextAlignment.Right:
					return av.GravityFlags.Right;
				case TextAlignment.Left:
					return av.GravityFlags.Left;
				default:
					throw new NotSupportedException();
			}
		}

		public static av.TextAlignment ToAndroid(this TextAlignment value)
		{
			switch (value)
			{
				case TextAlignment.Center:
					return av.TextAlignment.Center;
				case TextAlignment.Right:
					return av.TextAlignment.TextEnd;
				case TextAlignment.Left:
					return av.TextAlignment.TextStart;
				default:
					throw new NotSupportedException();
			}
		}

		public static TextAlignment ToEto(this av.TextAlignment value)
		{
			switch (value)
			{
				case av.TextAlignment.Center:
					return TextAlignment.Center;
				case av.TextAlignment.TextEnd:
				case av.TextAlignment.ViewEnd:
					return TextAlignment.Right;
				default:
					return TextAlignment.Left;
			}
		}

		public static VerticalAlignment ToEtoVertical(this av.GravityFlags gravity)
		{
			switch (gravity & av.GravityFlags.VerticalGravityMask)
			{
				case av.GravityFlags.CenterVertical:
					return VerticalAlignment.Center;
				case av.GravityFlags.Top:
					return VerticalAlignment.Top;
				case av.GravityFlags.Bottom:
					return VerticalAlignment.Bottom;
				case av.GravityFlags.FillVertical:
					return VerticalAlignment.Stretch;
				default:
					throw new NotSupportedException();
			}
		}

		public static av.GravityFlags ToAndroid(this VerticalAlignment value)
		{
			switch (value)
			{
				case VerticalAlignment.Center:
					return av.GravityFlags.CenterVertical;
				case VerticalAlignment.Top:
					return av.GravityFlags.Top;
				case VerticalAlignment.Bottom:
					return av.GravityFlags.Bottom;
				case VerticalAlignment.Stretch:
					return av.GravityFlags.FillVertical;
				default:
					throw new NotSupportedException();
			}
		}

		public static ag.Bitmap ToAndroid(this Image image)
		{
			var handler = (IAndroidImage)image.Handler;
			return handler.GetImageWithDensity(null);
		}

		public static ag.Drawables.BitmapDrawable ToAndroidDrawable(this Image image, Size? size = null)
		{
			var bmp = image.ToAndroid();
			var draw = bmp != null ? new ag.Drawables.BitmapDrawable(aa.Application.Context.Resources, bmp) : null;

			if(draw != null && size != null)
            {
				var Size = Platform.DpToPx(size.Value);
				draw.SetBounds(0, 0, Size.Width, Size.Height);
			}

			return draw;
		}

		public static Bitmap ToEto(this ag.Bitmap bitmap)
		{
			return new Bitmap(new BitmapHandler(bitmap));
		}


		public static ag.Typeface ToAndroid(this Font font)
		{
			return FontHandler.GetControl(font);
		}

		public static Font ToEto(this ag.Typeface typeface, float size)
		{
			return new Font(new FontHandler(typeface, size));
		}

		public static MouseEventArgs ToEto(this av.MotionEvent mevent)
		{
			if(mevent.ActionMasked == av.MotionEventActions.Down || mevent.ActionMasked == av.MotionEventActions.Up)
				return new MouseEventArgs(mevent.ButtonState.ToEto(), Keys.None, new PointF(mevent.GetX(), mevent.GetY()), null, mevent.Pressure);

			return new MouseEventArgs(mevent.ButtonState.ToEto(), Keys.None, PointF.Empty);
		}

		public static MouseButtons ToEto(this av.MotionEventButtonState buttons, Boolean nothingIsPrimary = true)
		{
			// A finger touch doesn't count as a 'button' so just act like a primary click.
			if (buttons == 0 && nothingIsPrimary)
				return MouseButtons.Primary;

			MouseButtons Result = MouseButtons.None;

			if ((buttons & av.MotionEventButtonState.Primary) > 0)
				Result |= MouseButtons.Primary;

			if ((buttons & av.MotionEventButtonState.Secondary) > 0)
				Result |= MouseButtons.Alternate;

			if ((buttons & av.MotionEventButtonState.Tertiary) > 0)
				Result |= MouseButtons.Middle;

			return Result;
		}
	}
}