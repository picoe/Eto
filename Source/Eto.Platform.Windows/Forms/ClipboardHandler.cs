using System;
using Eto.Forms;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Platform.Windows.Drawing;
using Eto.Drawing;
using System.Runtime.InteropServices;

namespace Eto.Platform.Windows.Forms
{
	public class ClipboardHandler : WidgetHandler<SWF.DataObject, Clipboard>, IClipboard
	{
		
		public ClipboardHandler ()
		{
			Control = new SWF.DataObject();
		}
		
		void Update()
		{
			SWF.Clipboard.SetDataObject (Control);
		}

		#region IClipboard implementation
		
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
				return SWF.Clipboard.GetText (SWF.TextDataFormat.Html);
			}
		}
		
		public string Text
		{
			set {
				Control.SetText(value);
				Update();
			}
			get {
				return SWF.Clipboard.GetText ();				
			}
		}
		
		public Image Image
		{
			set {
				var sdimage = value.ControlObject as SD.Image;
				if (sdimage != null)
				{
					Control.SetImage (sdimage);
					Update();
				}
			}
			get 
            {
                Image result = null;

                try
                {
                    var sdimage =
                        GetImageFromClipboard()
                        as SD.Bitmap;

                    if (sdimage != null)
                    {
                        var handler = 
                            new BitmapHandler(sdimage);

                        result = 
                            new Bitmap(Widget.Generator, handler);
                    }
                }
                catch (Exception)
                {
                }

                return result;
			}
		}

        /// <summary>
        /// see http://stackoverflow.com/questions/11273669/how-to-paste-a-transparent-image-from-the-clipboard-in-a-c-sharp-winforms-app
        /// </summary>
        private SD.Image GetImageFromClipboard()
        {
            if (SWF.Clipboard.GetDataObject() == null) 
                return null;

            if (SWF.Clipboard.GetDataObject().GetDataPresent(
                    SWF.DataFormats.Dib))
            {
                var dib = 
                    ((System.IO.MemoryStream)
                        SWF.Clipboard.GetData(
                            SWF.DataFormats.Dib))
                        .ToArray();

                var width = BitConverter.ToInt32(dib, 4);
                var height = BitConverter.ToInt32(dib, 8);
                var bpp = BitConverter.ToInt16(dib, 14);

                if (bpp == 32)
                {
                    var gch = 
                        GCHandle.Alloc(
                            dib, 
                            GCHandleType.Pinned);

                    SD.Bitmap bmp = null;

                    try
                    {
                        var ptr = 
                            new IntPtr(
                                (long)gch.AddrOfPinnedObject() 
                                + 40);

                        bmp = 
                            new SD.Bitmap(
                                width, 
                                height, 
                                width * 4, 
                                System.Drawing.Imaging.PixelFormat.Format32bppArgb, 
                                ptr);

                        var result = new SD.Bitmap(bmp);

                        // Images are rotated and flipped for some reason.
                        // This rotates them back.
                        result.RotateFlip(SD.RotateFlipType.Rotate180FlipX);

                        return result;
                    }
                    finally
                    {
                        gch.Free();

                        if (bmp != null) 
                            bmp.Dispose();
                    }
                }
            }

            return 
                SWF.Clipboard.ContainsImage() 
                ? SWF.Clipboard.GetImage() 
                : null;
        }

		public byte[] GetData (string type)
		{
			if (SWF.Clipboard.ContainsData (type))
				return SWF.Clipboard.GetData (type) as byte[];
			else
				return null;
		}
		
		public string GetString (string type)
		{
			if (SWF.Clipboard.ContainsData (type))
				return SWF.Clipboard.GetData (type) as string;
			else
				return null;
		}

		public string[] Types {
			get {
				var data = SWF.Clipboard.GetDataObject ();
				if (data != null)
					return data.GetFormats ();
				else
					return null;
			}
		}
		
		public void Clear ()
		{
			SWF.Clipboard.Clear ();
			Control = new SWF.DataObject();
		}
		
		#endregion
    }
}

