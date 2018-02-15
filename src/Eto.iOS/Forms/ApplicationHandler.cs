using System;
using Eto.Forms;
using UIKit;
using Foundation;
using SD = System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

namespace Eto.iOS.Forms
{
	public class ApplicationHandler : WidgetHandler<UIApplication, Application, Application.ICallback>, Application.IHandler
	{
		bool attached;

		public static ApplicationHandler Instance
		{
			get { return Application.Instance.Handler as ApplicationHandler; }
		}

		public string DelegateClassName { get; set; }

		public IUIApplicationDelegate AppDelegate { get; private set; }

		public ApplicationHandler()
		{
			DelegateClassName = "EtoAppDelegate";
			UIApplication.CheckForIllegalCrossThreadCalls = false;
		}

		public float StatusBarAdjustment
		{
			get
			{
				if (Platform.IsIos7)
				{
					var statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;
					return (float)Math.Min(statusBarFrame.Height, statusBarFrame.Width);
				}
				return 0f;
			}
		}

		public void Run()
		{
			if (!attached)
			{
				UIApplication.Main(new string[0], null, DelegateClassName);
			}
			else
			{
				Initialize(UIApplication.SharedApplication.Delegate);
			}
		}

		public void Attach(object context)
		{
			attached = true;
		}

		public void OnMainFormChanged()
		{
		}

		public void Initialize(IUIApplicationDelegate appdelegate)
		{
			AppDelegate = appdelegate;
			
			Callback.OnInitialized(Widget, EventArgs.Empty);
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
			UIApplication.SharedApplication.BeginInvokeOnMainThread(delegate
			{
				action(); 
			});
		}

		public void Quit()
		{
			//UIApplication.SharedApplication...SharedApplication.Terminate((NSObject)NSApplication.SharedApplication.KeyWindow ?? AppDelegate);
		}

		public bool QuitIsSupported { get { return false; } }

		public void Open(string url)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
		}

		public Keys CommonModifier
		{
			get { return Keys.Application; }
		}

		public Keys AlternateModifier
		{
			get { return Keys.Alt; }
		}

		UIUserNotificationType notificationTypes;
		UIRemoteNotificationType remoteNotificationTypes;
		void RegisterNotifications(UIUserNotificationType notifications, UIRemoteNotificationType remoteNotification)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				if (!notificationTypes.HasFlag(notifications))
				{
					notificationTypes |= notifications;
					var settings = UIUserNotificationSettings.GetSettingsForTypes(notificationTypes, null);
					UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
				}
			}
			else if (!remoteNotificationTypes.HasFlag(remoteNotification))
			{
				remoteNotificationTypes |= remoteNotification;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(remoteNotificationTypes);
			}
		}

		public string BadgeLabel
		{
			get { return UIApplication.SharedApplication.ApplicationIconBadgeNumber > 0 ? Convert.ToString(UIApplication.SharedApplication.ApplicationIconBadgeNumber) : null; }
			set
			{ 
				if (string.IsNullOrEmpty(value))
					UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
				else
				{
					RegisterNotifications(UIUserNotificationType.Badge, UIRemoteNotificationType.Badge);
					int result;
					if (Int32.TryParse(value, out result))
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = result;
					else
					{
						Debug.WriteLine("iOS: Application.BadgeLabel only supports numeric values");
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					}
				}
			}
		}


		public void Restart()
		{
			throw new NotImplementedException();
		}

		public void RunIteration()
		{
			throw new NotImplementedException();
		}
	}
}
