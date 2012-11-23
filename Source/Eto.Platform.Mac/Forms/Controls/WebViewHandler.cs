using System;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using System.Linq;
using System.Net;
using Eto.Drawing;
using wk = MonoMac.WebKit;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class WebViewHandler : MacView<wk.WebView, WebView>, IWebView
	{
		NewWindowHandler newWindowHandler;
		public WebViewHandler ()
		{
			Enabled = true;
			Control = new wk.WebView {
				UIDelegate = new UIDelegate { Handler = this }
			};
			HandleEvent (WebView.OpenNewWindowEvent); // needed to provide default implementation
			HandleEvent (WebView.DocumentLoadingEvent);
		}

		public class NewWindowHandler : NSObject
		{
			public WebViewHandler Handler { get; set; }

			public wk.WebView WebView { get; set; }

			public NewWindowHandler ()
			{
				WebView = new wk.WebView();
				WebView.WeakUIDelegate = this;
				WebView.WeakPolicyDelegate = this;
				WebView.WeakResourceLoadDelegate = this;
			}

			[Export("webView:decidePolicyForNavigationAction:request:frame:decisionListener:")]
			public void DecidePolicyForNavigation(wk.WebView webView, NSDictionary action, NSUrlRequest request, wk.WebFrame frame, NSObject listener)
			{
				var url = action.ObjectForKey (new NSString("WebActionOriginalURLKey")) as NSUrl;
				var args = new WebViewNewWindowEventArgs (new Uri(url.AbsoluteString), frame.Name);
				Handler.Widget.OnOpenNewWindow (args);
				if (!args.Cancel)
					NSWorkspace.SharedWorkspace.OpenUrl(url);
				listener.PerformSelector (new Selector ("ignore"), null, 0);
			}
		}

		class PromptDialog : Dialog
		{
			TextBox textBox;
			Label prompt;

			public string Prompt {
				get { return prompt.Text; }
				set { prompt.Text = value; }
			}

			public string Value {
				get { return textBox.Text; }
				set { textBox.Text = value; }
			}

			public PromptDialog (Eto.Generator generator)
				: base(generator)
			{
				this.MinimumSize = new Size (400, 0);
				var layout = new DynamicLayout (this, padding: new Padding (20, 10));
				layout.BeginVertical (padding: Padding.Empty, spacing: new Size (10, 10));
				layout.Add (prompt = new Label ());
				layout.Add (textBox = new TextBox (), yscale: true);
				layout.BeginVertical (padding: Padding.Empty);
				layout.AddRow (null, CancelButton (), OkButton ());
				layout.EndVertical ();
			}

			Control CancelButton ()
			{
				var button = new Button { Text = "Cancel" };
				AbortButton = button;
				button.Click += (sender, e) => {
					Close (DialogResult.Cancel);
				};
				return button;
			}

			Control OkButton ()
			{
				var button = new Button { Text = "OK" };
				DefaultButton = button;
				button.Click += (sender, e) => {
					Close (DialogResult.Ok);
				};
				return button;
			}
		}

		public class UIDelegate : wk.WebUIDelegate
		{
			public WebViewHandler Handler { get; set; }
			
			public override void UIRunJavaScriptAlertPanelMessage (wk.WebView sender, string withMessage, wk.WebFrame initiatedByFrame)
			{
				MessageBox.Show (Handler.Widget, withMessage);
			}
			
			public override bool UIRunJavaScriptConfirmationPanel (wk.WebView sender, string withMessage, wk.WebFrame initiatedByFrame)
			{
				return MessageBox.Show (Handler.Widget, withMessage, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}
			
			public override string UIRunJavaScriptTextInputPanelWithFrame (wk.WebView sender, string prompt, string defaultText, wk.WebFrame initiatedByFrame)
			{
				var dialog = new PromptDialog (Handler.Widget.Generator) {
					Prompt = prompt,
					Value = defaultText,
					Title = Handler.DocumentTitle
				};
				var result = dialog.ShowDialog (Handler.Widget);
				return (result == DialogResult.Ok) ? dialog.Value : string.Empty;
			}
			
			public override NSMenuItem[] UIGetContextMenuItems (wk.WebView sender, NSDictionary forElement, NSMenuItem[] defaultMenuItems)
			{
				if (Handler.BrowserContextMenuEnabled)
					return defaultMenuItems;
				else
					return null;
			}
			
			public override void UIRunOpenPanelForFileButton (wk.WebView sender, wk.WebOpenPanelResultListener resultListener)
			{
				var openDlg = new OpenFileDialog ();
				if (openDlg.ShowDialog (Handler.Widget.ParentWindow) == DialogResult.Ok) {
					resultListener.ChooseFilenames (openDlg.Filenames.ToArray ());
				}
			}
			
			public override void UIPrintFrameView (wk.WebView sender, wk.WebFrameView frameView)
			{
				var margin = 24f;
				var printOperation = frameView.GetPrintOperation (new MonoMac.AppKit.NSPrintInfo () {
					VerticallyCentered = false,
					LeftMargin = margin,
					RightMargin = margin,
					TopMargin = margin,
					BottomMargin = margin
				});
				printOperation.PrintPanel.Options = 
					MonoMac.AppKit.NSPrintPanelOptions.ShowsCopies | 
						MonoMac.AppKit.NSPrintPanelOptions.ShowsOrientation | 
						MonoMac.AppKit.NSPrintPanelOptions.ShowsPageRange | 
						MonoMac.AppKit.NSPrintPanelOptions.ShowsPageSetupAccessory | 
						MonoMac.AppKit.NSPrintPanelOptions.ShowsPaperSize | 
						MonoMac.AppKit.NSPrintPanelOptions.ShowsPreview | 
						MonoMac.AppKit.NSPrintPanelOptions.ShowsPrintSelection | 
						MonoMac.AppKit.NSPrintPanelOptions.ShowsScaling;
				printOperation.RunOperation ();
			}
			
			public override wk.WebView UICreateWebView (wk.WebView sender, NSUrlRequest request)
			{
				Handler.newWindowHandler = new NewWindowHandler { Handler = this.Handler };
				return Handler.newWindowHandler.WebView;
			}
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case WebView.DocumentLoadedEvent:
				this.Control.FinishedLoad += delegate(object sender, wk.WebFrameEventArgs e) {
					Widget.OnDocumentLoaded (new WebViewLoadedEventArgs (this.Url));
				};
				break;
			case WebView.DocumentLoadingEvent:
				this.Control.DecidePolicyForNavigation += (sender, e) => {
					var args = new WebViewLoadingEventArgs (new Uri (e.Request.Url.AbsoluteString), e.Frame == Control.MainFrame);
					Widget.OnDocumentLoading (args);
					if (args.Cancel)
						e.DecisionToken.PerformSelector (new Selector ("ignore"), null, 0);
					else
						e.DecisionToken.PerformSelector (new Selector ("use"), null, 0);
				};
				break;
			case WebView.OpenNewWindowEvent:
				this.Control.DecidePolicyForNewWindow += (sender, e) => {
					var args = new WebViewNewWindowEventArgs (new Uri (e.Request.Url.AbsoluteString), e.NewFrameName);
					Widget.OnOpenNewWindow (args);
					if (!args.Cancel)
						NSWorkspace.SharedWorkspace.OpenUrl(e.Request.Url);
					e.DecisionToken.PerformSelector (new Selector ("ignore"), null, 0);
				};
				break;
			case WebView.DocumentTitleChangedEvent:
				this.Control.ReceivedTitle += delegate(object sender, wk.WebFrameTitleEventArgs e) {
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
			Control.MainFrame.LoadHtmlString (html, baseUri.ToNS ());
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

		public void ShowPrintDialog ()
		{
			Control.Print (Control);
		}

		public bool BrowserContextMenuEnabled
		{
			get; set;
		}
	}
}

