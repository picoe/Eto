using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
    public class DrawableTests : TestBase
    {
		[ManualTest, Test]
		public void MappingPlatformCommandShouldNotCrash() => ManualForm("Click the Edit > Cut menu item, it should not crash", form =>
		{
			form.Menu = new MenuBar();

			var drawable = new Drawable();
			drawable.Content = "I should have focus!";
			drawable.Size = new Size(100, 100);
			drawable.MapPlatformCommand("cut", new Command((sender, e) => MessageBox.Show("You clicked me! woo!")));
			drawable.MapPlatformCommand("copy", null);

			drawable.BackgroundColor = Colors.Green;
			drawable.CanFocus = true;
			drawable.Focus();

			return drawable;

		});
        
    }
}