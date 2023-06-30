using NUnit.Framework;
using System.Runtime.ExceptionServices;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class WebViewTests : TestBase
	{
		void WebTest(Action<WebView> init, Action<WebView> test)
		{
			WebTest(init, webView =>
			{
				test(webView);
				return Task.CompletedTask;
			});
		}

		void WebTest(Action<WebView> init, Func<WebView, Task> test)
		{
			Exception exception = null;
			Form(form =>
			{
				var webView = new WebView();
				init(webView);
				webView.DocumentLoaded += async (sender, e) =>
				{
					try
					{
						await test(webView);
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						form.Close();
					}
				};
				form.Content = webView;
			});
			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();

		}

		[Test]
		public void ExecuteScriptShouldReturnValue()
		{
			WebTest(
				webView =>
				{
					webView.LoadHtml("<html><head><title>woo</title></head><body><div>Hello!</div></body></html>");
				},
				webView =>
				{
					var value = webView.ExecuteScript("return 'hello';");
					Assert.AreEqual("hello", value, "#1");
				});
		}

		[Test]
		public void ExecuteScriptAsyncShouldReturnValue()
		{
			WebTest(
				webView =>
				{
					webView.LoadHtml("<html><head><title>woo</title></head><body><div>Hello!</div></body></html>");
				},
				async webView =>
				{
					var value = await webView.ExecuteScriptAsync("return 'hello';");
					Assert.AreEqual("hello", value, "#1");
				});
		}
		
		[Test]
		public void WebViewClosedAsChildShouldNotCrash()
		{
			var mre = new ManualResetEvent(false);
			Invoke(() => {
				var parent = new Form();
				parent.ClientSize = new Size(300, 300);
				parent.Content = "This is the parent that will be closed";
				parent.Shown += (sender, e) => {
					var child = new Form();
					child.ClientSize = new Size(300, 300);
					var webView = new WebView { Url = new Uri("http://example.com") };
					child.Content = webView;
					child.Owner = parent;
					webView.DocumentLoaded += async (s2, e2) => {
						await Task.Delay(2000);
						parent.Close();
					};
					child.Show();
				};
				parent.Show();
				parent.Closed += (sender, e) => {
					mre.Set();	
				};
			});
			
			mre.WaitOne(5000);
		}
	}
}
