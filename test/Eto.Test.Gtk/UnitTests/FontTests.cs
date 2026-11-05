using Eto.GtkSharp.Drawing;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class FontTests : TestBase
	{
		[Test]
		public void NativeDescriptionShouldResolveMissingFamily() => Invoke(() =>
		{
			using var font = new Font(new FontHandler("Eto Missing Theme Font 8C797EBB 11"));
			using var recreated = new Font(font.FamilyName, font.Size);
			Assert.That(recreated.MeasureString("Test").Width, Is.GreaterThan(0));
		});
	}
}
