using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WC = System.Windows.Controls;
using WN = System.Windows.Navigation;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class WebViewHandler : WpfFrameworkElement<WC.WebBrowser, WebView>, IWebView
	{
		public WebViewHandler ()
		{
			Control = new WC.WebBrowser ();
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case WebView.DocumentLoadedEvent:
					Control.LoadCompleted += delegate (object sender, WN.NavigationEventArgs e) {
						var args = new WebViewLoadedEventArgs (e.Uri);
						Widget.OnDocumentLoaded (args);
					};
					break;
				case WebView.DocumentLoadingEvent:
					Control.Navigating += delegate (object sender, WN.NavigatingCancelEventArgs e) {
						var args = new WebViewLoadingEventArgs (e.Uri);
						Widget.OnDocumentLoading (args);
						e.Cancel = args.Cancel;
					};
					break;
				case WebView.DocumentTitleChangedEvent:
					Control.LoadCompleted += delegate (object sender, WN.NavigationEventArgs e) {
						//dynamic doc = Control.Document;
						var doc = Control.Document as mshtml.HTMLDocument;
						var args = new WebViewTitleEventArgs (doc.title);
						Widget.OnDocumentTitleChanged(args);
					};
					/*Control.Navigated += delegate (object sender, WN.NavigationEventArgs e) {
						//dynamic doc = Control.Document;
						var doc = Control.Document as mshtml.HTMLDocument;
						var args = new WebViewTitleEventArgs (doc.title);
						Widget.OnDocumentTitleChanged(args);
					};*/
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
				return Eto.Drawing.Color.Transparent;
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

		public void LoadHtml (string html)
		{
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
			
		}

		public void Reload ()
		{
			Control.Refresh ();
		}

		public string DocumentTitle
		{
			get { 
				var doc = Control.Document as mshtml.HTMLDocument;
				if (doc != null) return doc.title;
				else return null;
			}
		}

		public void ExecuteScript (string script)
		{
			var doc = Control.Document as mshtml.HTMLDocument;
			var scriptElement = doc.getElementById("eto_forms_execute_script");
			if (scriptElement == null) {
				scriptElement = doc.createElement("script");
				scriptElement.id = "eto_forms_execute_script";
				var head = doc.getElementsByTagName ("head");
				var headitem = head.OfType<mshtml.HTMLHeadElement>().FirstOrDefault();
				if (headitem != null)
					headitem.appendChild (scriptElement as mshtml.IHTMLDOMNode);
			}
			scriptElement.setAttribute ("text", "function eto_forms_execute_script() { " + script + " }");
			this.Control.InvokeScript ("eto_forms_execute_script");
		}
	}
}
