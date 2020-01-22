using System;


using AppKit;
using Foundation;

#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
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
