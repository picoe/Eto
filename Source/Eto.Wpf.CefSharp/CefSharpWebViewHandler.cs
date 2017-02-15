using Eto.CustomControls;
using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs = global::CefSharp;

namespace Eto.Wpf.CefSharp
{
	public class CefSharpWebViewHandler : WpfControl<cs.Wpf.ChromiumWebBrowser, WebView, WebView.ICallback>, WebView.IHandler, cs.IRequestHandler
	{
		public CefSharpWebViewHandler()
		{
			Control = new cs.Wpf.ChromiumWebBrowser();
		}

		public override System.Windows.Size GetPreferredSize(System.Windows.Size constraint)
		{
			return base.GetPreferredSize(new System.Windows.Size(100, 100));
		}

		public bool BrowserContextMenuEnabled { get; set; }

		public bool CanGoBack => Control.CanGoBack;

		public bool CanGoForward => Control.CanGoForward;

		public string DocumentTitle => Control.Title;

		public Uri Url
		{
			get
			{
				Uri url;
				if (Uri.TryCreate(Control.Address, UriKind.RelativeOrAbsolute, out url))
					return url;
				return null;
			}
			set
			{
				Control.Load(value?.AbsoluteUri);
			}
		}

		public string ExecuteScript(string script)
		{
			script = $"(function(){{{script}}})();";
			var response = Control.GetBrowser()?.MainFrame.EvaluateScriptAsync(script).Result;
			return Convert.ToString(response?.Result);
		}

		public void GoBack()
		{
			Control.BackCommand.Execute(null);
		}

		public void GoForward()
		{
			Control.ForwardCommand.Execute(null);
		}

		HttpServer server;
		public void LoadHtml(string html, Uri baseUri)
		{
			if (server == null)
				server = new HttpServer();
			server.SetHtml(html, baseUri != null ? baseUri.LocalPath : null);
			if (Control.Address == server.Url.AbsoluteUri)
				Control.ReloadCommand.Execute(null);
			else
				Control.Address = server.Url.AbsoluteUri;
		}

		public void Reload()
		{
			Control.ReloadCommand.Execute(null);
		}

		public void ShowPrintDialog()
		{
			Control.PrintCommand.Execute(null);
		}

		public void Stop()
		{
			Control.StopCommand.Execute(null);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case WebView.DocumentLoadedEvent:
					Control.FrameLoadEnd += (sender, e) =>
					{
						if (e.Frame.IsMain)
						{
							var url = e.Browser.MainFrame.Url;
							Application.Instance.AsyncInvoke(() => Callback.OnDocumentLoaded(Widget, new WebViewLoadedEventArgs(new Uri(url))));
						}
					};
					break;
				case WebView.DocumentLoadingEvent:
					Control.RequestHandler = this;
					break;
				case WebView.DocumentTitleChangedEvent:
					Control.TitleChanged += (sender, e) => Callback.OnDocumentTitleChanged(Widget, new WebViewTitleEventArgs(e.NewValue as string));
					break;
				case WebView.OpenNewWindowEvent:
					// todo
					break;
				case WebView.NavigatedEvent:
					Control.FrameLoadStart += (sender, e) =>
					{
						var url = e.Frame.Url;
						if (!string.IsNullOrEmpty(url) && e.Frame.IsMain)
						{
							Application.Instance.AsyncInvoke(() => Callback.OnNavigated(Widget, new WebViewLoadedEventArgs(new Uri(url))));
						}
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		bool cs.IRequestHandler.OnBeforeBrowse(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.IFrame frame, cs.IRequest request, bool isRedirect)
		{
			if (!string.IsNullOrEmpty(frame.Url))
			{
				var e = new WebViewLoadingEventArgs(new Uri(frame.Url), frame.IsMain);
				Application.Instance.Invoke(() => Callback.OnDocumentLoading(Widget, e));
				return e.Cancel;
			}
			return false;
		}

		bool cs.IRequestHandler.OnOpenUrlFromTab(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.IFrame frame, string targetUrl, cs.WindowOpenDisposition targetDisposition, bool userGesture)
		{
			return false;
		}

		bool cs.IRequestHandler.OnCertificateError(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.CefErrorCode errorCode, string requestUrl, cs.ISslInfo sslInfo, cs.IRequestCallback callback)
		{
			return false;
		}

		void cs.IRequestHandler.OnPluginCrashed(cs.IWebBrowser browserControl, cs.IBrowser browser, string pluginPath)
		{
		}

		cs.CefReturnValue cs.IRequestHandler.OnBeforeResourceLoad(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.IFrame frame, cs.IRequest request, cs.IRequestCallback callback)
		{
			return cs.CefReturnValue.Continue;
		}

		bool cs.IRequestHandler.GetAuthCredentials(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.IFrame frame, bool isProxy, string host, int port, string realm, string scheme, cs.IAuthCallback callback)
		{
			return false;
		}

		void cs.IRequestHandler.OnRenderProcessTerminated(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.CefTerminationStatus status)
		{
		}

		bool cs.IRequestHandler.OnQuotaRequest(cs.IWebBrowser browserControl, cs.IBrowser browser, string originUrl, long newSize, cs.IRequestCallback callback)
		{
			return false;
		}

		void cs.IRequestHandler.OnResourceRedirect(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.IFrame frame, cs.IRequest request, ref string newUrl)
		{
		}

		bool cs.IRequestHandler.OnProtocolExecution(cs.IWebBrowser browserControl, cs.IBrowser browser, string url)
		{
			return false;
		}

		void cs.IRequestHandler.OnRenderViewReady(cs.IWebBrowser browserControl, cs.IBrowser browser)
		{
		}

		bool cs.IRequestHandler.OnResourceResponse(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.IFrame frame, cs.IRequest request, cs.IResponse response)
		{
			return false;
		}

		cs.IResponseFilter cs.IRequestHandler.GetResourceResponseFilter(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.IFrame frame, cs.IRequest request, cs.IResponse response)
		{
			return null;
		}

		void cs.IRequestHandler.OnResourceLoadComplete(cs.IWebBrowser browserControl, cs.IBrowser browser, cs.IFrame frame, cs.IRequest request, cs.IResponse response, cs.UrlRequestStatus status, long receivedContentLength)
		{
		}
	}
}
