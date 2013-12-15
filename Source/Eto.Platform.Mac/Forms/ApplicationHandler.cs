using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Actions;
using MonoMac.ObjCRuntime;
using System.Collections.Generic;

namespace Eto.Platform.Mac.Forms
{
	public class ApplicationHandler : WidgetHandler<NSApplication, Application>, IApplication
	{
		bool attached;

		internal static bool QueueResizing { get; set; }

		public NSApplicationDelegate AppDelegate { get; set; }

		public bool AddFullScreenMenuItem { get; set; }

		public bool AddPrintingMenuItems { get; set; }

		public static ApplicationHandler Instance
		{
			get { return Application.Instance == null ? null : Application.Instance.Handler as ApplicationHandler; }
		}

		public string BadgeLabel
		{
			get
			{
				var badgeLabel = Control.DockTile.BadgeLabel;
				return string.IsNullOrEmpty(badgeLabel) ? null : badgeLabel;
			}
			set
			{
				Control.DockTile.BadgeLabel = value ?? string.Empty;
			}
		}

		public ApplicationHandler()
		{
			Control = NSApplication.SharedApplication;
		}

		static void restart_WillTerminate(object sender, EventArgs e)
		{
			// re-open after we terminate
			var args = new string[]
			{
				"-c",
				"open \"$1\"", 
				string.Empty,
				NSBundle.MainBundle.BundlePath
			};
			NSTask.LaunchFromPath("/bin/sh", args);
		}

		public void Invoke(Action action)
		{
			var thread = NSThread.Current;
			if (thread != null && thread.IsMainThread)
				action();
			else
			{
				Control.InvokeOnMainThread(delegate
				{
					action();
				});
			}
		}

		public void AsyncInvoke(Action action)
		{
			Control.BeginInvokeOnMainThread(delegate
			{
				action();
			});
		}

		public void Restart()
		{
			NSApplication.SharedApplication.WillTerminate += restart_WillTerminate;
			NSApplication.SharedApplication.Terminate(AppDelegate);

			// only get here if cancelled, remove event to restart
			NSApplication.SharedApplication.WillTerminate -= restart_WillTerminate;
		}

		public void RunIteration()
		{
			NSApplication.SharedApplication.NextEvent(NSEventMask.AnyEvent, NSDate.DistantFuture, NSRunLoop.NSDefaultRunLoopMode, true);
		}

		public void Attach(object context)
		{
			attached = true;
		}

		public void OnMainFormChanged()
		{
		}

		public void Run(string[] args)
		{
			if (!attached)
			{
				EtoBundle.Init();

				if (Control.Delegate == null)
					Control.Delegate = AppDelegate ?? new AppDelegate();
				NSApplication.Main(args);
			}
			else
				Initialize(Control.Delegate);
		}

		public void Initialize(NSApplicationDelegate appdelegate)
		{
			AppDelegate = appdelegate;
			Widget.OnInitialized(EventArgs.Empty);	
		}

		public void Quit()
		{
			NSApplication.SharedApplication.Terminate(AppDelegate);
		}

		public void Open(string url)
		{
			NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl(url));
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Application.TerminatingEvent:
				// handled by app delegate
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void EnableFullScreen()
		{
			if (Control.RespondsToSelector(new Selector("setPresentationOptions:")))
			{
				AddFullScreenMenuItem = true;
			}
		}

		public void GetSystemActions(List<CommandBase> actions, ISubMenuWidget menu, ToolBar toolBar, bool addStandardItems)
		{
			actions.Add(new CommandBase("mac_hide", string.Format("Hide {0}|Hide {0}|Hides the main {0} window", Widget.Name), delegate
			{
				NSApplication.SharedApplication.Hide(NSApplication.SharedApplication);
			}, Keys.H | Keys.Application));

			actions.Add(new CommandBase("mac_hideothers", "Hide Others|Hide Others|Hides all other application windows", delegate
			{
				NSApplication.SharedApplication.HideOtherApplications(NSApplication.SharedApplication);
			}, Keys.H | Keys.Application | Keys.Alt));

			actions.Add(new CommandBase("mac_showall", "Show All|Show All|Show All Windows", delegate
			{
				NSApplication.SharedApplication.UnhideAllApplications(NSApplication.SharedApplication);
			}));
			
			actions.Add(new MacButtonAction("mac_performMiniaturize", "Minimize", "performMiniaturize:") { Shortcut = Keys.Application | Keys.M });
			actions.Add(new MacButtonAction("mac_performZoom", "Zoom", "performZoom:"));
			actions.Add(new MacButtonAction("mac_performClose", "Close", "performClose:") { Shortcut = Keys.Application | Keys.W });
			actions.Add(new MacButtonAction("mac_arrangeInFront", "Bring All To Front", "arrangeInFront:"));
			actions.Add(new MacButtonAction("mac_cut", "Cut", "cut:") { Shortcut = Keys.Application | Keys.X });
			actions.Add(new MacButtonAction("mac_copy", "Copy", "copy:") { Shortcut = Keys.Application | Keys.C });
			actions.Add(new MacButtonAction("mac_paste", "Paste", "paste:") { Shortcut = Keys.Application | Keys.V });
			actions.Add(new MacButtonAction("mac_pasteAsPlainText", "Paste and Match Style", "pasteAsPlainText:") { Shortcut = Keys.Application | Keys.Alt | Keys.Shift | Keys.V });
			actions.Add(new MacButtonAction("mac_delete", "Delete", "delete:"));
			actions.Add(new MacButtonAction("mac_selectAll", "Select All", "selectAll:") { Shortcut = Keys.Application | Keys.A });
			actions.Add(new MacButtonAction("mac_undo", "Undo", "undo:") { Shortcut = Keys.Application | Keys.Z });
			actions.Add(new MacButtonAction("mac_redo", "Redo", "redo:") { Shortcut = Keys.Application | Keys.Shift | Keys.Z });
			actions.Add(new MacButtonAction("mac_toggleFullScreen", "Enter Full Screen", "toggleFullScreen:") { Shortcut = Keys.Application | Keys.Control | Keys.F });
			actions.Add(new MacButtonAction("mac_runPageLayout", "Page Setup...", "runPageLayout:") { Shortcut = Keys.Application | Keys.Shift | Keys.P });
			actions.Add(new MacButtonAction("mac_print", "Print...", "print:") { Shortcut = Keys.Application | Keys.P });

			if (addStandardItems)
			{
				var application = menu.GetSubmenu(Widget.Name ?? "Application", 100);
				application.AddSeparator(800);
				application.Add(actions, "mac_hide", 800);
				application.Add(actions, "mac_hideothers", 800);
				application.Add(actions, "mac_showall", 800);
				application.AddSeparator(801);

				var file = menu.GetSubmenu("&File", 100);
				file.AddSeparator(900);
				file.Add(actions, "mac_performClose", 900);

				if (AddPrintingMenuItems)
				{
					file.AddSeparator(1000);
					file.Add(actions, "mac_runPageLayout", 1000);
					file.Add(actions, "mac_print", 1000);
				}

				var edit = menu.GetSubmenu("&Edit", 200);
				edit.AddSeparator(100);
				edit.Add(actions, "mac_undo", 100);
				edit.Add(actions, "mac_redo", 100);
				edit.AddSeparator(101);
				
				edit.AddSeparator(200);
				edit.Add(actions, "mac_cut", 200);
				edit.Add(actions, "mac_copy", 200);
				edit.Add(actions, "mac_paste", 200);
				edit.Add(actions, "mac_delete", 200);
				edit.Add(actions, "mac_selectAll", 200);
				edit.AddSeparator(201);
				
				var window = menu.GetSubmenu("&Window", 900);
				window.AddSeparator(100);
				window.Add(actions, "mac_performMiniaturize", 100);
				window.Add(actions, "mac_performZoom", 100);
				window.AddSeparator(101);

				window.AddSeparator(200);
				window.Add(actions, "mac_arrangeInFront", 200);
				window.AddSeparator(201);

				if (AddFullScreenMenuItem)
				{
					var view = menu.GetSubmenu("&View", 300);
					view.AddSeparator(900);
					view.Add(actions, "mac_toggleFullScreen", 900);
					view.AddSeparator(901);
				}
				
				var help = menu.GetSubmenu("&Help", 900);

				// add separator so help menu is always shown even when empty
				help.AddSeparator(0);
			}
		}

		public Keys CommonModifier
		{
			get
			{
				return Keys.Application;
			}
		}

		public Keys AlternateModifier
		{
			get
			{
				return Keys.Alt;
			}
		}
	}
}
