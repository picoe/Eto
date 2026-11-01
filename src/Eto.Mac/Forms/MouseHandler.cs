namespace Eto.Mac.Forms
{
	public class MouseHandler : Mouse.IHandler
	{
		public void Initialize ()
		{
		}
		public Widget Widget { get; set; }

		public Eto.Platform Platform { get; set; }

		[DllImport(Constants.CoreGraphicsLibrary)]
 		static extern int CGWarpMouseCursorPosition(CGPoint point);

		[DllImport(Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGEventCreate(IntPtr source);

		[DllImport(Constants.CoreGraphicsLibrary)]
		static extern CGPoint CGEventGetLocation(IntPtr handle);

		[DllImport(Constants.CoreFoundationLibrary)]
		static extern void CFRelease(IntPtr handle);

		public void SetCursor(Cursor cursor) => cursor.ToNS().Set();

		public Eto.Drawing.PointF Position
		{
			get
			{
				// Avoid AppKit's main-thread requirement when reading the cursor position.
				var handle = CGEventCreate(IntPtr.Zero);
				if (handle == IntPtr.Zero)
					throw new InvalidOperationException("Could not get the current mouse position.");
				try
				{
					return CGEventGetLocation(handle).ToEto();
				}
				finally
				{
					CFRelease(handle);
				}
			}
			set => CGWarpMouseCursorPosition(value.ToNS());
		}

		public MouseButtons Buttons
		{
			get
			{
				var current = NSEvent.CurrentPressedMouseButtons;
				var buttons = MouseButtons.None;
				if ((current & 1) != 0)
					buttons |= MouseButtons.Primary;
				if ((current & 2) != 0)
					buttons |= MouseButtons.Alternate;
				if ((current & 4) != 0)
					buttons |= MouseButtons.Middle;
				return buttons;
			}
		}
	}
}

