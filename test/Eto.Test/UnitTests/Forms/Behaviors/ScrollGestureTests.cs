using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Behaviors
{
	[TestFixture]
	public class ScrollGestureTests : TestBase
	{
		/// <summary>
		/// Only macOS can tell an inverted scroll from a regular one, but every backend has to answer the
		/// question without blowing up, and without claiming to be inverted before anything has scrolled.
		/// </summary>
		[Test]
		public void IsDirectionInvertedShouldDefaultToFalse()
		{
			Invoke(() =>
			{
				if (!Platform.Instance.Supports<ScrollGesture>())
					Assert.Ignore("ScrollGesture is not supported by this platform");

				var gesture = new ScrollGesture();
				Assert.That(gesture.IsDirectionInverted, Is.False);
			});
		}
	}
}
