using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Actions;
using System.ComponentModel;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms
{
	public class ApplicationHandler : WidgetHandler<NSApplication, Application>, IApplication
	{
		public NSApplicationDelegate AppDelegate { get; set; }
		
		public bool AddFullScreenMenuItem { get; set; }
		
		public static ApplicationHandler Instance {
			get { return Application.Instance.Handler as ApplicationHandler; }
		}
		
		public ApplicationHandler ()
		{
			NSApplication.Init ();
			EtoBundle.Init();
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

		public void Invoke (System.Action action)
		{
			var thread = NSThread.Current;
			if (thread != null && thread.IsMainThread)
				action ();
			else {
				Control.InvokeOnMainThread (delegate {
					action ();
				});
			}
		}

		public void AsyncInvoke (System.Action action)
		{
			var thread = NSThread.Current;
			if (thread != null && thread.IsMainThread)
				action ();
			else {
				Control.BeginInvokeOnMainThread (delegate {
					action ();
				});
			}
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
			if (Control.Delegate == null)
				Control.Delegate = this.AppDelegate ?? new AppDelegate ();
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
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Application.TerminatingEvent:
				// handled by app delegate
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public void EnableFullScreen(NSApplicationPresentationOptions options = NSApplicationPresentationOptions.FullScreen | NSApplicationPresentationOptions.AutoHideToolbar | NSApplicationPresentationOptions.AutoHideMenuBar | NSApplicationPresentationOptions.AutoHideDock)
		{
			if (Control.RespondsToSelector (new Selector ("setPresentationOptions:"))) {
				if (options.HasFlag (NSApplicationPresentationOptions.FullScreen))
					AddFullScreenMenuItem = true;
				Control.PresentationOptions = options;
			}
		}
		
		public void GetSystemActions (GenerateActionArgs args, bool addStandardItems)
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
			args.Actions.Add (new MacButtonAction ("mac_performClose", "Close", "performClose:") { Accelerator = Key.Application | Key.W });
			args.Actions.Add (new MacButtonAction ("mac_arrangeInFront", "Bring All To Front", "arrangeInFront:"));
			args.Actions.Add (new MacButtonAction ("mac_cut", "Cut", "cut:") { Accelerator = Key.Application | Key.X });
			args.Actions.Add (new MacButtonAction ("mac_copy", "Copy", "copy:") { Accelerator = Key.Application | Key.C });
			args.Actions.Add (new MacButtonAction ("mac_paste", "Paste", "paste:") { Accelerator = Key.Application | Key.V });
			args.Actions.Add (new MacButtonAction ("mac_pasteAsPlainText", "Paste and Match Style", "pasteAsPlainText:") { Accelerator = Key.Application | Key.Alt | Key.Shift | Key.V });
			args.Actions.Add (new MacButtonAction ("mac_delete", "Delete", "delete:"));
			args.Actions.Add (new MacButtonAction ("mac_selectAll", "Select All", "selectAll:") { Accelerator = Key.Application | Key.A });
			args.Actions.Add (new MacButtonAction ("mac_undo", "Undo", "undo:") { Accelerator = Key.Application | Key.Z });
			args.Actions.Add (new MacButtonAction ("mac_redo", "Redo", "redo:") { Accelerator = Key.Application | Key.Shift | Key.Z });
			args.Actions.Add (new MacButtonAction ("mac_toggleFullScreen", "Enter Full Screen", "toggleFullScreen:") { Accelerator = Key.Application | Key.Control | Key.F });
			
			if (addStandardItems) {
				var application = args.Menu.FindAddSubMenu (Widget.Name ?? "Application", 100);
				application.Actions.AddSeparator (800);
				application.Actions.Add ("mac_hide", 800);
				application.Actions.Add ("mac_hideothers", 800);
				application.Actions.Add ("mac_showall", 800);
				application.Actions.AddSeparator (801);

				var file = args.Menu.FindAddSubMenu ("&File", 100);
				file.Actions.AddSeparator (900);
				file.Actions.Add ("mac_performClose", 900);
				
				var edit = args.Menu.FindAddSubMenu ("&Edit", 200);
				edit.Actions.AddSeparator (100);
				edit.Actions.Add ("mac_undo", 100);
				edit.Actions.Add ("mac_redo", 100);
				edit.Actions.AddSeparator (101);
				
				edit.Actions.AddSeparator (200);
				edit.Actions.Add ("mac_cut", 200);
				edit.Actions.Add ("mac_copy", 200);
				edit.Actions.Add ("mac_paste", 200);
				edit.Actions.Add ("mac_delete", 200);
				edit.Actions.Add ("mac_selectAll", 200);
				edit.Actions.AddSeparator (201);
				
				var window = args.Menu.FindAddSubMenu ("&Window", 900);
				window.Actions.AddSeparator (100);
				window.Actions.Add ("mac_performMiniaturize", 100);
				window.Actions.Add ("mac_performZoom", 100);
				window.Actions.AddSeparator (101);

				window.Actions.AddSeparator (200);
				window.Actions.Add ("mac_arrangeInFront", 200);
				window.Actions.AddSeparator (201);

				if (AddFullScreenMenuItem) {
					var view = args.Menu.FindAddSubMenu ("&View", 300);
					view.Actions.AddSeparator (900);
					view.Actions.Add ("mac_toggleFullScreen", 900);
					view.Actions.AddSeparator (901);
				}
				
				var help = args.Menu.FindAddSubMenu ("&Help", 900);

				// add separator so help menu is always shown even when empty
				help.Actions.AddSeparator (0);
			}
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
