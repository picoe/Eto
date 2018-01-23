using System;
using System.Linq;
using SHDocVw;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.CustomControls;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Eto.WinForms.Forms.Controls
{
	public class WebViewHandler : WindowsControl<swf.WebBrowser, WebView, WebView.ICallback>, WebView.IHandler
	{
		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
		internal interface IServiceProvider
		{
			[return: MarshalAs(UnmanagedType.IUnknown)]
			object QueryService(ref Guid guidService, ref Guid riid);
		}

		readonly HashSet<string> delayedEvents = new HashSet<string>();

		WebBrowser_V1 WebBrowserV1
		{
			get { return (WebBrowser_V1)Control.ActiveXInstance; }
		}

		public void AttachEvent(WebBrowser_V1 control, string handler)
		{
			switch (handler)
			{
				case WebView.OpenNewWindowEvent:
					control.NewWindow += WebBrowserV1_NewWindow;
					break;
			}
		}

		static readonly string[] ValidInputTags = { "input", "textarea" };

		public WebViewHandler()
		{
			Control = new swf.WebBrowser
			{
				IsWebBrowserContextMenuEnabled = false,
				WebBrowserShortcutsEnabled = false,
				AllowWebBrowserDrop = false,
				ScriptErrorsSuppressed = true
			};
			Control.HandleCreated += (sender, e) => HookDocumentEvents();
			Control.PreviewKeyDown += (sender, e) =>
			{
				switch (e.KeyCode)
				{
					case swf.Keys.Down:
					case swf.Keys.Up:
					case swf.Keys.Left:
					case swf.Keys.Right:
					case swf.Keys.PageDown:
					case swf.Keys.PageUp:
						// enable scrolling via keyboard
						e.IsInputKey = true;
						return;
				}

				var doc = Control.Document;
				if (!Control.WebBrowserShortcutsEnabled && doc != null)
				{
					// implement shortcut keys for copy/paste
					switch (e.KeyData)
					{
						case (swf.Keys.C | swf.Keys.Control):
							doc.ExecCommand("Copy", false, null);
							break;
						case (swf.Keys.V | swf.Keys.Control):
							if (doc.ActiveElement != null && ValidInputTags.Contains(doc.ActiveElement.TagName.ToLowerInvariant()))
								doc.ExecCommand("Paste", false, null);
							break;
						case (swf.Keys.X | swf.Keys.Control):
							if (doc.ActiveElement != null && ValidInputTags.Contains(doc.ActiveElement.TagName.ToLowerInvariant()))
								doc.ExecCommand("Cut", false, null);
							break;
						case (swf.Keys.A | swf.Keys.Control):
							doc.ExecCommand("SelectAll", false, null);
							break;
					}
				}
			};
		}

		void WebBrowserV1_NewWindow(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool processed)
		{
			var e = new WebViewNewWindowEventArgs(new Uri(url), targetFrameName);
			Callback.OnOpenNewWindow(Widget, e);
			processed = e.Cancel;
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
				server.SetHtml(html, baseUri.LocalPath);
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

