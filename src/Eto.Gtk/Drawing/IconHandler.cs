using System;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.GtkSharp.Drawing
{

	public class IconHandler : ImageHandler<Gtk.IconSet, Icon>, Icon.IHandler, IGtkPixbuf
	{
		readonly Dictionary<Size, Gdk.Pixbuf> sizes = new Dictionary<Size, Gdk.Pixbuf>();

		public Gdk.Pixbuf Pixbuf { get; set; }

		#region IIcon Members

		public void Create(Stream stream)
		{
			using (var ms = new MemoryStream())
			{
				stream.CopyTo(ms);
				ms.Position = 0;
				Pixbuf = new Gdk.Pixbuf(ms);
				Control = new Gtk.IconSet(Pixbuf);
				ms.Position = 0;
				CreateFrames(ms);
			}
		}

		public void Create(string fileName)
		{
			using (var fs = File.OpenRead(fileName))
			using (var ms = new MemoryStream())
			{
				fs.CopyTo(ms);
				ms.Position = 0;
				Pixbuf = new Gdk.Pixbuf(ms);
				Control = new Gtk.IconSet(Pixbuf);
				ms.Position = 0;
				CreateFrames(ms);
			}
		}

		const int sICONDIR = 6;            // sizeof(ICONDIR) 
		const int sICONDIRENTRY = 16;      // sizeof(ICONDIRENTRY)

		public Gdk.Pixbuf[] SplitIcon(MemoryStream input)
		{
			// Get multiple .ico file image.
			byte[] srcBuf = input.ToArray();

			var splitIcons = new List<Gdk.Pixbuf> ();
			int count = BitConverter.ToInt16 (srcBuf, 4); // ICONDIR.idCount

			for (int i = 0; i < count; i++) {
				using (var destStream = new MemoryStream ())
				using (var writer = new BinaryWriter (destStream)) {
					// Copy ICONDIR and ICONDIRENTRY.
					int pos = 0;
					writer.Write (srcBuf, pos, sICONDIR - 2);
					writer.Write ((short)1);    // ICONDIR.idCount == 1;

					pos += sICONDIR;
					pos += sICONDIRENTRY * i;

					writer.Write (srcBuf, pos, sICONDIRENTRY - 4); // write out icon info (minus old offset)
					writer.Write (sICONDIR + sICONDIRENTRY);    // write offset of icon data
					pos += 8;

					// Copy picture and mask data.
					int imgSize = BitConverter.ToInt32 (srcBuf, pos);       // ICONDIRENTRY.dwBytesInRes
					pos += 4;
					int imgOffset = BitConverter.ToInt32 (srcBuf, pos);    // ICONDIRENTRY.dwImageOffset
					if (imgOffset + imgSize > srcBuf.Length)
						throw new InvalidDataException("Icon not a valid format");
					writer.Write (srcBuf, imgOffset, imgSize);
					writer.Flush ();

					// Create new icon.
					destStream.Seek (0, SeekOrigin.Begin);
					splitIcons.Add (new Gdk.Pixbuf(destStream));
				}
			}

			return splitIcons.ToArray ();
		}


		void CreateFrames(MemoryStream input)
		{
			frames = new List<IconFrame>();
			foreach (var pb in SplitIcon(input))
			{
				frames.Add(IconFrame.FromControlObject(1, new Bitmap(new BitmapHandler(pb))));
			}
		}

		#endregion

		public override Size Size
		{
			get
			{
				return new Size(Pixbuf.Width, Pixbuf.Height);
			}
		}

		public override void SetImage(Gtk.Image imageView, Gtk.IconSize? iconSize)
		{
			if (iconSize != null)
				imageView.SetFromIconSet(Control, iconSize.Value);
			else
				imageView.Pixbuf = Pixbuf;
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var context = graphics.Control;
			context.Save();
			context.Rectangle(destination.ToCairo());
			double scalex = 1;
			double scaley = 1;
			var pb = Widget.GetFrame(1, Size.Ceiling(destination.Size));
			if (Math.Abs(source.Width - destination.Width) > 0.5f || Math.Abs(source.Height - destination.Height) > 0.5f)
			{
				scalex = (double)destination.Width / (double)source.Width;
				scaley = (double)destination.Height / (double)source.Height;
				scalex *= (double)Size.Width / (double)pb.PixelSize.Width;
				scaley *= (double)Size.Height / (double)pb.PixelSize.Height;
				context.Scale(scalex, scaley);
			}
			Gdk.CairoHelper.SetSourcePixbuf(context, pb.Bitmap.ToGdk(), (destination.Left / scalex) - source.Left, (destination.Top / scaley) - source.Top);
			var pattern = (Cairo.SurfacePattern)context.GetSource();
			pattern.Filter = graphics.ImageInterpolation.ToCairo();
			context.Fill();
			context.Restore();
			pattern.Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (Pixbuf != null)
				{
					Pixbuf.Dispose();
					Pixbuf = null;
				}
			}
		}

		public Gdk.Pixbuf GetPixbuf(Size maxSize, Gdk.InterpType interpolation = Gdk.InterpType.Bilinear, bool shrink = false)
		{
			Gdk.Pixbuf pixbuf = Pixbuf;
			if (pixbuf.Width > maxSize.Width && pixbuf.Height > maxSize.Height
				|| (shrink && (pixbuf.Width < maxSize.Width || pixbuf.Height < maxSize.Height)))
			{
				if (!sizes.TryGetValue(maxSize, out pixbuf))
				{
					pixbuf = Pixbuf.ScaleSimple(maxSize.Width, maxSize.Height, interpolation);
					sizes[maxSize] = pixbuf;
				}
			}
			
			return pixbuf;
		}

		List<IconFrame> frames;
		public void Create(IEnumerable<IconFrame> frames)
		{
			this.frames = new List<IconFrame>(frames);
			Control = new Gtk.IconSet();
			foreach (var frame in this.frames)
			{
				Control.AddSource(new Gtk.IconSource { Pixbuf = frame.Bitmap.ToGdk() });
			}
			Pixbuf = this.frames.FirstOrDefault(r => r.Scale == 1)?.Bitmap.ToGdk();
			if (Pixbuf == null)
			{
				var frame = this.frames.OrderBy(r => r.Scale).Last();
				Pixbuf = frame.Bitmap.ToGdk();

				Pixbuf = Widget.GetFrame(1).Bitmap.ToGdk().ScaleSimple(frame.Size.Width, frame.Size.Height, Gdk.InterpType.Bilinear);
			}
		}

		public IEnumerable<IconFrame> Frames
		{
			get
			{
				return frames;
			}
		}
	}
}
