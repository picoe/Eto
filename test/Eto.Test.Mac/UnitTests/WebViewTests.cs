using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class WebViewTests : TestBase
	{
		/// <summary>
		/// The print sheet calls back into a delegate that AppKit doesn't retain, so a GC while the sheet was open
		/// used to crash when it closed.
		/// </summary>
		[Test]
		public void ClosingPrintDialogAfterGarbageCollectionShouldNotCrash()
		{
			WebView webView = null;
			ShownAsync(form =>
			{
				webView = new WebView { Size = new Size(200, 200) };
				form.Content = webView;
			},
			async () =>
			{
				var loaded = WaitEventAsync<WebViewLoadedEventArgs>(h => webView.DocumentLoaded += h);
				webView.LoadHtml("<html><body><div>Print me!</div></body></html>");
				await loaded;

				var window = (NSWindow)webView.ParentWindow.ControlObject;
				webView.ShowPrintDialog();
				Assert.That(await WaitUntil(() => window.AttachedSheet != null), Is.True, "#1 - print sheet should be shown");

				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();

				window.EndSheet(window.AttachedSheet, NSModalResponse.Cancel);
				Assert.That(await WaitUntil(() => window.AttachedSheet == null), Is.True, "#2 - print sheet should be closed");

				// let AppKit finish the print operation, which is where it used to crash
				await Task.Delay(500);
			});
		}
	}
}
