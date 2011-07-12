using System;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms
{
	public class iosObject<T, W> : WidgetHandler<T, W>
		where T: NSObject 
		where W: IWidget
	{
		List<NSObject> notifications;
		
		protected void AddObserver(NSString key, Action<NSNotification> notification)
		{
			if (notifications == null) notifications = new List<NSObject>();
			notifications.Add(NSNotificationCenter.DefaultCenter.AddObserver(key, notification, Control));
		}

		protected void AddObserver(NSObject obj, NSString key, Action<NSNotification> notification)
		{
			if (notifications == null) notifications = new List<NSObject>();
			notifications.Add(NSNotificationCenter.DefaultCenter.AddObserver(key, notification, obj));
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			
			// dispose in finalizer as well
			if (notifications != null) {
				NSNotificationCenter.DefaultCenter.RemoveObservers(notifications);
				notifications = null;
			}
		}
	}
}

