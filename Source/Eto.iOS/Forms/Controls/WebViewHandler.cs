using System;
using UIKit;
using Eto.Forms;

namespace Eto.iOS.Forms.Controls
{
	public class WebViewHandler : IosView<UIWebView, WebView, WebView.ICallback>, WebView.IHandler
	{
		public override UIView ContainerControl { get { return Control; } }

		public WebViewHandler()
		{
			Control = new UIWebView();
		}

		public void LoadHtml (string html, Uri baseUri)
		{
			Control.LoadHtmlString (html, baseUri.ToNSUrl ());
		}

		public void GoBack ()
		{
			Control.GoBack ();
		}

		public void GoForward ()
		{
			Control.GoForward ();
		}

		public void Stop ()
		{
			Control.StopLoading ();
		}

		public void Reload ()
		{
			Control.Reload ();
		}

		public string ExecuteScript (string script)
		{
			return null;
		}

		public void ShowPrintDialog ()
		{
		}

		public Uri Url {
			get { return Control.Request.Url.ToUri (); }
			set { }
		}

		public bool CanGoBack {
			get { return Control.CanGoBack; }
		}

		public bool CanGoForward {
			get { return Control.CanGoForward; }
		}

		public string DocumentTitle {
			get { return null; }
		}

		public bool BrowserContextMenuEnabled
		{
			get;
			set;
		}
	}
}

