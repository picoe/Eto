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
using System.Text.RegularExpressions;

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

        public static sw.Rect ToWpf (this Rectangle value)
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

        public static Point ToEto (this sw.Point value)
        {
            return new Point ((int)value.X, (int)value.Y);
        }

        public static sw.Point ToWpf (this Point value)
        {
            return new sw.Point (value.X, value.Y);
        }


        public static string ToWpfMneumonic (this string value)
        {
            if (value == null) return null;
            return value.Replace ("_", "__").Replace ("&", "_");
        }

        public static string ConvertMneumonicFromWPF (object obj)
        {
            var value = obj as string;
            if (value == null) return null;
            return Regex.Replace (value, "(?<![_])[_]", (match) => { if (match.Value == "__") return "_"; else return "&"; });
        }

        public static KeyPressEventArgs ToEto (this swi.KeyEventArgs e)
        {
            var key = KeyMap.Convert (e.Key, swi.Keyboard.Modifiers);
            return new KeyPressEventArgs (key) {
                Handled = e.Handled
            };
        }

        public static MouseEventArgs ToEto (this swi.MouseEventArgs e, sw.IInputElement control)
        {
            var buttons = MouseButtons.None;
            if (e.LeftButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Primary;
            if (e.RightButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Alternate;
            if (e.MiddleButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Middle;
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
			if (value == null) return PageOrientation.Portrait;
			switch (value.Value) {
			case sp.PageOrientation.Landscape:
				return PageOrientation.Landscape;
			case sp.PageOrientation.Portrait:
				return PageOrientation.Portrait;
			default:
				throw new NotSupportedException ();
			}
		}
    }
}
