using System;
using Eto.Forms;
using System.Net;
using System.Web;
using System.Threading;
using Eto.Platform.GtkSharp.CustomControls;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class WebViewHandler : GtkControl<EtoWebView, WebView>, IWebView
	{
		Gtk.ScrolledWindow scroll;
		ManualResetEventSlim returnResetEvent = new ManualResetEventSlim ();
		string scriptReturnValue;
		const string EtoReturnPrefix = "etoscriptreturn://";


		public Gtk.ScrolledWindow Scroll
		{
			get { return scroll; }
		}

		public override Gtk.Widget ContainerControl
		{
			get { return scroll; }
		}
		
		public WebViewHandler ()
		{
			scroll = new Gtk.ScrolledWindow ();

			try {
				Control = new EtoWebView (); 
			} catch (Exception ex) {
				throw new EtoException ("GTK WebView is only supported on Linux, and requires webkit-sharp", ex);
			}
			Control.PopulatePopup += (o, args) => {
				if (BrowserContextMenuEnabled)
					return;
				// don't allow context menu by default
				foreach (var child in args.Menu.Children) {
					args.Menu.Remove (child);
				}
			};
			scroll.Add (Control);
			HandleEvent (WebView.DocumentLoadingEvent);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				Control.LoadFinished += delegate(object o, WebKit.LoadFinishedArgs args) {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (args.Frame.Uri != null ? new Uri (args.Frame.Uri) : null));
				};
				break;
			case WebView.DocumentLoadingEvent:
				Control.NavigationRequested += delegate(object o, WebKit.NavigationRequestedArgs args) {
					if (args.Request.Uri.StartsWith (EtoReturnPrefix)) {
						// pass back the response to ExecuteScript()
						this.scriptReturnValue = HttpUtility.UrlDecode (args.Request.Uri.Substring (EtoReturnPrefix.Length));
						returnResetEvent.Set ();
						args.RetVal = WebKit.NavigationResponse.Ignore;
					} else {
						var e = new WebViewLoadingEventArgs (new Uri (args.Request.Uri), false);
						Widget.OnDocumentLoading (e);
						if (e.Cancel)
							args.RetVal = WebKit.NavigationResponse.Ignore;
						else
							args.RetVal = WebKit.NavigationResponse.Accept;
					}
				};
				break;
			case WebView.OpenNewWindowEvent:
				// note: requires libwebkitgtk 1.1.4+
				Control.NewWindowPolicyDecisionRequested += (sender, args) => {
					var e = new WebViewNewWindowEventArgs (new Uri(args.Request.Uri), args.Frame.Name);
					Widget.OnOpenNewWindow (e);
					if (e.Cancel) args.Decision.Ignore ();
					else args.Decision.Use ();
					args.RetVal = true;
				};
				break;
			case WebView.DocumentTitleChangedEvent:
				Control.TitleChanged += delegate(object o, WebKit.TitleChangedArgs args) {
					Widget.OnDocumentTitleChanged (new WebViewTitleEventArgs (args.Title));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public Uri Url
		{
			get { return new Uri (Control.Uri); }
			set {
				if (value != null) {
					Control.LoadUri (value.AbsoluteUri);
				} else 
					Control.LoadHtmlString (string.Empty, string.Empty);
			}
		}
		
		public string DocumentTitle
		{
			get {
				return Control.MainFrame.Title;
			}
		}
		
		public string ExecuteScript (string script)
		{
			// no access to DOM or return value, so get return value via URL (limited length, but better than nothing)
			var getResultScript = @"try {{ var fn = function () {{ {0} }}; window.location.href = '" + EtoReturnPrefix + @"' + encodeURI(fn()); }} catch (e) {{ window.location.href = '" + EtoReturnPrefix + @"'; }}";
			returnResetEvent.Reset ();
			Control.ExecuteScript (string.Format (getResultScript, script));
			while (!returnResetEvent.Wait (0)) {
				Gtk.Application.RunIteration ();
			}
			return scriptReturnValue;
		}

		public void LoadHtml (string html, Uri baseUri)
		{
			Control.LoadHtmlString (html, baseUri != null ? baseUri.AbsoluteUri : null);
		}
		
		public void Stop ()
		{
			Control.StopLoading ();
		}
		
		public void Reload ()
		{
			Control.Reload ();
		}

		public void GoBack ()
		{
			Control.GoBack ();
		}

		public void GoForward ()
		{
			Control.GoForward ();
		}

		public bool CanGoBack
		{
			get { return Control.CanGoBack (); }
		}

		public bool CanGoForward
		{
			get { return Control.CanGoForward (); }
		}

		public void ShowPrintDialog ()
		{
			Control.ExecuteScript ("print();");
		}

		public bool BrowserContextMenuEnabled
		{
			get; set;
		}
	}
}
