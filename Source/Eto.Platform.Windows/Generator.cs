using System;
using System.Reflection;
using Eto.Forms;
using Eto.Drawing;
using Eto.IO;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public class Generator : Eto.Generator
	{
		public override string ID {
			get {
				return Generators.Windows;
			}
		}
		
		public static Padding Convert(SWF.Padding padding)
		{
			return new Padding(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}
		
		public static SWF.Padding Convert(Padding padding)
		{
			return new SWF.Padding(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}

		public static Color Convert(SD.Color color)
		{
			return new Color(color.R / 255f, color.G / 255f, color.B / 255f);
		}

		public static SD.Color Convert(Color color)
		{
			return SD.Color.FromArgb((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
		}

		public static Size Convert(SD.Size size)
		{
			return new Size(size.Width, size.Height);
		}

		public static SD.Size Convert(Size size)
		{
			return new SD.Size(size.Width, size.Height);
		}

		public static SizeF Convert(SD.SizeF size)
		{
			return new SizeF(size.Width, size.Height);
		}

		public static SD.SizeF Convert(SizeF size)
		{
			return new SD.SizeF(size.Width, size.Height);
		}

		public static Point Convert(SD.Point point)
		{
			return new Point(point.X, point.Y);
		}

		public static SD.Point Convert(Point point)
		{
			return new SD.Point(point.X, point.Y);
		}

		public static Rectangle Convert(SD.Rectangle rect)
		{
			return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static SD.Rectangle Convert(Rectangle rect)
		{
			return new SD.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static DialogResult Convert(SWF.DialogResult result)
		{
			DialogResult ret = DialogResult.None;
			if (result == SWF.DialogResult.OK) ret = DialogResult.Ok;
			else if (result == SWF.DialogResult.Cancel) ret = DialogResult.Cancel;
			else if (result == SWF.DialogResult.Yes) ret = DialogResult.Yes;
			else if (result == SWF.DialogResult.No) ret = DialogResult.No;
			else if (result == SWF.DialogResult.Abort) ret = DialogResult.Cancel;
			else if (result == SWF.DialogResult.Ignore) ret = DialogResult.Ignore;
			else if (result == SWF.DialogResult.Retry) ret = DialogResult.Retry;
			else if (result == SWF.DialogResult.None) ret = DialogResult.None;
			return ret;
		}
		
		public static SD.Imaging.ImageFormat Convert(ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.Jpeg: return SD.Imaging.ImageFormat.Jpeg;
				case ImageFormat.Bitmap: return SD.Imaging.ImageFormat.Bmp;
				case ImageFormat.Gif: return SD.Imaging.ImageFormat.Gif;
				case ImageFormat.Tiff: return SD.Imaging.ImageFormat.Tiff;
				case ImageFormat.Png: return SD.Imaging.ImageFormat.Png;
				default: throw new Exception("Invalid format specified");
			}
		}

		public static SD.FontFamily Convert(FontFamily type)
		{
			switch (type)
			{
				case FontFamily.Monospace: return SD.FontFamily.GenericMonospace;
				default: case FontFamily.Sans: return SD.FontFamily.GenericSansSerif;
				case FontFamily.Serif: return SD.FontFamily.GenericSerif;
			}
		}

		public static FontFamily Convert(SD.FontFamily family)
		{
			if (family == SD.FontFamily.GenericMonospace) return FontFamily.Monospace;
			else if (family == SD.FontFamily.GenericSansSerif) return FontFamily.Sans;
			else if (family == SD.FontFamily.GenericSerif) return FontFamily.Serif;
			else return FontFamily.Sans;
		}

		public static ImageInterpolation Convert (SD.Drawing2D.InterpolationMode value)
		{
			switch (value) {
			case SD.Drawing2D.InterpolationMode.NearestNeighbor:
				return ImageInterpolation.None;
			case SD.Drawing2D.InterpolationMode.Low:
				return ImageInterpolation.Low;
			case SD.Drawing2D.InterpolationMode.High:
				return ImageInterpolation.Medium;
			case SD.Drawing2D.InterpolationMode.HighQualityBilinear:
				return ImageInterpolation.High;
			case SD.Drawing2D.InterpolationMode.Default:
				return ImageInterpolation.Default;
			case SD.Drawing2D.InterpolationMode.HighQualityBicubic:
			case SD.Drawing2D.InterpolationMode.Bicubic:
			case SD.Drawing2D.InterpolationMode.Bilinear:
			default:
				throw new NotSupportedException();
			}
		}

		public static SD.Drawing2D.InterpolationMode Convert (ImageInterpolation value)
		{
			switch (value) {
			case ImageInterpolation.Default:
				return SD.Drawing2D.InterpolationMode.Default;
			case ImageInterpolation.None:
				return SD.Drawing2D.InterpolationMode.NearestNeighbor;
			case ImageInterpolation.Low:
				return SD.Drawing2D.InterpolationMode.Low;
			case ImageInterpolation.Medium:
				return SD.Drawing2D.InterpolationMode.High;
			case ImageInterpolation.High:
				return SD.Drawing2D.InterpolationMode.HighQualityBilinear;
			default:
				throw new NotSupportedException();
			}
		}
	}
}
