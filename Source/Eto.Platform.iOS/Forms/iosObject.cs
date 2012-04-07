using System;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms
{
	public class iosObject<T, W> : WidgetHandler<T, W>
		where T: NSObject 
		where W: Widget
	{
		List<NSObject> notifications;
		List<ControlObserver> observers;
		
		class ControlObserver : NSObject
		{
			public Action Action { get; set; }
			public NSString KeyPath { get; set; }
			
			[Export("observeValueForKeyPath:ofObject:change:context:")]
			public void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
			{
				Action();
			}
		}
		
		
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
		
		protected void AddControlObserver(NSString key, Action action)
		{
			if (observers == null) observers = new List<ControlObserver>();
			var observer = new ControlObserver{ Action = action, KeyPath = key };
			observers.Add (observer);
			Control.AddObserver(observer, key, NSKeyValueObservingOptions.New, IntPtr.Zero);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (observers != null) {
				foreach (var observer in observers)
					Control.RemoveObserver(observer, observer.KeyPath);
			}
			
			// dispose in finalizer as well
			if (notifications != null) {
				NSNotificationCenter.DefaultCenter.RemoveObservers(notifications);
				notifications = null;
			}
		}
	}
}

