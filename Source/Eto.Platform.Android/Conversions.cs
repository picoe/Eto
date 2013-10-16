using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Platform.Android
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

		public static MouseEventArgs ToEto(av.MotionEvent e)
		{
			if (e.Action == av.MotionEventActions.Down)
			{
				return new MouseEventArgs(MouseButtons.Primary, Key.None, new PointF(e.GetX(), e.GetY()));
			}
			// Is this correct? It generates a mouse event for pointer-up and cancel actions
			// See the iOS handler as well, which does something similar
			return new MouseEventArgs(MouseButtons.Primary, Key.None, Point.Empty);
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
			return new ag.Rect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static ag.RectF ToAndroid(this RectangleF rect)
		{
			return new ag.RectF(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static ag.Rect ToAndroidRectangle(this RectangleF rect)
		{
			return new ag.Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
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

		public static HorizontalAlign ToEtoHorizontal(this av.GravityFlags gravity)
		{
			switch (gravity & av.GravityFlags.HorizontalGravityMask)
			{
				case av.GravityFlags.CenterHorizontal:
					return HorizontalAlign.Center;
				case av.GravityFlags.Left:
					return HorizontalAlign.Left;
				case av.GravityFlags.Right:
					return HorizontalAlign.Right;
				default:
					throw new NotSupportedException();
			}
		}

		public static av.GravityFlags ToAndroid(this HorizontalAlign value)
		{
			switch (value)
			{
				case HorizontalAlign.Center:
					return av.GravityFlags.CenterHorizontal;
				case HorizontalAlign.Right:
					return av.GravityFlags.Right;
				case HorizontalAlign.Left:
					return av.GravityFlags.Left;
				default:
					throw new NotSupportedException();
			}
		}

		public static VerticalAlign ToEtoVertical(this av.GravityFlags gravity)
		{
			switch (gravity & av.GravityFlags.VerticalGravityMask)
			{
				case av.GravityFlags.CenterVertical:
					return VerticalAlign.Middle;
				case av.GravityFlags.Top:
					return VerticalAlign.Top;
				case av.GravityFlags.Bottom:
					return VerticalAlign.Bottom;
				default:
					throw new NotSupportedException();
			}
		}

		public static av.GravityFlags ToAndroid(this VerticalAlign value)
		{
			switch (value)
			{
				case VerticalAlign.Middle:
					return av.GravityFlags.CenterVertical;
				case VerticalAlign.Top:
					return av.GravityFlags.Top;
				case VerticalAlign.Bottom:
					return av.GravityFlags.Bottom;
				default:
					throw new NotSupportedException();
			}
		}
	}
}