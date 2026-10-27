using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;
using System.Reflection;
using System.Windows.Threading;

namespace Eto.Test.Wpf.UnitTests;

/// <summary>
/// The WebView2 browser process can die between initialization completing and the handler's queued
/// follow up work running a dispatcher turn later. CoreWebView2 then throws from VerifyBrowserNotCrashed,
/// and since that happens inside a dispatcher callback there is nothing to catch it - the application goes
/// down with no warning. The web view has to be abandoned instead.
/// </summary>
[TestFixture]
public class WebView2CrashTests : TestBase
{
	/// <summary>
	/// Marks the control as having lost its browser process, the same state WebView2 puts itself in when
	/// the process actually dies. Returns false when the field is gone, i.e. WebView2 changed its internals.
	/// </summary>
	static bool SimulateBrowserCrash(Microsoft.Web.WebView2.Wpf.WebView2 control)
	{
		// the flag lives on the control itself in older WebView2 releases, and on the shared
		// WebView2Base implementation the control delegates to in newer ones
		foreach (var target in new[] { control, GetWebView2Base(control) })
		{
			for (var type = target?.GetType(); type != null; type = type.BaseType)
			{
				var field = type.GetField("_browserCrashed", BindingFlags.NonPublic | BindingFlags.Instance);
				if (field != null)
				{
					field.SetValue(target, true);
					return true;
				}
			}
		}
		return false;
	}

	static object GetWebView2Base(object control) => control.GetType()
		.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
		.FirstOrDefault(f => f.Name.IndexOf("webview2Base", StringComparison.OrdinalIgnoreCase) >= 0)
		?.GetValue(control);

	[Test]
	public void BrowserCrashAfterInitializationShouldNotCrashApplication()
	{
		Exception unhandledException = null;
		bool? simulatedCrash = null;
		bool coreWebView2Throws = false;
		var finished = new TaskCompletionSource<bool>();
		Dispatcher dispatcher = null;

		void DispatcherUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			// without handling it here the test host itself goes down, which is precisely the bug under test
			unhandledException = e.Exception;
			e.Handled = true;
		}

		ShownAsync(form =>
		{
			dispatcher = Dispatcher.CurrentDispatcher;
			dispatcher.UnhandledException += DispatcherUnhandled;

			var webView = new WebView { Size = new Size(200, 200) };
			var handler = webView.Handler as WebView2Handler;
			if (handler == null)
			{
				Assert.Ignore("WebView2 is not the active WebView handler, is the runtime installed?");
				return null;
			}

			var control = handler.Control;
			// hooked after the handler's own subscription, so this runs in the same window the crash does:
			// after initialization has completed and before the handler's queued work runs
			control.CoreWebView2InitializationCompleted += (sender, e) =>
			{
				if (!e.IsSuccess)
				{
					finished.TrySetResult(false);
					return;
				}

				simulatedCrash = SimulateBrowserCrash(control);
				try
				{
					_ = control.CoreWebView2;
				}
				catch (InvalidOperationException)
				{
					coreWebView2Throws = true;
				}

				// queued after the handler's own follow up work, so by the time this runs the handler has
				// either survived the crashed browser or brought the dispatcher down with it
				Application.Instance.AsyncInvoke(() => finished.TrySetResult(true));
			};

			form.Content = webView;
			return webView;
		}, async webView =>
		{
			try
			{
				await Task.WhenAny(finished.Task, Task.Delay(10000));

				Assert.That(finished.Task.IsCompleted, Is.True, "WebView2 did not finish initializing in time");
				Assert.That(finished.Task.Result, Is.True, "WebView2 failed to initialize");
				Assert.That(simulatedCrash, Is.True, "Could not mark the browser as crashed, WebView2 internals have changed");
				Assert.That(coreWebView2Throws, Is.True, "CoreWebView2 did not throw for a crashed browser, WebView2 internals have changed");
				Assert.That(unhandledException, Is.Null, $"Crashed browser process brought down the dispatcher: {unhandledException}");
			}
			finally
			{
				dispatcher.UnhandledException -= DispatcherUnhandled;
			}
		}, timeout: 20000);
	}
}
