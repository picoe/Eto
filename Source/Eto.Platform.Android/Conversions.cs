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
	}
}

