using System;
using NUnit.Framework;
using Eto.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
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
	}
}
