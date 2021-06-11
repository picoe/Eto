
// #define TEST_INSTALL // test installation without actually installing it.

#if !NET45
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Eto.Forms;
using Eto.CustomControls;
using Eto.Drawing;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using Microsoft.Win32;
using System.Net;
using System.IO;
using System.Runtime.Serialization;

#if WINFORMS
using WebView2Control = Microsoft.Web.WebView2.WinForms.WebView2;
using BaseHandler = Eto.WinForms.Forms.WindowsControl<Microsoft.Web.WebView2.WinForms.WebView2, Eto.Forms.WebView, Eto.Forms.WebView.ICallback>;

namespace Eto.WinForms.Forms.Controls
#elif WPF
using WebView2Control = Microsoft.Web.WebView2.Wpf.WebView2;
using BaseHandler = Eto.Wpf.Forms.WpfFrameworkElement<Microsoft.Web.WebView2.Wpf.WebView2, Eto.Forms.WebView, Eto.Forms.WebView.ICallback>;

namespace Eto.Wpf.Forms.Controls
#endif
{
	/// <summary>
	/// Loader class and helper utilities to install the WebView2 runtime.
	/// </summary>
	public static class WebView2Loader
	{
		/// <summary>
		/// Creates the WetView2Handler
		/// </summary>
		/// <remarks>
		/// We create the handler here so that the TypeLoadException happens with this class vs. the calling class (e.g. Platform).
		/// </remarks>
		/// <returns>A new WebView2Handler instance</returns>
		public static WebView.IHandler Create() => new WebView2Handler();

		/// <summary>
		/// Link to download the bootstrapper to install WebView2 components
		/// </summary>
		public static Uri InstallLink = new Uri("https://go.microsoft.com/fwlink/p/?LinkId=2124703");

		/// <summary>
		/// Link for users to download the components manually
		/// </summary>
		public static Uri DownloadLink = new Uri("https://developer.microsoft.com/en-us/microsoft-edge/webview2/");

		/// <summary>
		/// Mode to use to install the WebView2 components if not already installed
		/// </summary>
		public static WebView2InstallMode InstallMode = WebView2InstallMode.Manual;


		const string reg64BitKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";
		const string reg32BitKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";

		/// <summary>
		/// Detects whether WebView2 is installed
		/// </summary>
		/// <returns>true if installed, false if not installed</returns>
		public static bool Detect()
		{
#if TEST_INSTALL
			return false;
#endif
			// https://docs.microsoft.com/en-us/microsoft-edge/webview2/concepts/distribution#deploying-the-evergreen-webview2-runtime
			var pv = Registry.GetValue(Environment.Is64BitOperatingSystem ? reg64BitKey : reg32BitKey, "pv", null);
			return pv is string s && !string.IsNullOrEmpty(s);
		}

		/// <summary>
		/// Detects the WebView2 runtime and installs it based on the current <see cref="InstallMode" />
		/// </summary>
		/// <remarks>
		/// This will throw <see cref="WebView2NotInstalledException" /> if the runtime cannot be installed or user opted not to install.
		/// </remarks>
		public static void EnsureWebView2Runtime()
		{
			if (Detect())
				return;

			switch (InstallMode)
			{
				case WebView2InstallMode.Manual:
					var dlg = new ManualInstallDialog();
					dlg.ShowModal();
					break;
				case WebView2InstallMode.Automatic:
					InstallWebView2WithUI();
					break;
				case WebView2InstallMode.Fallback:
					throw new WebView2NotInstalledException();
			}

			if (!Detect())
				throw new WebView2NotInstalledException();
		}

		static Task<int> RunProcessAsync(ProcessStartInfo startInfo)
		{
			var tcs = new TaskCompletionSource<int>();

			var process = new Process
			{
				StartInfo = startInfo,
				EnableRaisingEvents = true
			};

			process.Exited += (sender, args) =>
			{
				tcs.SetResult(process.ExitCode);
				process.Dispose();
			};

			process.Start();

			return tcs.Task;
		}

		internal static void CenterDialog(Dialog dialog)
		{
			// this should be in Eto, but alas..
#if WPF
			if (dialog.ControlObject is System.Windows.Window w)
			{
				w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			}
#elif WINFORMS
			if (dialog.ControlObject is System.Windows.Forms.Form f)
			{
				f.Load += (sender, e) =>
				{
					var screen = System.Windows.Forms.Screen.FromControl(f);

					var workingArea = screen.WorkingArea;
					f.Location = new System.Drawing.Point(
						Math.Max(workingArea.X, workingArea.X + (workingArea.Width - f.Width) / 2),
						Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - f.Height) / 2)
					);
				};
			}
#endif
		}

		/// <summary>
		/// Installs the WebView2 runtime, showing a form while it installs.
		/// </summary>
		public static void InstallWebView2WithUI(Control parent = null)
		{
			var dialog = new Dialog
			{
				Title = Loc("Installing WebView2 runtime"),
				WindowStyle = WindowStyle.Utility,
				Minimizable = false,
				Maximizable = false,
				MinimumSize = new Size(300, 100)
			};
			CenterDialog(dialog);
			var textLabel = new Label();
			var progressBar = new ProgressBar { MaxValue = 1000 };
			var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(5, 5) };
			layout.Add(textLabel);
			layout.Add(progressBar);
			dialog.Content = layout;

			void ReportProgress(WebView2ProgressInfo progress) => Application.Instance.Invoke(() =>
			{
				textLabel.Text = progress.Text;
				progressBar.Value = (int)(progress.Progress * progressBar.MaxValue);
			});

			dialog.LoadComplete += async (sender, e) =>
			{
				await Task.Run(() => InstallWebView2(ReportProgress));
				dialog.Close();
			};
			dialog.ShowModal(parent);
		}

		static string Loc(string str) => Application.Instance.Localize(typeof(WebView2Loader), str);
		/// <summary>
		/// Installs the WebView2 runtime by downloading the bootstrapper and running it in elevated permissions
		/// </summary>
		/// <param name="reportProgress">delegate to report progress</param>
		/// <returns>task for async</returns>
		public static async Task InstallWebView2(Action<WebView2ProgressInfo> reportProgress)
		{
			var info = new WebView2ProgressInfo();
			double totalProgress = 220;
			double currentProgress = 0;
			void Progress(double progress)
			{
				currentProgress = progress;
				info.Progress = currentProgress / totalProgress;
#if TEST_INSTALL
				Thread.Sleep(200);
#endif
				reportProgress?.Invoke(info);
			}
			// download bootstrapper to temp folder
			var tempFile = Path.GetTempFileName() + ".exe";
			try
			{
				info.Text = Loc("Downloading bootstrapper...");
				Progress(10);

#if TEST_INSTALL
				/* For testing.. */
				for (int i = 0; i < 10; i++)
					Progress(i * 5 + 10);

				return;
#endif

				using (var client = new WebClient())
				{
					var wait = new ManualResetEvent(false);
					var startProgress = currentProgress;
					client.DownloadProgressChanged += (sender, e) => Progress(startProgress + e.ProgressPercentage);
					await client.DownloadFileTaskAsync(InstallLink, tempFile);
				}
				info.Text = Loc("Installing WebView2...");
				Progress(120);
				// run with elevated privileges
				var startInfo = new ProcessStartInfo(tempFile);
				startInfo.Verb = "runas";
				startInfo.UseShellExecute = false;
				var result = await RunProcessAsync(startInfo);
				if (result != 0 && !Detect())
				{
					MessageBox.Show(Loc("Could not install WebView2 runtime. Try installing manually."), MessageBoxType.Error);
				}
			}
			finally
			{
				info.Text = Loc("Cleaning Up...");
				Progress(210);
				// delete bootstrapper
				if (File.Exists(tempFile))
					File.Delete(tempFile);
				Progress(totalProgress);
			}
		}
	}

	public class WebView2ProgressInfo
	{
		public bool Complete { get; set; }
		public string Text { get; set; }
		public double Progress { get; set; }
	}

	/// <summary>
	/// Modes for WebView2 runtime installation when used in Eto.
	/// </summary>
	public enum WebView2InstallMode
	{
		/// <summary>
		/// Shows a dialog to the user to install the WebView2 runtime
		/// </summary>
		Manual,
		/// <summary>
		/// Automatically installs the WebView2 runtime. Requires the user to elevate permissions.
		/// </summary>
		Automatic,
		/// <summary>
		/// Silently fall back to the old IE webview.
		/// </summary>
		Fallback,
	}

	class ManualInstallDialog : Dialog
	{
		UITimer timer;
		public ManualInstallDialog()
		{
			WebView2Loader.CenterDialog(this);
			string Loc(string str) => Application.Instance.Localize(typeof(WebView2Loader), str);

			Title = Loc("Install WebView2 Runtime");

			MinimumSize = new Size(300, 200);

			// controls

			var installButton = new Button { Text = Loc("Download && Install Now") };
			installButton.Click += OnInstall;

			var downloadButton = new LinkButton { Text = Loc("Open the WebView2 download page to install manually") };
			downloadButton.Click += OnDownload;

			var cancelButton = new Button { Text = Loc("Fallback to IE") };
			cancelButton.Click += OnCancel;

			var description = new Label
			{
				TextAlignment = TextAlignment.Center,
				Text = Loc("This application requires the Microsoft Edge WebView2 Runtime.\nPlease install it to continue.")
			};

			// layout

			var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(5, 5) };
			layout.AddSpace();
			layout.Add(description);
			layout.AddSpace();

			layout.AddSeparateRow(null, downloadButton);

			Content = layout;

			DefaultButton = installButton;
			PositiveButtons.Add(installButton);
			NegativeButtons.Add(cancelButton);

			Shown += (sender, e) => installButton.Focus();

			timer = new UITimer { Interval = 1.0 };
			timer.Elapsed += timer_Elapsed;
			timer.Start();
		}

		protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);

			timer?.Stop();
			timer = null;
		}

		private async void timer_Elapsed(object sender, EventArgs e)
		{
			if (WebView2Loader.Detect())
			{
				timer?.Stop();
				// wait for things to percolate
				await Task.Delay(2000);
				// ensure we didn't get closed already..
				if (timer != null)
				{
					Close();
				}
			}
		}

		private void OnCancel(object sender, EventArgs e)
		{
			Close();
			throw new WebView2NotInstalledException();
		}

		private void OnDownload(object sender, EventArgs e)
		{
			Application.Instance.Open(WebView2Loader.DownloadLink.ToString());
		}

		private void OnInstall(object sender, EventArgs e)
		{
			WebView2Loader.InstallWebView2WithUI(this);
			Close();
		}
	}


	public class WebView2NotInstalledException : System.Exception
	{
		public WebView2NotInstalledException() : base("The WebView2 runtime is not installed") { }
	}
	
	public class WebView2InitializationException : System.Exception
	{
		public WebView2InitializationException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class WebView2Handler : BaseHandler, WebView.IHandler
	{
		bool webView2Ready;
		List<Action> delayedActions;

		public WebView2Handler()
		{
			WebView2Loader.EnsureWebView2Runtime();
			Control = new WebView2Control();
			Control.CoreWebView2InitializationCompleted += Control_CoreWebView2Ready;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Size = new Size(100, 100);
		}

		void Control_CoreWebView2Ready(object sender, CoreWebView2InitializationCompletedEventArgs e)
		{
			if (!e.IsSuccess)
			{
				throw new WebView2InitializationException("Failed to initialze WebView2", e.InitializationException);
			}
			
			// can't actually do anything here, so execute them in the main loop
			Application.Instance.AsyncInvoke(RunDelayedActions);
		}

		private void RunDelayedActions()
		{
			Control.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
			Control.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
			webView2Ready = true;

			if (delayedActions != null)
			{
				for (int i = 0; i < delayedActions.Count; i++)
				{
					delayedActions[i].Invoke();
				}
				delayedActions = null;
			}
		}

		private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
		{
			var args = new WebViewNewWindowEventArgs(new Uri(e.Uri), null);
			Callback.OnOpenNewWindow(Widget, args);
			e.Handled = args.Cancel;
		}

		private void CoreWebView2_DocumentTitleChanged(object sender, object e)
		{
			Callback.OnDocumentTitleChanged(Widget, new WebViewTitleEventArgs(CoreWebView2.DocumentTitle));
		}

		void RunWhenReady(Action action)
		{
			if (delayedActions == null)
			{
				delayedActions = new List<Action>();
			}

			delayedActions.Add(action);
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case WebView.NavigatedEvent:
					Control.ContentLoading += Control_ContentLoading;
					break;
				case WebView.DocumentLoadedEvent:
					Control.NavigationCompleted += Control_NavigationCompleted;
					break;
				case WebView.DocumentLoadingEvent:
					Control.NavigationStarting += Control_NavigationStarting;
					break;
				case WebView.OpenNewWindowEvent:
					break;
				case WebView.DocumentTitleChangedEvent:
					break;
				default:
					base.AttachEvent(handler);
					break;
			}

		}

		private void Control_ContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e)
		{
			var args = new WebViewLoadedEventArgs(Control.Source);
			Callback.OnNavigated(Widget, args);
		}

		private void Control_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
		{
			var args = new WebViewLoadingEventArgs(new Uri(e.Uri), true);
			Callback.OnDocumentLoading(Widget, args);
			e.Cancel = args.Cancel;
		}

		private void Control_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
		{
			var args = new WebViewLoadedEventArgs(Control.Source);
			Application.Instance.AsyncInvoke(() => Callback.OnDocumentLoaded(Widget, args));
		}

		public Uri Url
		{
			get { return Control.Source; }
			set { Control.Source = value; }
		}

		public string DocumentTitle => CoreWebView2?.DocumentTitle;

		public string ExecuteScript(string script)
		{
			var fullScript = string.Format("var _fn = function() {{ {0} }}; _fn();", script);
			var task = Control.ExecuteScriptAsync(fullScript);
			while (!task.IsCompleted)
			{
				if (!Widget.Loaded)
					return null;
				Application.Instance.RunIteration();
				Thread.Sleep(10);
			}
			return Decode(task.Result);
		}

		public async Task<string> ExecuteScriptAsync(string script)
		{
			var fullScript = string.Format("var _fn = function() {{ {0} }}; _fn();", script);
			var result = await Control.ExecuteScriptAsync(fullScript);
			return Decode(result);
		}

		string Decode(string result)
		{
			// result is json-encoded. cool, but why?
			if (result == null)
				return null;
			if (result.StartsWith("\"") && result.EndsWith("\""))
			{
				return result.Substring(1, result.Length - 2);
			}
			return result;
		}

		public void Stop() => Control.Stop();

		public void Reload() => Control.Reload();

		public void GoBack() => Control.GoBack();

		public bool CanGoBack => Control.CanGoBack;

		public void GoForward() => Control.GoForward();

		public bool CanGoForward => Control.CanGoForward;

		HttpServer server;

		public void LoadHtml(string html, Uri baseUri)
		{
			if (!webView2Ready)
			{
				RunWhenReady(() => LoadHtml(html, baseUri));
				return;
			}
			if (baseUri != null)
			{
				if (server == null)
					server = new HttpServer();
				server.SetHtml(html, baseUri != null ? baseUri.LocalPath : null);
				Control.Source = server.Url;
			}
			else
			{
				Control.NavigateToString(html);
			}

		}

		Microsoft.Web.WebView2.Core.CoreWebView2 CoreWebView2
		{
			get
			{
				if (!webView2Ready)
					return null;
				return Control.CoreWebView2;
			}
		}

		public void ShowPrintDialog() => ExecuteScript("print()");


		static readonly object BrowserContextMenuEnabled_Key = new object();

		public bool BrowserContextMenuEnabled
		{
			get => CoreWebView2?.Settings.AreDefaultContextMenusEnabled ?? Widget.Properties.Get<bool>(BrowserContextMenuEnabled_Key, true);
			set
			{
				if (Widget.Properties.TrySet<bool>(BrowserContextMenuEnabled_Key, value, true))
				{
					if (!webView2Ready)
					{
						RunWhenReady(() => CoreWebView2.Settings.AreDefaultContextMenusEnabled = value);
						return;
					}
					CoreWebView2.Settings.AreDefaultContextMenusEnabled = value;
				}
			}
		}

#if WPF
		public override Color BackgroundColor
		{
			get => Colors.Transparent;
			set { }
		}
#endif

	}
}

#endif