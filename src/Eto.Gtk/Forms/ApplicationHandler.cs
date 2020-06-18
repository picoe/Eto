using System;
using Eto.Forms;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using Eto.GtkSharp.Drawing;
using Eto.Drawing;
using System.Collections.Generic;
using Eto.GtkSharp.Forms;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Eto.GtkSharp.Forms
{
#if GTK3
	public class ApplicationHandler : WidgetHandler<Gtk.Application, Eto.Forms.Application, Eto.Forms.Application.ICallback>, Eto.Forms.Application.IHandler
#else
	public class ApplicationHandler : WidgetHandler<object, Application, Application.ICallback>, Application.IHandler
#endif
	{
		internal static List<string> TempFiles = new List<string>();

		bool attached;
		Gtk.StatusIcon statusIcon;
		readonly List<ManualResetEvent> invokeResetEvents = new List<ManualResetEvent>();

		public static ApplicationHandler Instance => Eto.Forms.Application.Instance?.Handler as ApplicationHandler;

		protected override void Initialize()
		{
			base.Initialize();
			Helper.Init();

			if (SynchronizationContext.Current == null)
				SynchronizationContext.SetSynchronizationContext(new GtkSynchronizationContext());

#if GTK3
			Control = new Gtk.Application(null, GLib.ApplicationFlags.None);
			Control.Register(GLib.Cancellable.Current);
			Helper.UseHeaderBar = true;
#else
			Helper.UseHeaderBar = false;
#endif
		}

		void OnUnhandledException(GLib.UnhandledExceptionArgs e)
		{
			var unhandledExceptionArgs = new UnhandledExceptionEventArgs(e.ExceptionObject, e.IsTerminating);
			Callback.OnUnhandledException(Widget, unhandledExceptionArgs);
		}

		public static int MainThreadID { get; set; }

		public void RunIteration()
		{
			Gtk.Application.RunIteration();
		}

		public void Restart()
		{
			GLib.ExceptionManager.UnhandledException -= OnUnhandledException;
			Gtk.Application.Quit();

			RestartInternal();
		}

		string badgeLabel;

		public string BadgeLabel
		{
			get { return badgeLabel; }
			set
			{
				badgeLabel = value;
				if (!string.IsNullOrEmpty(badgeLabel))
				{
					if (statusIcon == null)
					{
						statusIcon = new Gtk.StatusIcon();
						statusIcon.Activate += delegate
						{
							var form = Eto.Forms.Application.Instance.MainForm;
							if (form != null)
								form.BringToFront();
						};
					}
					var bmp = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 32, 32);
					using (var graphics = new Graphics(new Bitmap(new BitmapHandler(bmp))))
					{
						graphics.Clear();
						DrawBadgeLabel(graphics, new Size(bmp.Width, bmp.Height), badgeLabel);
					}
					statusIcon.Pixbuf = bmp;
					statusIcon.Visible = true;
				}
				else if (statusIcon != null)
					statusIcon.Visible = false;
			}
		}

		protected virtual void DrawBadgeLabel(Graphics graphics, Size size, string badgeLabel)
		{
			var rect = new Rectangle(size);
			rect.Inflate(-2, -2);
			graphics.FillEllipse(Brushes.Red, rect);
			graphics.DrawEllipse(new Pen(Colors.White, 2), rect);
			var font = new Font(SystemFont.Bold, 10);
			var labelSize = graphics.MeasureString(font, badgeLabel);
			var labelPosition = ((PointF)(rect.Size - labelSize) / 2) + rect.Location;
			graphics.DrawText(font, Colors.White, labelPosition, badgeLabel);
			graphics.Flush();
		}

		public void Invoke(System.Action action)
		{
			if (Thread.CurrentThread.ManagedThreadId == ApplicationHandler.MainThreadID)
				action();
			else
			{
				var resetEvent = new ManualResetEvent(false);
				lock (invokeResetEvents)
					invokeResetEvents.Add(resetEvent);

				Gtk.Application.Invoke(delegate
				{
					try
					{
						action();
					}
					finally
					{
						resetEvent.Set();
					}
				});
				resetEvent.WaitOne();
				lock (invokeResetEvents)
					invokeResetEvents.Remove(resetEvent);
			}
		}

		public void AsyncInvoke(System.Action action)
		{
			Gtk.Application.Invoke(delegate
			{
				action();
			});
		}

		public void Attach(object context)
		{
			attached = true;
			Control = context as Gtk.Application;
		}

		public void OnMainFormChanged()
		{
		}

		public bool IsActive
		{
			get
			{
				if (_isActive != null)
					return _isActive.Value;

				var windows = Gtk.Window.ListToplevels();
				return windows.Any(r => r.HasFocus | r.HasToplevelFocus) | AnyIsActiveWindow(windows);
			}
		}


		protected virtual void OnIsActiveChanged(EventArgs e) => _IsActiveChanged?.Invoke(this, e);

		UITimer _timer;
		bool? _isActive;
		bool _didGetFocus;
		internal void TriggerIsActiveChanged(bool gotFocus)
		{
			// don't do anything until we really need to know.
			if (_IsActiveChanged == null)
				return;

			_didGetFocus |= gotFocus;

			if (_timer == null)
			{
				// use a timer as the active window does not get updated immediately or when showing the task switcher
				// not sure if there's a platform-specific way to do this that would work better...
				_timer = new UITimer();
				_timer.Elapsed += (sender, e) =>
				{
					var windows = Gtk.Window.ListToplevels();
					// first, check if any window has focus.. then yes, we are the active application.
					bool isActive = _didGetFocus | windows.Any(r => r.HasFocus | r.HasToplevelFocus);
					if (!isActive)
					{
						// no windows have focus, so check the active window
						isActive = AnyIsActiveWindow(windows);

						// we are "active" but not really as there's no top level window with focus.
						// so keep checking until one of the top levels actually has focus or the active window changes.
						if (isActive || _didGetFocus)
							_timer.Interval = 0.5;
						else
							_timer.Stop();

					}
					else
						_timer.Stop();

					_didGetFocus = false;

					if (_isActive != isActive)
					{
						_isActive = isActive;
						OnIsActiveChanged(EventArgs.Empty);
					}
				};
			}
			_timer.Interval = 0.2;
			_timer.Start();
		}

		private static bool AnyIsActiveWindow(Gtk.Window[] windows)
		{
			var activeWindow = Gdk.Screen.Default.ActiveWindow;
			if (activeWindow != null)
			{
				var activeWindowHandle = activeWindow.Handle;
				for (int i = 0; i < windows.Length; i++)
				{
					Gtk.Window window = windows[i];
					var gdkwindow = window.GetWindow();
					if (gdkwindow != null && gdkwindow.Handle == activeWindowHandle)
					{
						return true;
					}
				}
				activeWindow.Dispose();
			}

			return false;
		}

		public void RegisterIsActiveChanged(Gtk.Window window)
		{
			new WindowActiveHelper(window);
		}

		EventHandler _IsActiveChanged;
		public event EventHandler IsActiveChanged
		{
			add
			{
				if (_IsActiveChanged == null)
				{
					/*
					foreach (var window in Application.Instance.Windows)
					{
						new WindowActiveHelper(window.ToGtk());
					}*/

					/* Does not work... Windows array is always empty.
					Control.WindowAdded += (o, args) => new WindowActiveHelper(args.Window);
					for (int i = 0; i < Control.Windows.Length; i++)
					{
						var window = Control.Windows[i];
						new WindowActiveHelper(window);
					}*/
				}
				_IsActiveChanged += value;

			}
			remove
			{
				_IsActiveChanged -= value;
			}
		}

		class WindowActiveHelper
		{
			Gtk.Window _window;

			public WindowActiveHelper(Gtk.Window window)
			{
				if (window == null)
					return;
				_window = window;
				_window.AddEvents((int)Gdk.EventMask.FocusChangeMask);
				_window.FocusInEvent += _window_FocusInEvent;
				_window.FocusOutEvent += _window_FocusOutEvent;
			}

			private void _window_FocusOutEvent(object o, Gtk.FocusOutEventArgs args)
			{
				Instance.TriggerIsActiveChanged(false);
			}

			private void _window_FocusInEvent(object o, Gtk.FocusInEventArgs args)
			{
				Instance.TriggerIsActiveChanged(true);
			}

		}

		public void Run()
		{
			//if (!Platform.IsWindows) Gdk.Threads.Init(); // do this in windows, and it stalls!  ugh
			MainThreadID = Thread.CurrentThread.ManagedThreadId;

			if (EtoEnvironment.Platform.IsLinux)
				LinuxNotificationHandler.Init();
			Callback.OnInitialized(Widget, EventArgs.Empty);
			if (!attached)
			{
				Gtk.Application.Run();
				lock (invokeResetEvents)
				{
					// set all invokes so threads will continue when run loop exits
					foreach (var reset in invokeResetEvents)
						reset.Set();
					invokeResetEvents.Clear();
				}

				Gdk.Threads.Leave();
			}

			if (EtoEnvironment.Platform.IsLinux)
				LinuxNotificationHandler.DeInit();
			foreach (var file in TempFiles)
				File.Delete(file);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Application.TerminatingEvent:
					// called automatically
					break;
				case Eto.Forms.Application.UnhandledExceptionEvent:
					GLib.ExceptionManager.UnhandledException += OnUnhandledException;
					break;
				case Eto.Forms.Application.NotificationActivatedEvent:
					// handled by NotificationHandler
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void Quit()
		{
			var args = new CancelEventArgs();
			var mainForm = Widget.MainForm != null ? Widget.MainForm.Handler as IGtkWindow : null;
			if (mainForm != null)
				args.Cancel = !mainForm.CloseWindow(ce => Callback.OnTerminating(Widget, ce));
			else
				Callback.OnTerminating(Widget, args);

			if (!args.Cancel)
			{
				GLib.ExceptionManager.UnhandledException -= OnUnhandledException;
				Gtk.Application.Quit();
			}
		}

		public bool QuitIsSupported { get { return true; } }

		public void Open(string url)
		{
			try
			{
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
			}
			catch
			{
				// Use fallback for recent mono versions that do not support UseShellExecute.
				url = Uri.EscapeUriString(url);
				Process.Start("xdg-open", url);
			}
		}

		public Keys CommonModifier { get { return Keys.Control; } }

		public Keys AlternateModifier { get { return Keys.Alt; } }

		private void RestartInternal()
		{
			var cmdLine = GetCommandLineArgs();
			var entry = Assembly.GetEntryAssembly().Location;
			if (entry.EndsWith(".exe", StringComparison.InvariantCulture))
			{
				// mono or windows, use Process.Start()
				Process.Start(entry, cmdLine);
			}
			else if (entry.EndsWith(".dll", StringComparison.InvariantCulture))
			{
				// .net core, look for self-contained deployment
				var exeExtension = Environment.OSVersion.Platform == PlatformID.Win32NT ? ".exe" : null;
				var loader = Path.ChangeExtension(entry, exeExtension);

				if (File.Exists(loader))
				{
					// self contained deployment
					Process.Start(loader, cmdLine);
				}
				else
				{
					// use dotnet to run entry dll
					Process.Start("dotnet", $"{entry} {cmdLine}");
				}
			}
			else
			{
				// don't know how to handle this
				throw new NotImplementedException("Entry assembly has unknown extension " + entry);
			}
		}

		private string GetCommandLineArgs()
		{
			var cmdLine = string.Empty;
			var oldArgs = Environment.GetCommandLineArgs();
			if (oldArgs.Length > 1)
			{
				var args = new String[oldArgs.Length - 1];
				Array.Copy(oldArgs, 1, args, 0, args.Length);
				cmdLine = String.Join(" ", args);
			}

			return cmdLine;
		}
	}
}
