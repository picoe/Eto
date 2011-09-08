using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class WebViewHandler : WindowsControl<System.Windows.Forms.WebBrowser, WebView>, IWebView
	{
		public WebViewHandler ()
		{
			this.Control = new SWF.WebBrowser ();
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				this.Control.DocumentCompleted += delegate(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e) {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (e.Url));
				};
				break;
			case WebView.DocumentLoadingEvent:
				this.Control.Navigating += delegate(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e) {
					var args = new WebViewLoadingEventArgs (e.Url);
					Widget.OnDocumentLoading (args);
					e.Cancel = args.Cancel;
				};
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

		public Uri Url {
			get { return this.Control.Url; }
			set { this.Control.Url = value; }
		}
		
		public string DocumentTitle {
			get {
				return this.Control.DocumentTitle;
			}
		}
		
		public void ExecuteScript (string script)
		{
			var scriptElement = this.Control.Document.GetElementById ("eto_forms_execute_script");
			if (scriptElement == null) {
				scriptElement = this.Control.Document.CreateElement ("script");
				scriptElement.Id = "eto_forms_execute_script";
				var head = this.Control.Document.GetElementsByTagName ("head") [0];
				head.AppendChild (scriptElement);
			}
			scriptElement.SetAttribute("text", "function eto_forms_execute_script() { " + script + " }");
			this.Control.Document.InvokeScript ("eto_forms_execute_script");
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
		
		public void SetHtml (string html, string baseUrl)
		{
			// what do we do with base url?  can we support it?
			
			this.Control.DocumentText = html;
		}

	}
}

