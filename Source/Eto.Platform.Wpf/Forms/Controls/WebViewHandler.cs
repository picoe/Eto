﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using swn = System.Windows.Navigation;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Platform.CustomControls;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class WebViewHandler : WpfFrameworkElement<swc.WebBrowser, WebView>, IWebView
	{
		[ComImport, InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
		[Guid ("6d5140c1-7436-11ce-8034-00aa006009fa")]
		internal interface IServiceProvider
		{
			[return: MarshalAs (UnmanagedType.IUnknown)]
			object QueryService (ref Guid guidService, ref Guid riid);
		}

		static readonly Guid SID_SWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");

		Guid serviceGuid = SID_SWebBrowserApp;
		Guid iid = typeof (SHDocVw.IWebBrowser2).GUID;


		object QueryDocumentService
		{
			get
			{
				IServiceProvider serviceProvider = (IServiceProvider)Control.Document;
				if (serviceProvider == null) return null;
				return serviceProvider.QueryService (ref serviceGuid, ref iid);
			}
		}

		SHDocVw.DWebBrowserEvents_Event oldEventObject;

		SHDocVw.IWebBrowser2 WebBrowser2
		{
			get { return (SHDocVw.IWebBrowser2)QueryDocumentService; }
		}

		SHDocVw.DWebBrowserEvents_Event WebEvents
		{
			get { return (SHDocVw.DWebBrowserEvents_Event)QueryDocumentService; }
		}

		HashSet<string> delayedEvents = new HashSet<string> ();

		public WebViewHandler ()
		{
			Control = new swc.WebBrowser ();
			Control.Navigated += delegate {
				WebBrowser2.Silent = true;
			};
		}

		void HookDocumentEvents (string newEvent = null)
		{
			var webEvents = WebEvents;
			if (oldEventObject != null) {
				foreach (var handler in delayedEvents)
					RemoveEvent (webEvents, handler);
			}
			if (newEvent != null)
				delayedEvents.Add (newEvent);
			if (webEvents != null) {
				foreach (var handler in delayedEvents)
					AttachEvent (webEvents, handler);
			}
			oldEventObject = webEvents;
		}

		void RemoveEvent (SHDocVw.DWebBrowserEvents_Event webEvents, string handler)
		{
			switch (handler) {
				case WebView.DocumentTitleChangedEvent:
					WebEvents.TitleChange -= WebEvents_TitleChange;
					break;
			}
		}

		void AttachEvent (SHDocVw.DWebBrowserEvents_Event webEvents, string handler)
		{
			switch (handler) {
				case WebView.DocumentTitleChangedEvent:
					WebEvents.TitleChange += WebEvents_TitleChange;
					break;
			}
		}

		void WebEvents_TitleChange (string Text)
		{
			var args = new WebViewTitleEventArgs (Text);
			Widget.OnDocumentTitleChanged (args);
		}


		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case WebView.DocumentLoadedEvent:
					Control.LoadCompleted += delegate (object sender, swn.NavigationEventArgs e) {
						var args = new WebViewLoadedEventArgs (e.Uri);
						Widget.OnDocumentLoaded (args);
						Widget.OnDocumentTitleChanged (new WebViewTitleEventArgs (this.DocumentTitle));
						HookDocumentEvents ();
					};
					break;
				case WebView.DocumentLoadingEvent:
					Control.Navigating += delegate (object sender, swn.NavigatingCancelEventArgs e) {
						var args = new WebViewLoadingEventArgs (e.Uri);
						Widget.OnDocumentLoading (args);
						e.Cancel = args.Cancel;
						HookDocumentEvents ();
					};
					break;
				case WebView.DocumentTitleChangedEvent:
					HookDocumentEvents(handler);
					break;
				default:
					base.AttachEvent (handler);
					break;
			}
		}

		public override Eto.Drawing.Color BackgroundColor
		{
			get
			{
				return Eto.Drawing.Colors.Transparent;
			}
			set
			{
			}
		}

		public Uri Url
		{
			get
			{
				return Control.Source;
			}
			set
			{
				Control.Source = value;
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
				Control.NavigateToString (html);

		}

		public void GoBack ()
		{
			Control.GoBack ();
		}

		public bool CanGoBack
		{
			get { return Control.CanGoBack; }
		}

		public void GoForward ()
		{
			Control.GoForward ();
		}

		public bool CanGoForward
		{
			get { return Control.CanGoForward; }
		}

		public void Stop ()
		{
			var browser = WebBrowser2;
			if (browser != null)
				browser.Stop ();
		}

		public void Reload ()
		{
			Control.Refresh ();
		}

		public string DocumentTitle
		{
			get {
				var browser = WebBrowser2;
				if (browser != null && browser.Document != null)
					return Convert.ToString(browser.Document.Title);
				else return null;
			}
		}

		public string ExecuteScript (string script)
		{
			var fullScript = string.Format ("var fn = function() {{ {0} }}; fn();", script);
			return Convert.ToString(Control.InvokeScript ("eval", fullScript));
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			if (server != null) {
				server.Dispose ();
				server = null;
			}
		}

        public void ShowPrintDialog ()
        {
            WebBrowser2.Document.execCommand("Print", true, null);
        }
	}
}
