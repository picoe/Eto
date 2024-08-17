#if MONOMAC
using wk = MonoMac.WebKit;
#else
using wk = WebKit;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class WKWebViewHandler : MacView<wk.WKWebView, WebView, WebView.ICallback>, WebView.IHandler
	{
		public override NSView ContainerControl { get { return Control; } }

		public wk.WKWebViewConfiguration Configuration { get => Control.Configuration; }

		protected override wk.WKWebView CreateControl()
		{
			return new EtoWebView(this);
		}

		public class EtoNavigationDelegate : wk.WKNavigationDelegate
		{
			public EtoNavigationDelegate()
			{
			}
			public EtoNavigationDelegate(IntPtr handle)
				: base(handle)
			{
			}
			
			WeakReference handler;
			public WKWebViewHandler Handler { get => handler?.Target as WKWebViewHandler; set => handler = new WeakReference(value); }

			public override void DidFinishNavigation(wk.WKWebView webView, wk.WKNavigation navigation)
			{
				var h = Handler;
				if (h != null)
				{
					h.SetupContextMenu();
					var args = new WebViewLoadedEventArgs(h.Url);
					h.Callback.OnNavigated(h.Widget, args);
					h.Callback.OnDocumentLoaded(h.Widget, args);
				}
			}

			public override void DecidePolicy(wk.WKWebView webView, wk.WKNavigationAction navigationAction, Action<wk.WKNavigationActionPolicy> decisionHandler)
			{
				// support for <= 10.14.x
				DecidePolicy(webView, navigationAction, null, (policy, preferences) => decisionHandler(policy));
			}

			public override void DecidePolicy(wk.WKWebView webView, wk.WKNavigationAction navigationAction, wk.WKWebpagePreferences preferences, Action<wk.WKNavigationActionPolicy, wk.WKWebpagePreferences> decisionHandler)
			{
				var h = Handler;
				if (h == null)
					return;
					
				var requestUrl = navigationAction.Request.Url;
				if (h.EnablePrintRouting && requestUrl.AbsoluteString == "eto:print")
				{
					// WKWebKit doesn't enable printing, so we have to handle it manually...
					h.ShowPrintDialog();
					decisionHandler(wk.WKNavigationActionPolicy.Cancel, preferences);
					return;
				}
				if (navigationAction.TargetFrame == null)
				{
					// how do we get the name/target??
					var newWindowArgs = new WebViewNewWindowEventArgs(requestUrl, string.Empty);
					h.Callback.OnOpenNewWindow(h.Widget, newWindowArgs);
					if (!newWindowArgs.Cancel)
					{
						Application.Instance.Open(requestUrl.AbsoluteString);
					}
					decisionHandler(wk.WKNavigationActionPolicy.Cancel, preferences);
					return;
				}
				var args = new WebViewLoadingEventArgs(new Uri(requestUrl.AbsoluteString), navigationAction.TargetFrame?.MainFrame == true);
				h.Callback.OnDocumentLoading(h.Widget, args);
				var policy = args.Cancel ? wk.WKNavigationActionPolicy.Cancel : wk.WKNavigationActionPolicy.Allow;
				decisionHandler(policy, preferences);
			}

		}
		
		protected override void Initialize()
		{
			Enabled = true;
			base.Initialize();

			Control.NavigationDelegate = new EtoNavigationDelegate { Handler = this };
		}

		public class EtoWebView : wk.WKWebView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public WKWebViewHandler Handler { get { return (WKWebViewHandler)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

			public EtoWebView(WKWebViewHandler handler)
				: base(new CGRect(0, 0, 200, 200), handler.CreateConfiguration())
			{
				Handler = handler;
				UIDelegate = new EtoUIDelegate { Handler = handler };
			}
			
			public EtoWebView(IntPtr handle)
				: base(handle)
			{
			}
		}

		class ScriptMessageHandler : wk.WKScriptMessageHandler
		{
			private readonly WKWebViewHandler Handler;
			public ScriptMessageHandler(WKWebViewHandler handler) : base()
			{
				Handler = handler;
			}
			public override void DidReceiveScriptMessage(wk.WKUserContentController userContentController, wk.WKScriptMessage message)
			{
				Handler.Callback.OnMessageReceived(Handler.Widget, new WebViewMessageEventArgs(message.Body.ToString()));
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

		public class EtoUIDelegate : wk.WKUIDelegate
		{
			WeakReference handler;

			public WKWebViewHandler Handler { get { return (WKWebViewHandler)handler.Target; } set { handler = new WeakReference(value); } }


			public override void RunJavaScriptAlertPanel(wk.WKWebView webView, string message, wk.WKFrameInfo frame, Action completionHandler)
			{
				MessageBox.Show(Handler.Widget, message);
				completionHandler();
			}

			public override void RunJavaScriptConfirmPanel(wk.WKWebView webView, string message, wk.WKFrameInfo frame, Action<bool> completionHandler)
			{
				var result = MessageBox.Show(Handler.Widget, message, MessageBoxButtons.YesNo) == DialogResult.Yes;
				completionHandler(result);
			}

			public override void RunJavaScriptTextInputPanel(wk.WKWebView webView, string prompt, string defaultText, wk.WKFrameInfo frame, Action<string> completionHandler)
			{
				var dialog = new PromptDialog
				{
					Prompt = prompt,
					Value = defaultText,
					Title = Handler.DocumentTitle
				};
				var result = dialog.ShowModal(Handler.Widget) ? dialog.Value : string.Empty;
				completionHandler(result);
			}

			public override void RunOpenPanel(wk.WKWebView webView, wk.WKOpenPanelParameters parameters, wk.WKFrameInfo frame, Action<NSUrl[]> completionHandler)
			{
				var openDlg = new OpenFileDialog { MultiSelect = parameters.AllowsMultipleSelection };
				if (openDlg.ShowDialog(Handler.Widget.ParentWindow) == DialogResult.Ok)
				{
					completionHandler(openDlg.Filenames.Select(r => NSUrl.FromFilename(r)).ToArray());
				}
				else
				{
					completionHandler(null);
				}
			}

			public override wk.WKWebView CreateWebView(wk.WKWebView webView, wk.WKWebViewConfiguration configuration, wk.WKNavigationAction navigationAction, wk.WKWindowFeatures windowFeatures)
			{
				var h = Handler;
				if (h == null)
					return null;
				var requestUrl = navigationAction.Request.Url;
				if (navigationAction.TargetFrame == null)
				{
					var newWindowArgs = new WebViewNewWindowEventArgs(requestUrl, string.Empty);
					h.Callback.OnOpenNewWindow(h.Widget, newWindowArgs);
					if (!newWindowArgs.Cancel)
					{
						// open in new window
						Application.Instance.Open(requestUrl.AbsoluteString);
					}
					return null;
				}

				return new EtoWebView(Handler);
			}
		}

		static NSString s_titleKey = new NSString("title");
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
					// handled by delegates
					break;
				case WebView.DocumentTitleChangedEvent:
					AddControlObserver(s_titleKey, TitleChangedObserver);
					// todo. need to observe the Title property.
					break;
				case WebView.MessageReceivedEvent:
					// Handled in constructor
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private wk.WKWebViewConfiguration CreateConfiguration()
		{
			wk.WKUserContentController contentController = new();

			// Handle messages sent from JS window.webkit.messageHandlers.__eto__.postMessage
			contentController.AddScriptMessageHandler(new ScriptMessageHandler(this), "__eto__");

			// Wrap the handler for x-plat consistency as window.eto.postMessage
			const string wrapper = @"window.eto = { postMessage: function(message) { window.webkit.messageHandlers.__eto__.postMessage(message); } };";
			contentController.AddUserScript(new wk.WKUserScript(new NSString(wrapper), wk.WKUserScriptInjectionTime.AtDocumentStart, false));

			return new wk.WKWebViewConfiguration()
			{
				UserContentController = contentController
			};
		}

		static void TitleChangedObserver(ObserverActionEventArgs obj)
		{
			var h = obj.Handler as WKWebViewHandler;
			if (h == null)
				return;
			h.Callback.OnDocumentTitleChanged(h.Widget, new WebViewTitleEventArgs(h.DocumentTitle));
		}

		public Uri Url
		{
			get => (Uri)Control.Url;
			set => Control.LoadRequest(new NSUrlRequest(value));
		}

		public string DocumentTitle => Control.Title;

		public string ExecuteScript(string script)
		{
			var task = ExecuteScriptAsync(script);

			while (!task.IsCompleted)
			{
				Application.Instance.RunIteration();
			}

			return task.Result;
		}

		public async Task<string> ExecuteScriptAsync(string script)
		{
			var fullScript = string.Format("var _fn = function() {{ {0} }}; _fn();", script);
			var result = await Control.EvaluateJavaScriptAsync(fullScript);
			return result?.ToString();
		}

		public void LoadHtml(string html, Uri baseUri)
		{
			var baseNSUrl = baseUri.ToNS();
			if (baseNSUrl != null)
				Control.LoadFileUrl(baseNSUrl, baseNSUrl);
			Control.LoadHtmlString(html, baseNSUrl);
		}

		public void Stop() => Control.StopLoading();

		public void Reload() => Control.Reload();

		public void GoBack() => Control.GoBack();

		public void GoForward() => Control.GoForward();

		public bool CanGoBack => Control.CanGoBack;

		public bool CanGoForward => Control.CanGoForward;

		static Selector s_selGetPrintOperationInternal = new Selector("_printOperationWithPrintInfo:");
		static Selector s_selGetPrintOperation = new Selector("printOperationWithPrintInfo:");

		public void ShowPrintDialog()
		{
			const float margin = 24f;
			var printInfo = new NSPrintInfo 
			{ 
				VerticallyCentered = false, 
				LeftMargin = margin, 
				RightMargin = margin, 
				TopMargin = margin, 
				BottomMargin = margin 
			};
			NSPrintOperation printOperation = null;

			if (Control.RespondsToSelector(s_selGetPrintOperation))
			{
				// big sur
				printOperation = Control.GetPrintOperation(printInfo);
			}
			else if (Control.RespondsToSelector(s_selGetPrintOperationInternal))
			{
				// older versions have this but is undocumented and internal..
				printOperation = Runtime.GetNSObject<NSPrintOperation>(Messaging.IntPtr_objc_msgSend_IntPtr(Control.Handle, s_selGetPrintOperationInternal.Handle, printInfo.Handle));
			}
			if (printOperation != null)
			{
				// RunOperation() doesn't work..
				var printHelper = new PrintHelper();
				printOperation.RunOperationModal(Control.Window, printHelper, PrintHelper.PrintOperationDidRunSelector, IntPtr.Zero);
			}
		}

		class PrintHelper : NSObject
		{
			public static Selector PrintOperationDidRunSelector = new Selector("printOperationDidRun:success:contextInfo:");

			public bool Success { get; set; }

			[Export("printOperationDidRun:success:contextInfo:")]
			public void PrintOperationDidRun(IntPtr printOperation, bool success, IntPtr contextInfo)
			{
				Success = success;
			}
		}

		static readonly object BrowserContextMenuEnabled_Key = new object();
		public bool BrowserContextMenuEnabled
		{
			get => Widget.Properties.Get<bool>(BrowserContextMenuEnabled_Key, true);
			set
			{
				if (Widget.Properties.TrySet<bool>(BrowserContextMenuEnabled_Key, value, true))
				{
					// this interferes with the current page, but there's no other way that I can find..
					if (Control.IsLoading)
						return;
					if (value)
					{
						ExecuteScript("document.body.setAttribute('oncontextmenu', '');");
					}
					else
					{
						ExecuteScript("document.body.setAttribute('oncontextmenu', 'event.preventDefault();');");
					}
				}
			}
		}

		public bool EnablePrintRouting { get; set; } = true;

		void SetupContextMenu()
		{
			if (!BrowserContextMenuEnabled)
			{
				// no way to do this through code.. 
				var task = ExecuteScriptAsync("document.body.setAttribute('oncontextmenu', 'event.preventDefault();');");
			}
			if (EnablePrintRouting)
			{
				// no way to do this through code.. 
				var task = ExecuteScriptAsync(@"window.print = function () { window.location = 'eto:print'; };");
			}

		}
	}
}

