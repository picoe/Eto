using System;
using Eto.Forms;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using SD = System.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public class ApplicationHandler : WidgetHandler<UIApplication, Application>, IApplication
	{
		
		public static ApplicationHandler Instance 
		{
			get { return Application.Instance.Handler as ApplicationHandler; }
		}
		
		public string DelegateClassName { get; set; }
		
		public UIApplicationDelegate AppDelegate { get; private set; }
		
		public ApplicationHandler()
		{
			DelegateClassName = "EtoAppDelegate";
		}
				
		public void Run(string[] args)
		{
			UIApplication.Main(args, null, DelegateClassName);
		}
		
		public void Initialize(UIApplicationDelegate appdelegate)
		{
			this.AppDelegate = appdelegate;
			
			Widget.OnInitialized(EventArgs.Empty);
			
		}
		

		public void Quit()
		{
			//UIApplication.SharedApplication...SharedApplication.Terminate((NSObject)NSApplication.SharedApplication.KeyWindow ?? AppDelegate);
		}
		
		public void Open (string url)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
		}
		
		public void GetSystemActions (GenerateActionArgs args)
		{
			
		}
		
		public Key CommonModifier {
			get { return Key.Application; }
		}

		public Key AlternateModifier {
			get { return Key.Alt; }
		}


	}
}
