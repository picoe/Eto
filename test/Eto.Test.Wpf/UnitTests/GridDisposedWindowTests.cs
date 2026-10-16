using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class GridDisposedWindowTests : TestBase
{
	// RH-99306: the grid's native element can still be queued for layout after its Eto window is disposed,
	// and Eto still resolves the grid's parent to that dead window, so measuring must not read it.
	[Test]
	public void MeasureShouldNotCrashWhenParentWindowIsDisposed() => Async(async () =>
	{
		var grid = new GridView { Size = new Size(150, 100) };
		grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0) });
		grid.DataStore = Enumerable.Range(0, 100).Select(i => new[] { $"Item {i}" }).ToList();

		var form = new Form { Content = grid, Resizable = true, AutoSize = true };
		form.Show();
		await Task.Delay(100);

		var native = grid.ToNative();
		DetachFromNativeParent(native);
		form.Dispose();

		Assert.That(form.IsDisposed, Is.True, "#1");
		Assert.That(grid.IsDisposed, Is.False, "#2");
		Assert.That(grid.ParentWindow, Is.SameAs(form), "#3");

		// layout runs from the window's WndProc, so a throw there would take down the test host
		Exception unhandled = null;
		void OnUnhandled(object sender, sw.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			unhandled ??= e.Exception;
			e.Handled = true;
		}

		var nativeWindow = new sw.Window { Content = native, Width = 300, Height = 300 };
		sw.Application.Current.DispatcherUnhandledException += OnUnhandled;
		try
		{
			Assert.DoesNotThrow(() => nativeWindow.Show(), "#4");
			await Task.Delay(100);
			Assert.That(unhandled, Is.Null, "#5");
			Assert.DoesNotThrow(() => native.Measure(new sw.Size(200, 200)), "#6");
			Assert.That(native.ActualWidth, Is.GreaterThan(0), "#7");
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
