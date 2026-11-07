using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	public class RadioMenuItemTests : TestBase
	{
		static void AssertLinked(RadioMenuItem first, RadioMenuItem second)
		{
			first.Checked = true;
			second.Checked = true;
			Assert.That(first.Checked, Is.False, $"{first.Text} should be unchecked by {second.Text}");
			Assert.That(second.Checked, Is.True, $"{second.Text} should be checked");
		}

		[Test, InvokeOnUI]
		public void CheckingItemFromCodeShouldUncheckController()
		{
			var a1 = new RadioMenuItem { Text = "a1" };
			var a2 = new RadioMenuItem(a1) { Text = "a2" };
			AssertLinked(a1, a2);
			AssertLinked(a2, a1);
		}
	}
}
