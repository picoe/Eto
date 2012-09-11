using System;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using System.Linq;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class WebViewHandler : MacView<MonoMac.WebKit.WebView, WebView>, IWebView
	{
		class MyUIDelegate : MonoMac.WebKit.WebUIDelegate
		{
			public WebViewHandler Handler { get; set; }
			
			public override bool UIRunJavaScriptConfirmPanel (MonoMac.WebKit.WebView sender, string message)
			{
				return MessageBox.Show (Handler.Widget, message, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}
			
			public override void UIRunJavaScriptAlertPanel (MonoMac.WebKit.WebView sender, string message)
			{
				MessageBox.Show (Handler.Widget, message);
			}
			
			public override void UIRunOpenPanelForFileButton (MonoMac.WebKit.WebView sender, MonoMac.WebKit.WebOpenPanelResultListener resultListener)
			{
				var openDlg = new OpenFileDialog();

				if (openDlg.ShowDialog (Handler.Widget.ParentWindow) == DialogResult.Ok)
				{
					resultListener.ChooseFilenames(openDlg.Filenames.ToArray ());
				}
			}
			
		}
		
		public WebViewHandler ()
		{
			Enabled = true;
			Control = new MonoMac.WebKit.WebView ();
			Control.UIDelegate = new MyUIDelegate{ Handler = this };
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				this.Control.FinishedLoad += delegate(object sender, MonoMac.WebKit.WebFrameEventArgs e) {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (this.Url));
				};
				break;
			case WebView.DocumentLoadingEvent:
				/*this.Control.UICreateWebView = (sender, request) => {
					var args = new WebViewLoadingEventArgs (new Uri (request.Url.AbsoluteString));
					Widget.OnDocumentLoading (args);
					if (!args.Cancel) {
						// open new window event
					}
					return null;
				};*/
				this.Control.DecidePolicyForNavigation += (sender, e) => {
					var args = new WebViewLoadingEventArgs (new Uri (e.Request.Url.AbsoluteString));
					Widget.OnDocumentLoading (args);
					if (args.Cancel)
						e.DecisionToken.PerformSelector (new Selector ("ignore"), null, 0);
					else
						e.DecisionToken.PerformSelector (new Selector ("use"), null, 0);
				};
				break;
			case WebView.DocumentTitleChangedEvent:
				this.Control.ReceivedTitle += delegate(object sender, MonoMac.WebKit.WebFrameTitleEventArgs e) {
					Widget.OnDocumentTitleChanged (new WebViewTitleEventArgs (e.Title));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public Uri Url {
			get { 
				return new Uri (Control.MainFrameUrl);
			}
			set { 
				if (value != null)
					Control.MainFrameUrl = value.AbsoluteUri;
				else
					Control.MainFrameUrl = null;
			}
		}
		
		public string DocumentTitle {
			get {
				return Control.MainFrameTitle;
			}
		}
		
		public string ExecuteScript (string script)
		{
			var fullScript = string.Format ("var fn = function () {{ {0} }}; fn();", script);
			return Control.StringByEvaluatingJavaScriptFromString (fullScript);
		}

		public void LoadHtml (string html, Uri baseUri)
		{
			Control.MainFrame.LoadHtmlString (html, Generator.Convert (baseUri));
		}
		
		public void Stop ()
		{
			Control.MainFrame.StopLoading ();
		}
		
		public void Reload ()
		{
			Control.Reload (Control);
		}

		public void GoBack ()
		{
			Control.GoBack ();
		}

		public void GoForward ()
		{
			Control.GoForward ();
		}

		public override bool Enabled { get; set; }

		public bool CanGoBack {
			get {
				return Control.CanGoBack ();
			}
		}

		public bool CanGoForward {
			get {
				return Control.CanGoForward ();
			}
		}

        public void ShowPrintDialog()
        {
            Control.Print (Control);
        }
	}
}

