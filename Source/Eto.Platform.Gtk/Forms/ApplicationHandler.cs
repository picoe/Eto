using System;
using Eto.Forms;
using Eto.Misc;
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
		
		public void Run (string[] args)
		{
			//if (!Platform.IsWindows) Gdk.Threads.Init(); // do this in windows, and it stalls!  ugh
			MainThreadID = Thread.CurrentThread.ManagedThreadId;
			
			Widget.OnInitialized (EventArgs.Empty);
			if (Widget.MainForm != null) {
				((Gtk.Widget)Widget.MainForm.ControlObject).Destroyed += ApplicationHandler_Destroyed;
			}
			Gtk.Application.Run ();
			Gdk.Threads.Leave ();
		}

		public void Quit ()
		{
			if (CanQuit ())
				Gtk.Application.Quit ();
		}
		
		public void Open (string url)
		{
			var info = new ProcessStartInfo (url);
			Process.Start (info);
		}
		
		public void GetSystemActions (GenerateActionArgs args)
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

		void ApplicationHandler_Destroyed (object sender, EventArgs e)
		{
			if (CanQuit ())
				Gtk.Application.Quit ();
		}
	}
}
