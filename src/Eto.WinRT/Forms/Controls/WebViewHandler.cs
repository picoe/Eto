using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wc = WinRTXamlToolkit.Controls;
using swc = Windows.UI.Xaml.Controls;
using swn = Windows.UI.Xaml.Navigation;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
//using swf = Windows.UI.Xaml.Forms;
using Eto.Forms;
using System.Runtime.InteropServices;
//using Eto.Platform.CustomControls;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Web view handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>	
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class WebViewHandler : WpfFrameworkElement<wc.WebBrowser, WebView, WebView.ICallback>, WebView.IHandler
	{
		HashSet<string> delayedEvents = new HashSet<string>();

		public override wf.Size GetPreferredSize(wf.Size constraint)
		{
			var size = base.GetPreferredSize(constraint);
			return new wf.Size(Math.Min(size.Width, 100), Math.Min(size.Height, 100));
		}

#if TODO_XAML
		public void AttachEvent(SHDocVw.WebBrowser_V1 control, string handler)
		{
			switch (handler)
			{
				case WebView.OpenNewWindowEvent:
					control.NewWindow += WebBrowserV1_NewWindow;
					break;
			}
		}
#endif

		public WebViewHandler()
		{
			Control = new wc.WebBrowser
			{
#if TODO_XAML
				IsWebBrowserContextMenuEnabled = false,
				WebBrowserShortcutsEnabled = false,
				AllowWebBrowserDrop = false,
				ScriptErrorsSuppressed = true
#endif
			};
#if TODO_XAML
			Control.HandleCreated += (sender, e) => HookDocumentEvents();
#endif

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
#if TODO_XAML
				case WebView.NavigatedEvent:
					this.Control.Navigated += (sender, e) =>
					{
						Widget.OnNavigated(new WebViewLoadedEventArgs(e.Url));
					};
					break;
				case WebView.DocumentLoadedEvent:
					this.Control.DocumentCompleted += (sender, e) =>
					{
						Widget.OnDocumentLoaded(new WebViewLoadedEventArgs(e.Url));
					};
					break;
				case WebView.DocumentLoadingEvent:
					this.Control.Navigating += (sender, e) =>
					{
						var args = new WebViewLoadingEventArgs(e.Url, false);
						Widget.OnDocumentLoading(args);
						e.Cancel = args.Cancel;
					};
					break;
				case WebView.OpenNewWindowEvent:
					HookDocumentEvents(handler);
					break;
				case WebView.DocumentTitleChangedEvent:
					this.Control.DocumentTitleChanged += delegate
					{
						Widget.OnDocumentTitleChanged(new WebViewTitleEventArgs(Control.DocumentTitle));
					};
					break;
#endif
				default:
					base.AttachEvent(handler);
					break;
			}

		}

		void HookDocumentEvents(string newEvent = null)
		{
#if TODO_XAML
			if (newEvent != null)
				delayedEvents.Add(newEvent);
			if (Control.ActiveXInstance != null)
			{
				foreach (var handler in delayedEvents)
					AttachEvent(WebBrowserV1, handler);
				delayedEvents.Clear();
			}
#endif
		}

		Uri url;
		public Uri Url
		{
			get { return url; }
			set
			{
				url = value;
				this.Control.Navigate(value);
			}
		}

		public string DocumentTitle
		{
			get
			{
				return this.Control.Title;
			}
		}

		public string ExecuteScript(string script)
		{
#if TODO_XAML
			var fullScript = string.Format("var fn = function() {{ {0} }}; fn();", script);
			return Convert.ToString(Control.Document.InvokeScript("eval", new object[] { fullScript }));
#else
			return "";
#endif
		}

		public void Stop()
		{
#if TODO_XAML
			this.Control.Stop();
#endif
		}

		public void Reload()
		{
#if TODO_XAML
			this.Control.Refresh();
#endif
		}

		public void GoBack()
		{
#if TODO_XAML
			this.Control.GoBack();
#endif
		}

		public bool CanGoBack
		{
			get
			{
#if TODO_XAML
				return this.Control.CanGoBack;
#else
				return false;
#endif
			}
		}

		public void GoForward()
		{
#if TODO_XAML
			this.Control.GoForward();
#endif
		}

		public bool CanGoForward
		{
			get
			{
#if TODO_XAML
				return this.Control.CanGoForward;
#else
				return false;
#endif
			}
		}

#if TODO_XAML
		HttpServer server;
#endif

		public void LoadHtml(string html, Uri baseUri)
		{
			if (baseUri != null)
			{
#if TODO_XAML
				if (server == null)
					server = new HttpServer();
				server.SetHtml(html, baseUri != null ? baseUri.LocalPath : null);
				Control.Navigate(server.Url);
#endif
			}
			else
			{
#if TODO_XAML
				this.Control.DocumentText = html;
#endif
			}

		}

		public void ShowPrintDialog()
		{
#if TODO_XAML
			this.Control.ShowPrintDialog();
#endif
		}

		public bool BrowserContextMenuEnabled
		{
#if TODO_XAML
			get { return Control.IsWebBrowserContextMenuEnabled; }
			set { Control.IsWebBrowserContextMenuEnabled = value; }
#else
			get;
			set;
#endif
		}

		public override Eto.Drawing.Color BackgroundColor
		{
			get { return Eto.Drawing.Colors.Transparent; }
			set { }
		}
	}
}
