using System;
using System.Globalization;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
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

namespace Eto.Mac.Drawing
{

	public class IconFrameHandler : IconFrame.IHandler
	{
		public class LazyImageRep : NSImageRep
		{
			NSBitmapImageRep rep;
			CGSize? size;
			nint? pixelsHigh;
			nint? pixelsWide;
			public Func<Stream> Load { get; set; }

			static LazyImageRep()
			{
				NSImageRep.RegisterImageRepClass(new Class(typeof(LazyImageRep)));
			}

			static readonly IntPtr s_selAlloc_Handle = Selector.GetHandle("alloc");
			static readonly IntPtr s_bitmapImageRepClass = Class.GetHandle(typeof(NSBitmapImageRep));
			static readonly IntPtr selInitWithData_Handle = Selector.GetHandle("initWithData:");

			public NSBitmapImageRep Rep
			{
				get {
					if (rep != null)
						return rep;

					// fatal flaw in Xamarin.Mac/MonoMac here, so we can't use this constructor directly
					// see https://github.com/xamarin/xamarin-macios/issues/9478
					var data = NSData.FromStream(Load());
					var ptr = Messaging.IntPtr_objc_msgSend(s_bitmapImageRepClass, s_selAlloc_Handle);
					ptr = Messaging.IntPtr_objc_msgSend_IntPtr(ptr, selInitWithData_Handle, data.Handle);
					rep = Runtime.GetNSObject<NSBitmapImageRep>(ptr);

					// should be this:
					//rep = new NSBitmapImageRep(NSData.FromStream(Load()));
					
					rep.Size = new CGSize(rep.PixelsWide, rep.PixelsHigh); // ignore dpi from image
					return rep;
				}
			}

			public override CGImage AsCGImage(ref CGRect proposedDestRect, NSGraphicsContext context, NSDictionary hints)
			{
				return Rep.AsCGImage(ref proposedDestRect, context, hints);
			}

			public override nint BitsPerSample
			{
				get
				{
					return rep?.BitsPerSample ?? base.BitsPerSample;
				}
				set
				{
					Rep.BitsPerSample = value;
				}
			}

			public override string ColorSpaceName
			{
				get
				{
					return rep?.ColorSpaceName ?? base.ColorSpaceName;
				}
				set
				{
					Rep.ColorSpaceName = value;
				}
			}

			public override CGSize Size
			{
				get
				{
					return rep?.Size ?? size ?? Rep.Size;
				}
				set
				{
					size = value;
				}
			}

			public override nint PixelsHigh
			{
				get
				{
					return pixelsHigh ?? Rep.PixelsHigh;
				}
				set
				{
					pixelsHigh = value;
					base.PixelsHigh = value;
				}
			}

			public override nint PixelsWide
			{
				get
				{
					return pixelsWide ?? Rep.PixelsWide;
				}
				set
				{
					pixelsWide = value;
					base.PixelsWide = value;
				}
			}

			public override bool Draw()
			{
				return Rep.Draw();
			}

			public override bool DrawAtPoint(CGPoint point)
			{
				return Rep.DrawAtPoint(point);
			}

			static NSDictionary s_emptyDictionary = new NSDictionary();

			public override bool DrawInRect(CGRect dstSpacePortionRect, CGRect srcSpacePortionRect, NSCompositingOperation op, nfloat requestedAlpha, bool respectContextIsFlipped, NSDictionary hints)
			{
				// bug in Xamarin.Mac, hints can't be null when calling base..
				hints = hints ?? s_emptyDictionary;

				return Rep.DrawInRect(dstSpacePortionRect, srcSpacePortionRect, op, requestedAlpha, respectContextIsFlipped, hints);
			}

			public override bool DrawInRect(CGRect rect)
			{
				return Rep.DrawInRect(rect);
			}

			[Export("copyWithZone:")]
			public NSObject CopyWithZone(IntPtr zone)
			{
				var obj = new LazyImageRep {
					rep = rep?.Copy() as NSBitmapImageRep,
					pixelsHigh = pixelsHigh,
					pixelsWide = pixelsWide,
					size = size,
					Load = Load
				};
				obj.DangerousRetain();
				return obj;
			}
		}

		public object Create(IconFrame frame, Stream stream)
		{
			return new Bitmap(stream);
		}

		public object Create(IconFrame frame, Func<Stream> load)
		{
			var img = new NSImage();
			img.AddRepresentation(new LazyImageRep { Load = load });
			return new Bitmap(new BitmapHandler(img));
		}

		public object Create(IconFrame frame, Bitmap bitmap)
		{
			return bitmap;
		}

		public Bitmap GetBitmap(IconFrame frame)
		{
			return (Bitmap)frame.ControlObject;
		}

		public Size GetPixelSize(IconFrame frame)
		{
			return GetBitmap(frame).Size;
		}
	}
}
