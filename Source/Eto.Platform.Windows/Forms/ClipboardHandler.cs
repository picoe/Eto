using System;
using Eto.Forms;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using sdi = System.Drawing.Imaging;
using Eto.Platform.Windows.Drawing;
using Eto.Drawing;
using System.Runtime.InteropServices;

namespace Eto.Platform.Windows.Forms
{
	public class ClipboardHandler : WidgetHandler<swf.DataObject, Clipboard>, IClipboard
	{
		public ClipboardHandler ()
		{
			Control = new swf.DataObject();
		}
		
		void Update()
		{
			swf.Clipboard.SetDataObject (Control);
		}

		public void SetData (byte[] value, string type)
		{
			Control.SetData (type, value);
			Update();
		}
		
		public void SetString (string value, string type)
		{
			Control.SetData (type, value);
			Update();
		}
		
		public string Html
		{
			set {
				Control.SetText (value, System.Windows.Forms.TextDataFormat.Html);
				Update();
			}
			get {
				return swf.Clipboard.GetText (swf.TextDataFormat.Html);
			}
		}
		
		public string Text
		{
			set {
				Control.SetText(value);
				Update();
			}
			get {
				return swf.Clipboard.GetText ();				
			}
		}
		
		public Image Image
		{
			set
			{
				var sdimage = value.ControlObject as sd.Image;
				if (sdimage != null) {
					Control.SetImage (sdimage);
					Update ();
				}
			}
			get
			{
				Image result = null;
				
				try {
					var sdimage = GetImageFromClipboard () as sd.Bitmap;
					
					if (sdimage != null) {
						var handler = new BitmapHandler (sdimage);
						
						result = new Bitmap (Widget.Generator, handler);
					}
				} catch {
				}
				
				return result;
			}
		}

		/// <summary>
		/// see http://stackoverflow.com/questions/11273669/how-to-paste-a-transparent-image-from-the-clipboard-in-a-c-sharp-winforms-app
		/// </summary>
		sd.Image GetImageFromClipboard ()
		{
			if (swf.Clipboard.GetDataObject () == null)
				return null;

			if (swf.Clipboard.GetDataObject ().GetDataPresent (swf.DataFormats.Dib)) {
				var dib = ((System.IO.MemoryStream)swf.Clipboard.GetData (swf.DataFormats.Dib)).ToArray ();

				var width = BitConverter.ToInt32 (dib, 4);
				var height = BitConverter.ToInt32 (dib, 8);
				var bpp = BitConverter.ToInt16 (dib, 14);

				if (bpp == 32) {
					var gch = GCHandle.Alloc (dib, GCHandleType.Pinned);

					sd.Bitmap bmp = null;

					try {
						var ptr = new IntPtr ((long)gch.AddrOfPinnedObject () + 40);

						bmp = new sd.Bitmap (width, height, width * 4, sdi.PixelFormat.Format32bppArgb, ptr);

						var result = new sd.Bitmap (bmp);

						// Images are rotated and flipped for some reason.
						// This rotates them back.
						result.RotateFlip (sd.RotateFlipType.Rotate180FlipX);

						return result;
					} finally {
						gch.Free ();

						if (bmp != null)
							bmp.Dispose ();
					}
				}
			}
			if (swf.Clipboard.ContainsFileDropList())
			{
				var list = swf.Clipboard.GetFileDropList();
				if (list != null && list.Count > 0)
				{
					var path = list[0];
					sd.Image bmp = null;
					try
					{
						bmp = sd.Bitmap.FromFile(path);
						var result = new sd.Bitmap(bmp);
						return result;
					}
					catch (Exception)
					{
					}
					finally
					{
						if (bmp != null)
							bmp.Dispose();
					}
				}
			}

			return swf.Clipboard.ContainsImage () ? swf.Clipboard.GetImage () : null;
		}

		public byte[] GetData (string type)
		{
			if (swf.Clipboard.ContainsData (type))
				return swf.Clipboard.GetData (type) as byte[];
			else
				return null;
		}
		
		public string GetString (string type)
		{
			if (swf.Clipboard.ContainsData (type))
				return swf.Clipboard.GetData (type) as string;
			else
				return null;
		}

		public string[] Types {
			get {
				var data = swf.Clipboard.GetDataObject ();
				if (data != null)
					return data.GetFormats ();
				else
					return null;
			}
		}
		
		public void Clear ()
		{
			swf.Clipboard.Clear ();
			Control = new swf.DataObject();
		}
	}
}

