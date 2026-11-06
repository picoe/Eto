using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace Eto.Test.Wpf.UnitTests;

/// <summary>
/// WebView2 stores its data next to the .exe by default, which isn't writable for installed apps,
/// so Eto puts it in a per-app folder under LocalAppData instead.
/// </summary>
[TestFixture]
public class WebView2UserDataFolderTests : TestBase
{
	[Test]
	public void DefaultUserDataFolderShouldBeUnderLocalAppDataNamedForTheApp()
	{
		var folder = WebView2Handler.UserDataFolder;
		var localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
		string exePath;
		using (var process = Process.GetCurrentProcess())
			exePath = process.MainModule.FileName;
		var info = FileVersionInfo.GetVersionInfo(exePath);
		TestContext.Out.WriteLine($"exe: {exePath}, company: {info.CompanyName}, product: {info.ProductName}");
		TestContext.Out.WriteLine($"UserDataFolder: {folder}");

		Assert.That(folder, Is.Not.Null);
		Assert.That(folder, Does.StartWith(localAppData + Path.DirectorySeparatorChar));
		Assert.That(Path.GetFileName(folder), Is.EqualTo("WebView2"));
		Assert.That(folder, Does.Contain(Path.DirectorySeparatorChar + info.ProductName + Path.DirectorySeparatorChar));
		Assert.That(folder, Does.Not.StartWith(Path.GetDirectoryName(exePath)));
	}

	[Test]
	public void WebViewShouldUseTheUserDataFolder()
	{
		if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER")))
			Assert.Ignore("WEBVIEW2_USER_DATA_FOLDER is set, which overrides the folder");

		ShownAsync(form => form, async form =>
		{
			var webView = new WebView();
			var handler = webView.Handler as WebView2Handler;
			if (handler == null)
				Assert.Ignore("WebView2 is not the active WebView handler, is the runtime installed?");

			var control = handler.Control;
			var ready = new TaskCompletionSource<bool>();
			control.CoreWebView2InitializationCompleted += (sender, e) => ready.TrySetResult(e.IsSuccess);

			var window = new Form { ClientSize = new Size(200, 200), Content = webView };
			try
			{
				window.Show();
				await Task.WhenAny(ready.Task, Task.Delay(10000));
				Assert.That(ready.Task.IsCompleted, Is.True, "WebView2 did not finish initializing in time");
				Assert.That(ready.Task.Result, Is.True, "WebView2 failed to initialize");

				var actual = control.CoreWebView2.Environment.UserDataFolder;
				TestContext.Out.WriteLine($"WebView2 is using: {actual}");
				Assert.That(Path.GetFullPath(actual).TrimEnd('\\'), Is.EqualTo(Path.GetFullPath(WebView2Handler.UserDataFolder).TrimEnd('\\')).IgnoreCase);
				Assert.That(Directory.Exists(Path.Combine(actual, "EBWebView")), Is.True, "WebView2 did not create its data in the folder");
			}
			finally
			{
				window.Close();
			}
		}, timeout: 20000);
	}
}
