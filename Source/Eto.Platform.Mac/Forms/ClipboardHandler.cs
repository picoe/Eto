using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.IO;
using Eto.Platform.Mac.Drawing;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms
{
	public class ClipboardHandler : WidgetHandler<NSPasteboard, Clipboard>, IClipboard
	{
		public ClipboardHandler ()
		{
			Control = NSPasteboard.GeneralPasteboard;
		}

		#region IClipboard implementation
		
		public void SetData (byte[] value, string type)
		{
			Control.SetDataForType (NSData.FromArray (value), type);
		}
		
		public string Html
		{
			set { Control.SetStringForType (value, NSPasteboard.NSHtmlType); }
			get { return Control.GetStringForType (NSPasteboard.NSHtmlType); }
		}
		
		public void SetString (string value, string type)
		{
			Control.SetStringForType (value, type);
		}
		
		public string Text
		{
			set { Control.SetStringForType (value, NSPasteboard.NSStringType); }
			get { return Control.GetStringForType (NSPasteboard.NSStringType); }
		}
		
		public Image Image
		{
			set {
				var handler = value.Handler as BitmapHandler;
				if (handler != null) {
					var data = handler.Control.AsTiff ();
					Control.SetDataForType (data, NSPasteboard.NSTiffType);
				}
			}
			get {
				var image = new NSImage(Control);
				if (image.Handle == IntPtr.Zero) return null;
				return new Bitmap(Widget.Generator, new BitmapHandler(image));
			}
		}

		public unsafe byte[] GetData (string type)
		{
			var availableType = Control.GetAvailableTypeFromArray (new string[] { type });
		
			if (availableType != null) {
				var data = Control.GetDataForType (availableType);
				var bytes = new byte[data.Length];
				var stream = new UnmanagedMemoryStream ((byte*) data.Bytes, data.Length);
				stream.Read (bytes, 0, (int)data.Length);
				return bytes;
			}
			else
				return null;
		}
		
		public string GetString (string type)
		{
			return Control.GetStringForType (type);
		}

		public string[] Types {
			get { return Control.Types; }
		}
		
		public void Clear ()
		{
			Control.ClearContents ();
		}
		
		#endregion

	}
}

