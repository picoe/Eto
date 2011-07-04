using System;
using Eto.Forms;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Platform.Windows.Drawing;
using Eto.Drawing;

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
			get {
				var sdimage = SWF.Clipboard.GetImage () as SD.Bitmap;
				if (sdimage != null) {
					var handler = new BitmapHandler(sdimage);
					return new Bitmap(Widget.Generator, handler);
				}
				else throw new NotImplementedException();
			}
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

