using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace Eto.Test.Wpf.UnitTests;

/// <summary>
/// WebView2 is told it is visible as soon as WPF says so, before its window is actually on screen. If the
/// window is hidden and shown again in quick succession, as a floating window is when the application is
/// deactivated, the web view can stay blank until it is resized. It has to be told again once the window
/// really is visible.
/// </summary>
[TestFixture]
public class WebView2VisibilityTests : TestBase
{
	[DllImport("user32.dll")]
	static extern IntPtr GetAncestor(IntPtr hwnd, uint flags);

	[DllImport("user32.dll")]
	static extern bool IsWindowVisible(IntPtr hwnd);

	[DllImport("user32.dll")]
	static extern bool ShowWindow(IntPtr hwnd, int cmd);

	const uint GA_ROOT = 2;
	const int SW_HIDE = 0;
	const int SW_SHOWNA = 8;

	/// <summary>
	/// Shows a window with an initialized web view, then runs <paramref name="test"/>. The list it is given gets,
	/// for each time WebView2 is made visible, whether the window was on screen.
	/// </summary>
	/// <remarks>
	/// The web view gets a window of its own, because the test form runs its test again each time it is shown.
	/// </remarks>
	void WithWebView(Func<Form, Microsoft.Web.WebView2.Wpf.WebView2, List<bool>, Task> test)
	{
		ShownAsync(form => form, async form =>
		{
			var webView = new WebView();
			var handler = webView.Handler as WebView2Handler;
			if (handler == null)
				Assert.Ignore("WebView2 is not the active WebView handler, is the runtime installed?");

			var control = handler.Control;
			var ready = new TaskCompletionSource<bool>();
			var windowShownWhenMadeVisible = new List<bool>();
			control.CoreWebView2InitializationCompleted += (sender, e) => ready.TrySetResult(e.IsSuccess);
			// subscribed after the WebView2 control itself, so this sees each change right after the control
			// has passed it on to the browser
			control.IsVisibleChanged += (sender, e) =>
			{
				if ((bool)e.NewValue)
					windowShownWhenMadeVisible.Add(IsWindowVisible(GetAncestor(control.Handle, GA_ROOT)));
			};

			var window = new Form { ClientSize = new Size(200, 200), Content = webView };
			try
			{
				window.Show();
				await Task.WhenAny(ready.Task, Task.Delay(10000));
				Assert.That(ready.Task.IsCompleted, Is.True, "WebView2 did not finish initializing in time");
				Assert.That(ready.Task.Result, Is.True, "WebView2 failed to initialize");
				// let the handler finish hooking up the initialized web view
				await Task.Delay(200);

				windowShownWhenMadeVisible.Clear();
				await test(window, control, windowShownWhenMadeVisible);
			}
			finally
			{
				window.Close();
			}
		}, timeout: 20000);
	}

	[Test]
	public void WebViewShouldBeMadeVisibleAfterItsWindowIsShown() => WithWebView(async (window, control, windowShownWhenMadeVisible) =>
	{
		window.Visible = false;
		window.Visible = true;
		await Task.Delay(500);

		Assert.That(windowShownWhenMadeVisible, Is.Not.Empty, "WebView2 was never made visible again");
		Assert.That(windowShownWhenMadeVisible.Last(), Is.True, "WebView2 was last made visible while its window was still hidden");
	});

	[Test]
	public void WebViewShouldBeMadeVisibleWhenItsOwnWindowIsShownLater() => WithWebView(async (window, control, windowShownWhenMadeVisible) =>
	{
		window.Visible = false;
		window.Visible = true;
		// the window comes back but the web view's own window does not, until a moment later. Queued so it runs
		// after WPF's layout has shown that window and before the handler's first try to re-sync.
		_ = control.Dispatcher.BeginInvoke(new Action(() => ShowWindow(control.Handle, SW_HIDE)), System.Windows.Threading.DispatcherPriority.Background);
		await Task.Delay(300);
		Assert.That(IsWindowVisible(control.Handle), Is.False, "Test setup: the web view's window was shown again by something else");
		windowShownWhenMadeVisible.Clear();

		ShowWindow(control.Handle, SW_SHOWNA);
		await Task.Delay(300);

		Assert.That(windowShownWhenMadeVisible, Is.Not.Empty, "WebView2 was not made visible again once its window was shown");
		Assert.That(windowShownWhenMadeVisible.Last(), Is.True, "WebView2 was last made visible while its window was still hidden");
	});
}
