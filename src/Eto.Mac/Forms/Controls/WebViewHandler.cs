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

		protected override wk.WebView CreateControl()
		{
			return new EtoWebView(this);
		}

		public class EtoWebPolicyDelegate : wk.WebPolicyDelegate
		{
			WeakReference handler;
			public WebViewHandler Handler { get => handler?.Target as WebViewHandler; set => handler = new WeakReference(value); }

			public override void DecidePolicyForNavigation(wk.WebView webView, NSDictionary actionInformation, NSUrlRequest request, wk.WebFrame frame, NSObject decisionToken)
			{
				var h = Handler;
				if (h != null)
				{
					var args = new WebViewLoadingEventArgs(new Uri(request.Url.AbsoluteString), frame == h.Control.MainFrame);
					h.Callback.OnDocumentLoading(h.Widget, args);
					if (args.Cancel)
						decisionToken.PerformSelector(selIgnore, null, 0);
					else
						decisionToken.PerformSelector(selUse, null, 0);
				}
			}

			public override void DecidePolicyForNewWindow(wk.WebView webView, NSDictionary actionInformation, NSUrlRequest request, string newFrameName, NSObject decisionToken)
			{
				var h = Handler;
				if (h != null)
				{
					var args = new WebViewNewWindowEventArgs(new Uri(request.Url.AbsoluteString), newFrameName);
					h.Callback.OnOpenNewWindow(h.Widget, args);
					if (!args.Cancel)
						NSWorkspace.SharedWorkspace.OpenUrl(request.Url);
					decisionToken.PerformSelector(selIgnore, null, 0);
				}
			}

		}

		public class EtoWebFrameLoadDelegate : wk.WebFrameLoadDelegate
		{
			WeakReference handler;
			public WebViewHandler Handler { get => handler?.Target as WebViewHandler; set => handler = new WeakReference(value); }

			public override void FinishedLoad(wk.WebView sender, wk.WebFrame forFrame)
			{
				var h = Handler;
				if (h != null)
				{
					var args = new WebViewLoadedEventArgs(h.Url);
					if (forFrame == h.Control.MainFrame)
						h.Callback.OnNavigated(h.Widget, args);
					h.Callback.OnDocumentLoaded(h.Widget, args);
				}
			}

			public override void ReceivedTitle(wk.WebView sender, string title, wk.WebFrame forFrame)
			{
				var h = Handler;
				if (h != null)
				{
					h.Callback.OnDocumentTitleChanged(h.Widget, new WebViewTitleEventArgs(title));
				}
			}
		}

		protected override void Initialize()
		{
			Enabled = true;
			base.Initialize();

			Control.FrameLoadDelegate = new EtoWebFrameLoadDelegate { Handler = this };
			Control.PolicyDelegate = new EtoWebPolicyDelegate { Handler = this };
		}

		public class EtoWebView : wk.WebView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public WebViewHandler Handler { get { return (WebViewHandler)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

			public EtoWebView(WebViewHandler handler)
			{
				Handler = handler;
				UIDelegate = new EtoWebUIDelegate { Handler = handler };
			}
		}

		public class NewWindowHandler : NSObject
		{
			public WebViewHandler Handler => WebView.Handler;

			public EtoWebView WebView { get; set; }

			public NewWindowHandler(WebViewHandler handler)
			{
				WebView = new EtoWebView(handler);
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

		public class EtoWebUIDelegate : wk.WebUIDelegate
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
					wk.WebOpenPanelResultListener_Extensions.ChooseFilenames(resultListener, openDlg.Filenames.ToArray());
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
				Handler.newWindowHandler = new NewWindowHandler(Handler);
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
				case WebView.OpenNewWindowEvent:
				case WebView.DocumentLoadingEvent:
				case WebView.DocumentTitleChangedEvent:
					// handled by delegates
					break;
				default:
					base.AttachEvent(id);
					break;
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

