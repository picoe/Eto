using Eto.Test.UnitTests;
using Eto.Wpf.Forms;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class WindowTests : TestBase
{
	[Test]
	public void WindowShouldActuallyBeVisibleWhenShown() => Async(async () => {

		var form = new Form { Content = new Panel { Content = "Hello", Size = new Size(200, 200) } };

		// This resets the Visibility of the WPF window so it can cause the form not to show
		var size = form.GetPreferredSize();

		form.Show();
		
		await Task.Delay(100);

		var handler = form.Handler as FormHandler;

		Assert.That(handler.Control.Visibility, Is.EqualTo(sw.Visibility.Visible), "#1.1 Visibility should be visible");
		Assert.That(handler.Control.IsActive, Is.EqualTo(true), "#1.2 Form should be active");

		form.Close();
	});
	
}
