using System;
using Eto.Forms;
using System.IO;
using Eto.Mac.Drawing;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms
{
	public class ClipboardHandler : WidgetHandler<NSPasteboard, Clipboard>, Clipboard.IHandler
	{
		nint changeCount;

		protected override NSPasteboard CreateControl()
		{
			return NSPasteboard.GeneralPasteboard;
		}

		protected override bool DisposeControl { get { return false; } }

		void ClearIfNeeded()
		{
			if (Control.ChangeCount != changeCount)
				changeCount = Control.ClearContents();
		}

		public void SetData(byte[] value, string type)
		{
			ClearIfNeeded();
			Control.SetDataForType(NSData.FromArray(value), type);
		}

		public string Html
		{
			set
			{ 
				ClearIfNeeded();
				Control.SetStringForType(value, NSPasteboard.NSHtmlType);
			}
			get { return Control.GetStringForType(NSPasteboard.NSHtmlType); }
		}

		public void SetString(string value, string type)
		{
			ClearIfNeeded();
			Control.SetStringForType(value, type);
		}

		public string Text
		{
			set
			{
				ClearIfNeeded();
				NSPasteboard.GeneralPasteboard.SetStringForType(value, NSPasteboard.NSStringType); 
			}
			get { return Control.GetStringForType(NSPasteboard.NSStringType); }
		}

		public Image Image
		{
			set
			{
				ClearIfNeeded();
				var handler = value.Handler as BitmapHandler;
				if (handler != null)
				{
					var data = handler.Control.AsTiff();
					Control.SetDataForType(data, NSPasteboard.NSTiffType);
				}
			}
			get
			{
				var oldFail = Class.ThrowOnInitFailure;
				Class.ThrowOnInitFailure = false;
				var image = new NSImage(Control);
				Class.ThrowOnInitFailure = oldFail;
				if (image.Handle == IntPtr.Zero)
					return null;
				return new Bitmap(new BitmapHandler(image));
			}
		}

		public unsafe byte[] GetData(string type)
		{
			var availableType = Control.GetAvailableTypeFromArray(new string[] { type });
		
			if (availableType != null)
			{
				var data = Control.GetDataForType(availableType);
				if (data == null)
					return null;
				var bytes = new byte[data.Length];
				var stream = new UnmanagedMemoryStream((byte*)data.Bytes, (long)data.Length);
				stream.Read(bytes, 0, (int)data.Length);
				return bytes;
			}
			return null;
		}

		public string GetString(string type)
		{
			return Control.GetStringForType(type);
		}

		public string[] Types
		{
			get { return Control.Types; }
		}

		public void Clear()
		{
			changeCount = Control.ClearContents();
		}
		
	}
}

