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
	public class WebViewHandler : MacView<MonoMac.WebKit.WebView, WebView>, IWebView
	{
		NewWindowHandler newWindowHandler;
		public WebViewHandler ()
		{
			Enabled = true;
			Control = new MonoMac.WebKit.WebView ();
			SetUIEvents ();
			SetUIPrintFrameView ();
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

		void SetUIEvents ()
		{
			Control.UIRunJavaScriptConfirmationPanel = (sender, message, withFrame) => {
				return MessageBox.Show (Widget, message, MessageBoxButtons.YesNo) == DialogResult.Yes;
			};

			Control.UIRunJavaScriptAlertPanelMessage += (sender, e) => {
				MessageBox.Show (Widget, e.WithMessage);
			};

			Control.UIRunJavaScriptTextInputPanelWithFrame = (sender, prompt, defaultText, initiatedByFrame) => {
				var dialog = new PromptDialog (Widget.Generator) {
					Prompt = prompt,
					Value = defaultText,
					Title = this.DocumentTitle
				};
				var result = dialog.ShowDialog (Widget);
				return (result == DialogResult.Ok) ? dialog.Value : string.Empty;
			};
			
			Control.UIGetContextMenuItems = (sender, forElement, defaultMenuItems) => {
				return defaultMenuItems;
			};

			Control.UIRunOpenPanelForFileButton += (sender, e) => {
				var openDlg = new OpenFileDialog ();
				if (openDlg.ShowDialog (Widget.ParentWindow) == DialogResult.Ok) {
					e.ResultListener.ChooseFilenames (openDlg.Filenames.ToArray ());
				}
			};
		}

		void SetUIPrintFrameView ()
		{
			Control.UIPrintFrameView += (sender, e) => {
				var margin = 24f;
				var printOperation = e.FrameView.GetPrintOperation (new MonoMac.AppKit.NSPrintInfo () {
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
			};
			Control.UIGetHeaderHeight = new MonoMac.WebKit.WebViewGetFloat (x => {
				return 0f; });
			Control.UIGetFooterHeight = new MonoMac.WebKit.WebViewGetFloat (x => {
				return 0f; });
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
				this.Control.UICreateWebView = new MonoMac.WebKit.CreateWebViewFromRequest((sender, e) => {
					newWindowHandler = new NewWindowHandler { Handler = this };
					return newWindowHandler.WebView;
				});
				this.Control.DecidePolicyForNewWindow += (sender, e) => {
					var args = new WebViewNewWindowEventArgs (new Uri (e.Request.Url.AbsoluteString), e.NewFrameName);
					Widget.OnOpenNewWindow (args);
					if (!args.Cancel)
						NSWorkspace.SharedWorkspace.OpenUrl(e.Request.Url);
					e.DecisionToken.PerformSelector (new Selector ("ignore"), null, 0);
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

		public void ShowPrintDialog ()
		{
			Control.Print (Control);
		}
	}
}

