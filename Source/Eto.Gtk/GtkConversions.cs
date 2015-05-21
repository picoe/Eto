using System;
using System.Globalization;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp
{
	public static class GtkConversions
	{
		public static Gdk.Color ToGdk(this Color color)
		{
			return new Gdk.Color((byte)(color.R * byte.MaxValue), (byte)(color.G * byte.MaxValue), (byte)(color.B * byte.MaxValue));
		}

		public static Cairo.Color ToCairo(this Color color)
		{
			return new Cairo.Color((double)color.R, (double)color.G, (double)color.B, (double)color.A);
		}

		#if GTK3
		public static Cairo.Color ToCairo(this Gdk.RGBA color)
		{
			return new Cairo.Color(color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static Gdk.RGBA ToRGBA(this Color color)
		{
			return new Gdk.RGBA { Red = color.R, Green = color.G, Blue = color.B, Alpha = color.A };
		}
#endif

		public static Color ToEto(this Cairo.Color color)
		{
			return new Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static Cairo.Rectangle ToCairo(this Rectangle rectangle)
		{
			return new Cairo.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Cairo.Rectangle ToCairo(this RectangleF rectangle)
		{
			return new Cairo.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Rectangle ToEto(this Cairo.Rectangle rectangle)
		{
			return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
		}

		public static Cairo.Filter ToCairo(this ImageInterpolation value)
		{
			switch (value)
			{
				case ImageInterpolation.Default:
					return Cairo.Filter.Bilinear;
				case ImageInterpolation.None:
					return Cairo.Filter.Nearest;
				case ImageInterpolation.High:
					return Cairo.Filter.Best;
				case ImageInterpolation.Low:
					return Cairo.Filter.Fast;
				case ImageInterpolation.Medium:
					return Cairo.Filter.Good;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gdk.InterpType ToGdk(this ImageInterpolation value)
		{

			switch (value)
			{
				case ImageInterpolation.Default:
					return Gdk.InterpType.Bilinear;
				case ImageInterpolation.None:
					return Gdk.InterpType.Nearest;
				case ImageInterpolation.High:
					return Gdk.InterpType.Hyper;
				case ImageInterpolation.Low:
					return Gdk.InterpType.Tiles;
				case ImageInterpolation.Medium:
					return Gdk.InterpType.Bilinear;
				default:
					throw new NotSupportedException();
			}
		}

		public static Color ToEto(this Gdk.Color color)
		{
			return new Color((float)color.Red / ushort.MaxValue, (float)color.Green / ushort.MaxValue, (float)color.Blue / ushort.MaxValue);
		}

		public static Gdk.Size ToGdk(this Size size)
		{
			return new Gdk.Size(size.Width, size.Height);
		}

		public static Size ToEto(this Gdk.Size size)
		{
			return new Size(size.Width, size.Height);
		}

		public static Size ToEto(this Gtk.Requisition req)
		{
			return new Size(req.Width, req.Height);
		}

		public static Gdk.Point ToGdk(this Point point)
		{
			return new Gdk.Point(point.X, point.Y);
		}

		public static Point ToEto(this Gdk.Point point)
		{
			return new Point(point.X, point.Y);
		}

		public static Gdk.Rectangle ToGdk(this Rectangle rect)
		{
			return new Gdk.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Rectangle ToEto(this Gdk.Rectangle rect)
		{
			return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static DialogResult ToEto(this Gtk.ResponseType result)
		{
			switch (result)
			{
				case Gtk.ResponseType.None:
					return DialogResult.None;
				case Gtk.ResponseType.Reject:
					return DialogResult.Abort;
				case Gtk.ResponseType.Accept:
					return DialogResult.Ignore;
				case Gtk.ResponseType.Ok:
					return DialogResult.Ok;
				case Gtk.ResponseType.Cancel:
					return DialogResult.Cancel;
				case Gtk.ResponseType.Yes:
					return DialogResult.Yes;
				case Gtk.ResponseType.No:
					return DialogResult.No;
				default:
					return DialogResult.None;
			}
		}

		public static string ToGdk(this ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.Jpeg:
					return "jpeg";
				case ImageFormat.Bitmap:
					return "bmp";
				case ImageFormat.Gif:
					return "gif";
				case ImageFormat.Tiff:
					return "tiff";
				case ImageFormat.Png:
					return "png";
				default:
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid format specified"));
			}
		}

		public static Gdk.CursorType ToGdk(this CursorType cursor)
		{
			switch (cursor)
			{
				case CursorType.Arrow:
					return Gdk.CursorType.Arrow;
				case CursorType.Crosshair:
					return Gdk.CursorType.Crosshair;
				case CursorType.Default:
					return Gdk.CursorType.Arrow;
				case CursorType.HorizontalSplit:
					return Gdk.CursorType.SbHDoubleArrow;
				case CursorType.VerticalSplit:
					return Gdk.CursorType.SbVDoubleArrow;
				case CursorType.IBeam:
					return Gdk.CursorType.Xterm;
				case CursorType.Move:
					return Gdk.CursorType.Fleur;
				case CursorType.Pointer:
					return Gdk.CursorType.Hand2;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gtk.ButtonsType ToGtk(this MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				default:
					return Gtk.ButtonsType.Ok;
				case MessageBoxButtons.OKCancel:
					return Gtk.ButtonsType.OkCancel;
				case MessageBoxButtons.YesNo:
					return Gtk.ButtonsType.YesNo;
				case MessageBoxButtons.YesNoCancel:
					return Gtk.ButtonsType.YesNo;
			}
		}

		public static Gtk.ResponseType ToGtk(this MessageBoxDefaultButton button, MessageBoxButtons buttons)
		{
			switch (button)
			{
				case MessageBoxDefaultButton.OK:
					if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
						return Gtk.ResponseType.Yes;
					return Gtk.ResponseType.Ok;
				case MessageBoxDefaultButton.No:
					return Gtk.ResponseType.No;
				case MessageBoxDefaultButton.Cancel:
					return Gtk.ResponseType.Cancel;
				case MessageBoxDefaultButton.Default:
					switch (buttons)
					{
						case MessageBoxButtons.OK:
							return Gtk.ResponseType.Ok;
						case MessageBoxButtons.OKCancel:
						case MessageBoxButtons.YesNoCancel:
							return Gtk.ResponseType.Cancel;
						case MessageBoxButtons.YesNo:
							return Gtk.ResponseType.No;
						default:
							throw new NotSupportedException();
					}
				default:
					throw new NotSupportedException();
			}
		}

		public static Gtk.MessageType ToGtk(this MessageBoxType type)
		{
			switch (type)
			{
				default:
					return Gtk.MessageType.Info;
				case MessageBoxType.Error:
					return Gtk.MessageType.Error;
				case MessageBoxType.Warning:
					return Gtk.MessageType.Warning;
				case MessageBoxType.Question:
					return Gtk.MessageType.Question;
			}
		}

		public static Gtk.PageOrientation ToGtk(this PageOrientation value)
		{
			switch (value)
			{
				case PageOrientation.Landscape:
					return Gtk.PageOrientation.Landscape;
				case PageOrientation.Portrait:
					return Gtk.PageOrientation.Portrait;
				default:
					throw new NotSupportedException();
			}
		}

		public static PageOrientation ToEto(this Gtk.PageOrientation value)
		{
			switch (value)
			{
				case Gtk.PageOrientation.Landscape:
					return PageOrientation.Landscape;
				case Gtk.PageOrientation.Portrait:
					return PageOrientation.Portrait;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gtk.PageRange ToGtkPageRange(this Range<int> range)
		{
			return new Gtk.PageRange { Start = range.Start - 1, End = range.End - 1 };
		}

		public static Range<int> ToEto(this Gtk.PageRange range)
		{
			return new Range<int>(range.Start + 1, range.End);
		}

		public static Gtk.PrintPages ToGtk(this PrintSelection value)
		{
			switch (value)
			{
				case PrintSelection.AllPages:
					return Gtk.PrintPages.All;
				case PrintSelection.SelectedPages:
					return Gtk.PrintPages.Ranges;
				default:
					throw new NotSupportedException();
			}
		}

		public static PrintSelection ToEto(this Gtk.PrintPages value)
		{
			switch (value)
			{
				case Gtk.PrintPages.All:
					return PrintSelection.AllPages;
				case Gtk.PrintPages.Ranges:
					return PrintSelection.SelectedPages;
				default:
					throw new NotSupportedException();
			}
		}

		public static void Apply(this Pen pen, GraphicsHandler graphics)
		{
			((PenHandler)pen.Handler).Apply(pen, graphics);
		}

		public static void Apply(this Brush brush, GraphicsHandler graphics)
		{
			((BrushHandler)brush.Handler).Apply(brush.ControlObject, graphics);
		}

		public static Cairo.LineJoin ToCairo(this PenLineJoin value)
		{
			switch (value)
			{
				case PenLineJoin.Miter:
					return Cairo.LineJoin.Miter;
				case PenLineJoin.Bevel:
					return Cairo.LineJoin.Bevel;
				case PenLineJoin.Round:
					return Cairo.LineJoin.Round;
				default:
					throw new NotSupportedException();
			}
		}

		public static PenLineJoin ToEto(this Cairo.LineJoin value)
		{
			switch (value)
			{
				case Cairo.LineJoin.Bevel:
					return PenLineJoin.Bevel;
				case Cairo.LineJoin.Miter:
					return PenLineJoin.Miter;
				case Cairo.LineJoin.Round:
					return PenLineJoin.Round;
				default:
					throw new NotSupportedException();
			}
		}

		public static Cairo.LineCap ToCairo(this PenLineCap value)
		{
			switch (value)
			{
				case PenLineCap.Butt:
					return Cairo.LineCap.Butt;
				case PenLineCap.Round:
					return Cairo.LineCap.Round;
				case PenLineCap.Square:
					return Cairo.LineCap.Square;
				default:
					throw new NotSupportedException();
			}
		}

		public static PenLineCap ToEto(this Cairo.LineCap value)
		{
			switch (value)
			{
				case Cairo.LineCap.Butt:
					return PenLineCap.Butt;
				case Cairo.LineCap.Round:
					return PenLineCap.Round;
				case Cairo.LineCap.Square:
					return PenLineCap.Square;
				default:
					throw new NotSupportedException();
			}
		}

		public static Cairo.PointD ToCairo(this PointF point)
		{
			return new Cairo.PointD(point.X, point.Y);
		}

		public static PointF ToEto(this Cairo.PointD point)
		{
			return new PointF((float)point.X, (float)point.Y);
		}

		public static GraphicsPathHandler ToHandler(this IGraphicsPath path)
		{
			return ((GraphicsPathHandler)path.ControlObject);
		}

		public static void Apply(this IGraphicsPath path, Cairo.Context context)
		{
			((GraphicsPathHandler)path.ControlObject).Apply(context);
		}

		public static Cairo.Matrix ToCairo(this IMatrix matrix)
		{
			return (Cairo.Matrix)matrix.ControlObject;
		}

		public static IMatrix ToEto(this Cairo.Matrix matrix)
		{
			return new MatrixHandler(matrix);
		}

		public static Gdk.Pixbuf ToGdk(this Image image)
		{
			var handler = image.Handler as IGtkPixbuf;
			return handler != null ? handler.Pixbuf : null;
		}

		public static void SetCairoSurface(this Image image, Cairo.Context context, float x, float y)
		{
			Gdk.CairoHelper.SetSourcePixbuf(context, image.ToGdk(), x, y);
		}

		public static GradientWrapMode ToEto(this Cairo.Extend extend)
		{
			switch (extend)
			{
				case Cairo.Extend.Reflect:
					return GradientWrapMode.Reflect;
				case Cairo.Extend.Repeat:
					return GradientWrapMode.Repeat;
				case Cairo.Extend.Pad:
					return GradientWrapMode.Pad;
				default:
					throw new NotSupportedException();
			}
		}

		public static Cairo.Extend ToCairo(this GradientWrapMode wrap)
		{
			switch (wrap)
			{
				case GradientWrapMode.Reflect:
					return Cairo.Extend.Reflect;
				case GradientWrapMode.Repeat:
					return Cairo.Extend.Repeat;
				case GradientWrapMode.Pad:
					return Cairo.Extend.Pad;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gtk.Image ToGtk(this Image image, Gtk.IconSize? size = null)
		{
			if (image == null)
				return null;
			var handler = (IImageHandler)image.Handler;
			var gtkimage = new Gtk.Image();
			handler.SetImage(gtkimage, size);
			return gtkimage;
		}

		public static void SetGtkImage(this Image image, Gtk.Image gtkimage, Gtk.IconSize? size = null)
		{
			if (image == null)
				return;
			var handler = (IImageHandler)image.Handler;
			handler.SetImage(gtkimage, size);
		}

		public static Cairo.FillRule ToCairo(this FillMode value)
		{
			switch (value)
			{
				case FillMode.Alternate:
					return Cairo.FillRule.EvenOdd;
				case FillMode.Winding:
					return Cairo.FillRule.Winding;
				default:
					throw new NotSupportedException();
			}
		}

		public static KeyEventArgs ToEto(this Gdk.EventKey args)
		{
			Keys key = args.Key.ToEto() | args.State.ToEtoKey();

			if (key != Keys.None)
			{
				Keys modifiers = (key & Keys.ModifierMask);
				if (args.KeyValue <= 128 && ((modifiers & ~Keys.Shift) == 0))
					return new KeyEventArgs(key, KeyEventType.KeyDown, (char)args.KeyValue);
				return new KeyEventArgs(key, KeyEventType.KeyDown);
			}
			return args.KeyValue <= 128 ? new KeyEventArgs(key, KeyEventType.KeyDown, (char)args.KeyValue) : null;
		}

		public static MouseButtons ToEtoMouseButtons(this Gdk.ModifierType modifiers)
		{
			MouseButtons buttons = MouseButtons.None;
			if (modifiers.HasFlag(Gdk.ModifierType.Button1Mask))
				buttons |= MouseButtons.Primary;
			if (modifiers.HasFlag(Gdk.ModifierType.Button2Mask))
				buttons |= MouseButtons.Middle;
			if (modifiers.HasFlag(Gdk.ModifierType.Button3Mask))
				buttons |= MouseButtons.Alternate;
			return buttons;
		}

		public static MouseButtons ToEtoMouseButtons(this Gdk.EventButton ev)
		{
			switch (ev.Button)
			{
				case 1:
					return MouseButtons.Primary;
				case 2:
					return MouseButtons.Middle;
				case 3:
					return MouseButtons.Alternate;
				default:
					return MouseButtons.None;
			}
		}

		public static DrawableCellStates ToEto(this Gtk.CellRendererState value)
		{
			if (value.HasFlag(Gtk.CellRendererState.Selected))
				return DrawableCellStates.Selected;
			return DrawableCellStates.None;
		}

		public static TextAlignment ToEto(this Gtk.Justification justification)
		{
			switch (justification)
			{
				case Gtk.Justification.Left:
					return TextAlignment.Left;
				case Gtk.Justification.Right:
					return TextAlignment.Right;
				case Gtk.Justification.Center:
					return TextAlignment.Center;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gtk.Justification ToGtk(this TextAlignment align)
		{
			switch (align)
			{
				case TextAlignment.Left:
					return Gtk.Justification.Left;
				case TextAlignment.Center:
					return Gtk.Justification.Center;
				case TextAlignment.Right:
					return Gtk.Justification.Right;
				default:
					throw new NotSupportedException();
			}
		}

		public static Pango.FontDescription ToPango(this Font font)
		{
			return font == null ? null : ((FontHandler)font.Handler).Control;
		}

		public static Pango.FontFamily ToPango(this FontFamily family)
		{
			if (family == null)
				return null;
			return ((FontFamilyHandler)family.Handler).Control;
		}

		public static Font ToEto(this Pango.FontDescription fontDesc, string familyName = null)
		{
			return fontDesc == null ? null : new Font(new FontHandler(fontDesc, familyName));
		}
	}
}
