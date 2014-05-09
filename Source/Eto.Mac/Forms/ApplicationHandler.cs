using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Mac.Forms.Actions;
using MonoMac.ObjCRuntime;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Mac.Forms
{
	public class ApplicationHandler : WidgetHandler<NSApplication, Application, Application.ICallback>, Application.IHandler
	{
		bool attached;

		internal static bool QueueResizing { get; set; }

		public NSApplicationDelegate AppDelegate { get; set; }

		public bool AddFullScreenMenuItem { get; set; }

		public bool AddPrintingMenuItems { get; set; }

		public bool AllowClosingMainForm { get; set; }

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

		public bool ShouldCloseForm(Window window, bool wasClosed)
		{
			if (ReferenceEquals(window, Widget.MainForm))
			{
				if (AllowClosingMainForm && wasClosed)
					Widget.MainForm = null;
				return AllowClosingMainForm;
			}

			return true;
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
				Control.InvokeOnMainThread(() => action());
			}
		}

		public void AsyncInvoke(Action action)
		{
			Control.BeginInvokeOnMainThread(() => action());
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
			Callback.OnInitialized(Widget, EventArgs.Empty);
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

		public IEnumerable<Command> GetSystemCommands()
		{
			yield return new Command((sender, e) => NSApplication.SharedApplication.Hide(NSApplication.SharedApplication))
			{ 
				ID = "mac_hide", 
				MenuText = string.Format("Hide {0}", Widget.Name),
				ToolBarText = "Hide",
				ToolTip = string.Format("Hides the main {0} window", Widget.Name), 
				Shortcut = Keys.H | Keys.Application
			};

			yield return new Command((sender, e) => NSApplication.SharedApplication.HideOtherApplications(NSApplication.SharedApplication))
			{
				ID = "mac_hideothers", 
				MenuText = "Hide Others",
				ToolBarText = "Hide Others",
				ToolTip = "Hides all other application windows",
				Shortcut = Keys.H | Keys.Application | Keys.Alt
			};

			yield return new Command((sender, e) => NSApplication.SharedApplication.UnhideAllApplications(NSApplication.SharedApplication))
			{
				ID = "mac_showall",
				MenuText = "Show All",
				ToolBarText = "Show All",
				ToolTip = "Show All Windows"
			};

			yield return new MacCommand("mac_performMiniaturize", "Minimize", "performMiniaturize:") { Shortcut = Keys.Application | Keys.M };
			yield return new MacCommand("mac_performZoom", "Zoom", "performZoom:");
			yield return new MacCommand("mac_performClose", "Close", "performClose:") { Shortcut = Keys.Application | Keys.W };
			yield return new MacCommand("mac_arrangeInFront", "Bring All To Front", "arrangeInFront:");
			yield return new MacCommand("mac_cut", "Cut", "cut:") { Shortcut = Keys.Application | Keys.X };
			yield return new MacCommand("mac_copy", "Copy", "copy:") { Shortcut = Keys.Application | Keys.C };
			yield return new MacCommand("mac_paste", "Paste", "paste:") { Shortcut = Keys.Application | Keys.V };
			yield return new MacCommand("mac_pasteAsPlainText", "Paste and Match Style", "pasteAsPlainText:") { Shortcut = Keys.Application | Keys.Alt | Keys.Shift | Keys.V };
			yield return new MacCommand("mac_delete", "Delete", "delete:");
			yield return new MacCommand("mac_selectAll", "Select All", "selectAll:") { Shortcut = Keys.Application | Keys.A };
			yield return new MacCommand("mac_undo", "Undo", "undo:") { Shortcut = Keys.Application | Keys.Z };
			yield return new MacCommand("mac_redo", "Redo", "redo:") { Shortcut = Keys.Application | Keys.Shift | Keys.Z };
			yield return new MacCommand("mac_toggleFullScreen", "Enter Full Screen", "toggleFullScreen:") { Shortcut = Keys.Application | Keys.Control | Keys.F };
			yield return new MacCommand("mac_runPageLayout", "Page Setup...", "runPageLayout:") { Shortcut = Keys.Application | Keys.Shift | Keys.P };
			yield return new MacCommand("mac_print", "Print...", "print:") { Shortcut = Keys.Application | Keys.P };
		}

		public void CreateStandardMenu(MenuItemCollection menu, IEnumerable<Command> commands)
		{
			var lookup = commands.ToLookup(r => r.ID);
			var application = menu.GetSubmenu(Widget.Name ?? "Application", 100);
			application.Items.AddSeparator(800);
			application.Items.AddRange(lookup["mac_hide"], 800);
			application.Items.AddRange(lookup["mac_hideothers"], 800);
			application.Items.AddRange(lookup["mac_showall"], 800);
			application.Items.AddSeparator(801);

			var file = menu.GetSubmenu("&File", 100);
			file.Items.AddSeparator(900);
			file.Items.AddRange(lookup["mac_performClose"], 900);

			if (AddPrintingMenuItems)
			{
				file.Items.AddSeparator(1000);
				file.Items.AddRange(lookup["mac_runPageLayout"], 1000);
				file.Items.AddRange(lookup["mac_print"], 1000);
			}

			var edit = menu.GetSubmenu("&Edit", 200);
			edit.Items.AddSeparator(100);
			edit.Items.AddRange(lookup["mac_undo"], 100);
			edit.Items.AddRange(lookup["mac_redo"], 100);
			edit.Items.AddSeparator(101);
			
			edit.Items.AddSeparator(200);
			edit.Items.AddRange(lookup["mac_cut"], 200);
			edit.Items.AddRange(lookup["mac_copy"], 200);
			edit.Items.AddRange(lookup["mac_paste"], 200);
			edit.Items.AddRange(lookup["mac_delete"], 200);
			edit.Items.AddRange(lookup["mac_selectAll"], 200);
			edit.Items.AddSeparator(201);
			
			var window = menu.GetSubmenu("&Window", 900);
			window.Items.AddSeparator(100);
			window.Items.AddRange(lookup["mac_performMiniaturize"], 100);
			window.Items.AddRange(lookup["mac_performZoom"], 100);
			window.Items.AddSeparator(101);

			window.Items.AddSeparator(200);
			window.Items.AddRange(lookup["mac_arrangeInFront"], 200);
			window.Items.AddSeparator(201);

			if (AddFullScreenMenuItem)
			{
				var view = menu.GetSubmenu("&View", 300);
				view.Items.AddSeparator(900);
				view.Items.AddRange(lookup["mac_toggleFullScreen"], 900);
				view.Items.AddSeparator(901);
			}
			
			var help = menu.GetSubmenu("&Help", 900);
			// always show help menu
			help.Trim = false;
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
