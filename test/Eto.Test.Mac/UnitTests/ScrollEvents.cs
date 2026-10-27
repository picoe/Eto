using Eto.Test.UnitTests;

namespace Eto.Test.Mac.UnitTests;

/// <summary>
/// Synthesizes scroll wheel <see cref="NSEvent"/>s, including ones the OS reports as coming from a device with
/// natural (inverted) scrolling turned on.
/// </summary>
static class ScrollEvents
{
	// kCGScrollWheelEventScrollIsInverted, which is what -[NSEvent isDirectionInvertedFromDevice] reads.
	// It isn't in the public CGEventField enum, and there is no other way to synthesize an inverted scroll.
	const int ScrollIsInvertedField = 137;

	/// <param name="precise">true to report precise (trackpad) deltas, false for wheel notch deltas</param>
	public static NSEvent CreateScrollWheelEvent(int deltaX, int deltaY, bool isDirectionInverted, bool precise = false)
	{
#if MONOMAC
		// MonoMac has no CGEvent binding, and CGEventCreateScrollWheelEvent is variadic so it can't be
		// p/invoked portably - build a generic event and turn it into a scroll instead.
		var cgEvent = CGEventCreate(IntPtr.Zero);
		if (cgEvent == IntPtr.Zero)
			throw new InvalidOperationException("Could not create scroll wheel event");
		try
		{
			CGEventSetType(cgEvent, 22); // kCGEventScrollWheel
			CGEventSetIntegerValueField(cgEvent, 11, deltaY); // kCGScrollWheelEventDeltaAxis1
			CGEventSetIntegerValueField(cgEvent, 12, deltaX); // kCGScrollWheelEventDeltaAxis2
			CGEventSetIntegerValueField(cgEvent, ScrollIsInvertedField, isDirectionInverted ? 1 : 0);
			if (precise)
			{
				CGEventSetIntegerValueField(cgEvent, 88, 1); // kCGScrollWheelEventIsContinuous
				CGEventSetIntegerValueField(cgEvent, 96, deltaY); // kCGScrollWheelEventPointDeltaAxis1
				CGEventSetIntegerValueField(cgEvent, 97, deltaX); // kCGScrollWheelEventPointDeltaAxis2
			}
			return NSEvent.EventWithCGEvent(cgEvent);
		}
		finally
		{
			CFRelease(cgEvent);
		}
#else
		using var cgEvent = new CGEvent(null, precise ? CGScrollEventUnit.Pixel : CGScrollEventUnit.Line, deltaY, deltaX);
		cgEvent.SetValueField((CGEventField)ScrollIsInvertedField, isDirectionInverted ? 1 : 0);
		if (precise)
		{
			cgEvent.SetValueField(CGEventField.ScrollWheelEventIsContinuous, 1);
			cgEvent.SetValueField(CGEventField.ScrollWheelEventPointDeltaAxis1, deltaY);
			cgEvent.SetValueField(CGEventField.ScrollWheelEventPointDeltaAxis2, deltaX);
		}
		return NSEvent.Create(cgEvent);
#endif
	}

#if MONOMAC
	[DllImport(Constants.CoreGraphicsLibrary)]
	static extern IntPtr CGEventCreate(IntPtr source);

	[DllImport(Constants.CoreGraphicsLibrary)]
	static extern void CGEventSetType(IntPtr cgEvent, uint type);

	[DllImport(Constants.CoreGraphicsLibrary)]
	static extern void CGEventSetIntegerValueField(IntPtr cgEvent, uint field, long value);

	[DllImport(Constants.CoreFoundationLibrary)]
	static extern void CFRelease(IntPtr value);
#endif
}
