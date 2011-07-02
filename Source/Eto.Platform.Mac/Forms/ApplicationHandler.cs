using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Actions;

namespace Eto.Platform.Mac
{
	public class ApplicationHandler : WidgetHandler<NSApplication, Application>, IApplication
	{
		public NSApplicationDelegate AppDelegate { get; private set; }
		
		public static ApplicationHandler Instance
		{
			get { return Application.Instance.Handler as ApplicationHandler; }
		}
		
		public ApplicationHandler()
		{
			NSApplication.Init();
		}

		public void RunIteration()
		{
			NSApplication.SharedApplication.NextEvent(NSEventMask.AnyEvent, NSDate.DistantFuture, NSRunLoop.NSDefaultRunLoopMode, true);
		}
		
		public void Run()
		{
			NSApplication.Main(new string[] {});
		}
		
		public void Initialize(NSApplicationDelegate appdelegate)
		{
			this.AppDelegate = appdelegate;
			Widget.OnInitialized(EventArgs.Empty);	
		}
		

		public void Quit()
		{
			NSApplication.SharedApplication.Terminate(AppDelegate);
		}
		
		public void Open (string url)
		{
			NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl(url));
		}
		
		public void GetSystemActions (GenerateActionArgs args)
		{
			args.Actions.AddButton ("mac_hide", string.Format("Hide {0}|Hide {0}|Hides the main {0} window", Widget.Name), delegate {
				NSApplication.SharedApplication.Hide(NSApplication.SharedApplication);
			}, Key.H | Key.Application);
			args.Actions.AddButton ("mac_hideothers", "Hide Others|Hide Others|Hides all other application windows", delegate {
				NSApplication.SharedApplication.HideOtherApplications(NSApplication.SharedApplication);
			}, Key.H | Key.Application | Key.Alt);
			args.Actions.AddButton ("mac_showall", "Show All|Show All|Show All Windows", delegate {
				NSApplication.SharedApplication.UnhideAllApplications(NSApplication.SharedApplication);
			});
			
			args.Actions.Add(new MacButtonAction("mac_performMiniaturize", "Minimize", "performMiniaturize:"){ Accelerator = Key.Application | Key.M });
			args.Actions.Add(new MacButtonAction("mac_performZoom", "Zoom", "performZoom:"));
			
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
