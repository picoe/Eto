using System;
using System.Collections.Generic;
using System.Globalization;
using Eto.Drawing;
using System.IO;
using sw = System.Windows;
using swi = System.Windows.Interop;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;
using sd = System.Drawing;

namespace Eto.Wpf.Drawing
{
	public interface IWpfImage
	{
		swmi.BitmapSource GetImageClosestToSize (int? width);
	}

	public class IconHandler : WidgetHandler<swmi.BitmapFrame, Icon>, Icon.IHandler, IWpfImage
	{
		swmi.BitmapSource[] icons;

		public IconHandler ()
		{
		}

		public IconHandler (sd.Icon icon)
		{
			var rect = new sw.Int32Rect (0, 0, icon.Width, icon.Height);
			var img = swi.Imaging.CreateBitmapSourceFromHIcon (icon.Handle, rect, swmi.BitmapSizeOptions.FromEmptyOptions ());
			Control = swmi.BitmapFrame.Create (img);
		}

		public IconHandler (swmi.BitmapFrame control)
		{
			this.Control = control;
		}

		public static void CopyStream (Stream input, Stream output)
		{
			var buffer = new byte[32768];
			int read;
			while ((read = input.Read (buffer, 0, buffer.Length)) > 0) {
				output.Write (buffer, 0, read);
			}
		}

		public static MemoryStream CreateStream (Stream input)
		{
			var ms = new MemoryStream ();
			CopyStream (input, ms);
			ms.Position = 0;
			return ms;
		}

		public void Create(Stream stream)
		{
			var ms = CreateStream (stream);
			Control = swmi.BitmapFrame.Create (ms);
			ms.Position = 0;
			icons = SplitIcon (Control, ms);
		}

		public void Create(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				var ms = CreateStream (stream);
				Control = swmi.BitmapFrame.Create (ms);
				ms.Position = 0;
				icons = SplitIcon (Control, ms);
			}
		}

		public Size Size
		{
			get
			{
				var largest = GetLargestIcon ();
				return new Size ((int)largest.Width, (int)largest.Height);
			}
		}

		public swmi.BitmapSource GetLargestIcon ()
		{
			var curicon = icons[0];
			foreach (var icon in icons) {
				if (icon.Width > curicon.Width)
					curicon = icon;
			}
			return curicon;
		}

		public swmi.BitmapSource GetImageClosestToSize (int? width)
		{
			if (width == null)
				return GetLargestIcon ();
			var curicon = icons[0];
			if ((int)curicon.Width == width.Value)
				return curicon;
			foreach (var icon in icons) {
				if (icon.Width > width && icon.Width - width.Value < curicon.Width - width.Value)
					curicon = icon;
			}
			return GetLargestIcon ();
		}

		const int sICONDIR = 6;            // sizeof(ICONDIR) 
		const int sICONDIRENTRY = 16;      // sizeof(ICONDIRENTRY)

		public swmi.BitmapSource[] SplitIcon (swmi.BitmapFrame icon, MemoryStream input)
		{
			if (icon == null) {
				throw new ArgumentNullException ("icon");
			}

			// Get multiple .ico file image.
			byte[] srcBuf = input.ToArray();

			var splitIcons = new List<swmi.BitmapSource> ();
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
						throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, "Icon not a valid format"));
					writer.Write (srcBuf, imgOffset, imgSize);
					writer.Flush ();

					// Create new icon.
					destStream.Seek (0, SeekOrigin.Begin);
					splitIcons.Add (swmi.BitmapFrame.Create (destStream));
				}
			}

			return splitIcons.ToArray ();
		}

		public swmi.BitmapSource GetImageWithSize (int? size)
		{
			return size != null ? GetImageClosestToSize(size.Value) : Control;
		}

	}
}
