#if GTK2
using System;
using System.Globalization;
using System.Threading;
using Eto.Forms;
#if GTK3
using NewWindowPolicyDecisionRequestedArgs = WebKit.NewWindowPolicyDecisionRequestedArgs;
#endif

namespace Eto.GtkSharp.Forms.Controls
{
	public class WebViewHandler : GtkControl<EtoWebView, WebView, WebView.ICallback>, WebView.IHandler
	{
		readonly Gtk.ScrolledWindow scroll;
		readonly ManualResetEventSlim returnResetEvent = new ManualResetEventSlim();
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

		public WebViewHandler()
		{
			scroll = new Gtk.ScrolledWindow();

			try
			{
				Control = new EtoWebView(); 
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "GTK WebView is only supported on Linux, and requires webkit-sharp", ex));
			}
			scroll.Add(Control);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.PopulatePopup += Connector.HandlePopulatePopup;
			HandleEvent(WebView.DocumentLoadingEvent);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case WebView.NavigatedEvent:
					HandleEvent(WebView.DocumentLoadedEvent);
					break;
				case WebView.DocumentLoadedEvent:
					Control.LoadFinished += Connector.HandleLoadFinished;
					break;
				case WebView.DocumentLoadingEvent:
#if GTK2
					Control.NavigationRequested += Connector.HandleNavigationRequested;
#else
					Control.NavigationPolicyDecisionRequested += Connector.HandleNavigationPolicyDecisitionRequested;
#endif
					break;
				case WebView.OpenNewWindowEvent:
					// note: requires libwebkitgtk 1.1.4+
					Control.NewWindowPolicyDecisionRequested += Connector.HandleNewWindowPolicyDecisionRequested;
					break;
				case WebView.DocumentTitleChangedEvent:
					Control.TitleChanged += Connector.HandleTitleChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new WebViewConnector Connector { get { return (WebViewConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new WebViewConnector();
		}

		protected class WebViewConnector : GtkControlConnector
		{
			public new WebViewHandler Handler { get { return (WebViewHandler)base.Handler; } }

			public void HandlePopulatePopup(object o, WebKit.PopulatePopupArgs args)
			{
				var handler = Handler;
				if (handler.BrowserContextMenuEnabled)
					return;
				// don't allow context menu by default
				foreach (var child in args.Menu.Children)
				{
					args.Menu.Remove(child);
				}
			}

			public void HandleLoadFinished(object o, WebKit.LoadFinishedArgs args)
			{
				var handler = Handler;
				var uri = args.Frame.Uri != null ? new Uri(args.Frame.Uri) : null;
				var e = new WebViewLoadedEventArgs(uri);
				if (args.Frame == handler.Control.MainFrame)
					handler.Callback.OnNavigated(handler.Widget, e);
				handler.Callback.OnDocumentLoaded(handler.Widget, e);
			}
#if GTK2
			public void HandleNavigationRequested(object o, WebKit.NavigationRequestedArgs args)
			{
				var handler = Handler;
				if (args.Request.Uri.StartsWith(EtoReturnPrefix, StringComparison.Ordinal))
				{
					// pass back the response to ExecuteScript()
					handler.scriptReturnValue = Uri.UnescapeDataString(args.Request.Uri.Substring(EtoReturnPrefix.Length).Replace('+', ' '));
					handler.returnResetEvent.Set();
					args.RetVal = WebKit.NavigationResponse.Ignore;
				}
				else
				{
					var e = new WebViewLoadingEventArgs(new Uri(args.Request.Uri), false);
					handler.Callback.OnDocumentLoading(handler.Widget, e);
					args.RetVal = e.Cancel ? WebKit.NavigationResponse.Ignore : WebKit.NavigationResponse.Accept;
				}
			}
			
#else
			public void HandleNavigationPolicyDecisitionRequested(object o, WebKit.NavigationPolicyDecisionRequestedArgs args)
			{
				var handler = Handler;
				if (args.Request.Uri.StartsWith(EtoReturnPrefix, StringComparison.Ordinal))
				{
					// pass back the response to ExecuteScript()
					handler.scriptReturnValue = Uri.UnescapeDataString(args.Request.Uri.Substring(EtoReturnPrefix.Length).Replace('+', ' '));
					handler.returnResetEvent.Set();
					args.PolicyDecision.Ignore();
				}
				else
				{
					var e = new WebViewLoadingEventArgs(new Uri(args.Request.Uri), false);
					handler.Callback.OnDocumentLoading(handler.Widget, e);
					if (e.Cancel)
						args.PolicyDecision.Ignore();
					else
						args.PolicyDecision.Use();
				}
			}
#endif

			public void HandleNewWindowPolicyDecisionRequested(object sender, NewWindowPolicyDecisionRequestedArgs args)
			{
				var handler = Handler;
				var e = new WebViewNewWindowEventArgs(new Uri(args.Request.Uri), args.Frame.Name);
				handler.Callback.OnOpenNewWindow(handler.Widget, e);
				#if GTK2
				var decision = args.Decision;
				#else
				var decision = args.PolicyDecision;
				#endif
				if (decision != null)
				{
					if (e.Cancel)
						decision.Ignore();
					else
					{
						decision.Use();
						Application.Instance.Open(args.Request.Uri);
					}
				}
				args.RetVal = true;
			}

			public void HandleTitleChanged(object o, WebKit.TitleChangedArgs args)
			{
				Handler.Callback.OnDocumentTitleChanged(Handler.Widget, new WebViewTitleEventArgs(args.Title));
			}
		}

		public Uri Url
		{
			get { return new Uri(Control.Uri); }
			set
			{
				if (value != null)
				{
					Control.LoadUri(value.AbsoluteUri);
				}
				else
					#pragma warning disable 612
					Control.LoadHtmlString(string.Empty, string.Empty);
					#pragma warning restore 612
			}
		}

		public string DocumentTitle
		{
			get
			{
				return Control.MainFrame.Title;
			}
		}

		public string ExecuteScript(string script)
		{
			// no access to DOM or return value, so get return value via URL (limited length, but better than nothing)
			var getResultScript = @"try {{ var fn = function () {{ {0} }}; window.location.href = '" + EtoReturnPrefix + @"' + encodeURIComponent(fn()); }} catch (e) {{ window.location.href = '" + EtoReturnPrefix + @"'; }}";
			returnResetEvent.Reset();
			Control.ExecuteScript(string.Format(getResultScript, script));
			while (!returnResetEvent.Wait(0))
			{
				Gtk.Application.RunIteration();
			}
			return scriptReturnValue;
		}

		public void LoadHtml(string html, Uri baseUri)
		{
			#pragma warning disable 612
			Control.LoadHtmlString(html, baseUri != null ? baseUri.AbsoluteUri : null);
			#pragma warning restore 612
		}

		public void Stop()
		{
			Control.StopLoading();
		}

		public void Reload()
		{
			Control.Reload();
		}

		public void GoBack()
		{
			Control.GoBack();
		}

		public void GoForward()
		{
			Control.GoForward();
		}

		public bool CanGoBack
		{
			get { return Control.CanGoBack(); }
		}

		public bool CanGoForward
		{
			get { return Control.CanGoForward(); }
		}

		public void ShowPrintDialog()
		{
			Control.ExecuteScript("print();");
		}

		public bool BrowserContextMenuEnabled
		{
			get;
			set;
		}
	}
}
#endif