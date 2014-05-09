using System;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.CustomControls;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Eto.WinForms.Forms.Controls
{
	public class WebViewHandler : WindowsControl<SWF.WebBrowser, WebView, WebView.ICallback>, WebView.IHandler
	{
		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
		internal interface IServiceProvider
		{
			[return: MarshalAs(UnmanagedType.IUnknown)]
			object QueryService(ref Guid guidService, ref Guid riid);
		}

		HashSet<string> delayedEvents = new HashSet<string>();

		SHDocVw.WebBrowser_V1 WebBrowserV1
		{
			get { return (SHDocVw.WebBrowser_V1)Control.ActiveXInstance; }
		}

		public void AttachEvent(SHDocVw.WebBrowser_V1 control, string handler)
		{
			switch (handler)
			{
				case WebView.OpenNewWindowEvent:
					control.NewWindow += WebBrowserV1_NewWindow;
					break;
			}
		}

		public WebViewHandler()
		{
			this.Control = new SWF.WebBrowser
			{
				IsWebBrowserContextMenuEnabled = false,
				WebBrowserShortcutsEnabled = false,
				AllowWebBrowserDrop = false,
				ScriptErrorsSuppressed = true
			};
			this.Control.HandleCreated += (sender, e) =>
			{
				HookDocumentEvents();
			};
		}

		void WebBrowserV1_NewWindow(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed)
		{
			var e = new WebViewNewWindowEventArgs(new Uri(URL), TargetFrameName);
			Callback.OnOpenNewWindow(Widget, e);
			Processed = e.Cancel;
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case WebView.NavigatedEvent:
					Control.Navigated += (s, e) => Callback.OnNavigated(Widget, new WebViewLoadedEventArgs(e.Url));
					break;
				case WebView.DocumentLoadedEvent:
					Control.DocumentCompleted += (sender, e) => Callback.OnDocumentLoaded(Widget, new WebViewLoadedEventArgs(e.Url));
					break;
				case WebView.DocumentLoadingEvent:
					Control.Navigating += (sender, e) =>
					{
						var args = new WebViewLoadingEventArgs(e.Url, false);
						Callback.OnDocumentLoading(Widget, args);
						e.Cancel = args.Cancel;
					};
					break;
				case WebView.OpenNewWindowEvent:
					HookDocumentEvents(handler);
					break;
				case WebView.DocumentTitleChangedEvent:
					Control.DocumentTitleChanged += (sender, e) => Callback.OnDocumentTitleChanged(Widget, new WebViewTitleEventArgs(Control.DocumentTitle));
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
			
		}

		void HookDocumentEvents(string newEvent = null)
		{
			if (newEvent != null)
				delayedEvents.Add(newEvent);
			if (Control.ActiveXInstance != null)
			{
				foreach (var handler in delayedEvents)
					AttachEvent(WebBrowserV1, handler);
				delayedEvents.Clear();
			}
		}

		public Uri Url
		{
			get { return Control.Url; }
			set { Control.Url = value; }
		}

		public string DocumentTitle
		{
			get
			{
				return Control.DocumentTitle;
			}
		}

		public string ExecuteScript(string script)
		{
			var fullScript = string.Format("var fn = function() {{ {0} }}; fn();", script);
			return Convert.ToString(Control.Document.InvokeScript("eval", new object[] { fullScript }));
		}

		public void Stop()
		{
			Control.Stop();
		}

		public void Reload()
		{
			Control.Refresh();
		}

		public void GoBack()
		{
			Control.GoBack();
		}

		public bool CanGoBack
		{
			get
			{
				return Control.CanGoBack;
			}
		}

		public void GoForward()
		{
			Control.GoForward();
		}

		public bool CanGoForward
		{
			get
			{
				return Control.CanGoForward;
			}
		}

		HttpServer server;

		public void LoadHtml(string html, Uri baseUri)
		{
			if (baseUri != null)
			{
				if (server == null)
					server = new HttpServer();
				server.SetHtml(html, baseUri != null ? baseUri.LocalPath : null);
				Control.Navigate(server.Url);
			}
			else
				Control.DocumentText = html;

		}

		public void ShowPrintDialog()
		{
			Control.ShowPrintDialog();
		}

		public bool BrowserContextMenuEnabled
		{
			get { return Control.IsWebBrowserContextMenuEnabled; }
			set { Control.IsWebBrowserContextMenuEnabled = value; }
		}
	}
}

