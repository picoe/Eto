using System.Windows.Threading;
namespace Eto.Wpf.Forms
{
	public class ApplicationHandler : WidgetHandler<sw.Application, Application, Application.ICallback>, Application.IHandler
	{
		bool attached;
		bool shutdown;
		string badgeLabel;
		static ApplicationHandler instance;
		List<sw.Window> delayShownWindows;
		static Dispatcher dispatcher;

		public static ApplicationHandler Instance
		{
			get { return instance; }
		}

		public static bool EnableVisualStyles = true;

		/// <summary>
		/// Enable custom eto-defined themes for standard or extended wpf toolkit controls.
		/// </summary>
		/// <remarks>
		/// Set this before creating the Eto Application instance.
		/// </remarks>
		public static bool EnableCustomThemes = true;

		public static void InvokeIfNecessary(Action action)
		{
			if (dispatcher == null || Thread.CurrentThread == dispatcher.Thread)
				action();
			else
			{
				sw.Application.Current.Dispatcher.Invoke(action);
			}
		}

		public static T InvokeIfNecessary<T>(Func<T> action)
		{
			if (dispatcher == null || Thread.CurrentThread == dispatcher.Thread)
				return action();
			else
			{
				T ret = default(T);
				dispatcher.Invoke(new Action(() =>
				{
					ret = action();
				}));
				return ret;
			}
		}

		public List<sw.Window> DelayShownWindows
		{
			get
			{
				if (delayShownWindows == null)
					delayShownWindows = new List<sw.Window>();
				return delayShownWindows;
			}
		}

		public bool IsStarted { get; private set; }

		void ApplyThemes()
		{
			if (!EnableCustomThemes)
				return;

			// ensure the Xceed.Wpf.Toolkit assembly is loaded here.
			// kind of pointless, but adding the resource dictionary below fails unless this assembly is loaded..
			var xceedWpfToolkit = typeof(Xceed.Wpf.Toolkit.ButtonSpinner).Assembly;
			if (xceedWpfToolkit == null)
				throw new InvalidOperationException("Could not load Xceed.Wpf.Toolkit");

			// Add themes to our controls
			var assemblyName = typeof(ApplicationHandler).Assembly.GetName().Name;
			Control.Resources.MergedDictionaries.Add(new sw.ResourceDictionary { Source = new Uri($"pack://application:,,,/{assemblyName};component/themes/generic.xaml", UriKind.RelativeOrAbsolute) });
		}

		protected override void Initialize()
		{
			if (SynchronizationContext.Current == null)
				SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

			Control = sw.Application.Current;
			if (Control == null)
			{
				Control = new sw.Application { ShutdownMode = sw.ShutdownMode.OnExplicitShutdown };
				sw.Forms.Application.EnableVisualStyles();
			}

			// Prevent race condition with volatile font collection field in WPF when measuring a window the first time
			// When running on non-english windows it can cause a NullReferenceException in System.Windows.Media.FontFamily.LookupFontFamilyAndFace
			// This is a hack, but no way around it thus far..
			var temp = sw.SystemFonts.MessageFontFamily.Baseline;

			dispatcher = sw.Application.Current.Dispatcher ?? Dispatcher.CurrentDispatcher;
			instance = this;
			Control.Startup += HandleStartup;
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

		void HandleStartup(object sender, sw.StartupEventArgs e)
		{
			IsStarted = true;
			IsActive = Win32.ApplicationIsActivated();
			Control.Activated += (sender2, e2) => IsActive = true;
			Control.Deactivated += (sender2, e2) => IsActive = false;
			if (delayShownWindows != null)
			{
				foreach (var window in delayShownWindows)
				{
					window.Show();
				}
				delayShownWindows = null;
			}
		}

		bool _isActive;
		public bool IsActive
		{
			get => _isActive;
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
			get { return badgeLabel; }
			set
			{
				badgeLabel = value;
				var mainWindow = sw.Application.Current.MainWindow;
				if (mainWindow != null)
				{
					if (mainWindow.TaskbarItemInfo == null)
						mainWindow.TaskbarItemInfo = new sw.Shell.TaskbarItemInfo();
					if (!string.IsNullOrEmpty(badgeLabel))
					{
						// scale by the current pixel size
						var m = sw.PresentationSource.FromVisual(mainWindow).CompositionTarget.TransformToDevice;
						var scale = (float)m.M22;

						var bmp = GenerateBadge(scale, badgeLabel);
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
			bool cancel = false;
			foreach (sw.Window window in Control.Windows)
			{
				window.Close();
				cancel |= window.IsVisible;
			}
			if (!cancel)
			{
				Control.Shutdown();
				shutdown = true;
			}
		}

		public bool QuitIsSupported { get { return true; } }

		public void Invoke(Action action)
		{
			ApplicationHandler.InvokeIfNecessary(action);
		}

		public void AsyncInvoke(Action action)
		{
			Control.Dispatcher.BeginInvoke(action, sw.Threading.DispatcherPriority.Normal);
		}

		public Keys CommonModifier
		{
			get { return Keys.Control; }
		}

		public Keys AlternateModifier
		{
			get { return Keys.Alt; }
		}

		public void Open(string url)
		{
			Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
		}

		public void Run()
		{
			Callback.OnInitialized(Widget, EventArgs.Empty);
			if (!attached)
			{
				if (shutdown)
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
			attached = true;
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
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
