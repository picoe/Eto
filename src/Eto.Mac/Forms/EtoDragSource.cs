using System;


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
