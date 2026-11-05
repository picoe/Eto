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
				var webView = new WebView { Size = new Size(200, 200) };
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
					Assert.That(value, Is.EqualTo("hello"), "#1");
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
					Assert.That(value, Is.EqualTo("hello"), "#1");
				});
		}

		[Test]
		public void LoadHtmlLargerThan2MBShouldLoad()
		{
			// WebView2's NavigateToString refuses pages over 2 MB
			var html = "<html><head><title>big</title></head><body><div>" + new string('x', 3_000_000) + "</div></body></html>";
			WebTest(
				webView => webView.LoadHtml(html),
				async webView =>
				{
					var title = await webView.ExecuteScriptAsync("return document.title;");
					Assert.That(title, Is.EqualTo("big"), "#1");
					var length = await webView.ExecuteScriptAsync("return document.body.innerText.length;");
					Assert.That(length, Is.EqualTo("3000000"), "#2");
				});
		}

		[Test]
		public void LoadHtmlWithBaseUriShouldLoadRelativeFiles()
		{
			var directory = Path.Combine(Path.GetTempPath(), "Eto.WebViewTests." + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(directory);
			try
			{
				File.WriteAllText(Path.Combine(directory, "script.js"), "window.etoValue = 'loaded';");
				var baseUri = new Uri(directory + Path.DirectorySeparatorChar);
				WebTest(
					webView => webView.LoadHtml("<html><head><script src=\"script.js\"></script></head><body></body></html>", baseUri),
					async webView =>
					{
						var value = await webView.ExecuteScriptAsync("return window.etoValue;");
						Assert.That(value, Is.EqualTo("loaded"), "#1");
					});
			}
			finally
			{
				Directory.Delete(directory, true);
			}
		}

		void ResourceHandlerTest(Action<WebView> init, Func<WebView, Task> test)
		{
			var supported = true;
			WebTest(
				webView =>
				{
					supported = webView.SupportsResourceHandlers;
					if (supported)
						init(webView);
					else
						webView.LoadHtml("<html></html>"); // still loads, so the form closes
				},
				webView => supported ? test(webView) : Task.CompletedTask);
			if (!supported)
				Assert.Ignore("This platform does not support WebView resource handlers");
		}

		[Test]
		public void ResourceHandlerShouldServeContent()
		{
			var requests = new List<WebViewResourceRequest>();
			ResourceHandlerTest(
				webView =>
				{
					webView.Url = webView.AddResourceHandler("eto-test.example", request =>
					{
						requests.Add(request);
						switch (request.Uri.AbsolutePath)
						{
							case "/":
								return new WebViewResourceResponse("<html><head><title>served</title><script src=\"app.js\"></script></head><body></body></html>", "text/html");
							case "/app.js":
								return new WebViewResourceResponse("window.etoValue = 'loaded';", "text/javascript");
							default:
								return null;
						}
					});
				},
				async webView =>
				{
					Assert.That(await webView.ExecuteScriptAsync("return document.title;"), Is.EqualTo("served"), "#1");
					Assert.That(await webView.ExecuteScriptAsync("return window.etoValue;"), Is.EqualTo("loaded"), "#2");
					// the browser may also ask for /favicon.ico
					Assert.That(requests.Select(r => r.Uri.AbsolutePath), Is.SupersetOf(new[] { "/", "/app.js" }), "#3");
					Assert.That(requests.Select(r => r.Method), Is.All.EqualTo("GET"), "#4");
					Assert.That(requests.Select(r => r.Uri.Scheme), Is.All.EqualTo("eto"), "#5");
				});
		}

		[Test]
		public void ResourceHandlerHeadersShouldReachPage()
		{
			ResourceHandlerTest(
				webView =>
				{
					webView.Url = webView.AddResourceHandler("eto-test.example", request =>
					{
						switch (request.Uri.AbsolutePath)
						{
							case "/":
								return new WebViewResourceResponse("<html><body></body></html>", "text/html");
							case "/api":
								var response = new WebViewResourceResponse("{}", "application/json") { StatusCode = 201 };
								response.Headers["X-Eto"] = "yes";
								return response;
							default:
								return null;
						}
					});
				},
				async webView =>
				{
					await webView.ExecuteScriptAsync("fetch('/api').then(r => window.result = r.status + '|' + r.headers.get('Content-Type') + '|' + r.headers.get('X-Eto')).catch(e => window.result = 'error: ' + e); return null;");
					// fetch is asynchronous, so poll until the page has the answer
					string result = null;
					for (int i = 0; i < 100 && string.IsNullOrEmpty(result); i++)
					{
						await Task.Delay(50);
						result = await webView.ExecuteScriptAsync("return window.result || '';");
					}
					Assert.That(result, Is.EqualTo("201|application/json; charset=utf-8|yes"));
				});
		}

		[Test]
		public void ResourceHandlerFailuresShouldBeErrorsNotCrashes()
		{
			ResourceHandlerTest(
				webView =>
				{
					webView.Url = webView.AddResourceHandler("eto-test.example", request =>
					{
						switch (request.Uri.AbsolutePath)
						{
							case "/":
								return new WebViewResourceResponse(
									"<html><head>" +
									"<script src=\"missing.js\" onerror=\"window.missing = 'error'\"></script>" +
									"<script src=\"throws.js\" onerror=\"window.throws = 'error'\"></script>" +
									"</head><body></body></html>", "text/html");
							case "/throws.js":
								throw new InvalidOperationException("Handler failure");
							default:
								return null;
						}
					});
				},
				async webView =>
				{
					Assert.That(await webView.ExecuteScriptAsync("return window.missing;"), Is.EqualTo("error"), "#1");
					Assert.That(await webView.ExecuteScriptAsync("return window.throws;"), Is.EqualTo("error"), "#2");
				});
		}

		[Test]
		public void WebViewClosedAsChildShouldNotCrash()
		{
			var mre = new ManualResetEvent(false);
			Invoke(() =>
			{
				var parent = new Form();
				parent.ClientSize = new Size(300, 300);
				parent.Content = "This is the parent that will be closed";
				parent.Shown += (sender, e) =>
				{
					var child = new Form();
					child.ClientSize = new Size(300, 300);
					var webView = new WebView { Url = new Uri("http://example.com") };
					child.Content = webView;
					child.Owner = parent;
					webView.DocumentLoaded += async (s2, e2) =>
					{
						// let the web view settle after loading before the owner window is closed under it
						await Task.Delay(250);
						parent.Close();
					};
					child.Show();
				};
				parent.Show();
				parent.Closed += (sender, e) =>
				{
					mre.Set();
				};
			});

			mre.WaitOne(5000);
		}

		[Test]
		public void MessageReceivedShouldWork()
		{
			WebTest(
				webView =>
				{
					webView.LoadHtml("<html><head><title>woo</title></head><body><div>Hello!</div></body></html>");
				},
				async webView =>
				{
					Exception exception = null;
					bool messageRecieved = false;
					var mre = new ManualResetEvent(false);
					webView.MessageReceived += (sender, e) =>
					{
						try
						{
							Assert.That(e.Message, Is.EqualTo("hello"), "#1");
							messageRecieved = true;
						}
						catch (Exception ex)
						{
							exception = ex;
						}
						finally
						{
							mre.Set();
						}
					};
					await webView.ExecuteScriptAsync("window.eto.postMessage('hello');");
					mre.WaitOne(2000);
					Assert.That(messageRecieved, Is.True, "#2");
					
					if (exception != null)
						ExceptionDispatchInfo.Capture(exception).Throw();
				});
		}
	}
}
