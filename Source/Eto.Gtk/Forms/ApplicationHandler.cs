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

namespace Eto.GtkSharp.Forms
{
#if GTK3
	public class ApplicationHandler : WidgetHandler<Gtk.Application, Application, Application.ICallback>, Application.IHandler
#else
	public class ApplicationHandler : WidgetHandler<object, Application, Application.ICallback>, Application.IHandler
#endif
	{
		internal static List<string> TempFiles = new List<string>(); 

		bool attached;
		Gtk.StatusIcon statusIcon;
		readonly List<ManualResetEvent> invokeResetEvents = new List<ManualResetEvent>();

		protected override void Initialize()
		{
			base.Initialize();
			Helper.Init();

			if (SynchronizationContext.Current == null)
				SynchronizationContext.SetSynchronizationContext(new GtkSynchronizationContext());

#if GTK3
			Control = new Gtk.Application(null, GLib.ApplicationFlags.None);
			Control.Register(GLib.Cancellable.Current);
			Helper.UseHeaderBar = Gtk.Global.MinorVersion >= 10 && NativeMethods.gtk_application_prefers_app_menu(Control.Handle);
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

			// TODO: restart!
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
							var form = Application.Instance.MainForm;
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

		public void Invoke(Action action)
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

		public void AsyncInvoke(Action action)
		{
			Gtk.Application.Invoke(delegate
			{
				action();
			});
		}

		public void Attach(object context)
		{
			attached = true;
		}

		public void OnMainFormChanged()
		{
		}

		public void Run()
		{
			//if (!Platform.IsWindows) Gdk.Threads.Init(); // do this in windows, and it stalls!  ugh
			MainThreadID = Thread.CurrentThread.ManagedThreadId;

			if (EtoEnvironment.Platform.IsLinux)
				LinuxNotificationHandler.notify_init(Assembly.GetExecutingAssembly().FullName);
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
				LinuxNotificationHandler.notify_uninit();
			foreach (var file in TempFiles)
				File.Delete(file);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Application.TerminatingEvent:
					// called automatically
					break;
				case Application.UnhandledExceptionEvent:
					GLib.ExceptionManager.UnhandledException += OnUnhandledException;
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
			var info = new ProcessStartInfo(url);
			Process.Start(info);
		}

		public Keys CommonModifier { get { return Keys.Control; } }

		public Keys AlternateModifier { get { return Keys.Alt; } }
	}
}
