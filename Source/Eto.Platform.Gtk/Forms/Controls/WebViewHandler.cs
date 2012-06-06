
//#define USE_MONO_WEBBROWSER

using System;
using Eto.Forms;
using System.Net;
using System.Web;
using System.Threading;

#if USE_MONO_WEBBROWSER
using Mono.WebBrowser;
#endif

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	
	
	public class WebViewHandler : GtkControl<Gtk.ScrolledWindow, WebView>, IWebView
	{
#if USE_MONO_WEBBROWSER
		
		Mono.WebBrowser.IWebBrowser webView;
		
		public WebViewHandler ()
		{
			Control = new Gtk.ScrolledWindow ();
			
			//Environment.SetEnvironmentVariable("MONO_BROWSER_ENGINE", "webkit");

			webView = Mono.WebBrowser.Manager.GetNewInstance (Mono.WebBrowser.Platform.Gtk);
			
			// TODO: how do we get the handle to the Gtk widget?!
			Control.Add (webView.Window as Gtk.Widget);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				webView.LoadFinished += delegate(object sender, LoadFinishedEventArgs e) {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (new Uri (e.Uri)));
				};
				break;
			case WebView.DocumentLoadingEvent:
				webView.NavigationRequested += delegate(object sender, NavigationRequestedEventArgs e) {
					var args = new WebViewLoadingEventArgs (new Uri (e.Uri));
					Widget.OnDocumentLoading (args);
					e.Cancel = args.Cancel;
				};
				break;
			case WebView.DocumentTitleChangedEvent:
				webView.LoadFinished += delegate(object sender, LoadFinishedEventArgs e) {
					Widget.OnDocumentTitleChanged(new WebViewTitleEventArgs(webView.Document.Title));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public Uri Url {
			get { return new Uri (webView.Document.Url); }
			set {
				if (value != null) {
					webView.Navigation.Go (value.AbsoluteUri);
				} else 
					webView.Render (string.Empty);
			}
		}
		
		public string DocumentTitle {
			get {
				return webView.Document.Title;
			}
		}
		
		public void ExecuteScript (string script)
		{
			
			webView.ExecuteScript(script);
		}

		public void LoadHtml (string html, Uri baseUri)
		{
			if (baseUri == null)
				webView.Render (html);
			else
				webView.Render (html, baseUri.AbsoluteUri, "text/html");
		}
		
		public void Stop ()
		{
			webView.Navigation.Stop ();
		}
		
		public void Reload ()
		{
			webView.Navigation.Reload ();
		}

		public void GoBack ()
		{
			webView.Navigation.Back ();
		}

		public void GoForward ()
		{
			webView.Navigation.Forward ();
		}

		public bool CanGoBack {
			get { return webView.Navigation.CanGoBack; }
		}

		public bool CanGoForward {
			get { return webView.Navigation.CanGoForward; }
		}
#else
		WebKit.WebView webView;
		ManualResetEventSlim returnResetEvent = new ManualResetEventSlim();
		string scriptReturnValue;
		const string EtoReturnPrefix = "etoscriptreturn://";
		
		public WebViewHandler ()
		{
			Control = new Gtk.ScrolledWindow ();

			try {
				webView = new WebKit.WebView (); 
			}
			catch (Exception ex)
			{
				throw new EtoException("GTK WebView is only supported on Linux, and requires webkit-sharp", ex);
			}
			
			Control.Add (webView);
			HandleEvent (WebView.DocumentLoadingEvent);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				webView.LoadFinished += delegate(object o, WebKit.LoadFinishedArgs args) {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (args.Frame.Uri != null ? new Uri (args.Frame.Uri) : null));
				};
				break;
			case WebView.DocumentLoadingEvent:
				webView.NavigationRequested += delegate(object o, WebKit.NavigationRequestedArgs args) {
					if (args.Request.Uri.StartsWith (EtoReturnPrefix)) {
						// pass back the response to ExecuteScript()
						this.scriptReturnValue = HttpUtility.UrlDecode (args.Request.Uri.Substring (EtoReturnPrefix.Length));
						returnResetEvent.Set ();
						args.RetVal = WebKit.NavigationResponse.Ignore;
					}
					else {
						var e = new WebViewLoadingEventArgs (new Uri (args.Request.Uri));
						Widget.OnDocumentLoading (e);
						if (e.Cancel)
							args.RetVal = WebKit.NavigationResponse.Ignore;
						else
							args.RetVal = WebKit.NavigationResponse.Accept;
					}
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
		
		public string ExecuteScript (string script)
		{
			// no access to DOM or return value, so get return value via URL (limited length, but better than nothing)
			var getResultScript = @"try {{ var fn = function () {{ {0} }}; window.location.href = '" + EtoReturnPrefix + @"' + encodeURI(fn()); }} catch (e) {{ window.location.href = '" + EtoReturnPrefix + @"'; }}";
			returnResetEvent.Reset ();
			webView.ExecuteScript(string.Format (getResultScript, script));
			while (!returnResetEvent.Wait (0)) {
				Gtk.Application.RunIteration();
			}
			return scriptReturnValue;
		}

		public void LoadHtml (string html, Uri baseUri)
		{
			webView.LoadHtmlString (html, baseUri != null ? baseUri.AbsoluteUri : null);
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

#endif

	}
}

