using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using wk = WebKit;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using wk = MonoMac.WebKit;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class WebViewHandler : MacView<wk.WebView, WebView, WebView.ICallback>, WebView.IHandler
	{
		static readonly Selector selIgnore = new Selector("ignore");
		static readonly Selector selUse = new Selector("use");

		public override NSView ContainerControl { get { return Control; } }

		NewWindowHandler newWindowHandler;

		public WebViewHandler()
		{
			Enabled = true;
			Control = new EtoWebView
			{
				Handler = this,
				UIDelegate = new UIDelegate { Handler = this }
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(WebView.OpenNewWindowEvent); // needed to provide default implementation
			HandleEvent(WebView.DocumentLoadingEvent);
		}

		public class EtoWebView : wk.WebView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public WebViewHandler Handler { get { return (WebViewHandler)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }
		}

		public class NewWindowHandler : NSObject
		{
			public WebViewHandler Handler { get { return WebView.Handler; } set { WebView.Handler = value; } }

			public EtoWebView WebView { get; set; }

			public NewWindowHandler()
			{
				WebView = new EtoWebView();
				WebView.WeakUIDelegate = this;
				WebView.WeakPolicyDelegate = this;
				WebView.WeakResourceLoadDelegate = this;
			}

			[Export("webView:decidePolicyForNavigationAction:request:frame:decisionListener:")]
			public void DecidePolicyForNavigation(wk.WebView webView, NSDictionary action, NSUrlRequest request, wk.WebFrame frame, NSObject listener)
			{
				var url = (NSUrl)action.ObjectForKey(new NSString("WebActionOriginalURLKey"));
				var args = new WebViewNewWindowEventArgs(new Uri(url.AbsoluteString), frame.Name);
				Handler.Callback.OnOpenNewWindow(Handler.Widget, args);
				if (!args.Cancel)
					NSWorkspace.SharedWorkspace.OpenUrl(url);
				listener.PerformSelector(selIgnore, null, 0);
			}
		}

		class PromptDialog : Dialog<bool>
		{
			readonly TextBox textBox;
			readonly Label prompt;

			public string Prompt
			{
				get { return prompt.Text; }
				set { prompt.Text = value; }
			}

			public string Value
			{
				get { return textBox.Text; }
				set { textBox.Text = value; }
			}

			public PromptDialog()
			{
				this.MinimumSize = new Size(400, 0);
				var layout = new DynamicLayout { Padding = new Padding(20, 10) };
				layout.BeginVertical(padding: Padding.Empty, spacing: new Size(10, 10));
				layout.Add(prompt = new Label());
				layout.Add(textBox = new TextBox(), yscale: true);
				layout.BeginVertical(padding: Padding.Empty);
				layout.AddRow(null, CancelButton(), OkButton());
				layout.EndVertical();

				Content = layout;
			}

			Control CancelButton()
			{
				var button = new Button { Text = "Cancel" };
				AbortButton = button;
				button.Click += (sender, e) => Close(false);
				return button;
			}

			Control OkButton()
			{
				var button = new Button { Text = "OK" };
				DefaultButton = button;
				button.Click += (sender, e) => Close(true);
				return button;
			}
		}

		public class UIDelegate : wk.WebUIDelegate
		{
			WeakReference handler;

			public WebViewHandler Handler { get { return (WebViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override void UIRunJavaScriptAlertPanelMessage(wk.WebView sender, string withMessage, wk.WebFrame initiatedByFrame)
			{
				MessageBox.Show(Handler.Widget, withMessage);
			}

			public override bool UIRunJavaScriptConfirmationPanel(wk.WebView sender, string withMessage, wk.WebFrame initiatedByFrame)
			{
				return MessageBox.Show(Handler.Widget, withMessage, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}

			public override string UIRunJavaScriptTextInputPanelWithFrame(wk.WebView sender, string prompt, string defaultText, wk.WebFrame initiatedByFrame)
			{
				var dialog = new PromptDialog
				{
					Prompt = prompt,
					Value = defaultText,
					Title = Handler.DocumentTitle
				};
				return dialog.ShowModal(Handler.Widget) ? dialog.Value : string.Empty;
			}

			public override NSMenuItem[] UIGetContextMenuItems(wk.WebView sender, NSDictionary forElement, NSMenuItem[] defaultMenuItems)
			{
				return Handler.BrowserContextMenuEnabled ? defaultMenuItems : null;
			}

			#if XAMMAC2
			public override void UIRunOpenPanelForFileButton(wk.WebView sender, wk.IWebOpenPanelResultListener resultListener)
			{
				var openDlg = new OpenFileDialog();
				if (openDlg.ShowDialog(Handler.Widget.ParentWindow) == DialogResult.Ok)
				{
					var resultListenerObj = resultListener as wk.WebOpenPanelResultListener;
					if (resultListenerObj != null)
						resultListenerObj.ChooseFilenames(openDlg.Filenames.ToArray());
				}
			}
			#else
			public override void UIRunOpenPanelForFileButton(wk.WebView sender, wk.WebOpenPanelResultListener resultListener)
			{
				var openDlg = new OpenFileDialog();
				if (openDlg.ShowDialog(Handler.Widget.ParentWindow) == DialogResult.Ok)
				{
					resultListener.ChooseFilenames(openDlg.Filenames.ToArray());
				}
			}
			#endif

			public override void UIPrintFrameView(wk.WebView sender, wk.WebFrameView frameView)
			{
				const float margin = 24f;
				var printOperation = frameView.GetPrintOperation(new NSPrintInfo
				{
					VerticallyCentered = false,
					LeftMargin = margin,
					RightMargin = margin,
					TopMargin = margin,
					BottomMargin = margin
				});
				printOperation.PrintPanel.Options = 
					NSPrintPanelOptions.ShowsCopies | 
					NSPrintPanelOptions.ShowsOrientation | 
					NSPrintPanelOptions.ShowsPageSetupAccessory | 
					NSPrintPanelOptions.ShowsPageRange | 
					NSPrintPanelOptions.ShowsPaperSize | 
					NSPrintPanelOptions.ShowsPreview | 
					NSPrintPanelOptions.ShowsPrintSelection | 
					NSPrintPanelOptions.ShowsScaling;
				printOperation.RunOperation();
			}

			public override wk.WebView UICreateWebView(wk.WebView sender, NSUrlRequest request)
			{
				Handler.newWindowHandler = new NewWindowHandler { Handler = Handler };
				return Handler.newWindowHandler.WebView;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case WebView.NavigatedEvent:
					HandleEvent(WebView.DocumentLoadedEvent);
					break;
				case WebView.DocumentLoadedEvent:
					Control.FinishedLoad += HandleFinishedLoad;
					break;
				case WebView.DocumentLoadingEvent:
					Control.DecidePolicyForNavigation += HandleDecidePolicyForNavigation;
					break;
				case WebView.OpenNewWindowEvent:
					Control.DecidePolicyForNewWindow += HandleDecidePolicyForNewWindow;
					break;
				case WebView.DocumentTitleChangedEvent:
					Control.ReceivedTitle += HandleReceivedTitle;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static void HandleReceivedTitle(object sender, wk.WebFrameTitleEventArgs e)
		{
			var handler = GetHandler(e.ForFrame.WebView) as WebViewHandler;
			if (handler != null)
			{
				handler.Callback.OnDocumentTitleChanged(handler.Widget, new WebViewTitleEventArgs(e.Title));
			}
		}

		static void HandleDecidePolicyForNewWindow(object sender, wk.WebNewWindowPolicyEventArgs e)
		{
			var handler = GetHandler(sender) as WebViewHandler;
			if (handler != null)
			{
				var args = new WebViewNewWindowEventArgs(new Uri(e.Request.Url.AbsoluteString), e.NewFrameName);
				handler.Callback.OnOpenNewWindow(handler.Widget, args);
				if (!args.Cancel)
					NSWorkspace.SharedWorkspace.OpenUrl(e.Request.Url);
				e.DecisionToken.PerformSelector(selIgnore, null, 0);
			}
		}

		static void HandleDecidePolicyForNavigation(object sender, wk.WebNavigationPolicyEventArgs e)
		{
			var handler = GetHandler(e.Frame.WebView) as WebViewHandler;
			if (handler != null)
			{
				var args = new WebViewLoadingEventArgs(new Uri(e.Request.Url.AbsoluteString), e.Frame == handler.Control.MainFrame);
				handler.Callback.OnDocumentLoading(handler.Widget, args);
				if (args.Cancel)
					e.DecisionToken.PerformSelector(selIgnore, null, 0);
				else
					e.DecisionToken.PerformSelector(selUse, null, 0);
			}
		}

		static void HandleFinishedLoad(object sender, wk.WebFrameEventArgs e)
		{
			var handler = GetHandler(e.ForFrame.WebView) as WebViewHandler;
			if (handler != null)
			{
				var args = new WebViewLoadedEventArgs(handler.Url);
				if (e.ForFrame == handler.Control.MainFrame)
					handler.Callback.OnNavigated(handler.Widget, args);
				handler.Callback.OnDocumentLoaded(handler.Widget, args);
			}
		}

		public Uri Url
		{
			get { return new Uri(Control.MainFrameUrl); }
			set
			{ 
				Control.MainFrameUrl = value == null ? null : value.AbsoluteUri;
			}
		}

		public string DocumentTitle
		{
			get { return Control.MainFrameTitle; }
		}

		public string ExecuteScript(string script)
		{
			var fullScript = string.Format("var fn = function () {{ {0} }}; fn();", script);
			return Control.StringByEvaluatingJavaScriptFromString(fullScript);
		}

		public void LoadHtml(string html, Uri baseUri)
		{
			Control.MainFrame.LoadHtmlString(html, baseUri.ToNS());
		}

		public void Stop()
		{
			Control.MainFrame.StopLoading();
		}

		public void Reload()
		{
			Control.Reload(Control);
		}

		public void GoBack()
		{
			Control.GoBack();
		}

		public void GoForward()
		{
			Control.GoForward();
		}

		public override bool Enabled { get; set; }

		public bool CanGoBack
		{
			get { return Control.CanGoBack(); }
		}

		public bool CanGoForward
		{
			get { return Control.CanGoForward(); }
		}

		public void ShowPrintDialog()
		{
			Control.Print(Control);
		}

		public bool BrowserContextMenuEnabled
		{
			get;
			set;
		}
	}
}

