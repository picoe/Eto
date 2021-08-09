using System;
using Eto.Forms;
using Eto.Drawing;
using System.IO;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using ImageIO;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
using MonoMac.ImageIO;
using NSRectEdge = MonoMac.AppKit.NSRectEdge;
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
	public class CursorHandler : WidgetHandler<NSCursor, Cursor>, Cursor.IHandler
	{
		public CursorHandler()
		{
		}

		public CursorHandler(NSCursor control)
		{
			Control = control;
		}

		public void Create(CursorType cursor)
		{
			switch (cursor)
			{
				case CursorType.Arrow:
					Control = NSCursor.ArrowCursor;
					break;
				case CursorType.Crosshair:
					Control = NSCursor.CrosshairCursor;
					break;
				case CursorType.Default:
					Control = NSCursor.CurrentSystemCursor;
					break;
				case CursorType.HorizontalSplit:
					Control = NSCursor.ResizeUpDownCursor;
					break;
				case CursorType.IBeam:
					Control = NSCursor.IBeamCursor;
					break;
				case CursorType.Move:
					Control = NSCursor.OpenHandCursor;
					break;
				case CursorType.Pointer:
					Control = NSCursor.PointingHandCursor;
					break;
				case CursorType.VerticalSplit:
					Control = NSCursor.ResizeLeftRightCursor;
					break;
				case CursorType.SizeAll:
					Control = GetHICursor("move") ?? NSCursor.ArrowCursor;
					break;
				case CursorType.SizeLeft:
				case CursorType.SizeRight:
					Control = GetHICursor("resizeeastwest") ?? GetNonStandardCursor("_windowResizeEastWestCursor") ?? NSCursor.ResizeLeftRightCursor;
					break;
				case CursorType.SizeTop:
				case CursorType.SizeBottom:
					Control = GetHICursor("resizenorthsouth") ?? GetNonStandardCursor("_windowResizeNorthSouthCursor") ?? NSCursor.ResizeUpDownCursor;
					break;
				case CursorType.SizeTopLeft:
				case CursorType.SizeBottomRight:
					Control = GetHICursor("resizenorthwestsoutheast") ?? GetNonStandardCursor("_windowResizeNorthWestSouthEastCursor") ?? NSCursor.ArrowCursor;
					break;
				case CursorType.SizeTopRight:
				case CursorType.SizeBottomLeft:
					Control = GetHICursor("resizenortheastsouthwest") ?? GetNonStandardCursor("_windowResizeNorthEastSouthWestCursor") ?? NSCursor.ArrowCursor;
					break;
				default:
					throw new NotSupportedException();
			}
		}

		static IntPtr cursorClassHandle = Class.GetHandle(typeof(NSCursor));

		static NSCursor GetNonStandardCursor(string name)
		{
			// note, using private apis here so won't work on sandboxed apps.
			try
			{
				var handle = Messaging.IntPtr_objc_msgSend(cursorClassHandle, Selector.GetHandle(name));
				if (handle == IntPtr.Zero)
					return null;
				return Runtime.GetNSObject<NSCursor>(handle);
			}
			catch
			{
				return null;
			}
		}
		
		static NSCursor GetHICursor(string cursorName)
		{
			var cursorPath = Path.Combine("/System/Library/Frameworks/ApplicationServices.framework/Versions/A/Frameworks/HIServices.framework/Versions/A/Resources/cursors", cursorName);
			var pdfPath = Path.Combine(cursorPath, "cursor.pdf");
			var infoPath = Path.Combine(cursorPath, "info.plist");
			if (!File.Exists(pdfPath) || !File.Exists(infoPath))
				return null;
			var image = new NSImage(pdfPath);
			var info = new NSDictionary(infoPath);
			return new NSCursor(image, new CGPoint(((NSNumber)info.ValueForKey((NSString)"hotx")).DoubleValue, ((NSNumber)info.ValueForKey((NSString)"hoty")).DoubleValue));
		}

		public void Create(Image image, PointF hotspot)
		{
			var nsimage = image.ToNS();
			Control = new NSCursor(nsimage, hotspot.ToNS());
		}

		public void Create(string fileName)
		{
			using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
			{
				Create(stream);
			}
		}

		public void Create(Stream stream)
		{
			var data = NSData.FromStream(stream);
			var imageSource = CGImageSource.FromData(data);
			//var imageSource = CGImageSource.FromDataProvider(nsimage.CGImage.DataProvider);
			// try go get hotspot
			var properties = imageSource?.CopyProperties((NSDictionary)null, 0);

			var hotspot = CGPoint.Empty;
			if (properties != null)
			{
				hotspot.X = (properties["hotspotX"] as NSNumber)?.FloatValue ?? 0;
				hotspot.Y = (properties["hotspotY"] as NSNumber)?.FloatValue ?? 0;
			}

			var nsimage = new NSImage(data);
			Control = new NSCursor(nsimage, hotspot);
		}
	}
}

