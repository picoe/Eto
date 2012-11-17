using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public static class Conversions
	{
		public static Gdk.Color ToGdk (this Color color)
		{
			return new Gdk.Color ((byte)(color.R * byte.MaxValue), (byte)(color.G * byte.MaxValue), (byte)(color.B * byte.MaxValue));
		}
		
		public static Cairo.Color ToCairo (this Color color)
		{
			return new Cairo.Color ((double)color.R, (double)color.G, (double)color.B, (double)color.A);
		}
		
		public static Cairo.Rectangle ToCairo (this Rectangle rectangle)
		{
			return new Cairo.Rectangle (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
		
		public static Rectangle ToEto (this Cairo.Rectangle rectangle)
		{
			return new Rectangle ((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
		}
		
		public static Cairo.Filter ToCairo (this ImageInterpolation value)
		{
			switch (value) {
			case ImageInterpolation.Default:
				return  Cairo.Filter.Bilinear;
			case ImageInterpolation.None:
				return Cairo.Filter.Nearest;
			case ImageInterpolation.High:
				return  Cairo.Filter.Best;
			case ImageInterpolation.Low:
				return  Cairo.Filter.Fast;
			case ImageInterpolation.Medium:
				return  Cairo.Filter.Good;
			default:
				throw new NotSupportedException();
			}
		}
		
		public static Color ToEto (this Gdk.Color color)
		{
			return new Color ((float)color.Red / ushort.MaxValue, (float)color.Green / ushort.MaxValue, (float)color.Blue / ushort.MaxValue);
		}
		
		public static Gdk.Size ToGdk (this Size size)
		{
			return new Gdk.Size (size.Width, size.Height);
		}
		
		public static Size ToEto (this Gdk.Size size)
		{
			return new Size (size.Width, size.Height);
		}
		
		public static Size ToEto (this Gtk.Requisition req)
		{
			return new Size (req.Width, req.Height);
		}
		
		public static Gdk.Point ToGdk (this Point point)
		{
			return new Gdk.Point (point.X, point.Y);
		}
		
		public static Point ToEto (this Gdk.Point point)
		{
			return new Point (point.X, point.Y);
		}
		
		public static Gdk.Rectangle ToGdk (this Rectangle rect)
		{
			return new Gdk.Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public static Rectangle ToEto (this Gdk.Rectangle rect)
		{
			return new Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public static DialogResult ToEto (this Gtk.ResponseType result)
		{
			DialogResult ret = DialogResult.None;
			if (result == Gtk.ResponseType.Ok)
				ret = DialogResult.Ok;
			else if (result == Gtk.ResponseType.Cancel)
				ret = DialogResult.Cancel;
			else if (result == Gtk.ResponseType.Yes)
				ret = DialogResult.Yes;
			else if (result == Gtk.ResponseType.No)
				ret = DialogResult.No;
			else if (result == Gtk.ResponseType.None)
				ret = DialogResult.None;
			else if (result == Gtk.ResponseType.Accept)
				ret = DialogResult.Ignore;
			else if (result == Gtk.ResponseType.Reject)
				ret = DialogResult.Abort;
			else
				ret = DialogResult.None;
			
			return ret;
		}
		
		public static string ToGdk (this ImageFormat format)
		{
			switch (format) {
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
				throw new Exception ("Invalid format specified");
			}
		}
		
		public static Gdk.CursorType ToGdk (this CursorType cursor)
		{
			switch (cursor) {
			case CursorType.Arrow: return Gdk.CursorType.Arrow;
			case CursorType.Crosshair: return Gdk.CursorType.Crosshair;
			case CursorType.Default: return Gdk.CursorType.Arrow;
			case CursorType.HorizontalSplit: return Gdk.CursorType.SbHDoubleArrow;
			case CursorType.VerticalSplit: return Gdk.CursorType.SbVDoubleArrow;
			case CursorType.IBeam: return Gdk.CursorType.Xterm;
			case CursorType.Move: return Gdk.CursorType.Fleur;
			case CursorType.Pointer: return Gdk.CursorType.Hand2;
			default:
				throw new NotSupportedException();
			}
		}

		public static Gtk.ButtonsType ToGtk (this MessageBoxButtons buttons)
		{
			switch (buttons) {
			default:
			case MessageBoxButtons.OK:
				return Gtk.ButtonsType.Ok;
			case MessageBoxButtons.OKCancel:
				return Gtk.ButtonsType.OkCancel;
			case MessageBoxButtons.YesNo:
				return Gtk.ButtonsType.YesNo;
			case MessageBoxButtons.YesNoCancel:
				return Gtk.ButtonsType.YesNo;
			}
		}
		
		public static Gtk.MessageType ToGtk (this MessageBoxType type)
		{
			switch (type) {
			default:
			case MessageBoxType.Information:
				return Gtk.MessageType.Info;
			case MessageBoxType.Error:
				return Gtk.MessageType.Error;
			case MessageBoxType.Warning:
				return Gtk.MessageType.Warning;
			case MessageBoxType.Question:
				return Gtk.MessageType.Question;
			}
		}

		public static Gtk.PageOrientation ToGtk (this PageOrientation value)
		{
			switch (value) {
			case PageOrientation.Landscape:
				return Gtk.PageOrientation.Landscape;
			case PageOrientation.Portrait:
				return Gtk.PageOrientation.Portrait;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PageOrientation ToEto (this Gtk.PageOrientation value)
		{
			switch (value) {
			case Gtk.PageOrientation.Landscape:
				return PageOrientation.Landscape;
			case Gtk.PageOrientation.Portrait:
				return PageOrientation.Portrait;
			default:
				throw new NotSupportedException ();
			}
		}

		public static Gtk.PageRange ToGtkPageRange (this Range range)
		{
			return new Gtk.PageRange { Start = range.Start - 1, End = range.End - 1 };
		}

		public static Range ToEto (this Gtk.PageRange range)
		{
			return new Range (range.Start + 1, range.End - range.Start + 1);
		}

		public static Gtk.PrintPages ToGtk (this PrintSelection value)
		{
			switch (value) {
			case PrintSelection.AllPages:
				return Gtk.PrintPages.All;
			case PrintSelection.SelectedPages:
				return Gtk.PrintPages.Ranges;
			default:
				throw new NotSupportedException ();
			}
		}

		public static PrintSelection ToEto (this Gtk.PrintPages value)
		{
			switch (value) {
			case Gtk.PrintPages.All:
				return PrintSelection.AllPages;
			case Gtk.PrintPages.Ranges:
				return PrintSelection.SelectedPages;
			default:
				throw new NotSupportedException ();
			}
		}
	}
}