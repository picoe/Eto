using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Windows.Drawing;
using sd = System.Drawing;
using sdp = System.Drawing.Printing;
using sd2 = System.Drawing.Drawing2D;
using swf = System.Windows.Forms;

namespace Eto.Platform.Windows
{
    public static partial class Conversions
    {
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
            return new Color(
                color.R / 255f,
                color.G / 255f,
                color.B / 255f,
                color.A / 255f);
        }

        public static sd.Color ToSD(this Color color)
        {
            return sd.Color.FromArgb(
                (byte)(color.A * 255),
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255));
        }

        public static DialogResult ToEto(this swf.DialogResult result)
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

        public static sd.Imaging.ImageFormat ToSD(this ImageFormat format)
        {
            switch (format)
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

        public static ImageInterpolation ToEto(this sd.Drawing2D.InterpolationMode value)
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
                    throw new NotSupportedException();
            }
        }

        public static sd.Drawing2D.InterpolationMode ToSD(this ImageInterpolation value)
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
                    throw new NotSupportedException();
            }
        }

        public static sd.FontStyle ToSD(this FontStyle style)
        {
            sd.FontStyle ret = sd.FontStyle.Regular;
			if ((style & FontStyle.Bold) != 0)
				ret |= sd.FontStyle.Bold;
			if ((style & FontStyle.Italic) != 0)
				ret |= sd.FontStyle.Italic;
            return ret;
        }

        public static sdp.PrintRange ToSDP(this PrintSelection value)
        {
            switch (value)
            {
                case PrintSelection.AllPages:
                    return sdp.PrintRange.AllPages;
                case PrintSelection.SelectedPages:
                    return sdp.PrintRange.SomePages;
                case PrintSelection.Selection:
                    return sdp.PrintRange.Selection;
                default:
                    throw new NotSupportedException();
            }
        }

        public static PrintSelection ToEto(this sdp.PrintRange value)
        {
            switch (value)
            {
                case sdp.PrintRange.AllPages:
                    return PrintSelection.AllPages;
                case sdp.PrintRange.SomePages:
                    return PrintSelection.SelectedPages;
                case sdp.PrintRange.Selection:
                    return PrintSelection.Selection;
                default:
                    throw new NotSupportedException();
            }
        }

        public static FontStyle ToEto(this sd.FontStyle style)
        {
            var ret = FontStyle.Normal;
			if (style.HasFlag (sd.FontStyle.Bold))
				ret |= FontStyle.Bold;
			if (style.HasFlag (sd.FontStyle.Italic))
				ret |= FontStyle.Italic;
            return ret;
		}

		public static PointF ToEto (this sd.PointF point)
		{
			return new PointF (point.X, point.Y);
		}

		public static sd.PointF ToSD (this PointF point)
		{
			return new sd.PointF (point.X, point.Y);
		}

        public static sd.Point ToSD(this Point point)
        {
            return new sd.Point(point.X, point.Y);
        }
       
        public static sd.Point ToSDPoint(this PointF point)
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

		public static RectangleF ToEto (this sd.RectangleF rect)
		{
			return new RectangleF (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.RectangleF ToSD (this RectangleF rect)
		{
			return new sd.RectangleF (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static sd.Rectangle ToSDRectangle (this RectangleF rect)
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

		public static sd.Drawing2D.GraphicsPath ToSD (this GraphicsPath graphicsPath)
		{
			var h = (GraphicsPathHandler)graphicsPath.Handler;
			return h.Control;
		}

		public static sd.Image ToSD (this Image graphics)
		{
			var h = (BitmapHandler)graphics.Handler;
			return h.Control;
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
			var modifiers = KeyMap.Convert (swf.Control.ModifierKeys);

			var result = new MouseEventArgs (buttons, modifiers, point);
            result.Delta = e.Delta;

			return result;
		}

		private static MouseButtons ToEto (this swf.MouseButtons button)
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

		public static sd.Drawing2D.Matrix ToSD (this IMatrix m)
		{
			return (sd.Drawing2D.Matrix)m.ControlObject;
		}

		public static float DegreesToRadians (float angle)
		{
			return (float)Math.PI * angle / 180.0f;
        }

        internal static DragDropEffects ToEto(this swf.DragDropEffects effects)
        {
            return (DragDropEffects)effects;
        }

        internal static swf.DragDropEffects ToSWF(this 
            DragDropEffects effects)
        {
            return (swf.DragDropEffects)effects;
        }

        internal static DragEventArgs ToEto(this 
            swf.DragEventArgs e)
        {
            var result =
                new DragEventArgs(
                    new DataObject(e.Data),
                    e.X,
                    e.Y,
                    ToEto(e.AllowedEffect),
                    ToEto(e.Effect));

            return result;
        }

        internal static GiveFeedbackEventArgs ToEto(
            this swf.GiveFeedbackEventArgs e)
        {
            return
                new GiveFeedbackEventArgs(
                    ToEto(e.Effect),
                    e.UseDefaultCursors);
        }

        internal static QueryContinueDragEventArgs ToEto(
            this swf.QueryContinueDragEventArgs e)
        {
            return
                new QueryContinueDragEventArgs(
                    e.KeyState,
                    e.EscapePressed,
                    ToEto(e.Action));
        }

        private static DragAction ToEto(this 
            swf.DragAction dragAction)
        {
            return (DragAction)dragAction;
        }

        public static Graphics ToEto(this sd.Graphics g)
        {
            return
                new Graphics(
                    new GraphicsHandler(
                        g));
        }

        public static PaintEventArgs ToEto(this 
            swf.PaintEventArgs e)
        {
            return
                new Eto.Forms.PaintEventArgs(
                    ToEto(e.Graphics),
                    e.ClipRectangle.ToEto());
        }

        public static ITreeItem ToEto(this swf.TreeNode treeNode)
        {
            return
                treeNode != null
                ? treeNode.Tag as ITreeItem
                : null;
        }

        public static TreeNodeMouseClickEventArgs ToEto(this 
            swf.TreeNodeMouseClickEventArgs e)
        {
            var mouseEventArgs =
                ToEto((swf.MouseEventArgs)e);

            return new TreeNodeMouseClickEventArgs(
                mouseEventArgs,
                ToEto(e.Node));
        }

        public static TreeViewItemEventArgs ToEto(this swf.TreeViewEventArgs e)
        {
            return
                new TreeViewItemEventArgs(
                    ToEto(e.Node))
                {
                    Action = (Eto.Forms.TreeViewAction)e.Action,
                };

        }

        public static TreeViewItemEventArgs ToEto(this swf.NodeLabelEditEventArgs e)
        {
            return
                new TreeViewItemEventArgs(
                    ToEto(e.Node))
                    {
                        CancelEdit = e.CancelEdit,
                        Label = e.Label
                    };

        }

        public static ItemDragEventArgs ToEto(this swf.ItemDragEventArgs e)
        {
            return new ItemDragEventArgs()
            {
                Buttons = ToEto(e.Button),
                Item = ToEto(e.Item as swf.TreeNode)
            };
        }

        public static sd.Drawing2D.Matrix ToSD(this Matrix m)
        {
            var h = (MatrixHandler)m.Handler;
            return h.Control;
        }
    }
}
