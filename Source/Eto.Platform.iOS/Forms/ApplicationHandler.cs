using System;
using Eto.Forms;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using SD = System.Drawing;
using System.Diagnostics;

namespace Eto.Platform.iOS.Forms
{
	public class ApplicationHandler : WidgetHandler<UIApplication, Application>, IApplication
	{
		bool attached;

		public static ApplicationHandler Instance
		{
			get { return Application.Instance.Handler as ApplicationHandler; }
		}

		public string DelegateClassName { get; set; }

		public UIApplicationDelegate AppDelegate { get; private set; }

		public ApplicationHandler()
		{
			Control = UIApplication.SharedApplication;
			DelegateClassName = "EtoAppDelegate";
			UIApplication.CheckForIllegalCrossThreadCalls = false;
		}

		public void Run(string[] args)
		{
			if (!attached)
			{
				UIApplication.Main(args, null, DelegateClassName);
			}
			else
			{
				Initialize(Control.Delegate);
			}
		}

		public void Attach(object context)
		{
			attached = true;
		}

		public void OnMainFormChanged()
		{
		}

		public void Initialize(UIApplicationDelegate appdelegate)
		{
			AppDelegate = appdelegate;
			
			Widget.OnInitialized(EventArgs.Empty);
		}

		public void Invoke(Action action)
		{
			var thread = NSThread.Current;
			if (thread != null && thread.IsMainThread)
				action();
			else
			{
				UIApplication.SharedApplication.InvokeOnMainThread(delegate
				{
					action(); 
				});
			}
		}

		public void AsyncInvoke(Action action)
		{
			var thread = NSThread.Current;
			if (thread != null && thread.IsMainThread)
				action();
			else
				UIApplication.SharedApplication.BeginInvokeOnMainThread(delegate
				{
					action(); 
				});
		}

		public virtual void GetSystemActions(GenerateActionArgs args, bool addStandardItems)
		{
		}

		public void Quit()
		{
			//UIApplication.SharedApplication...SharedApplication.Terminate((NSObject)NSApplication.SharedApplication.KeyWindow ?? AppDelegate);
		}

		public void Open(string url)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
		}

		public void GetSystemActions(GenerateActionArgs args)
		{
			
		}

		public Key CommonModifier
		{
			get { return Key.Application; }
		}

		public Key AlternateModifier
		{
			get { return Key.Alt; }
		}

		public string BadgeLabel
		{
			get { return Control.ApplicationIconBadgeNumber > 0 ? Convert.ToString(Control.ApplicationIconBadgeNumber) : null; }
			set
			{ 
				if (string.IsNullOrEmpty(value))
					Control.ApplicationIconBadgeNumber = 0;
				else
				{
					int result;
					if (Int32.TryParse(value, out result))
						Control.ApplicationIconBadgeNumber = result;
					else
					{
						Debug.WriteLine("iOS: Application.BadgeLabel only supports numeric values");
						Control.ApplicationIconBadgeNumber = 0;
					}
				}
			}
		}
	}
}
