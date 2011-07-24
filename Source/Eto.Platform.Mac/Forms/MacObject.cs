using System;
using MonoMac.Foundation;
using System.Collections.Generic;

namespace Eto.Platform.Mac
{
	public class MacObject<T, W> : WidgetHandler<T, W>
		where T: NSObject 
		where W: Widget
	{
		List<NSObject> notifications;
		
		
		public class ObserverActionArgs : EventArgs
		{
			public W Widget { get; set; }
			public NSNotification Notification { get; set; }
		}
		
		class ObserverWrapper
		{
			public WeakReference Widget { get; set; }
			public WeakReference Action { get; set; }
			
			public void Run(NSNotification notification)
			{
				var action = Action.Target as Action<ObserverActionArgs>;
				var widget = (W)Widget.Target; 
				if (action != null && widget != null) {
					action(new ObserverActionArgs{ Widget = widget, Notification = notification});
				}
			}
		}
		
		protected void AddObserver(NSString key, Action<ObserverActionArgs> action, NSObject control = null)
		{
			if (notifications == null) notifications = new List<NSObject>();
			if (control == null) control = Control;
			
			//NSNotificationCenter.DefaultCenter.AddObserver(key, action, control);
			
			var wrap = new ObserverWrapper{ Action = new WeakReference(action), Widget = new WeakReference(this.Widget) };
			notifications.Add(NSNotificationCenter.DefaultCenter.AddObserver(key, wrap.Run, control));
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

