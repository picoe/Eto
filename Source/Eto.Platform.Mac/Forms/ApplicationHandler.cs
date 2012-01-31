using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Actions;
using System.ComponentModel;

namespace Eto.Platform.Mac
{
	public class ApplicationHandler : WidgetHandler<NSApplication, Application>, IApplication
	{
		public NSApplicationDelegate AppDelegate { get; private set; }
		
		public static ApplicationHandler Instance {
			get { return Application.Instance.Handler as ApplicationHandler; }
		}
		
		public ApplicationHandler ()
		{
			NSApplication.Init ();
			Control = NSApplication.SharedApplication;
		}
		
		static void restart_WillTerminate (object sender, EventArgs e)
		{
			// re-open after we terminate
			var args = new string[] {
				"-c",
				"open \"$1\"", 
				string.Empty,
				NSBundle.MainBundle.BundlePath
			};
			NSTask.LaunchFromPath ("/bin/sh", args);
		}

		public void InvokeOnMainThread (System.Action action)
		{
			var thread = NSThread.Current;
			if (thread != null && thread.IsMainThread)
				action ();
			else
				Control.InvokeOnMainThread (delegate {
					action ();
				});
		}
		
		public void Restart ()
		{
			NSApplication.SharedApplication.WillTerminate += restart_WillTerminate;
			NSApplication.SharedApplication.Terminate (AppDelegate);

			// only get here if cancelled, remove event to restart
			NSApplication.SharedApplication.WillTerminate -= restart_WillTerminate;
		}

		public void RunIteration ()
		{
			NSApplication.SharedApplication.NextEvent (NSEventMask.AnyEvent, NSDate.DistantFuture, NSRunLoop.NSDefaultRunLoopMode, true);
		}
		
		public void Run (string[] args)
		{
			NSApplication.Main (args);
		}
		
		public void Initialize (NSApplicationDelegate appdelegate)
		{
			this.AppDelegate = appdelegate;
			Widget.OnInitialized (EventArgs.Empty);	
		}

		public void Quit ()
		{
			NSApplication.SharedApplication.Terminate (AppDelegate);
		}
		
		public void Open (string url)
		{
			NSWorkspace.SharedWorkspace.OpenUrl (new NSUrl (url));
		}
		
		public void GetSystemActions (GenerateActionArgs args)
		{
			args.Actions.AddButton ("mac_hide", string.Format ("Hide {0}|Hide {0}|Hides the main {0} window", Widget.Name), delegate {
				NSApplication.SharedApplication.Hide (NSApplication.SharedApplication);
			}, Key.H | Key.Application);
			args.Actions.AddButton ("mac_hideothers", "Hide Others|Hide Others|Hides all other application windows", delegate {
				NSApplication.SharedApplication.HideOtherApplications (NSApplication.SharedApplication);
			}, Key.H | Key.Application | Key.Alt);
			args.Actions.AddButton ("mac_showall", "Show All|Show All|Show All Windows", delegate {
				NSApplication.SharedApplication.UnhideAllApplications (NSApplication.SharedApplication);
			});
			
			args.Actions.Add (new MacButtonAction ("mac_performMiniaturize", "Minimize", "performMiniaturize:"){ Accelerator = Key.Application | Key.M });
			args.Actions.Add (new MacButtonAction ("mac_performZoom", "Zoom", "performZoom:"));
			
		}
		
		public Key CommonModifier {
			get {
				return Key.Application;
			}
		}

		public Key AlternateModifier {
			get {
				return Key.Alt;
			}
		}


	}
}
