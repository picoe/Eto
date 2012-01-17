using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Platform.Wpf
{
	public class Generator : Eto.Generator
	{
		public const string GeneratorID = "wpf";

		public override string ID
		{
			get { return GeneratorID; }
		}

		public override void ExecuteOnMainThread(Action action)
		{
			// TODO: use dispatcher
			
			base.ExecuteOnMainThread(action);
		}


		public static System.Windows.Media.Color Convert(Color value)
		{
			return System.Windows.Media.Color.FromScRgb(value.A, value.R, value.G, value.B);
		}

		public static Color Convert(System.Windows.Media.Color value)
		{
			return new Color { A = value.ScA, R = value.ScR, G = value.ScG, B = value.ScB };
		}

		public static Padding Convert (System.Windows.Thickness thickness)
		{
			return new Padding ((int)thickness.Left, (int)thickness.Top, (int)thickness.Right, (int)thickness.Bottom);
		}

		public static System.Windows.Thickness Convert (Padding value)
		{
			return new System.Windows.Thickness (value.Left, value.Top, value.Right, value.Bottom);
		}

		public static Rectangle Convert (System.Windows.Rect rect)
		{
			return new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		public static Size Convert (System.Windows.Size size)
		{
			return new Size ((int)size.Width, (int)size.Height);
		}

		public static System.Windows.Size Convert (Size value)
		{
			return new System.Windows.Size (value.Width, value.Height);
		}
	}
}
