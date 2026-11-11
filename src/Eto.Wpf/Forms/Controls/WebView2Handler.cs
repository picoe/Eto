
// #define TEST_INSTALL // test installation without actually installing it.

using Eto.CustomControls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using System.Net;
using System.Net.Http;

#if WINFORMS
using WebView2Control = Microsoft.Web.WebView2.WinForms.WebView2;
using BaseHandler = Eto.WinForms.Forms.WindowsControl<Microsoft.Web.WebView2.WinForms.WebView2, Eto.Forms.WebView, Eto.Forms.WebView.ICallback>;

namespace Eto.WinForms.Forms.Controls;
#elif WPF
using WebView2Control = Microsoft.Web.WebView2.Wpf.WebView2;
using BaseHandler = Eto.Wpf.Forms.WpfFrameworkElement<Microsoft.Web.WebView2.Wpf.WebView2, Eto.Forms.WebView, Eto.Forms.WebView.ICallback>;
using BaseHost = Eto.Wpf.Forms.EtoBorder;

namespace Eto.Wpf.Forms.Controls;
#endif

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

	/// <summary>
	/// Detects whether WebView2 is installed
	/// </summary>
	/// <returns>true if installed, false if not installed</returns>
	public static bool Detect()
	{
#if TEST_INSTALL
			return false;
#endif
		try
		{
			var versionInfo = CoreWebView2Environment.GetAvailableBrowserVersionString();
			return versionInfo != null;
		}
		catch (WebView2RuntimeNotFoundException)
		{
			return false;
		}
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
		var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()) + ".exe";
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

#if NETFRAMEWORK
				using (var client = new WebClient())
				{
					var wait = new ManualResetEvent(false);
					var startProgress = currentProgress;
					client.DownloadProgressChanged += (sender, e) => Progress(startProgress + e.ProgressPercentage);
					await client.DownloadFileTaskAsync(InstallLink, tempFile);
				}
#else
			using (var client = new HttpClient())
			{
				var wait = new ManualResetEvent(false);
				var startProgress = currentProgress;
				using (var file = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					await client.DownloadAsync(InstallLink, file, progress => Progress(startProgress + progress));
				}
			}
#endif
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

#if WPF
public class WebView2Host : BaseHost
{
}
#endif

public class WebView2Handler : BaseHandler, WebView.IHandler
{
	bool webView2Ready;
	protected bool WebView2Ready => webView2Ready;
	CoreWebView2Environment _environment;
	List<Action> delayedActions;

	public WebView2Handler()
	{
		WebView2Loader.EnsureWebView2Runtime();
		Control = new WebView2Control();
		Control.CoreWebView2InitializationCompleted += Control_CoreWebView2Ready;
#if WPF
		_host = new WebView2Host();
		_host.Child = Control;
		_host.Handler = this;
		Control.IsVisibleChanged += Control_IsVisibleChanged;
		Control.MessageHook += Control_MessageHook;
#endif
	}

#if WPF
	WebView2Host _host;
	public override System.Windows.FrameworkElement ContainerControl => _host;

	bool _resyncingVisibility;
	bool _resyncPending;

	const int WM_SHOWWINDOW = 0x0018;
	const int WM_WINDOWPOSCHANGED = 0x0047;

	// WebView2 is told it is visible before its window is actually shown. When the window is hidden and
	// shown again in quick succession (e.g. a floating window as the app is deactivated) it can then stay
	// blank until resized, so tell it again once the window really is on screen.
	void Control_IsVisibleChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
	{
		if (_resyncingVisibility || !(bool)e.NewValue)
			return;
		_resyncPending = true;
		Control.Dispatcher.BeginInvoke(new Action(ResyncVisibility), swt.DispatcherPriority.ContextIdle);
	}

	// The web view's own window can still be hidden when the re-sync first runs, so try again once it is shown.
	IntPtr Control_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (_resyncPending && (msg == WM_SHOWWINDOW || msg == WM_WINDOWPOSCHANGED))
			Control.Dispatcher.BeginInvoke(new Action(ResyncVisibility), swt.DispatcherPriority.ContextIdle);
		return IntPtr.Zero;
	}

	void ResyncVisibility()
	{
		if (!_resyncPending)
			return;
		if (Widget.IsDisposed || !webView2Ready || !Control.IsVisible)
		{
			_resyncPending = false;
			return;
		}
		if (!Win32.IsWindowVisible(Control.Handle))
			return;

		_resyncPending = false;
		_resyncingVisibility = true;
		try
		{
			Control.Visibility = sw.Visibility.Hidden;
			Control.Visibility = sw.Visibility.Visible;
		}
		finally
		{
			_resyncingVisibility = false;
		}
	}
#endif
	/// <summary>
	/// The default environment to use if none is specified with <see cref="Environment"/>.
	/// </summary>
	public static CoreWebView2Environment CoreWebView2Environment;

	/// <summary>
	/// Specifies a function to call when we need the default environment, if not already specified
	/// </summary>
	public static Func<Task<CoreWebView2Environment>> GetCoreWebView2Environment;

	/// <summary>
	/// Gets or sets the environment to use, defaulting to <see cref="CoreWebView2Environment"/>.
	/// This can only be set once during construction or with a style for this handler.
	/// </summary>
	/// <value>Environment to use to initialize WebView2</value>
	public CoreWebView2Environment Environment
	{
		get => _environment ?? CoreWebView2Environment;
		set => _environment = value;
	}

	/// <summary>
	/// Set to true when every environment the app supplies (with <see cref="Environment"/>, <see cref="CoreWebView2Environment"/>,
	/// <see cref="GetCoreWebView2Environment"/> or by overriding <see cref="OnInitializeWebView2Async"/>) was created with options
	/// registering <see cref="CreateResourceSchemeRegistration"/>, so <see cref="WebView.AddResourceHandler"/> serves eto:// like other platforms instead of https://.
	/// </summary>
	public static bool CustomEnvironmentsHaveResourceScheme { get; set; }

	/// <summary>
	/// Creates the registration for the eto:// scheme served by <see cref="WebView.AddResourceHandler"/>, for an environment the app creates itself.
	/// </summary>
	/// <remarks>
	/// Pass it in the options' <c>customSchemeRegistrations</c>, e.g.
	/// <c>new CoreWebView2EnvironmentOptions(customSchemeRegistrations: new() { WebView2Handler.CreateResourceSchemeRegistration() })</c>.
	/// Also set <see cref="CustomEnvironmentsHaveResourceScheme"/> to true so web views use it.
	/// </remarks>
	public static CoreWebView2CustomSchemeRegistration CreateResourceSchemeRegistration() => new CoreWebView2CustomSchemeRegistration(ResourceScheme)
	{
		TreatAsSecure = true,
		HasAuthorityComponent = true
	};

	static string s_userDataFolder;

	/// <summary>
	/// Gets or sets the folder the default environment stores its browser data in.
	/// </summary>
	/// <remarks>
	/// Defaults to <c>%LOCALAPPDATA%\&lt;Company&gt;\&lt;Product&gt;\WebView2</c>, using the version info of the running .exe.
	/// Set before the first web view is created. The <c>WEBVIEW2_USER_DATA_FOLDER</c> environment variable still takes precedence.
	/// </remarks>
	public static string UserDataFolder
	{
		get => s_userDataFolder ??= GetDefaultUserDataFolder();
		set => s_userDataFolder = value;
	}

	static string GetDefaultUserDataFolder()
	{
		try
		{
			// WebView2's own default is next to the .exe, which isn't writable for installed apps.
			// The version resource works for native hosts too, where there is no entry assembly.
			string exePath;
			using (var process = Process.GetCurrentProcess())
				exePath = process.MainModule.FileName;
			var info = FileVersionInfo.GetVersionInfo(exePath);
			var company = SanitizeFolderName(info.CompanyName);
			var product = SanitizeFolderName(info.ProductName) ?? Path.GetFileNameWithoutExtension(exePath);
			// SDK projects default Company to the assembly name, so avoid MyApp\MyApp.
			var appFolder = company == null || company == product ? product : Path.Combine(company, product);
			return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), appFolder, "WebView2");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Could not determine the WebView2 user data folder, using the WebView2 default: {ex}");
			return null;
		}
	}

	static string SanitizeFolderName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return null;
		foreach (var ch in Path.GetInvalidFileNameChars())
			name = name.Replace(ch, '_');
		name = name.Trim().TrimEnd('.');
		return name.Length > 0 ? name : null;
	}

	// Schemes can only be registered when an environment is created, so Eto creates the default one itself.
	static Task<CoreWebView2Environment> s_defaultEnvironment;

	static Task<CoreWebView2Environment> GetDefaultEnvironmentAsync()
	{
		if (s_defaultEnvironment == null)
		{
			var options = new CoreWebView2EnvironmentOptions(customSchemeRegistrations: new List<CoreWebView2CustomSchemeRegistration> { CreateResourceSchemeRegistration() });
			s_defaultEnvironment = CoreWebView2Environment.CreateAsync(null, UserDataFolder, options);
		}
		return s_defaultEnvironment;
	}

	bool UsesDefaultEnvironment => Environment == null && GetCoreWebView2Environment == null && Control.CreationProperties == null;

	/// <summary>
	/// Override to use your own WebView2 initialization, if necessary
	/// </summary>
	/// <returns>Task</returns>
	protected async virtual Task OnInitializeWebView2Async()
	{
		var env = Environment;
		if (env == null && GetCoreWebView2Environment != null)
		{
			env = CoreWebView2Environment = await GetCoreWebView2Environment();
		}
		else if (UsesDefaultEnvironment)
		{
			env = await GetDefaultEnvironmentAsync();
		}

		await Control.EnsureCoreWebView2Async(env);
	}

	async void InitializeAsync() => await OnInitializeWebView2Async();

	protected override void Initialize()
	{
		base.Initialize();
		Size = new Size(100, 100);
#if WPF
		// We need these for HasFocus to return a proper value
		HandleEvent(Eto.Forms.Control.GotFocusEvent);
		HandleEvent(Eto.Forms.Control.LostFocusEvent);
#endif
	}

	protected override void OnInitializeComplete()
	{
		base.OnInitializeComplete();

		// initialize webview2 after styles are applied, since styles might be used to configure the Environment or CoreWebView2Environment
		InitializeAsync();
	}

	// E_ABORT - reported when initialization is aborted because the control was
	// torn down before EnsureCoreWebView2Async completed.
	const int E_ABORT = unchecked((int)0x80004004);

	void Control_CoreWebView2Ready(object sender, CoreWebView2InitializationCompletedEventArgs e)
	{
		if (!e.IsSuccess)
		{
			// When the control is disposed/unloaded before WebView2 finishes initializing
			// (e.g. closing a panel or switching the window/viewport layout) the
			// initialization is aborted with E_ABORT. This is benign and must not be
			// allowed to escape this native callback, since it would surface as an
			// unhandled exception on the dispatcher and crash the application.
			// https://github.com/MicrosoftEdge/WebView2Feedback/issues/2410
			if (Widget.IsDisposed || e.InitializationException?.HResult == E_ABORT)
				return;

			throw new WebView2InitializationException("Failed to initialize WebView2", e.InitializationException);
		}

		// Hosts must be intercepted before the control navigates to a Url set while it was initializing.
		var initializedCoreWebView2 = TryGetCoreWebView2();
		if (initializedCoreWebView2 != null && resourceHandlers != null)
		{
			foreach (var host in resourceHandlers.Keys)
				UpdateResourceFilter(initializedCoreWebView2, host);
		}

		// can't actually do anything here, so execute them in the main loop
		Application.Instance.AsyncInvoke(RunDelayedActions);
	}

	private void RunDelayedActions()
	{
		if (Widget.IsDisposed)
			return;

		// This runs a dispatcher turn after initialization completed, and the browser
		// process can die in between - CoreWebView2 then throws InvalidOperationException
		// from VerifyBrowserNotCrashed. There is nothing left to hook up when that has
		// happened, and letting it escape this dispatcher callback crashes the application.
		CoreWebView2 coreWebView2;
		try
		{
			coreWebView2 = Control.CoreWebView2;
		}
		catch (InvalidOperationException)
		{
			return;
		}
		if (coreWebView2 == null)
			return;

		coreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
		coreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
		coreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoadedInjectScripts;
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

	public override void OnUnLoad(EventArgs e)
	{
		base.OnUnLoad(e);

		// Fixes crash when shown as a child window and the parent is closed
		// https://github.com/MicrosoftEdge/WebView2Feedback/issues/1971
		// See WebViewTests.WebViewClosedAsChildShouldNotCrash for repro
#if WPF
		_host.Child = null;
#endif
	}

	public override void OnLoad(EventArgs e)
	{
#if WPF
		if (_host.Child == null)
			_host.Child = Control;
#endif
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

	protected void RunWhenReady(Action action)
	{
		if (webView2Ready)
		{
			// already ready, run now!
			action();
			return;
		}

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
#if WPF
			case Eto.Forms.Control.GotFocusEvent:
				Control.GotFocus += Control_GotFocus;
				break;
			case Eto.Forms.Control.LostFocusEvent:
				Control.LostFocus += Control_LostFocus;
				break;
#endif
			case WebView.MessageReceivedEvent:
				Control.WebMessageReceived += Control_WebMessageReceived;
				break;
			default:
				base.AttachEvent(handler);
				break;
		}

	}

#if WPF
	static readonly object HasFocus_Key = new object();

	public override bool HasFocus => Widget.Properties.Get<bool>(HasFocus_Key);

	private void Control_LostFocus(object sender, System.Windows.RoutedEventArgs e)
	{
		if (Widget.Properties.TrySet(HasFocus_Key, false))
		{
			Callback.OnLostFocus(Widget, EventArgs.Empty);
		}
	}

	private void Control_GotFocus(object sender, System.Windows.RoutedEventArgs e)
	{
		// IsFocused/IsKeyboardFocused/IsKeyboardFocusWithin doesn't appear to work with WebView2
		if (Widget.Properties.TrySet(HasFocus_Key, true))
		{
			Callback.OnGotFocus(Widget, EventArgs.Empty);
		}
	}
#endif

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
		Application.Instance.AsyncInvoke(() =>
		{
			if (Widget.IsDisposed)
				return;
			Callback.OnDocumentLoaded(Widget, args);
		});
	}


	/// <summary>
	/// Called from JavaScript via window.eto.postMessage (window.chrome.webview.postMessage)
	/// </summary>
	private void Control_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
	{
		string content = null;
		if (e.TryGetWebMessageAsString() is string str && !string.IsNullOrEmpty(str))
		{
			content = str;
		}
		else
		{
			content = e.WebMessageAsJson;
		}
		if (content != null)
		{
			Application.Instance.AsyncInvoke(() =>
			{
				if (Widget.IsDisposed)
					return;
				Callback.OnMessageReceived(Widget, new WebViewMessageEventArgs(content));
			});
		}
	}

	/// <summary>
	/// Wraps window.chrome.webview.postMessage to window.eto.postMessage for platform consistency
	/// </summary>
	private void CoreWebView2_DOMContentLoadedInjectScripts(object sender, CoreWebView2DOMContentLoadedEventArgs e)
	{
		CoreWebView2.ExecuteScriptAsync("window.eto = { postMessage: function(message) { window.chrome.webview.postMessage(message); } };");
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

	Dictionary<string, Func<WebViewResourceRequest, WebViewResourceResponse>> resourceHandlers;
	HashSet<string> filteredHosts;
	bool resourceRequestsHooked;

	public void LoadHtml(string html, Uri baseUri)
	{
		if (!webView2Ready)
		{
			RunWhenReady(() => LoadHtml(html, baseUri));
			return;
		}
		if (baseUri == null)
		{
			try
			{
				Control.NavigateToString(html);
				return;
			}
			catch (ArgumentException)
			{
				// NavigateToString rejects pages over 2 MB, so serve those from memory instead.
			}
		}

		var baseDirectory = baseUri != null && baseUri.IsFile ? baseUri.LocalPath : null;
		var htmlUri = AddResourceHandler(WebViewHtmlServer.Host, request => WebViewHtmlServer.Serve(html, baseDirectory, request));
		// Navigate rather than setting Source, so loading new html at the same address still reloads.
		CoreWebView2.Navigate(htmlUri.AbsoluteUri);
	}

	public bool SupportsResourceHandlers => true;

	public Uri AddResourceHandler(string host, Func<WebViewResourceRequest, WebViewResourceResponse> handler)
	{
		resourceHandlers ??= new Dictionary<string, Func<WebViewResourceRequest, WebViewResourceResponse>>(StringComparer.OrdinalIgnoreCase);
		resourceHandlers[host] = handler;
		// Before initialization, Control_CoreWebView2Ready adds the filter instead.
		var coreWebView2 = TryGetCoreWebView2();
		if (coreWebView2 != null)
			UpdateResourceFilter(coreWebView2, host);
		return new Uri(GetResourceRoot(host));
	}

	public void RemoveResourceHandler(string host)
	{
		if (resourceHandlers == null || !resourceHandlers.Remove(host))
			return;
		var coreWebView2 = TryGetCoreWebView2();
		if (coreWebView2 != null)
			UpdateResourceFilter(coreWebView2, host);
	}

	CoreWebView2 TryGetCoreWebView2()
	{
		try
		{
			return Control.CoreWebView2;
		}
		catch (InvalidOperationException)
		{
			// the browser process has gone away
			return null;
		}
	}

	const string ResourceScheme = "eto";
	bool? usesResourceScheme;

	// Decided once, so every address already handed out keeps working.
	string GetResourceRoot(string host)
	{
		usesResourceScheme ??= UsesDefaultEnvironment || CustomEnvironmentsHaveResourceScheme;
		return (usesResourceScheme.Value ? ResourceScheme : "https") + "://" + host.ToLowerInvariant() + "/";
	}

	// Only hosts with a handler are intercepted, so other requests never pay for the round trip.
	void UpdateResourceFilter(CoreWebView2 coreWebView2, string host)
	{
		if (!resourceRequestsHooked)
		{
			coreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
			resourceRequestsHooked = true;
		}
		filteredHosts ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var filter = GetResourceRoot(host) + "*";
		var hasHandler = resourceHandlers != null && resourceHandlers.ContainsKey(host);
		if (hasHandler && filteredHosts.Add(host))
			coreWebView2.AddWebResourceRequestedFilter(filter, CoreWebView2WebResourceContext.All);
		else if (!hasHandler && filteredHosts.Remove(host))
			coreWebView2.RemoveWebResourceRequestedFilter(filter, CoreWebView2WebResourceContext.All);
	}

	private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
	{
		var uri = new Uri(e.Request.Uri);
		if (resourceHandlers == null || !resourceHandlers.TryGetValue(uri.Host, out var handler))
			return;

		var environment = ((CoreWebView2)sender).Environment;
		WebViewResourceResponse response;
		try
		{
			response = handler(new WebViewResourceRequest(uri, e.Request.Method));
		}
		catch (Exception ex)
		{
			// Letting this escape the WebView2 callback would crash the application.
			Debug.WriteLine("WebView resource handler failed for " + uri + ":\n" + ex);
			e.Response = environment.CreateWebResourceResponse(null, (int)HttpStatusCode.InternalServerError, "Internal Server Error", string.Empty);
			return;
		}

		if (response == null)
		{
			e.Response = environment.CreateWebResourceResponse(null, (int)HttpStatusCode.NotFound, "Not Found", string.Empty);
			return;
		}

		var headers = new List<string>();
		if (!string.IsNullOrEmpty(response.ContentType))
			headers.Add("Content-Type: " + response.ContentType);
		foreach (var header in response.Headers)
			headers.Add(header.Key + ": " + header.Value);
		var reasonPhrase = Enum.IsDefined(typeof(HttpStatusCode), response.StatusCode) ? ((HttpStatusCode)response.StatusCode).ToString() : string.Empty;
		e.Response = environment.CreateWebResourceResponse(response.Content, response.StatusCode, reasonPhrase, string.Join("\r\n", headers));
	}

	protected Microsoft.Web.WebView2.Core.CoreWebView2 CoreWebView2
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
