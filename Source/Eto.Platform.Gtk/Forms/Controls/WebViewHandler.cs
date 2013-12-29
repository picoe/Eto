using System;
using Eto.Forms;
using System.Web;
using System.Threading;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class WebViewHandler : GtkControl<EtoWebView, WebView>, IWebView
	{
		readonly Gtk.ScrolledWindow scroll;
		readonly ManualResetEventSlim returnResetEvent = new ManualResetEventSlim ();
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
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent (WebView.DocumentLoadingEvent);
		}
		
		public override void AttachEvent (string id)
		{
			switch (id) {
			case WebView.NavigatedEvent:
				HandleEvent (WebView.DocumentLoadedEvent);
				break;
			case WebView.DocumentLoadedEvent:
				Control.LoadFinished += (o, args) => {
					var uri = args.Frame.Uri != null ? new Uri (args.Frame.Uri) : null;
					var e = new WebViewLoadedEventArgs (uri);
					if (args.Frame == Control.MainFrame)
						Widget.OnNavigated (e);
					Widget.OnDocumentLoaded (e);
				};
				break;
			case WebView.DocumentLoadingEvent:
#if GTK2
				Control.NavigationRequested += (sender, args) => {
					if (args.Request.Uri.StartsWith(EtoReturnPrefix, StringComparison.Ordinal)) {
						// pass back the response to ExecuteScript()
						scriptReturnValue = HttpUtility.UrlDecode (args.Request.Uri.Substring (EtoReturnPrefix.Length));
						returnResetEvent.Set ();
						args.RetVal = WebKit.NavigationResponse.Ignore;
					} else {
						var e = new WebViewLoadingEventArgs (new Uri(args.Request.Uri), false);
						Widget.OnDocumentLoading (e);
						args.RetVal = e.Cancel ? WebKit.NavigationResponse.Ignore : WebKit.NavigationResponse.Accept;
					}
				};
#else
				Control.NavigationPolicyDecisionRequested += (sender, args) => {
					if (args.Request.Uri.StartsWith (EtoReturnPrefix)) {
						// pass back the response to ExecuteScript()
						this.scriptReturnValue = HttpUtility.UrlDecode (args.Request.Uri.Substring (EtoReturnPrefix.Length));
						returnResetEvent.Set ();
						args.PolicyDecision.Ignore();
					} else {
						var e = new WebViewLoadingEventArgs (new Uri(args.Request.Uri), false);
						Widget.OnDocumentLoading (e);
						if (e.Cancel)
							args.PolicyDecision.Ignore();
						else
							args.PolicyDecision.Use();
					}
				};
#endif
				break;
			case WebView.OpenNewWindowEvent:
				// note: requires libwebkitgtk 1.1.4+
				Control.NewWindowPolicyDecisionRequested += (sender, args) => {
					var e = new WebViewNewWindowEventArgs (new Uri(args.Request.Uri), args.Frame.Name);
					Widget.OnOpenNewWindow (e);
#if GTK2
					if (e.Cancel) args.Decision.Ignore ();
					else args.Decision.Use ();
#else
					if (e.Cancel) args.PolicyDecision.Ignore();
					else args.PolicyDecision.Use ();
#endif
					args.RetVal = true;
				};
				break;
			case WebView.DocumentTitleChangedEvent:
				Control.TitleChanged += (sender, args) => Widget.OnDocumentTitleChanged(new WebViewTitleEventArgs(args.Title));
				break;
			default:
				base.AttachEvent (id);
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
