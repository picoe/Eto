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
			if(object.ReferenceEquals(value, ag.Paint.Join.Bevel))
				return PenLineJoin.Bevel;
			if(object.ReferenceEquals(value, ag.Paint.Join.Miter))
				return PenLineJoin.Miter;
			if(object.ReferenceEquals(value, ag.Paint.Join.Round))
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
				if(object.ReferenceEquals(value, ag.Paint.Cap.Butt))
					return PenLineCap.Butt;
				if(object.ReferenceEquals(value, ag.Paint.Cap.Round))
					return PenLineCap.Round;
				if(object.ReferenceEquals(value, ag.Paint.Cap.Square))
					return PenLineCap.Square;
				throw new NotSupportedException();
		}
	}
}

