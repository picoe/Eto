using System;
using System.Collections.Generic;
using System.Globalization;
using Eto.Drawing;
using Eto.Forms;
#if IOS
using NSView = UIKit.UIView;
using ObjCRuntime;
using Foundation;
#elif XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms
{
	public interface IMacControlHandler
	{
		NSView ContainerControl { get; }

		Size MinimumSize { get; set; }

		bool IsEventHandled(string eventName);

		NSView ContentControl { get; }

		NSView EventControl { get; }

		NSView FocusControl { get; }

		bool AutoSize { get; }

		SizeF GetPreferredSize(SizeF availableSize);

		void RecalculateKeyViewLoop(ref NSView last);

		void InvalidateMeasure();
	}

	[Register("ObserverHelper")]
	public class ObserverHelper : NSObject
	{
		bool isNotification;
		bool isControl;

		public Action<ObserverActionEventArgs> Action { get; set; }

		public NSString KeyPath { get; set; }

		WeakReference control;

		public NSObject Control { get { return (NSObject)(control != null ? control.Target : null); } set { control = new WeakReference(value); } }

		WeakReference widget;

		public Widget Widget { get { return (Widget)(widget != null ? widget.Target : null); } set { widget = new WeakReference(value); } }

		WeakReference handler;

		public object Handler { get { return handler != null ? handler.Target : null; } set { handler = new WeakReference(value); } }

		static readonly Selector selPerformAction = new Selector("performAction:");

		[Export("performAction:")]
		public void PerformAction(NSNotification notification)
		{
			Action(new ObserverActionEventArgs(this, notification));
		}

		public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			Action(new ObserverActionEventArgs(this, null));
		}

		public void AddToNotificationCenter()
		{
			var c = Control;
			if (!isNotification && c != null)
			{
				NSNotificationCenter.DefaultCenter.AddObserver(this, selPerformAction, KeyPath, c);
				c.DangerousRetain();
				isNotification = true;
			}
		}

		public void AddToControl()
		{
			var c = Control;
			if (!isControl && c != null)
			{
				//Console.WriteLine ("{0}: 3. Adding observer! {1}, {2}", ((IRef)this.Handler).WidgetID, this.GetType (), Control.GetHashCode ());
				c.AddObserver(this, KeyPath, NSKeyValueObservingOptions.New, IntPtr.Zero);
				c.DangerousRetain();
				isControl = true;
			}
		}

		public void Remove()
		{
			var c = Control;
			if (isNotification)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(this);
				if (c != null)
					c.DangerousRelease();
				isNotification = false;
			}
			if (isControl && c != null && KeyPath != null)
			{
				//Console.WriteLine ("{0}: 4. Removing observer! {1}, {2}", ((IRef)this.Handler).WidgetID, Handler.GetType (), Control.GetHashCode ());
				c.RemoveObserver(this, KeyPath);
				c.DangerousRelease();
				isControl = false;
			}
		}

		protected override void Dispose(bool disposing)
		{
			Remove();
			base.Dispose(disposing);
		}
	}

	public class ObserverActionEventArgs : EventArgs
	{
		readonly ObserverHelper observer;

		public ObserverActionEventArgs(ObserverHelper observer, NSNotification notification)
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
		WeakReference WeakHandler { get; set; }
	}

	static class MacBase
	{
		public static object GetHandler(IntPtr sender) => GetHandler(Runtime.GetNSObject(sender));

		public static object GetHandler(object control)
		{
			var notification = control as NSNotification;
			if (notification != null)
				control = notification.Object;

			var macControl = control as IMacControl;
			if (macControl == null || macControl.WeakHandler == null)
				return null;
			return macControl.WeakHandler.Target;
		}

	}

	public abstract class MacBase<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>
		where TControl: class
		where TWidget: Widget
	{
		protected override void Initialize()
		{
			base.Initialize();

			var control = Control as IMacControl;
			if (control != null)
				control.WeakHandler = new WeakReference(this);
		}


		List<ObserverHelper> observers;

		public static object GetHandler(IntPtr sender) => MacBase.GetHandler(Runtime.GetNSObject(sender));

		public static object GetHandler(object control) => MacBase.GetHandler(control);

		public bool AddMethod(IntPtr selector, Delegate action, string arguments, object control)
		{
			var type = control.GetType();
			#if OSX
			if (!typeof(IMacControl).IsAssignableFrom(type))
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Control '{0}' does not inherit from IMacControl", type));
			if (((IMacControl)control).WeakHandler?.Target == null)
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Control '{0}' has a null handler", type));
			#endif
			var classHandle = Class.GetHandle(type);

			return ObjCExtensions.AddMethod(classHandle, selector, action, arguments);
		}

		public bool HasMethod(IntPtr selector, object control)
		{
			var type = control.GetType();
			var classHandle = Class.GetHandle(type);
			return ObjCExtensions.GetInstanceMethod(classHandle, selector) != IntPtr.Zero;
		}


		public NSObject AddObserver(NSString key, Action<ObserverActionEventArgs> action, NSObject control)
		{
			if (observers == null)
			{
				observers = new List<ObserverHelper>();
				// ensure we finalize to clean this up later
				GC.ReRegisterForFinalize(this);
			}
			var observer = new ObserverHelper
			{
				Action = action,
				KeyPath = key,
				Control = control,
				Widget = Widget,
				Handler = this
			};
			observer.AddToNotificationCenter();
			observers.Add(observer);
			return observer;
		}

		public void AddControlObserver(NSString key, Action<ObserverActionEventArgs> action, NSObject control)
		{
			if (observers == null)
			{
				observers = new List<ObserverHelper>();
				// ensure we finalize to clean this up later
				GC.ReRegisterForFinalize(this);
			}
			var observer = new ObserverHelper
			{
				Action = action,
				KeyPath = key,
				Control = control,
				Widget = Widget,
				Handler = this
			};
			observer.AddToControl();
			observers.Add(observer);
		}

		~MacBase()
		{
			//Console.WriteLine("Finalizing {0}", GetType());
			// need to remove observers so we can GC the native object
			Dispose(false);
		}

		public MacBase()
		{
			// finalizer is not actually needed until we add an observer.
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (observers != null)
			{
				foreach (var observer in observers)
				{
					observer.Remove();
				}
				observers = null;
			}

			base.Dispose(disposing);
		}
	}
}

