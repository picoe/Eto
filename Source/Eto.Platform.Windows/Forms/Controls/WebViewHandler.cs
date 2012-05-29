using System;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Platform.CustomControls;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class WebViewHandler : WindowsControl<SWF.WebBrowser, WebView>, IWebView
	{
		public WebViewHandler ()
		{
			this.Control = new SWF.WebBrowser { IsWebBrowserContextMenuEnabled = false };
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				this.Control.DocumentCompleted += delegate(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e) {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (e.Url));
				};
				break;
			case WebView.DocumentLoadingEvent:
				this.Control.Navigating += delegate(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e) {
					var args = new WebViewLoadingEventArgs (e.Url);
					Widget.OnDocumentLoading (args);
					e.Cancel = args.Cancel;
				};
				break;
			case WebView.DocumentTitleChangedEvent:
				this.Control.DocumentTitleChanged += delegate {
					Widget.OnDocumentTitleChanged (new WebViewTitleEventArgs (Control.DocumentTitle));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
			
		}

		public Uri Url {
			get { return this.Control.Url; }
			set { this.Control.Url = value; }
		}
		
		public string DocumentTitle {
			get {
				return this.Control.DocumentTitle;
			}
		}
		
		public void ExecuteScript (string script)
		{
			this.Control.Document.InvokeScript ("execScript", new object[] { script, "JavaScript" });
		}
		
		public void Stop ()
		{
			this.Control.Stop ();
		}
		
		public void Reload ()
		{
			this.Control.Refresh ();
		}
		
		public void GoBack ()
		{
			this.Control.GoBack ();
		}
		
		public bool CanGoBack {
			get {
				return this.Control.CanGoBack;
			}
		}
		
		public void GoForward ()
		{
			this.Control.GoForward ();
		}
		
		public bool CanGoForward {
			get {
				return this.Control.CanGoForward;
			}
		}
		
		HttpServer server;

		public void LoadHtml (string html, Uri baseUri)
		{
			if (baseUri != null) {
				if (server == null)
					server = new HttpServer ();
				server.SetHtml (html, baseUri != null ? baseUri.LocalPath : null);
				Control.Navigate (server.Url);
			}
			else
				this.Control.DocumentText = html;

		}
		
	}
}

