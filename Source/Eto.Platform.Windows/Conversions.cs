using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using sdp = System.Drawing.Printing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public static partial class Conversions
	{
		public static Padding ToEto (this swf.Padding padding)
		{
			return new Padding (padding.Left, padding.Top, padding.Right, padding.Bottom);
		}

		public static swf.Padding ToSWF (this Padding padding)
		{
			return new swf.Padding (padding.Left, padding.Top, padding.Right, padding.Bottom);
		}

		public static Color ToEto (this sd.Color color)
		{
			return new Color (color.R / 255f, color.G / 255f, color.B / 255f);
		}

		public static sd.Color ToSD (this Color color)
		{
			return sd.Color.FromArgb ((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
		}

		public static DialogResult ToEto (this swf.DialogResult result)
		{
			DialogResult ret = DialogResult.None;
			if (result == swf.DialogResult.OK) ret = DialogResult.Ok;
			else if (result == swf.DialogResult.Cancel) ret = DialogResult.Cancel;
			else if (result == swf.DialogResult.Yes) ret = DialogResult.Yes;
			else if (result == swf.DialogResult.No) ret = DialogResult.No;
			else if (result == swf.DialogResult.Abort) ret = DialogResult.Cancel;
			else if (result == swf.DialogResult.Ignore) ret = DialogResult.Ignore;
			else if (result == swf.DialogResult.Retry) ret = DialogResult.Retry;
			else if (result == swf.DialogResult.None) ret = DialogResult.None;
			return ret;
		}

		public static sd.Imaging.ImageFormat ToSD (this ImageFormat format)
		{
			switch (format)
			{
			case ImageFormat.Jpeg: return sd.Imaging.ImageFormat.Jpeg;
			case ImageFormat.Bitmap: return sd.Imaging.ImageFormat.Bmp;
			case ImageFormat.Gif: return sd.Imaging.ImageFormat.Gif;
			case ImageFormat.Tiff: return sd.Imaging.ImageFormat.Tiff;
			case ImageFormat.Png: return sd.Imaging.ImageFormat.Png;
			default: throw new Exception ("Invalid format specified");
			}
		}

		public static ImageInterpolation ToEto (this sd.Drawing2D.InterpolationMode value)
		{
			switch (value)
			{
			case sd.Drawing2D.InterpolationMode.NearestNeighbor:
				return ImageInterpolation.None;
			case sd.Drawing2D.InterpolationMode.Low:
				return ImageInterpolation.Low;
			case sd.Drawing2D.InterpolationMode.High:
				return ImageInterpolation.Medium;
			case sd.Drawing2D.InterpolationMode.HighQualityBilinear:
				return ImageInterpolation.High;
			case sd.Drawing2D.InterpolationMode.Default:
				return ImageInterpolation.Default;
			case sd.Drawing2D.InterpolationMode.HighQualityBicubic:
			case sd.Drawing2D.InterpolationMode.Bicubic:
			case sd.Drawing2D.InterpolationMode.Bilinear:
			default:
				throw new NotSupportedException ();
			}
		}

		public static sd.Drawing2D.InterpolationMode ToSD (this ImageInterpolation value)
		{
			switch (value)
			{
			case ImageInterpolation.Default:
				return sd.Drawing2D.InterpolationMode.Default;
			case ImageInterpolation.None:
				return sd.Drawing2D.InterpolationMode.NearestNeighbor;
			case ImageInterpolation.Low:
				return sd.Drawing2D.InterpolationMode.Low;
			case ImageInterpolation.Medium:
				return sd.Drawing2D.InterpolationMode.High;
			case ImageInterpolation.High:
				return sd.Drawing2D.InterpolationMode.HighQualityBilinear;
			default:
				throw new NotSupportedException ();
			}
		}

		public static sd.FontStyle ToSD (this FontStyle style)
		{
			sd.FontStyle ret = sd.FontStyle.Regular;
			if ((style & FontStyle.Bold) != 0) ret |= sd.FontStyle.Bold;
			if ((style & FontStyle.Italic) != 0) ret |= sd.FontStyle.Italic;
			return ret;
		}

		public static sdp.PrintRange ToSDP (this PrintSelection value)
		{
			switch (value) {
			case PrintSelection.AllPages:
				return sdp.PrintRange.AllPages;
			case PrintSelection.SelectedPages:
				return sdp.PrintRange.SomePages;
			case PrintSelection.Selection:
				return sdp.PrintRange.Selection;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PrintSelection ToEto (this sdp.PrintRange value)
		{
			switch (value) {
			case sdp.PrintRange.AllPages:
				return PrintSelection.AllPages;
			case sdp.PrintRange.SomePages:
				return PrintSelection.SelectedPages;
			case sdp.PrintRange.Selection:
				return PrintSelection.Selection;
			default:
				throw new NotSupportedException ();
			}
		}

		public static FontStyle ToEto (this sd.FontStyle style)
		{
			var ret = FontStyle.Normal;
			if (style.HasFlag (sd.FontStyle.Bold)) ret |= FontStyle.Bold;
			if (style.HasFlag (sd.FontStyle.Italic)) ret |= FontStyle.Italic;
			return ret;
		}
	}
}
