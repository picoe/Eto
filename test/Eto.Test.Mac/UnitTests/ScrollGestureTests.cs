using Eto.Mac.Forms.Controls;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests;

[TestFixture]
public class ScrollGestureTests : TestBase
{
	/// <summary>
	/// Issue: RH-97914 - the gesture reports whether the user has natural scrolling turned on for the device
	/// that generated the scroll, the same as <see cref="MouseEventArgs.IsDirectionInverted"/> does.
	/// </summary>
	[TestCase(false)]
	[TestCase(true)]
	[InvokeOnUI]
	public void ScrollGestureShouldReportInvertedScrollDirection(bool isDirectionInverted)
	{
		var gesture = new ScrollGesture();
		var handler = (ScrollGestureHandler)((IHandlerSource)gesture).Handler;
		using var scrollEvent = ScrollEvents.CreateScrollWheelEvent(3, 5, isDirectionInverted, precise: true);

		// synthesizing an inverted scroll relies on a CGEventField that isn't public API, so treat it as a
		// precondition (inconclusive) rather than a failure if macOS ever stops honouring it.
		Assume.That(scrollEvent.HasPreciseScrollingDeltas, Is.True, "Could not synthesize a trackpad scroll event");
		Assume.That(scrollEvent.IsDirectionInvertedFromDevice, Is.EqualTo(isDirectionInverted), "Could not synthesize a scroll wheel event with an inverted direction");

		Assert.That(handler.OnScrollWheel(scrollEvent), Is.True, "Scroll event was not handled by the gesture");

		Assert.Multiple(() =>
		{
			Assert.That(gesture.IsDirectionInverted, Is.EqualTo(isDirectionInverted));
			Assert.That(gesture.Delta, Is.EqualTo(new SizeF(3, 5)));
		});
	}
}
