using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using Eto.Platform.GtkSharp.Drawing;
using System.Threading;

namespace Eto.Platform.GtkSharp
{
	public class Generator : Eto.Generator
	{ 	
		public const string GeneratorID = "gtk";
		
		public override string ID {
			get {
				return GeneratorID;
			}
		}
		
		public Generator()
		{
			Gtk.Application.Init();
			
			Gdk.Threads.Enter();
		}
		
		
		public override void ExecuteOnMainThread (System.Action action)
		{
			if (Thread.CurrentThread.ManagedThreadId == ApplicationHandler.MainThreadID)
				action();
			else
				Gtk.Application.Invoke(delegate { action(); });
		}

		public static Gdk.Color Convert(Color color)
		{
			return new Gdk.Color(color.R, color.G, color.B);
		}

		public static Color Convert(Gdk.Color color)
		{
			return new Color((byte)color.Red, (byte)color.Green, (byte)color.Blue);
		}

		public static Gdk.Size Convert(Size size)
		{
			return new Gdk.Size(size.Width, size.Height);
		}

		public static Size Convert(Gdk.Size size)
		{
			return new Size(size.Width, size.Height);
		}

		public static Gdk.Point Convert(Point point)
		{
			return new Gdk.Point(point.X, point.Y);
		}

		public static Point Convert(Gdk.Point point)
		{
			return new Point(point.X, point.Y);
		}

		public static Gdk.Rectangle Convert(Rectangle rect)
		{
			return new Gdk.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Rectangle Convert(Gdk.Rectangle rect)
		{
			return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static DialogResult Convert(Gtk.ResponseType result)
		{
			DialogResult ret = DialogResult.None;
			if (result == Gtk.ResponseType.Ok) ret = DialogResult.Ok;
			else if (result == Gtk.ResponseType.Cancel) ret = DialogResult.Cancel;
			else if (result == Gtk.ResponseType.Yes) ret = DialogResult.Yes;
			else if (result == Gtk.ResponseType.No) ret = DialogResult.No;
			else if (result == Gtk.ResponseType.None) ret = DialogResult.None;
			else if (result == Gtk.ResponseType.Accept) ret = DialogResult.Ignore;
			else if (result == Gtk.ResponseType.Reject) ret = DialogResult.Abort;
			else ret = DialogResult.None;

			return ret;
		}

		public static string Convert(ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.Jpeg: return "jpeg";
				case ImageFormat.Bitmap: return "bmp";
				case ImageFormat.Gif: return "gif";
				case ImageFormat.Tiff: return "tiff";
				case ImageFormat.Png: return "png";
				default: throw new Exception("Invalid format specified");
			}
		}
	}
}
