using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void Blar(IntPtr block);

	public class WKWebViewHandler : MacView<wk.WKWebView, WebView, WebView.ICallback>, WebView.IHandler
	{
		static readonly Selector selIgnore = new Selector("ignore");
		static readonly Selector selUse = new Selector("use");

		public override NSView ContainerControl { get { return Control; } }

		public wk.WKWebViewConfiguration Configuration { get; set; } = new wk.WKWebViewConfiguration();

		protected override wk.WKWebView CreateControl()
		{
			return new EtoWebView(this);
		}

		public class EtoNavigationDelegate : wk.WKNavigationDelegate
		{
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

			public override void DecidePolicy(wk.WKWebView webView, wk.WKNavigationAction navigationAction, wk.WKWebpagePreferences preferences, Action<wk.WKNavigationActionPolicy, wk.WKWebpagePreferences> decisionHandler)
			{
				var h = Handler;
				if (h != null)
				{
					var args = new WebViewLoadingEventArgs(new Uri(navigationAction.Request.Url.AbsoluteString), navigationAction.TargetFrame?.MainFrame == true);
					h.Callback.OnDocumentLoading(h.Widget, args);
					var policy = args.Cancel ? wk.WKNavigationActionPolicy.Cancel : wk.WKNavigationActionPolicy.Allow;
					decisionHandler(policy, preferences);
				}
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
				: base(new CGRect(0, 0, 200, 200), handler.Configuration)
			{
				Handler = handler;
				UIDelegate = new EtoUIDelegate { Handler = handler };
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
				var openDlg = new OpenFileDialog();
				if (openDlg.ShowDialog(Handler.Widget.ParentWindow) == DialogResult.Ok)
				{
					completionHandler(openDlg.Filenames.Select(r => new NSUrl(r)).ToArray());
				}
				else
				{
					completionHandler(null);
				}
			}
			public override wk.WKWebView CreateWebView(wk.WKWebView webView, wk.WKWebViewConfiguration configuration, wk.WKNavigationAction navigationAction, wk.WKWindowFeatures windowFeatures)
			{
				return new EtoWebView(Handler);
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
			Control.LoadHtmlString(html, baseUri.ToNS());
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
			var printInfo = NSPrintInfo.SharedPrintInfo;
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
			printOperation?.RunOperation();
		}

		public bool BrowserContextMenuEnabled
		{
			get;
			set;
		}

		void SetupContextMenu()
		{
			if (!BrowserContextMenuEnabled)
			{
				// no way to do this through code.. 
				ExecuteScript("document.body.setAttribute('oncontextmenu', 'event.preventDefault();');");
			}

		}		
	}
}

