using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class TabPageTests : TestBase
	{
		[TestCase("&File && literal_name")]
		[TestCase(null)]
		public void TextShouldPreserveAssignedTitle(string text)
		{
			Invoke(() =>
			{
				using var page = new TabPage { Text = text };
				Assert.That(page.Text, Is.EqualTo(text ?? string.Empty));
			});
		}
	}
}
