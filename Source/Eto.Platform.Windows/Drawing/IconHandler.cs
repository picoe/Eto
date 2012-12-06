using System;
using System.IO;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Drawing
{
	public class IconHandler : WidgetHandler<SD.Icon, Icon>, IIcon, IWindowsImage
	{
		SD.Icon[] icons;
		
		public IconHandler (SD.Icon control)
		{
			this.Control = control;
		}
		
		public IconHandler ()
		{
		}

		public Size Size {
			get { return new Size (Control.Width, Control.Height); }
		}

		#region IIcon Members

		public void Create (Stream stream)
		{
			Control = new SD.Icon (stream);
		}
		
		public void Create (string fileName)
		{
			Control = new SD.Icon (fileName);
		}

		#endregion
		

		public SD.Icon GetLargestIcon ()
		{
			var icons = SplitIcon (Control);
			var curicon = icons [0];
			foreach (var icon in icons) {
				if (icon.Width > curicon.Width)
					curicon = icon;
			}
			return curicon;
		}

		public SD.Icon GetIconClosestToSize (int width)
		{
			var icons = SplitIcon (Control);
			var curicon = icons [0];
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
		
		public SD.Icon[] SplitIcon (SD.Icon icon)
		{
			if (icons != null)
				return icons;
			if (icon == null) {
				throw new ArgumentNullException ("icon");
			}
			
			// Get multiple .ico file image.
			byte[] srcBuf = null;
			using (MemoryStream stream = new MemoryStream()) {
				icon.Save (stream);
				stream.Flush ();
				srcBuf = stream.ToArray ();
			}
			
			List<SD.Icon> splitIcons = new List<SD.Icon> ();
			int count = BitConverter.ToInt16 (srcBuf, 4); // ICONDIR.idCount
			
			for (int i = 0; i < count; i++) {
				using (MemoryStream destStream = new MemoryStream())
				using (BinaryWriter writer = new BinaryWriter(destStream)) {
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
						throw new Exception ("ugh");
					writer.Write (srcBuf, imgOffset, imgSize);
					writer.Flush ();
					
					// Create new icon.
					destStream.Seek (0, SeekOrigin.Begin);
					splitIcons.Add (new SD.Icon (destStream));
				}
			}
  
			icons = splitIcons.ToArray ();
			return icons;
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing) {
				if (icons != null) {
					foreach (var icon in icons) {
						icon.Dispose ();
					}
					icons = null;
				}
			}
		}

		public SD.Image GetImageWithSize (int? size)
		{
			if (size != null) {
				var icon = GetIconClosestToSize (size.Value);
				return icon.ToBitmap ();
			}
			else return this.GetLargestIcon().ToBitmap ();
		}


		public void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var image = GetImageWithSize((int)Math.Max(destination.Width, destination.Height));
			graphics.Control.DrawImage (image, source.ToSD (), destination.ToSD (), SD.GraphicsUnit.Pixel);
		}

		public void DrawImage (GraphicsHandler graphics, float x, float y)
		{
			graphics.Control.DrawIcon (Control, (int)x, (int)y);
		}

		public void DrawImage (GraphicsHandler graphics, float x, float y, float width, float height)
		{
			graphics.Control.DrawIcon (Control, new SD.Rectangle ((int)x, (int)y, (int)width, (int)height));
		}
	}
}
