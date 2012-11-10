using System;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Platform.CustomControls;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class WebViewHandler : WindowsControl<SWF.WebBrowser, WebView>, IWebView
	{
		[ComImport, InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
		[Guid ("6d5140c1-7436-11ce-8034-00aa006009fa")]
		internal interface IServiceProvider
		{
			[return: MarshalAs (UnmanagedType.IUnknown)]
			object QueryService (ref Guid guidService, ref Guid riid);
		}

		HashSet<string> delayedEvents = new HashSet<string> ();

#if !__MonoCS__
		SHDocVw.WebBrowser_V1 WebBrowserV1
		{
			get { return (SHDocVw.WebBrowser_V1)Control.ActiveXInstance; }
		}

		public void AttachEvent (SHDocVw.WebBrowser_V1 control, string handler)
		{
			switch (handler)
			{
			case WebView.OpenNewWindowEvent:
				control.NewWindow += WebBrowserV1_NewWindow;
				break;
			}
		}
#endif

		public WebViewHandler ()
		{
			this.Control = new SWF.WebBrowser {
				IsWebBrowserContextMenuEnabled = false,
				WebBrowserShortcutsEnabled = false,
				AllowWebBrowserDrop = false,
				ScriptErrorsSuppressed = true
			};
			this.Control.HandleCreated += (sender, e) => {
				HookDocumentEvents ();
			};
		}

		void WebBrowserV1_NewWindow (string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed)
		{
			var e = new WebViewNewWindowEventArgs (new Uri (URL), TargetFrameName);
			Widget.OnOpenNewWindow (e);
			Processed = e.Cancel;
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				this.Control.DocumentCompleted += (sender, e) => {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (e.Url));
				};
				break;
			case WebView.DocumentLoadingEvent:
				this.Control.Navigating += (sender, e) => {
					var args = new WebViewLoadingEventArgs (e.Url, false);
					Widget.OnDocumentLoading (args);
					e.Cancel = args.Cancel;
				};
				break;
			case WebView.OpenNewWindowEvent:
				HookDocumentEvents (handler);
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

		void HookDocumentEvents (string newEvent = null)
		{
			if (newEvent != null)
				delayedEvents.Add (newEvent);
			if (Control.ActiveXInstance != null)
			{
#if !__MonoCS__
				foreach (var handler in delayedEvents)
					AttachEvent (WebBrowserV1, handler);
#endif
				delayedEvents.Clear ();
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
		
		public string ExecuteScript (string script)
		{
			var fullScript = string.Format ("var fn = function() {{ {0} }}; fn();", script);
			return Convert.ToString (Control.Document.InvokeScript ("eval", new object[] { fullScript }));
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

        public void ShowPrintDialog ()
        {
            this.Control.ShowPrintDialog();
        }

		public bool BrowserContextMenuEnabled
		{
			get { return Control.IsWebBrowserContextMenuEnabled; }
			set { Control.IsWebBrowserContextMenuEnabled = value; }
		}
	}
}

