using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Direct2D.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using smi = SharpDX.Mathematics.Interop;

namespace Eto.Direct2D
{
	/// <summary>
	/// Direct2D conversions. This file is part of the Direct2D platform library
	/// and is also linked into the Xaml platform library.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Conversions
	{
		public static s.Color4 ToDx(this Color color)
		{
			return new s.Color4(color.R, color.G, color.B, color.A);
		}

		public static s.Color ToDxColor(this Color color)
		{
			return new s.Color(color.R, color.G, color.B, color.A);
		}

		public static Color ToEto(this s.Color4 value)
		{
			return new Color { A = value.Alpha, R = value.Red, G = value.Green, B = value.Blue };
		}

		public static Rectangle ToEto(this s.RectangleF value)
		{
			return new Rectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
		}

		public static Rectangle ToEto(this smi.RawRectangleF value)
		{
			return new Rectangle((int)value.Left, (int)value.Top, (int)(value.Right - value.Left), (int)(value.Bottom - value.Top));
		}


		public static RectangleF ToEtoF(this s.RectangleF value)
		{
			return new RectangleF((float)value.X, (float)value.Y, (float)value.Width, (float)value.Height);
		}

		public static s.RectangleF ToDxF(this Rectangle value)
		{
			return new s.RectangleF(value.X, value.Y, value.Width, value.Height);
		}

		public static s.Rectangle ToDx(this Rectangle value)
		{
			return new s.Rectangle(value.X, value.Y, value.Width, value.Height);
		}

		public static s.RectangleF ToDx(this RectangleF value)
		{
			return new s.RectangleF(value.X, value.Y, value.Width, value.Height);
		}

		public static Size ToEto(this s.Size2 value)
		{
			return new Size(value.Width, value.Height);
		}

		public static s.Size2 ToDx(this Size value)
		{
			return new s.Size2(value.Width, value.Height);
		}

		public static SizeF ToEto(this s.Size2F value)
		{
			return new SizeF(value.Width, value.Height);
		}

		public static s.Size2F ToDx(this SizeF value)
		{
			return new s.Size2F(value.Width, value.Height);
		}

		public static PointF ToEto(this s.Vector2 value)
		{
			return new PointF(value.X, value.Y);
		}

		public static s.Vector2 ToDx(this PointF value)
		{
			return new s.Vector2(value.X, value.Y);
		}

		public static string ToWpfMneumonic(this string value)
		{
			if (value == null) return null;
			return value.Replace("_", "__").Replace("&", "_");
		}

		public static sd.BitmapInterpolationMode ToDx(this ImageInterpolation value)
		{
			switch (value)
			{
				case ImageInterpolation.None:
					return sd.BitmapInterpolationMode.NearestNeighbor;
				case ImageInterpolation.Default:
				case ImageInterpolation.Low:
				case ImageInterpolation.Medium:
				case ImageInterpolation.High:
					return sd.BitmapInterpolationMode.Linear;
				default:
					throw new NotSupportedException();
			}
		}

		public static ImageInterpolation ToEto(this sd.BitmapInterpolationMode value)
		{
			switch (value)
			{
				case sd.BitmapInterpolationMode.Linear:
					return ImageInterpolation.High;
				case sd.BitmapInterpolationMode.NearestNeighbor:
					return ImageInterpolation.None;
				default:
					throw new NotSupportedException();
			}
		}

		public static sd.Bitmap ToDx(this Image image, sd.RenderTarget target, Size? fittingSize = null)
		{
			if (fittingSize != null)
			{
				var icon = image as Icon;
				if (icon != null)
				{
					return icon.GetFrame(1f, fittingSize).Bitmap.ToDx(target);
				}
			}
			var handler = (ID2DBitmapHandler)image.Handler;
			return target != null ? handler.GetBitmap(target) : null;
		}

		public static s.WIC.Bitmap ToWic(this Image image)
		{
			var handler = (ID2DBitmapHandler)image.Handler;
			return handler?.Control;
		}

		public static FontStyle ToEtoStyle(this sw.Font font)
		{
			var style = FontStyle.None;
			if (font.Style == sw.FontStyle.Italic || font.Style == sw.FontStyle.Oblique)
				style |= FontStyle.Italic;

			if (font.Weight.IsBold())
				style |= FontStyle.Bold;
			return style;
		}

		public static bool IsBold(this sw.FontWeight weight)
		{
			switch (weight)
			{
				case SharpDX.DirectWrite.FontWeight.Bold:
				case SharpDX.DirectWrite.FontWeight.DemiBold:
				case SharpDX.DirectWrite.FontWeight.ExtraBold:
				case SharpDX.DirectWrite.FontWeight.Heavy:
					return true;
				case SharpDX.DirectWrite.FontWeight.ExtraBlack:
				case SharpDX.DirectWrite.FontWeight.ExtraLight:
				case SharpDX.DirectWrite.FontWeight.Light:
				case SharpDX.DirectWrite.FontWeight.Medium:
				case SharpDX.DirectWrite.FontWeight.Normal:
				case SharpDX.DirectWrite.FontWeight.Thin:
				default:
					return false;
			}
		}

		public static void Convert(FontStyle value, out sw.FontStyle fontStyle, out sw.FontWeight fontWeight)
		{
			fontStyle = sw.FontStyle.Normal;
			fontWeight = sw.FontWeight.Normal;

			if (value.HasFlag(FontStyle.Italic))
				fontStyle = sw.FontStyle.Italic;

			if (value.HasFlag(FontStyle.Bold))
				fontWeight = sw.FontWeight.Bold;
		}

		public static sd.DashStyle ToDx(this DashStyle d)
		{
			if (d == DashStyles.Solid)
				return sd.DashStyle.Solid;
			else if (d == DashStyles.Dash)
				return sd.DashStyle.Dash;
			else if (d == DashStyles.DashDot)
				return sd.DashStyle.DashDot;
			else if (d == DashStyles.DashDotDot)
				return sd.DashStyle.DashDotDot;
			else if (d == DashStyles.Dot)
				return sd.DashStyle.Dot;
			else
			{
				return sd.DashStyle.Custom;
			}
		}

		public static sd.LineJoin ToDx(this PenLineJoin lineJoin)
		{
			switch (lineJoin)
			{
				case PenLineJoin.Miter:
					return sd.LineJoin.MiterOrBevel;
				case PenLineJoin.Bevel:
					return sd.LineJoin.Bevel;
				case PenLineJoin.Round:
					return sd.LineJoin.Round;
				default:
					throw new NotSupportedException();
			}
		}

		public static sd.CapStyle ToDx(this PenLineCap lineCap)
		{
			switch (lineCap)
			{
				case PenLineCap.Square:
					return sd.CapStyle.Square;
				case PenLineCap.Butt:
					return sd.CapStyle.Flat;
				case PenLineCap.Round:
					return sd.CapStyle.Round;
				default:
					throw new NotSupportedException();
			}
		}

		public static DashStyle ToEto(this sd.DashStyle d)
		{
			if (object.ReferenceEquals(d, sd.DashStyle.Dash))
				return DashStyles.Dash;
			else if (object.ReferenceEquals(d, sd.DashStyle.DashDot))
				return DashStyles.DashDot;
			else if (object.ReferenceEquals(d, sd.DashStyle.DashDotDot))
				return DashStyles.DashDotDot;
			else if (object.ReferenceEquals(d, sd.DashStyle.Dot))
				return DashStyles.Dot;
			else
				return DashStyles.Solid;
		}

		public static FillMode ToEto(this sd.FillMode f)
		{
			return
				f == sd.FillMode.Alternate
				? FillMode.Alternate
				: FillMode.Winding;
		}

		public static sd.FillMode ToDx(this FillMode f)
		{
			return
				f == FillMode.Alternate
				? sd.FillMode.Alternate
				: sd.FillMode.Winding;
		}

		public static s.Vector2[] ToDx(this PointF[] points)
		{
			var p = new s.Vector2[points.Length];
			for (var i = 0; i < points.Length; ++i)
				p[i] = points[i].ToDx();
			return p;
		}

		public static s.Matrix3x2 ToDx(this IMatrix m)
		{
			return m != null ? (s.Matrix3x2)m.ControlObject : s.Matrix3x2.Identity;
		}

		public static IMatrix ToEto(this s.Matrix3x2 m)
		{
			return new MatrixHandler(ref m);
		}

		public static float DegreesToRadians(float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

		public static PenData ToPenData(this Pen pen)
		{
			return (PenData)pen.ControlObject;
		}

		public static sd.Brush ToDx(this Brush brush, sd.RenderTarget target)
		{
			var obj = (BrushData)brush.ControlObject;
			return obj.Get(target);
		}

		public static GraphicsPathHandler ToHandler(this IGraphicsPath path)
		{
			return path as GraphicsPathHandler ?? ((IHandlerSource)path).Handler as GraphicsPathHandler;
		}

		public static sd.Geometry ToGeometry(this IGraphicsPath path)
		{
			var handler = path.ToHandler();
			handler.CloseSink();
			return handler.Control;
		}

		public static sd.PixelFormat ToDx(this PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.Format32bppRgb:
					return new sd.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, sd.AlphaMode.Ignore);
				case PixelFormat.Format24bppRgb:
					return new sd.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, sd.AlphaMode.Ignore);
				case PixelFormat.Format32bppRgba:
					return new sd.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, sd.AlphaMode.Premultiplied);
				default:
					throw new NotSupportedException();
			}
		}

		public static Guid ToWic(this PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.Format32bppRgb:
					return s.WIC.PixelFormat.Format32bppBGR;
				case PixelFormat.Format24bppRgb:
					return s.WIC.PixelFormat.Format24bppBGR;
				case PixelFormat.Format32bppRgba:
					return s.WIC.PixelFormat.Format32bppPBGRA;
				default:
					throw new NotSupportedException();
			}
		}

#if WINFORMS
		public static System.Drawing.Imaging.PixelFormat ToSDPixelFormat(this Guid pixelFormat)
		{
			if (pixelFormat == s.WIC.PixelFormat.Format32bppBGR)
				return System.Drawing.Imaging.PixelFormat.Format32bppRgb;
			if (pixelFormat == s.WIC.PixelFormat.Format32bppPBGRA)
				return System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
			if (pixelFormat == s.WIC.PixelFormat.Format24bppBGR)
				return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
			if (pixelFormat == s.WIC.PixelFormat.Format8bppIndexed)
				return System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
			throw new NotSupportedException();
		}
#endif

		public static Guid ToWic(this ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.Jpeg:
					return s.WIC.ContainerFormatGuids.Jpeg;
				case ImageFormat.Bitmap:
					return s.WIC.ContainerFormatGuids.Bmp;
				case ImageFormat.Tiff:
					return s.WIC.ContainerFormatGuids.Tiff;
				case ImageFormat.Png:
					return s.WIC.ContainerFormatGuids.Png;
				case ImageFormat.Gif:
					return s.WIC.ContainerFormatGuids.Gif;
				default:
					throw new NotSupportedException();
			}
		}

#if WINFORMS
		public static System.Drawing.Bitmap ToSD(this s.WIC.Bitmap bmp)
		{
			using (var bl = bmp.Lock(s.WIC.BitmapLockFlags.Read))
			{
				var pixelFormat = bmp.PixelFormat.ToSDPixelFormat();
				
				return new System.Drawing.Bitmap(bmp.Size.Width, bmp.Size.Height, bl.Data.Pitch, pixelFormat, bl.Data.DataPointer);
			}
		}
#endif

		public static s.WIC.Bitmap ToBitmap(this s.WIC.BitmapSource bmp, Guid? pixelFormat = null)
		{
			using (var converter = new s.WIC.FormatConverter(SDFactory.WicImagingFactory))
			{
				converter.Initialize(bmp, pixelFormat ?? s.WIC.PixelFormat.Format32bppPBGRA);

				return new s.WIC.Bitmap(SDFactory.WicImagingFactory, converter, s.WIC.BitmapCreateCacheOption.CacheOnLoad);
			}
		}

		public static sd.ExtendMode ToDx(this GradientWrapMode value)
		{
			switch (value)
			{
				case GradientWrapMode.Repeat:
					return sd.ExtendMode.Wrap;
				case GradientWrapMode.Reflect:
					return sd.ExtendMode.Mirror;
				case GradientWrapMode.Pad:
					return sd.ExtendMode.Clamp;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
