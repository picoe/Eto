using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Windows.Drawing;
using sd = System.Drawing;
using sdp = System.Drawing.Printing;
using sd2 = System.Drawing.Drawing2D;
using swf = System.Windows.Forms;
using sdi = System.Drawing.Imaging;

namespace Eto.Platform.Windows
{
	public static partial class Conversions
	{
		public const float WHEEL_DELTA = 120f;

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
			return new Color (color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
		}

		public static sd.Color ToSD (this Color color)
		{
			return sd.Color.FromArgb ((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
		}

		public static DialogResult ToEto (this swf.DialogResult result)
		{
			DialogResult ret = DialogResult.None;
			if (result == swf.DialogResult.OK)
				ret = DialogResult.Ok;
			else if (result == swf.DialogResult.Cancel)
				ret = DialogResult.Cancel;
			else if (result == swf.DialogResult.Yes)
				ret = DialogResult.Yes;
			else if (result == swf.DialogResult.No)
				ret = DialogResult.No;
			else if (result == swf.DialogResult.Abort)
				ret = DialogResult.Cancel;
			else if (result == swf.DialogResult.Ignore)
				ret = DialogResult.Ignore;
			else if (result == swf.DialogResult.Retry)
				ret = DialogResult.Retry;
			else if (result == swf.DialogResult.None)
				ret = DialogResult.None;
			return ret;
		}

		public static sd.Imaging.ImageFormat ToSD (this ImageFormat format)
		{
			switch (format) {
			case ImageFormat.Jpeg:
				return sd.Imaging.ImageFormat.Jpeg;
			case ImageFormat.Bitmap:
				return sd.Imaging.ImageFormat.Bmp;
			case ImageFormat.Gif:
				return sd.Imaging.ImageFormat.Gif;
			case ImageFormat.Tiff:
				return sd.Imaging.ImageFormat.Tiff;
			case ImageFormat.Png:
				return sd.Imaging.ImageFormat.Png;
			default:
				throw new Exception ("Invalid format specified");
			}
		}

		public static ImageInterpolation ToEto (this sd2.InterpolationMode value)
		{
			switch (value) {
			case sd2.InterpolationMode.NearestNeighbor:
				return ImageInterpolation.None;
			case sd2.InterpolationMode.Low:
				return ImageInterpolation.Low;
			case sd2.InterpolationMode.High:
				return ImageInterpolation.Medium;
			case sd2.InterpolationMode.HighQualityBilinear:
				return ImageInterpolation.High;
			case sd2.InterpolationMode.Default:
				return ImageInterpolation.Default;
			case sd2.InterpolationMode.HighQualityBicubic:
			case sd2.InterpolationMode.Bicubic:
			case sd2.InterpolationMode.Bilinear:
			default:
				throw new NotSupportedException ();
			}
		}

		public static sd2.InterpolationMode ToSD (this ImageInterpolation value)
		{
			switch (value) {
			case ImageInterpolation.Default:
				return sd2.InterpolationMode.High;
			case ImageInterpolation.None:
				return sd2.InterpolationMode.NearestNeighbor;
			case ImageInterpolation.Low:
				return sd2.InterpolationMode.Low;
			case ImageInterpolation.Medium:
				return sd2.InterpolationMode.High;
			case ImageInterpolation.High:
				return sd2.InterpolationMode.HighQualityBilinear;
			default:
				throw new NotSupportedException ();
			}
		}

		public static sd.FontStyle ToSD (this FontStyle style)
		{
			sd.FontStyle ret = sd.FontStyle.Regular;
			if ((style & FontStyle.Bold) != 0)
				ret |= sd.FontStyle.Bold;
			if ((style & FontStyle.Italic) != 0)
				ret |= sd.FontStyle.Italic;
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
			if (style.HasFlag (sd.FontStyle.Bold))
				ret |= FontStyle.Bold;
			if (style.HasFlag (sd.FontStyle.Italic))
				ret |= FontStyle.Italic;
			return ret;
		}

		public static Point ToEto (this sd.Point point)
		{
			return new Point (point.X, point.Y);
		}

		public static PointF ToEto (this sd.PointF point)
		{
			return new PointF (point.X, point.Y);
		}

		public static sd.PointF ToSD (this PointF point)
		{
			return new sd.PointF (point.X, point.Y);
		}

		public static sd.Point ToSDPoint (this PointF point)
		{
			return new sd.Point ((int)point.X, (int)point.Y);
		}

		public static Size ToEto (this sd.Size size)
		{
			return new Size (size.Width, size.Height);
		}

		public static sd.Size ToSD (this Size size)
		{
			return new sd.Size (size.Width, size.Height);
		}

		public static Size ToEtoF (this sd.SizeF size)
		{
			return new Size ((int)size.Width, (int)size.Height);
		}

		public static SizeF ToEto (this sd.SizeF size)
		{
			return new SizeF (size.Width, size.Height);
		}

		public static sd.SizeF ToSD (this SizeF size)
		{
			return new sd.SizeF (size.Width, size.Height);
		}

		public static Rectangle ToEto (this sd.Rectangle rect)
		{
			return new Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToEto (this sd.RectangleF rect)
		{
			return new RectangleF (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.Rectangle ToSD (this Rectangle rect)
		{
			return new sd.Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.RectangleF ToSD (this RectangleF rect)
		{
			return new sd.RectangleF (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.Rectangle ToSDRectangle(this RectangleF rect)
		{
			return new sd.Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		internal static sd.Point[] ToSD (this Point[] points)
		{
			var result =
				new sd.Point[points.Length];

			for (var i = 0;
				i < points.Length;
				++i) {
				var p = points [i];
				result [i] =
					new sd.Point (p.X, p.Y);
			}

			return result;
		}

		internal static sd.PointF[] ToSD (this PointF[] points)
		{
			var result =
				new sd.PointF[points.Length];

			for (var i = 0;
				i < points.Length;
				++i) {
				var p = points [i];
				result [i] =
					new sd.PointF (p.X, p.Y);
			}

			return result;
		}

		internal static PointF[] ToEto (this sd.PointF[] points)
		{
			var result =
				new PointF[points.Length];

			for (var i = 0;
				i < points.Length;
				++i) {
				var p = points [i];
				result [i] =
					new PointF (p.X, p.Y);
			}

			return result;
		}

		public static sd.Graphics ToSD (this Graphics graphics)
		{
			var h = (GraphicsHandler)graphics.Handler;
			return h.Control;
		}

		public static sd.Image ToSD (this Image image, int? size = null)
		{
			if (image == null)
				return null;
			var h = (IWindowsImage)image.Handler;
			return h.GetImageWithSize (size);
		}

		public static sd.Font ToSD (this Font font)
		{
			var h = (FontHandler)font.Handler;
			return h.Control;
		}

		public static MouseEventArgs ToEto (this swf.MouseEventArgs e)
		{
			var point = new Point (e.X, e.Y);
			var buttons = ToEto (e.Button);
			var modifiers = swf.Control.ModifierKeys.ToEto ();

			var result = new MouseEventArgs (buttons, modifiers, point);
			result.Delta = new SizeF (0, (float)e.Delta / WHEEL_DELTA);

			return result;
		}

		public static MouseButtons ToEto (this swf.MouseButtons button)
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

		public static Graphics ToEto (this sd.Graphics g, Eto.Generator generator)
		{
			return new Graphics (generator, new GraphicsHandler (g));
		}

		public static PaintEventArgs ToEto (this swf.PaintEventArgs e, Eto.Generator generator)
		{
			return new Eto.Forms.PaintEventArgs (ToEto (e.Graphics, generator), e.ClipRectangle.ToEto ());
		}

		public static sd.Image ToSD (this IImage image)
		{
			if (image == null)
				return null;
			else
				return image.ControlObject as sd.Image;
		}

		public static sd2.PixelOffsetMode ToSD (this PixelOffsetMode mode)
		{
			switch (mode) {
			case PixelOffsetMode.None:
				return sd2.PixelOffsetMode.None;
			case PixelOffsetMode.Half:
				return sd2.PixelOffsetMode.Half;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PixelOffsetMode ToEto (this sd2.PixelOffsetMode mode)
		{
			switch (mode) {
			case sd2.PixelOffsetMode.None:
				return PixelOffsetMode.None;
			case sd2.PixelOffsetMode.Half:
				return PixelOffsetMode.Half;
			default:
				throw new NotSupportedException ();
			}
		}

		public static sd2.Matrix ToSD (this IMatrix m)
		{
			return (sd2.Matrix)m.ControlObject;
		}

		public static IMatrix ToEto (this sd2.Matrix matrix)
		{
			return new MatrixHandler (matrix);
		}

		public static float DegreesToRadians (float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

        public static ITreeItem ToEto(this swf.TreeNode treeNode)
        {
            return
                treeNode != null
                ? treeNode.Tag as ITreeItem
                : null;
        }

		public static sd.Pen ToSD (this Pen pen)
		{
			return (sd.Pen)pen.ControlObject;
		}

		public static sd.Brush ToSD (this Brush brush)
		{
			return ((BrushHandler)brush.Handler).GetBrush(brush);
		}

		public static sd2.LineJoin ToSD (this PenLineJoin value)
		{
			switch (value) {
			case PenLineJoin.Miter:
				return sd2.LineJoin.Miter;
			case PenLineJoin.Bevel:
				return sd2.LineJoin.Bevel;
			case PenLineJoin.Round:
				return sd2.LineJoin.Round;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PenLineJoin ToEto (this sd2.LineJoin value)
		{
			switch (value) {
			case sd2.LineJoin.Bevel:
				return PenLineJoin.Bevel;
			case sd2.LineJoin.Miter:
				return PenLineJoin.Miter;
			case sd2.LineJoin.Round:
				return PenLineJoin.Round;
			default:
				throw new NotSupportedException ();
			}
		}

		public static sd2.LineCap ToSD (this PenLineCap value)
		{
			switch (value) {
			case PenLineCap.Butt:
				return sd2.LineCap.Flat;
			case PenLineCap.Round:
				return sd2.LineCap.Round;
			case PenLineCap.Square:
				return sd2.LineCap.Square;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PenLineCap ToEto (this sd2.LineCap value)
		{
			switch (value) {
			case sd2.LineCap.Flat:
				return PenLineCap.Butt;
			case sd2.LineCap.Round:
				return PenLineCap.Round;
			case sd2.LineCap.Square:
				return PenLineCap.Square;
			default:
				throw new NotSupportedException ();
			}
		}

		public static sd2.GraphicsPath ToSD (this IGraphicsPath path)
		{
			return (sd2.GraphicsPath)path.ControlObject;
		}

		public static sd2.WrapMode ToSD (this GradientWrapMode wrap)
		{
			switch (wrap) {
			case GradientWrapMode.Reflect:
				return sd2.WrapMode.TileFlipXY;
			case GradientWrapMode.Repeat:
				return sd2.WrapMode.Tile;
			default:
				throw new NotSupportedException ();
			}
		}

		public static GradientWrapMode ToEtoGradientWrap (this sd2.WrapMode wrapMode)
		{
			switch (wrapMode) {
			case sd2.WrapMode.TileFlipXY:
				return GradientWrapMode.Reflect;
			case sd2.WrapMode.Tile:
				return GradientWrapMode.Repeat;
			default:
				throw new NotSupportedException ();
			}
		}

		public static int BitsPerPixel (this sdi.PixelFormat format)
		{
			switch (format) {
			case sdi.PixelFormat.Format1bppIndexed:
				return 1;
			case sdi.PixelFormat.Format4bppIndexed:
				return 4;
			case sdi.PixelFormat.Format8bppIndexed:
				return 8;
			case sdi.PixelFormat.Format24bppRgb:
				return 24;
			case sdi.PixelFormat.Format32bppArgb:
			case sdi.PixelFormat.Format32bppPArgb:
			case sdi.PixelFormat.Format32bppRgb:
				return 32;
			default:
				throw new NotSupportedException ();
			}
		}

		public static swf.TextImageRelation ToSD (this ButtonImagePosition value)
		{
			switch (value) {
			case ButtonImagePosition.Left:
				return swf.TextImageRelation.ImageBeforeText;
			case ButtonImagePosition.Right:
				return swf.TextImageRelation.TextBeforeImage;
			case ButtonImagePosition.Above:
				return swf.TextImageRelation.ImageAboveText;
			case ButtonImagePosition.Below:
				return swf.TextImageRelation.TextAboveImage;
			case ButtonImagePosition.Overlay:
				return swf.TextImageRelation.Overlay;
			default:
				throw new NotSupportedException ();
			}
		}

		public static ButtonImagePosition ToEto (this swf.TextImageRelation value)
		{
			switch (value) {
			case swf.TextImageRelation.ImageAboveText:
				return ButtonImagePosition.Above;
			case swf.TextImageRelation.ImageBeforeText:
				return ButtonImagePosition.Left;
			case swf.TextImageRelation.Overlay:
				return ButtonImagePosition.Overlay;
			case swf.TextImageRelation.TextAboveImage:
				return ButtonImagePosition.Below;
			case swf.TextImageRelation.TextBeforeImage:
				return ButtonImagePosition.Left;
			default:
				throw new NotSupportedException ();
			}
		}

		public static bool IsResizable (this swf.FormBorderStyle style)
		{
			switch (style) {
			case swf.FormBorderStyle.Fixed3D:
			case swf.FormBorderStyle.FixedDialog:
			case swf.FormBorderStyle.FixedSingle:
			case swf.FormBorderStyle.FixedToolWindow:
			case swf.FormBorderStyle.None:
				return false;
			case swf.FormBorderStyle.Sizable:
			case swf.FormBorderStyle.SizableToolWindow:
				return true;
			default:
				throw new NotSupportedException ();
			}
		}

		public static WindowStyle ToEto (this swf.FormBorderStyle style)
		{
			switch (style) {
			case swf.FormBorderStyle.Fixed3D:
			case swf.FormBorderStyle.Sizable:
            case swf.FormBorderStyle.SizableToolWindow:
            case swf.FormBorderStyle.FixedDialog:
                return WindowStyle.Default;
			case swf.FormBorderStyle.None:
				return WindowStyle.None;
			default:
				throw new NotSupportedException ();
			}
		}

        public static swf.FormBorderStyle ToSWF(this WindowStyle style, bool resizable, swf.FormBorderStyle defaultStyle)
        {
            switch (style)
            {
                case WindowStyle.Default:
                    return resizable ? swf.FormBorderStyle.Sizable : defaultStyle;
                case WindowStyle.None:
                    return swf.FormBorderStyle.None;
                default:
                    throw new NotSupportedException();
            }
        }

		public static DrawableCellState ToEto(this swf.DataGridViewElementStates state)
		{
			if ( (state & swf.DataGridViewElementStates.Selected) == swf.DataGridViewElementStates.Selected)
				return DrawableCellState.Selected;

			return DrawableCellState.Normal;
		}		
	}
}
