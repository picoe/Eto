using System;
using System.Collections.Generic;
using Eto.Forms;
using System.Diagnostics;
using sw = System.Windows;
using swm = System.Windows.Media;
using System.Threading;

namespace Eto.Wpf.Forms
{
	public class ApplicationHandler : WidgetHandler<System.Windows.Application, Application, Application.ICallback>, Application.IHandler
	{
		bool attached;
		bool shutdown;
		string badgeLabel;
		static ApplicationHandler instance;
		List<sw.Window> delayShownWindows;

		public static ApplicationHandler Instance
		{
			get { return instance; }
		}

		public static bool EnableVisualStyles = true;

		public static void InvokeIfNecessary(Action action)
		{
			if (sw.Application.Current == null || Thread.CurrentThread == sw.Application.Current.Dispatcher.Thread)
				action();
			else
			{
				sw.Application.Current.Dispatcher.Invoke(action);
			}
		}

		public static T InvokeIfNecessary<T>(Func<T> action)
		{
			if (sw.Application.Current == null || Thread.CurrentThread == sw.Application.Current.Dispatcher.Thread)
				return action();
			else
			{
				T ret = default(T);
				sw.Application.Current.Dispatcher.Invoke(new Action(() =>
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

		protected override void Initialize()
		{
			base.Initialize();
			Control = sw.Application.Current;
			if (Control == null)
			{
				Control = new sw.Application { ShutdownMode = sw.ShutdownMode.OnExplicitShutdown };
				System.Windows.Forms.Application.EnableVisualStyles();
			}
			instance = this;
			Control.Startup += HandleStartup;
		}

		void HandleStartup(object sender, sw.StartupEventArgs e)
		{
			IsActive = true;
			IsStarted = true;
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

		public bool IsActive { get; private set; }

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
						var ctl = new CustomControls.OverlayIcon();
						ctl.Content = badgeLabel;
						ctl.Measure(new sw.Size(16, 16));
						var size = ctl.DesiredSize;

						var m = sw.PresentationSource.FromVisual(mainWindow).CompositionTarget.TransformToDevice;

						var bmp = new swm.Imaging.RenderTargetBitmap((int)size.Width, (int)size.Height, m.M22 * 96, m.M22 * 96, swm.PixelFormats.Default);
						ctl.Arrange(new sw.Rect(size));
						bmp.Render(ctl);
						mainWindow.TaskbarItemInfo.Overlay = bmp;
					}
					else
						mainWindow.TaskbarItemInfo.Overlay = null;
				}
			}
		}


		public void RunIteration()
		{
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
			Control.Dispatcher.Invoke(action, sw.Threading.DispatcherPriority.Background);
		}

		public void AsyncInvoke(Action action)
		{
			Control.Dispatcher.BeginInvoke(action);
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
			Process.Start(url);
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
					Control.Run((System.Windows.Window)Widget.MainForm.ControlObject);
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
		}

		public void Restart()
		{
			Process.Start(System.Windows.Application.ResourceAssembly.Location);
			System.Windows.Application.Current.Shutdown();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Application.TerminatingEvent:
					// handled by WpfWindow
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
