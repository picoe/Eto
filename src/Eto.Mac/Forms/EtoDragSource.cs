using System;

#if XAMMAC2
using AppKit;
using Foundation;
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

namespace Eto.Mac.Forms
{
	class EtoDragSource : NSDraggingSource
	{
		[Export("sourceView")]
		public NSView SourceView { get; set; }

		[Export("allowedOperation")]
		public NSDragOperation AllowedOperation { get; set; }

		[Export("draggingSession:sourceOperationMaskForDraggingContext:")]
		public NSDragOperation DraggingSessionSourceOperationMask(NSDraggingSession session, IntPtr context)
		{
			return AllowedOperation;
		}
	}
}
