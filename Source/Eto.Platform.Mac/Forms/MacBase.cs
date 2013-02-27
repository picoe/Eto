using System;
using System.Collections.Generic;

#if OSX
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms
#elif IOS
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using NSView = MonoTouch.UIKit.UIView;

namespace Eto.Platform.iOS.Forms
#endif
{
	
	[Register("ObserverHelper")]
	public class ObserverHelper : NSObject
	{
		bool isNotification;
		bool isControl;
		
		public Action<ObserverActionArgs> Action { get; set; }
		
		public NSString KeyPath { get; set; }
		
		public NSObject Control { get; set; }
		
		public Widget Widget { get; set; }
		
		public object Handler { get; set; }
		
		static Selector selPerformAction = new Selector("performAction:");
		
		[Export("performAction:")]
		public void Doit (NSNotification notification)
		{
			Action (new ObserverActionArgs (this, notification));
		}
		
		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			Action (new ObserverActionArgs (this, null));
		}
		
		public void AddToNotificationCenter ()
		{
			if (!isNotification) {
				NSNotificationCenter.DefaultCenter.AddObserver (this, selPerformAction, KeyPath, Control);
				isNotification = true;
			}
		}
		
		public void AddToControl ()
		{
			if (!isControl) {
				//Console.WriteLine ("{0}: 3. Adding observer! {1}, {2}", ((IRef)this.Handler).WidgetID, this.GetType (), Control.GetHashCode ());
				Control.AddObserver (this, KeyPath, NSKeyValueObservingOptions.New, IntPtr.Zero);
				isControl = true;
			}
		}
		
		public void Remove ()
		{
			if (isNotification) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (this);
				isNotification = false;
			}
			if (isControl) {
				//Console.WriteLine ("{0}: 4. Removing observer! {1}, {2}", ((IRef)this.Handler).WidgetID, Handler.GetType (), Control.GetHashCode ());
				Control.RemoveObserver (this, KeyPath);
				isControl = false;
			}
		}

		protected override void Dispose (bool disposing)
		{
			Remove ();
			base.Dispose (disposing);
		}
	}
	
	public class ObserverActionArgs : EventArgs
	{
		ObserverHelper observer;
		
		public ObserverActionArgs (ObserverHelper observer, NSNotification notification)
		{
			this.observer = observer;
			this.Notification = notification;
		}
		
		public Widget Widget { get { return observer.Widget; } }
		
		public object Handler { get { return observer.Handler; } }
		
		public object Control { get { return observer.Control; } }
		
		public NSString KeyPath { get { return observer.KeyPath; } }
		
		public NSNotification Notification { get; private set; }
	}
	
	public interface IMacControl
	{
		object Handler { get; }
	}

	public class MacBase<T, W> : WidgetHandler<T, W>
		where T: class
		where W: InstanceWidget
	{
		List<ObserverHelper> observers;

		public void AddMethod (Selector selector, Delegate action, string arguments, object control)
		{
			var type = control.GetType ();
#if OSX
			if (!typeof(IMacControl).IsAssignableFrom (type))
				throw new EtoException ("Control does not inherit from IMacControl");
#endif
			var cls = new Class (type);
			cls.AddMethod (selector.Handle, action, arguments);
		}
		
		public NSObject AddObserver (NSString key, Action<ObserverActionArgs> action, NSObject control)
		{
			if (observers == null)
				observers = new List<ObserverHelper> ();
			var observer = new ObserverHelper{ Action = action, KeyPath = key, Control = control, Widget = this.Widget, Handler = this };
			observer.AddToNotificationCenter ();
			observers.Add (observer);
			return observer;
		}
		
		public void AddControlObserver (NSString key, Action<ObserverActionArgs> action, NSObject control)
		{
			if (observers == null)
				observers = new List<ObserverHelper> ();
			var observer = new ObserverHelper{ Action = action, KeyPath = key, Control = control, Widget = this.Widget, Handler = this };
			observer.AddToControl ();
			observers.Add (observer);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (observers != null) {
				foreach (var observer in observers) {
					observer.Remove ();
				}
				observers = null;
			}

			base.Dispose (disposing);
		}
	}
}

