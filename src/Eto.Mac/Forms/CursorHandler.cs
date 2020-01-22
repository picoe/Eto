using System;
using Eto.Forms;
using Eto.Drawing;
using System.IO;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using ImageIO;

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
				default:
					throw new NotSupportedException();
			}
		}

		public void Create(Bitmap image, PointF hotspot)
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

