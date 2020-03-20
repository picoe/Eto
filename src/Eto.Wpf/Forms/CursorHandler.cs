using System;
using System.IO;
using Eto.Drawing;
using Eto.Forms;
using sw = System.Windows;
using swi = System.Windows.Input;

namespace Eto.Wpf.Forms
{
	public class CursorHandler : WidgetHandler<swi.Cursor, Cursor>, Cursor.IHandler
	{
		public void Create (CursorType cursor)
		{
			switch (cursor) {
				case CursorType.Arrow:
					Control = swi.Cursors.Arrow;
					break;
				case CursorType.Crosshair:
					Control = swi.Cursors.Cross;
					break;
				case CursorType.Default:
					Control = swi.Cursors.Arrow;
					break;
				case CursorType.HorizontalSplit:
					Control = swi.Cursors.SizeNS;
					break;
				case CursorType.IBeam:
					Control = swi.Cursors.IBeam;
					break;
				case CursorType.Move:
					Control = swi.Cursors.SizeAll;
					break;
				case CursorType.Pointer:
					Control = swi.Cursors.Hand;
					break;
				case CursorType.VerticalSplit:
					Control = swi.Cursors.SizeWE;
					break;
				default:
					throw new NotSupportedException ();
			}
		}

		public void Create(Image image, PointF hotspot)
		{
			if (image is Bitmap bitmap)
				CreateBitmap(bitmap, hotspot);
			else if (image is Icon icon)
			{
				var frame = icon.GetFrame(Screen.PrimaryScreen.LogicalPixelSize);
				CreateBitmap(frame.Bitmap, hotspot * frame.Scale);
			}
		}

		void CreateBitmap(Bitmap image, PointF hotspot)
		{
			using (var pngStream = new MemoryStream())
			{
				image.Save(pngStream, ImageFormat.Png);

				using (var cursorStream = new MemoryStream())
				{
					cursorStream.Write(new byte[2] { 0x00, 0x00 }, 0, 2);                 // ICONDIR: Reserved. Must always be 0.
					cursorStream.Write(new byte[2] { 0x02, 0x00 }, 0, 2);                 // ICONDIR: Specifies image type: 1 for icon (.ICO) image, 2 for cursor (.CUR) image. Other values are invalid
					cursorStream.Write(new byte[2] { 0x01, 0x00 }, 0, 2);                 // ICONDIR: Specifies number of images in the file.
					cursorStream.Write(new byte[1] { (byte)image.Width }, 0, 1);          // ICONDIRENTRY: Specifies image width in pixels. Can be any number between 0 and 255. Value 0 means image width is 256 pixels.
					cursorStream.Write(new byte[1] { (byte)image.Height }, 0, 1);         // ICONDIRENTRY: Specifies image height in pixels. Can be any number between 0 and 255. Value 0 means image height is 256 pixels.
					cursorStream.Write(new byte[1] { 0x00 }, 0, 1);                       // ICONDIRENTRY: Specifies number of colors in the color palette. Should be 0 if the image does not use a color palette.
					cursorStream.Write(new byte[1] { 0x00 }, 0, 1);                       // ICONDIRENTRY: Reserved. Should be 0.
					cursorStream.Write(new byte[2] { (byte)hotspot.X, 0x00 }, 0, 2);      // ICONDIRENTRY: Specifies the horizontal coordinates of the hotspot in number of pixels from the left.
					cursorStream.Write(new byte[2] { (byte)hotspot.Y, 0x00 }, 0, 2);      // ICONDIRENTRY: Specifies the vertical coordinates of the hotspot in number of pixels from the top.
					cursorStream.Write(new byte[4] {                                      // ICONDIRENTRY: Specifies the size of the image's data in bytes
                                          (byte)((pngStream.Length & 0x000000FF)),
										  (byte)((pngStream.Length & 0x0000FF00) >> 0x08),
										  (byte)((pngStream.Length & 0x00FF0000) >> 0x10),
										  (byte)((pngStream.Length & 0xFF000000) >> 0x18)
									   }, 0, 4);
					cursorStream.Write(new byte[4] {                                      // ICONDIRENTRY: Specifies the offset of BMP or PNG data from the beginning of the ICO/CUR file
                                          (byte)0x16,
										  (byte)0x00,
										  (byte)0x00,
										  (byte)0x00,
									   }, 0, 4);

					// copy PNG stream to cursor stream
					pngStream.Seek(0, SeekOrigin.Begin);
					pngStream.CopyTo(cursorStream);
					// return cursor stream
					cursorStream.Seek(0, SeekOrigin.Begin);
					Control = new swi.Cursor(cursorStream);
				}
			}
		}

		public void Create(string fileName)
		{
			Control = new swi.Cursor(fileName);
		}

		public void Create(Stream stream)
		{
			Control = new swi.Cursor(stream);
		}
	}
}

