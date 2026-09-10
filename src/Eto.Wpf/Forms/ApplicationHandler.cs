using System.Windows.Threading;
using Microsoft.Win32;

namespace Eto.Wpf.Forms;

public class ApplicationHandler : WidgetHandler<sw.Application, Application, Application.ICallback>, Application.IHandler
{
	bool _attached;
	bool _shutdown;
	string _badgeLabel;
	List<FormHandler> _delayShownWindows;
	Dispatcher _dispatcher;
	bool? _isActive;
	ThemeStyle? _lastDetectedTheme;

	public static ApplicationHandler Instance => Application.Instance?.Handler as ApplicationHandler;

	public static bool EnableVisualStyles = true;

	/// <summary>
	/// Enable custom eto-defined themes for standard or extended wpf toolkit controls.
	/// </summary>
	/// <remarks>
	/// Set this before creating the Eto Application instance.
	/// </remarks>
	public static bool EnableCustomThemes = true;

	/// <summary>
	/// Suppress WPF's "Resource not found" trace warnings while the theme's resource dictionaries
	/// are being swapped.
	/// </summary>
	/// <remarks>
	/// Changing a theme swaps resource dictionaries under a visual tree that is already on screen.
	/// WPF re-resolves every DynamicResource in that tree as each dictionary is merged or removed,
	/// while the controls still carry the outgoing theme's templates - so any key the new theme
	/// does not define (all of the palette theme's Eto.Palette.* keys, when switching to a theme
	/// that isn't palette-driven) is looked up once per reference and traced as a warning, even
	/// though nothing is wrong: those templates are replaced moments later.
	///
	/// Only lookups made during the swap itself are hidden, and only on the ResourceDictionary
	/// trace source, which is put back to the level it was at as soon as the swap is done - so a
	/// key genuinely missing from the theme in effect is still traced when something looks it up
	/// outside of a theme change. Set to false to see the warnings anyway.
	/// </remarks>
	public static bool SuppressThemeChangeResourceWarnings = true;

	internal List<FormHandler> DelayShownWindows => _delayShownWindows ??= new List<FormHandler>();

	public bool IsStarted { get; private set; }

	sw.ResourceDictionary _themeResources;

	void ApplyThemes()
	{
		if (!EnableCustomThemes)
			return;

		if (_currentTheme == null)
		{
			_currentTheme = ThemesHandler.GetNone();
			ApplyTheme(_currentTheme);
		}
	}

	protected override void Initialize()
	{
		Control = sw.Application.Current;
		if (Control == null)
		{
			Control = new sw.Application { ShutdownMode = sw.ShutdownMode.OnExplicitShutdown };
			sw.Forms.Application.EnableVisualStyles();
			Control.Startup += (s, e) => HandleStartup();
		}
		else
		{
			HandleStartup();
		}

		// Prevent race condition with volatile font collection field in WPF when measuring a window the first time
		// When running on non-english windows it can cause a NullReferenceException in System.Windows.Media.FontFamily.LookupFontFamilyAndFace
		// This is a hack, but no way around it thus far..
		var temp = sw.SystemFonts.MessageFontFamily.Baseline;

		_dispatcher = Dispatcher.CurrentDispatcher;

		if (SynchronizationContext.Current == null)
			SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(_dispatcher));
		ApplyThemes();
		base.Initialize();
	}

	void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		var unhandledExceptionArgs = new UnhandledExceptionEventArgs(e.Exception, true);
		Callback.OnUnhandledException(Widget, unhandledExceptionArgs);
	}

	void OnCurrentDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
	{
		var unhandledExceptionArgs = new UnhandledExceptionEventArgs(e.ExceptionObject, e.IsTerminating);
		Callback.OnUnhandledException(Widget, unhandledExceptionArgs);
	}

	void HandleStartup()
	{
		IsStarted = true;
		Control.Activated += (sender2, e2) => Callback.OnIsActiveChanged(Widget, EventArgs.Empty);
		Control.Deactivated += (sender2, e2) => Callback.OnIsActiveChanged(Widget, EventArgs.Empty);
		if (_delayShownWindows != null)
		{
			foreach (var window in _delayShownWindows)
			{
				window.Show();
			}
			_delayShownWindows = null;
		}

		// Set up system theme change detection
		SetupSystemThemeChangeListener();
	}

	private void SetupSystemThemeChangeListener()
	{
		try
		{
			// Register for system theme change notifications via registry
			SystemEvents.UserPreferenceChanged += OnSystemThemeChanged;
		}
		catch
		{
			// If system event registration fails, theme changes won't be detected
			// but the app will still work with the theme set at startup
		}
	}

	private void OnSystemThemeChanged(object sender, UserPreferenceChangedEventArgs e)
	{
		if (e.Category == UserPreferenceCategory.General)
		{
			CheckAndUpdateSystemTheme();
		}
	}

	private void CheckAndUpdateSystemTheme()
	{
		try
		{
			// Only trigger theme change if we're using System theme
#if NET9_0_OR_GREATER
			if (Control.ThemeMode != sw.ThemeMode.System)
				return;
#endif

			var nowDetectedTheme = GetSystemThemeStyle();
			if (_lastDetectedTheme != nowDetectedTheme)
			{
				_lastDetectedTheme = nowDetectedTheme;
				// Trigger the CurrentThemeChanged event
				// This will refresh the theme resources
				Callback.OnThemeChanged(Widget, EventArgs.Empty);
			}
		}
		catch
		{
			// Silently handle any errors in theme detection
		}
	}

	internal static ThemeStyle GetSystemThemeStyle()
	{
		try
		{
			using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
			{
				if (key?.GetValue("AppsUseLightTheme") is int value)
				{
					return value == 1 ? ThemeStyle.Light : ThemeStyle.Dark;
				}
			}
		}
		catch
		{
			// If reading registry fails, default to light
		}
		
		return ThemeStyle.Light;
	}

	public bool IsActive
	{
		get => _isActive ?? Win32.ApplicationIsActivated();
		set
		{
			if (_isActive != value)
			{
				_isActive = value;
				Callback.OnIsActiveChanged(Widget, EventArgs.Empty);
			}
		}
	}

	public string BadgeLabel
	{
		get { return _badgeLabel; }
		set
		{
			_badgeLabel = value;
			var mainWindow = sw.Application.Current.MainWindow;
			if (mainWindow != null)
			{
				if (mainWindow.TaskbarItemInfo == null)
					mainWindow.TaskbarItemInfo = new sw.Shell.TaskbarItemInfo();
				if (!string.IsNullOrEmpty(_badgeLabel))
				{
					// scale by the current pixel size
					var m = sw.PresentationSource.FromVisual(mainWindow).CompositionTarget.TransformToDevice;
					var scale = (float)m.M22;

					var bmp = GenerateBadge(scale, _badgeLabel);
					mainWindow.TaskbarItemInfo.Overlay = bmp;
				}
				else
					mainWindow.TaskbarItemInfo.Overlay = null;
			}
		}
	}

	protected virtual swm.Imaging.BitmapSource GenerateBadge(float scale, string label)
	{
		var size = Size.Round(new SizeF(14, 14) * scale);
		var bmp = new Bitmap(size, PixelFormat.Format32bppRgba);

		using (var graphics = new Graphics(bmp))
		{
			var font = SystemFonts.Bold(6 * scale);

			var textSize = graphics.MeasureString(font, label);
			graphics.FillEllipse(Brushes.Red, new Rectangle(size));

			var pt = new PointF((bmp.Width - textSize.Width) / 2, (bmp.Height - textSize.Height - scale) / 2);
			graphics.DrawText(font, Brushes.White, pt, label);
		}

		return bmp.ToWpf();
	}

	public void RunIteration()
	{
		var frame = new DispatcherFrame();
		Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
		Dispatcher.PushFrame(frame);
		WpfFrameworkElementHelper.ShouldCaptureMouse = false;
	}

	static object ExitFrame(object f)
	{
		((DispatcherFrame)f).Continue = false;
		return null;
	}

	public void Quit()
	{
		// Clean up system theme change listener
		try
		{
			SystemEvents.UserPreferenceChanged -= OnSystemThemeChanged;
		}
		catch
		{
			// Ignore any errors during cleanup
		}

		bool cancel = false;
		foreach (sw.Window window in Control.Windows)
		{
			window.Close();
			cancel |= window.IsVisible;
		}
		if (!cancel)
		{
			Control.Shutdown();
			_shutdown = true;
		}
	}

	public bool QuitIsSupported { get { return true; } }

	public void Invoke(Action action)
	{
		if (_dispatcher == null || Thread.CurrentThread == _dispatcher.Thread)
			action();
		else
		{
			_dispatcher.Invoke(action);
		}
	}

	public void AsyncInvoke(Action action)
	{
		_dispatcher.BeginInvoke(action, sw.Threading.DispatcherPriority.Normal);
	}

	public Keys CommonModifier
	{
		get { return Keys.Control; }
	}

	public Keys AlternateModifier
	{
		get { return Keys.Alt; }
	}

	Theme _currentTheme;
	public Theme Theme
	{
		get
		{
#if NET9_0_OR_GREATER
			return _currentTheme ??= Control.ThemeMode != sw.ThemeMode.None ? new Theme(new FluentThemeHandler(Control.ThemeMode)) : null;
#else
			return _currentTheme ??= ThemesHandler.GetNone();
#endif
		}
		set
		{
			var previousTheme = _currentTheme;
			_currentTheme = value;
			ApplyTheme(value, previousTheme);

			Callback.OnThemeChanged(Widget, EventArgs.Empty);
		}
	}

	private void ApplyTheme(Theme value, Theme previousTheme = null)
	{
		// A theme applies to the process-wide WPF Application, which has thread affinity. A
		// secondary UI thread that creates its own Eto Application (Application.Attach) gets its
		// own handler, and that handler must leave the theme alone: the thread that owns the WPF
		// Application has already applied it, and touching Application.ThemeMode or .Resources
		// from here throws a cross-thread InvalidOperationException.
		if (Control == null || Control.Dispatcher != Dispatcher.CurrentDispatcher)
			return;

		// Every merge/remove below invalidates the DynamicResource references of the visual tree
		// that is already on screen, which is noisy by nature - see
		// SuppressThemeChangeResourceWarnings. All of it happens synchronously here, so the trace
		// source only has to be quietened for the duration of this method.
		var trace = SuppressThemeChangeResourceWarnings ? PresentationTraceSources.ResourceDictionarySource : null;
		var previousTraceLevel = trace?.Switch.Level ?? default;
		if (trace != null)
			trace.Switch.Level = SourceLevels.Off;
		try
		{
			SwapThemeResources(value, previousTheme);
		}
		finally
		{
			if (trace != null)
				trace.Switch.Level = previousTraceLevel;
		}
	}

	private void SwapThemeResources(Theme value, Theme previousTheme)
	{
		if (!ReferenceEquals(previousTheme, value) && previousTheme?.Handler is IThemeHandler previousHandler)
			previousHandler.UnsetTheme();

		// Note the outgoing dictionaries are removed BEFORE the incoming ones are merged, and each
		// change is left to raise its own notification. Overlapping the two (merging first, so the
		// keys the outgoing templates reference stay resolvable) and batching them into a single
		// notification with BeginInit/EndInit both cut the warnings down further, but each leaves
		// controls mis-rendered afterwards - stale visual states on CheckBox/ProgressBar for the
		// former, washed-out foregrounds on the menu bar, tool bar and tab headers for the latter.
		// The warnings are cosmetic; getting the tree correctly re-styled is not.
		var mergedDicts = Control.Resources.MergedDictionaries;
		if (_themeResources != null)
		{
			mergedDicts.Remove(_themeResources);
			_themeResources = null;
		}

		if (value?.Handler is IThemeHandler handler)
		{
			handler.SetTheme();
			var uris = handler.GetResourceUris()?.ToList();
			if (uris != null)
			{
				_themeResources = new sw.ResourceDictionary();
				foreach (var uri in uris)
				{
					var resource = new sw.ResourceDictionary { Source = uri };
					_themeResources.MergedDictionaries.Add(resource);
				}
				mergedDicts.Add(_themeResources);
			}

			// Now that the theme's dictionaries (including the palette) are merged, let the
			// handler apply any overrides that depend on the merged values.
			handler.ThemeResourcesMerged();
		}
	}

	public void Open(string url)
	{
		Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
	}

	public void Run()
	{
#if NET9_0_OR_GREATER
		//		Control.ThemeMode = sw.ThemeMode.System;
#endif
		Callback.OnInitialized(Widget, EventArgs.Empty);
		if (!_attached)
		{
			if (_shutdown)
				return;
			if (Widget.MainForm != null)
			{
				Control.ShutdownMode = sw.ShutdownMode.OnMainWindowClose;
				Control.Run((sw.Window)Widget.MainForm.ControlObject);
			}
			else
			{
				Control.Run();
			}
		}
	}

	public void Attach(object context)
	{
		_attached = true;
		Control = sw.Application.Current;
	}

	public void OnMainFormChanged()
	{
		sw.Application.Current.MainWindow = Widget.MainForm.ToNative();
	}

	public void Restart()
	{
		System.Windows.Forms.Application.Restart();
		sw.Application.Current.Shutdown();
	}

	public override void AttachEvent(string id)
	{
		switch (id)
		{
			case Application.TerminatingEvent:
				// handled by WpfWindow
				break;
			case Application.UnhandledExceptionEvent:
				AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
				sw.Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
				break;
			case Application.NotificationActivatedEvent:
				// handled by NotificationHandler
				break;
			case Application.IsActiveChangedEvent:
				// handled always
				break;
			case Application.ThemeChangedEvent:
				
				break;
			default:
				base.AttachEvent(id);
				break;
		}
	}
}
