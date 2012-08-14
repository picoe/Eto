using System;
using Eto.Forms;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace Eto.Platform.GtkSharp
{
	public class ApplicationHandler : WidgetHandler<object, Application>, IApplication
	{
		public static int MainThreadID { get; set; }
		
		public void RunIteration ()
		{
			Gtk.Application.RunIteration ();
		}

		public void Restart ()
		{
			Gtk.Application.Quit ();

			// TODO: restart!
		}

		public void Invoke (System.Action action)
		{
			if (Thread.CurrentThread.ManagedThreadId == ApplicationHandler.MainThreadID)
				action ();
			else {
				var resetEvent = new ManualResetEvent (false);

				Gtk.Application.Invoke (delegate {
					action ();
					resetEvent.Set ();
				});

				resetEvent.WaitOne ();
			}
		}

		public void AsyncInvoke (System.Action action)
		{
			if (Thread.CurrentThread.ManagedThreadId == ApplicationHandler.MainThreadID)
				action ();
			else {
				Gtk.Application.Invoke (delegate {
					action ();
				});
			}
		}
		
		public void Run (string[] args)
		{
			//if (!Platform.IsWindows) Gdk.Threads.Init(); // do this in windows, and it stalls!  ugh
			MainThreadID = Thread.CurrentThread.ManagedThreadId;
			
			Widget.OnInitialized (EventArgs.Empty);
			if (Widget.MainForm != null) {
				((Gtk.Widget)Widget.MainForm.ControlObject).DeleteEvent += HandleDeleteEvent;
			}
			Gtk.Application.Run ();
			Gdk.Threads.Leave ();
		}

		void HandleDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			if (CanQuit () && !object.Equals (args.RetVal, true)) {
				Gtk.Application.Quit ();
			} else
				args.RetVal = true; // cancel!
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Application.TerminatingEvent:
				// called automatically
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public void Quit ()
		{
			bool shouldClose = true;
			var mainForm = Widget.MainForm != null ? Widget.MainForm.Handler as IGtkWindow : null;
			if (mainForm != null) {
				shouldClose &= mainForm.CloseWindow ();
			}
			if (shouldClose && CanQuit ()) {
				Gtk.Application.Quit ();
			}
		}
		
		public void Open (string url)
		{
			var info = new ProcessStartInfo (url);
			Process.Start (info);
		}
		
		public void GetSystemActions (GenerateActionArgs args, bool addStandardItems)
		{
		}
		
		public Key CommonModifier {
			get {
				return Key.Control;
			}
		}

		public Key AlternateModifier {
			get {
				return Key.Alt;
			}
		}
		
		bool CanQuit ()
		{
			var args = new CancelEventArgs ();
			Widget.OnTerminating (args);
			return !args.Cancel;
		}
	}
}
