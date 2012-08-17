using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using System.IO;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;

namespace Eto.Platform.Wpf.Drawing
{
	public interface IWpfImage
	{
		swm.ImageSource GetIconClosestToSize (int width);
	}

	public class IconHandler : WidgetHandler<swmi.BitmapFrame, Icon>, IIcon, IWpfImage
	{
		swm.ImageSource[] icons;

		public static void CopyStream (Stream input, Stream output)
		{
			byte[] buffer = new byte[32768];
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

		public void Create(System.IO.Stream stream)
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
			get { return new Size(Control.PixelWidth, Control.PixelHeight); }
		}

		public swm.ImageSource GetLargestIcon ()
		{
			var curicon = icons[0];
			foreach (var icon in icons) {
				if (icon.Width > curicon.Width)
					curicon = icon;
			}
			return curicon;
		}

		public swm.ImageSource GetIconClosestToSize (int width)
		{
			var curicon = icons[0];
			if (curicon.Width == width)
				return curicon;
			foreach (var icon in icons) {
				if (icon.Width > width && icon.Width - width < curicon.Width - width)
					curicon = icon;
			}
			return curicon;
		}

		private const int sICONDIR = 6;            // sizeof(ICONDIR) 
		private const int sICONDIRENTRY = 16;      // sizeof(ICONDIRENTRY)

		public swm.ImageSource[] SplitIcon (swmi.BitmapFrame icon, MemoryStream input)
		{
			if (icon == null) {
				throw new ArgumentNullException ("icon");
			}

			// Get multiple .ico file image.
			byte[] srcBuf = input.ToArray();

			var splitIcons = new List<swm.ImageSource> ();
			int count = BitConverter.ToInt16 (srcBuf, 4); // ICONDIR.idCount

			for (int i = 0; i < count; i++) {
				using (MemoryStream destStream = new MemoryStream ())
				using (BinaryWriter writer = new BinaryWriter (destStream)) {
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
						throw new InvalidDataException ("Icon not a valid format");
					writer.Write (srcBuf, imgOffset, imgSize);
					writer.Flush ();

					// Create new icon.
					destStream.Seek (0, SeekOrigin.Begin);
					splitIcons.Add (swmi.BitmapFrame.Create (destStream));
				}
			}

			return splitIcons.ToArray ();
		}

		public swm.ImageSource GetImageWithSize (int? size)
		{
			if (size != null) {
				return GetIconClosestToSize (size.Value);
			}
			else return Control;
		}

	}
}
