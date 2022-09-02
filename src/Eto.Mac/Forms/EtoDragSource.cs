using System;
using Eto.Forms;

namespace Eto.Mac.Forms
{
	class EtoDragSource : NSDraggingSource
	{
		public DataObject Data { get; set; }
		public IMacViewHandler Handler { get; set; }

		[Export("sourceView")]
		public NSView SourceView { get; set; }

		[Export("allowedOperation")]
		public NSDragOperation AllowedOperation { get; set; }

		[Export("draggingSession:sourceOperationMaskForDraggingContext:")]
		public NSDragOperation DraggingSessionSourceOperationMask(NSDraggingSession session, IntPtr context)
		{
			return AllowedOperation;
		}
		
		[Export("draggingSession:endedAtPoint:operation:")]
		public void DraggingSessionEnded(NSDraggingSession session, CGPoint point, NSDragOperation operation)
		{
			var h = Handler;
			if (h == null)
				return;
			var args = new DragEventArgs(h.Widget, Data, AllowedOperation.ToEto(), point.ToEto(), Keyboard.Modifiers, Mouse.Buttons, this);
			args.Effects = operation.ToEto();
			h.Callback.OnDragEnd(h.Widget, args);
		}

		[Export("ignoreModifierKeysForDraggingSession:")]
		public bool IgnoreModifierKeysForDraggingSession(NSDraggingSession session) => true;
	}
}
