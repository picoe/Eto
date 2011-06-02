using System;
using MonoMac.Foundation;
using System.Collections.Generic;

namespace Eto.Platform.Mac
{
	public class MacObject<T, W> : WidgetHandler<T, W>
		where T: NSObject 
		where W: IWidget
	{
		List<NSObject> notifications;
		
		protected void AddObserver(NSString key, Action<NSNotification> notification, NSObject control = null)
		{
			if (notifications == null) notifications = new List<NSObject>();
			if (control == null) control = Control;
			notifications.Add(NSNotificationCenter.DefaultCenter.AddObserver(key, notification, control));
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

