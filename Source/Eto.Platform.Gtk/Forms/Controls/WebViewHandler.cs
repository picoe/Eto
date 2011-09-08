using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class WebViewHandler : GtkControl<Gtk.ScrolledWindow, WebView>, IWebView
	{
		WebKit.WebView webView;
		
		public WebViewHandler ()
		{
			Control = new Gtk.ScrolledWindow ();

			webView = new WebKit.WebView (); 
			
			Control.Add (webView);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				webView.LoadFinished += delegate(object o, WebKit.LoadFinishedArgs args) {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (new Uri (args.Frame.Uri)));
				};
				break;
			case WebView.DocumentLoadingEvent:
				webView.NavigationRequested += delegate(object o, WebKit.NavigationRequestedArgs args) {
					var e = new WebViewLoadingEventArgs (new Uri (args.Request.Uri));
					Widget.OnDocumentLoading (e);
					if (e.Cancel)
						args.RetVal = WebKit.NavigationResponse.Ignore;
					else
						args.RetVal = WebKit.NavigationResponse.Accept;
				};
				break;
			case WebView.DocumentTitleChangedEvent:
				webView.TitleChanged += delegate(object o, WebKit.TitleChangedArgs args) {
					Widget.OnDocumentTitleChanged(new WebViewTitleEventArgs(args.Title));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public Uri Url {
			get { return new Uri (webView.Uri); }
			set {
				if (value != null) {
					webView.LoadUri (value.AbsoluteUri);
				} else 
					webView.LoadHtmlString (string.Empty, string.Empty);
			}
		}
		
		public string DocumentTitle {
			get {
				return webView.MainFrame.Title;
			}
		}
		
		public void ExecuteScript (string script)
		{
			
			webView.ExecuteScript(script);
		}

		public void LoadHtml (string html)
		{
			webView.LoadHtmlString (html, null);
		}
		
		public void Stop ()
		{
			webView.StopLoading ();
		}
		
		public void Reload ()
		{
			webView.Reload ();
		}

		public void GoBack ()
		{
			webView.GoBack ();
		}

		public void GoForward ()
		{
			webView.GoForward ();
		}

		public bool CanGoBack {
			get { return webView.CanGoBack (); }
		}

		public bool CanGoForward {
			get { return webView.CanGoForward (); }
		}

	}
}

