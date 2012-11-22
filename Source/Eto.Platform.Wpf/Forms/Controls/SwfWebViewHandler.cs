using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using swn = System.Windows.Navigation;
using sw = System.Windows;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Platform.CustomControls;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class SwfWebViewHandler : WpfFrameworkElement<swf.Integration.WindowsFormsHost, WebView>, IWebView
	{
		public swf.WebBrowser Browser { get; private set; }

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
			get { return (SHDocVw.WebBrowser_V1)Browser.ActiveXInstance; }
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

		public SwfWebViewHandler ()
		{
			this.Control = new swf.Integration.WindowsFormsHost ();
			this.Browser = new swf.WebBrowser {
				IsWebBrowserContextMenuEnabled = false,
				WebBrowserShortcutsEnabled = false,
				AllowWebBrowserDrop = false,
				ScriptErrorsSuppressed = true
			};
			this.Browser.HandleCreated += (sender, e) => {
				HookDocumentEvents ();
			};

			this.Control.Child = this.Browser;
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
				this.Browser.DocumentCompleted += (sender, e) => {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (e.Url));
				};
				break;
			case WebView.DocumentLoadingEvent:
				this.Browser.Navigating += (sender, e) => {
					var args = new WebViewLoadingEventArgs (e.Url, false);
					Widget.OnDocumentLoading (args);
					e.Cancel = args.Cancel;
				};
				break;
			case WebView.OpenNewWindowEvent:
				HookDocumentEvents (handler);
				break;
			case WebView.DocumentTitleChangedEvent:
				this.Browser.DocumentTitleChanged += delegate {
					Widget.OnDocumentTitleChanged (new WebViewTitleEventArgs (Browser.DocumentTitle));
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
			if (Browser.ActiveXInstance != null)
			{
#if !__MonoCS__
				foreach (var handler in delayedEvents)
					AttachEvent (WebBrowserV1, handler);
#endif
				delayedEvents.Clear ();
			}
		}

		public Uri Url {
			get { return this.Browser.Url; }
			set { this.Browser.Url = value; }
		}
		
		public string DocumentTitle {
			get {
				return this.Browser.DocumentTitle;
			}
		}
		
		public string ExecuteScript (string script)
		{
			var fullScript = string.Format ("var fn = function() {{ {0} }}; fn();", script);
			return Convert.ToString (Browser.Document.InvokeScript ("eval", new object[] { fullScript }));
		}
		
		public void Stop ()
		{
			this.Browser.Stop ();
		}
		
		public void Reload ()
		{
			this.Browser.Refresh ();
		}
		
		public void GoBack ()
		{
			this.Browser.GoBack ();
		}
		
		public bool CanGoBack {
			get {
				return this.Browser.CanGoBack;
			}
		}
		
		public void GoForward ()
		{
			this.Browser.GoForward ();
		}
		
		public bool CanGoForward {
			get {
				return this.Browser.CanGoForward;
			}
		}
		
		HttpServer server;

		public void LoadHtml (string html, Uri baseUri)
		{
			if (baseUri != null) {
				if (server == null)
					server = new HttpServer ();
				server.SetHtml (html, baseUri != null ? baseUri.LocalPath : null);
				Browser.Navigate (server.Url);
			}
			else
				this.Browser.DocumentText = html;

		}

        public void ShowPrintDialog ()
        {
			this.Browser.ShowPrintDialog ();
        }

		public bool BrowserContextMenuEnabled
		{
			get { return Browser.IsWebBrowserContextMenuEnabled; }
			set { Browser.IsWebBrowserContextMenuEnabled = value; }
		}

		public override Eto.Drawing.Color BackgroundColor
		{
			get { return Eto.Drawing.Colors.Transparent; }
			set { }
		}
	}
}
