using System;
using Eto.Forms;
using System.IO;
using Eto.Drawing;
using UIKit;
using Foundation;
using MobileCoreServices;
using Eto.iOS.Drawing;

namespace Eto.iOS.Forms
{
	public class ClipboardHandler : WidgetHandler<UIPasteboard, Clipboard>, Clipboard.IHandler
	{
		nint changeCount;

		public ClipboardHandler()
		{
			Control = UIPasteboard.General;
		}

		void ClearIfNeeded()
		{
			if (Control.ChangeCount != changeCount)
			{
				Control.String = string.Empty;
				changeCount = Control.ChangeCount;
			}
		}

		public void SetData(byte[] value, string type)
		{
			ClearIfNeeded();
			Control.SetData(NSData.FromArray(value), type);
		}

		public string Html
		{
			set
			{ 
				ClearIfNeeded();
				Control.SetData(NSData.FromString(value), UTType.HTML);
			}
			get
			{ 
				var data = Control.DataForPasteboardType(UTType.HTML); 
				return data != null ? data.ToString() : null;
			}
		}

		public void SetString(string value, string type)
		{
			ClearIfNeeded();
			Control.SetData(NSData.FromString(value), type);
		}

		public string Text
		{
			set
			{
				ClearIfNeeded();
				Control.SetData(NSData.FromString(value), UTType.Text);
			}
			get
			{ 
				var data = Control.DataForPasteboardType(UTType.Text);
				return data != null ? data.ToString() : null;
			}
		}

		public Image Image
		{
			set
			{
				ClearIfNeeded();
				var handler = value.Handler as BitmapHandler;
				if (handler != null)
				{
					var data = handler.Control.AsPNG();
					Control.SetData(data, UTType.PNG);
				}
			}
			get
			{
				var data = Control.DataForPasteboardType(UTType.PNG);
				if (data == null || data.Handle == IntPtr.Zero)
					return null;
				return new Bitmap(new BitmapHandler(new UIImage(data)));
			}
		}

		public unsafe byte[] GetData(string type)
		{
			var data = Control.DataForPasteboardType(type);
			if (data == null)
				return null;
			var bytes = new byte[data.Length];
			var stream = new UnmanagedMemoryStream((byte*)data.Bytes, (long)data.Length);
			stream.Read(bytes, 0, (int)data.Length);
			return bytes;
		}

		public string GetString(string type)
		{
			var data = Control.DataForPasteboardType(type);
			return data != null ? data.ToString() : null;
		}

		public string[] Types
		{
			get { return Control.Types; }
		}

		public void Clear()
		{
			Control.String = string.Empty;
			changeCount = Control.ChangeCount;
		}

	}
}

