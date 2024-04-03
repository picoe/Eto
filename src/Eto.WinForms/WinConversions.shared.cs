using sdp = System.Drawing.Printing;
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
