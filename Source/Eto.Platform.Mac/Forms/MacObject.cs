using System;
using MonoMac.Foundation;
using System.Collections.Generic;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms
{
	public interface IMacControl
	{
		object Handler { get; }
	}

	
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
			
			public void Run (NSNotification notification)
			{
				var action = Action.Target as Action<ObserverActionArgs>;
				var widget = (W)Widget.Target; 
				if (action != null && widget != null) {
					action (new ObserverActionArgs{ Widget = widget, Notification = notification});
				}
			}
		}
		
		public virtual object EventObject
		{
			get { return Control; }
		}
		
		protected void RemoveObserver (NSObject observer)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (observer);
			notifications.Remove (observer);
		}

		public void AddMethod (Selector selector, Delegate action, string arguments, object control = null)
		{
			control = control ?? EventObject;
			var type = control.GetType ();
			if (!typeof(IMacControl).IsAssignableFrom (type))
				throw new EtoException("Control does not inherit from IMacControl");
			var cls = new Class(type);
			cls.AddMethod (selector.Handle, action, arguments);
		}
		
		public NSObject AddObserver (NSString key, Action<ObserverActionArgs> action, NSObject control = null)
		{
			if (notifications == null)
				notifications = new List<NSObject> ();
			if (control == null)
				control = Control;
			var wrap = new ObserverWrapper{ Action = new WeakReference (action), Widget = new WeakReference (this.Widget) };
			var observer = NSNotificationCenter.DefaultCenter.AddObserver (key, wrap.Run, control);
			notifications.Add (observer);
			return observer;
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			
			// dispose in finalizer as well
			if (notifications != null) {
				NSNotificationCenter.DefaultCenter.RemoveObservers (notifications);
				notifications = null;
			}
		}
	}
}

