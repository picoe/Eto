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

		const int kCGEventSourceStateCombinedSessionState = 0;

		[DllImport(Constants.CoreGraphicsLibrary)]
		[return: MarshalAs(UnmanagedType.I1)]
		static extern bool CGEventSourceButtonState(int stateID, uint button);

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
				// Avoid AppKit's main-thread requirement when reading the button state.
				var buttons = MouseButtons.None;
				if (CGEventSourceButtonState(kCGEventSourceStateCombinedSessionState, 0))
					buttons |= MouseButtons.Primary;
				if (CGEventSourceButtonState(kCGEventSourceStateCombinedSessionState, 1))
					buttons |= MouseButtons.Alternate;
				if (CGEventSourceButtonState(kCGEventSourceStateCombinedSessionState, 2))
					buttons |= MouseButtons.Middle;
				return buttons;
			}
		}
	}
}

