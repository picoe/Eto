using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

namespace Eto.Platform.iOS.Forms
{
	[Register("ControlObserver")]
	class ControlObserver : NSObject
	{
		public Action Action { get; set; }
		public NSString KeyPath { get; set; }
		public NSObject Control { get; set; }
		
		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			Action();
		}
	}

	public class iosObject<T, W> : WidgetHandler<T, W>
		where T: NSObject 
		where W: InstanceWidget
	{
		List<NSObject> notifications;
		List<ControlObserver> observers;
		bool disposedHooked;
		

		public void AddMethod (IntPtr selector, Delegate action, string arguments, object control = null)
		{
			control = control ?? Control;
			var type = control.GetType ();
			/*if (!typeof(IMacControl).IsAssignableFrom (type))
				throw new EtoException("Control does not inherit from IMacControl");*/
			var cls = new Class(type);
			cls.AddMethod (selector, action, arguments);
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
			var observer = new ControlObserver{ Action = action, KeyPath = key, Control = this.Control };
			observers.Add (observer);
			Console.WriteLine ("{0}: 3. Adding observer! {1}, {2}", this.WidgetID, this.GetType (), Control.GetHashCode ());
			Control.AddObserver(observer, key, NSKeyValueObservingOptions.New, IntPtr.Zero);
		}

		void DisposeInternal ()
		{
			if (observers != null) {
				foreach (var observer in observers) {
					Console.WriteLine ("{0}: 4. Removing observer! {1}, {2}", this.WidgetID, this.GetType (), observer.Control.GetHashCode ());
					Control.RemoveObserver(observer, observer.KeyPath);
				}
				observers = null;
			}

			// dispose in finalizer as well
			if (notifications != null) {
				NSNotificationCenter.DefaultCenter.RemoveObservers(notifications);
				notifications = null;
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				/*if (!NSThread.Current.IsMainThread)
					UIApplication.SharedApplication.InvokeOnMainThread (DisposeInternal);
				else*/
					DisposeInternal ();
			}

			base.Dispose (disposing);
		}
	}
}

