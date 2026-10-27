using Eto.Test.UnitTests;

namespace Eto.Test.Mac.UnitTests;

/// <summary>
/// Synthesizes key <see cref="NSEvent"/>s.
/// </summary>
static class KeyEvents
{
	/// <summary>
	/// Creates a key event for a physical key.  -[NSEvent keyEventWithType:...] does not keep the
	/// keyCode that is passed to it, so anything that needs a real keyCode has to go through CoreGraphics.
	/// </summary>
	public static NSEvent CreatePhysicalKeyEvent(int keyCode, bool keyDown = true, NSEventModifierMask modifiers = 0)
	{
		// CGEventFlags and NSEvent's modifier flags share the same bits
		var flags = (ulong)modifiers;
#if MONOMAC
		var cgEvent = CGEventCreateKeyboardEvent(IntPtr.Zero, checked((ushort)keyCode), keyDown);
		if (cgEvent == IntPtr.Zero)
			throw new InvalidOperationException("Could not create keyboard event");
		try
		{
			CGEventSetFlags(cgEvent, flags);
			return NSEvent.EventWithCGEvent(cgEvent);
		}
		finally
		{
			CFRelease(cgEvent);
		}
#else
		using var cgEvent = new CGEvent(null, checked((ushort)keyCode), keyDown)
		{
			Flags = (CGEventFlags)flags
		};
		return NSEvent.Create(cgEvent);
#endif
	}

#if MONOMAC
	[DllImport(Constants.CoreGraphicsLibrary)]
	static extern IntPtr CGEventCreateKeyboardEvent(
		IntPtr source,
		ushort virtualKey,
		[MarshalAs(UnmanagedType.I1)] bool keyDown);

	[DllImport(Constants.CoreGraphicsLibrary)]
	static extern void CGEventSetFlags(IntPtr cgEvent, ulong flags);

	[DllImport(Constants.CoreFoundationLibrary)]
	static extern void CFRelease(IntPtr value);
#endif
}
