using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class ImageViewTests : TestBase
{
	// RH-99305: Eto still resolves the ImageView's parent to the disposed form, so showing the icon
	// (e.g. when WPF raises Loaded for the re-hosted element) must not read the dead window.
	[Test]
	public void IconShouldNotCrashWhenParentWindowIsDisposed() => Async(async () =>
	{
		var imageView = new ImageView { Image = TestIcons.TestIcon, Size = new Size(64, 64) };
		var form = new Form { Content = imageView, ClientSize = new Size(200, 200) };
		form.Show();
		await Task.Delay(100);

		var native = imageView.ToNative();
		DetachFromNativeParent(native);
		form.Dispose();

		Assert.That(form.IsDisposed, Is.True, "#1");
		Assert.That(imageView.IsDisposed, Is.False, "#2");
		Assert.That(imageView.ParentWindow, Is.SameAs(form), "#3");

		// Loaded fires from the window's WndProc, so a throw there would take down the test host
		Exception unhandled = null;
		void OnUnhandled(object sender, sw.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			unhandled ??= e.Exception;
			e.Handled = true;
		}

		var nativeWindow = new sw.Window { Content = native, Width = 200, Height = 200 };
		sw.Application.Current.DispatcherUnhandledException += OnUnhandled;
		try
		{
			Assert.DoesNotThrow(() => nativeWindow.Show(), "#4");
			await Task.Delay(100);
			Assert.That(unhandled, Is.Null, "#5");
			Assert.DoesNotThrow(() => imageView.Image = TestIcons.Logo, "#6");
			Assert.DoesNotThrow(() => imageView.Image = TestIcons.TestIcon, "#7");
		}
		finally
		{
			sw.Application.Current.DispatcherUnhandledException -= OnUnhandled;
			nativeWindow.Close();
		}
	});

	static void DetachFromNativeParent(sw.FrameworkElement element)
	{
		switch (sw.LogicalTreeHelper.GetParent(element))
		{
			case swc.Decorator decorator:
				decorator.Child = null;
				break;
			case swc.Panel panel:
				panel.Children.Remove(element);
				break;
			case swc.ContentControl contentControl:
				contentControl.Content = null;
				break;
			case var other:
				Assert.Fail($"Unexpected native parent {other?.GetType()}");
				break;
		}
	}
}
