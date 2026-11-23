using Eto.Forms;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using swf = System.Windows.Forms;

namespace Eto.Test.Wpf.UnitTests;

/// <summary>
/// When a web view is hosted in an ElementHost, destroying the native window around it while WebView2 is still
/// initializing makes initialization fail. That failure must not crash the application.
/// </summary>
[TestFixture]
public class WebView2DestroyedWhileInitializingTests : TestBase
{
	[DllImport("user32.dll")]
	static extern bool DestroyWindow(IntPtr hwnd);

	// Delays try to catch initialization at different stages after the web view gets its window.
	[TestCase(0)]
	[TestCase(10)]
	[TestCase(20)]
	[TestCase(40)]
	public void WebViewInElementHostDestroyedWhileInitializingShouldNotCrash(int delay)
	{
		Exception unhandledException = null;
		Dispatcher dispatcher = null;

		void DispatcherUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			// without handling it here the test host itself goes down
			unhandledException = e.Exception;
			e.Handled = true;
		}

		ShownAsync(f =>
		{
			dispatcher = Dispatcher.CurrentDispatcher;
			dispatcher.UnhandledException += DispatcherUnhandled;
		}, async () =>
		{
			try
			{
				var webView = new WebView();
				var handler = webView.Handler as WebView2Handler;
				if (handler == null)
					Assert.Ignore("WebView2 is not the active WebView handler, is the runtime installed?");
				webView.LoadHtml("<html><body></body></html>");
				var initialized = new TaskCompletionSource<bool>();
				handler.Control.CoreWebView2InitializationCompleted += (sender, e) => initialized.TrySetResult(e.IsSuccess);

				var nativeWindow = new swf.Form { ClientSize = new System.Drawing.Size(200, 200) };
				var elementHost = new swf.Integration.ElementHost { Dock = swf.DockStyle.Fill, Child = webView.ToNative(true) };
				nativeWindow.Controls.Add(elementHost);
				nativeWindow.Show();
				for (int i = 0; i < 1000 && handler.Control.Handle == IntPtr.Zero; i++)
					await Task.Delay(1);
				Assert.That(handler.Control.Handle, Is.Not.EqualTo(IntPtr.Zero), "Web view never got a window");

				await Task.Delay(delay);
				var finishedFirst = initialized.Task.IsCompleted;
				// destroyed from outside then disposed a moment later, as when a native docking window is torn down
				DestroyWindow(nativeWindow.Handle);
				Application.Instance.AsyncInvoke(() =>
				{
					webView.Dispose();
					elementHost.Dispose();
				});
				await Task.WhenAny(initialized.Task, Task.Delay(5000));
				await Task.Delay(500);

				Assert.That(unhandledException, Is.Null, $"Destroying the window brought down the dispatcher: {unhandledException}");
				if (finishedFirst)
					Assert.Inconclusive("Initialization finished before the window was destroyed");
			}
			finally
			{
				dispatcher.UnhandledException -= DispatcherUnhandled;
			}
		}, timeout: 20000);
	}
}
