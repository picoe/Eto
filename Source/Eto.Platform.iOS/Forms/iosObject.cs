using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

namespace Eto.Platform.iOS.Forms
{
	public class iosObject<T, W> : MacBase<T, W>
		where T: NSObject 
		where W: InstanceWidget
	{
		public virtual object EventObject
		{
			get { return Control; }
		}

		public new void AddMethod (Selector selector, Delegate action, string arguments, object control = null)
		{
			base.AddMethod (selector, action, arguments, control ?? EventObject);
		}
		
		public new NSObject AddObserver (NSString key, Action<ObserverActionArgs> action, NSObject control = null)
		{
			return base.AddObserver (key, action, control ?? Control);
		}
		
		public new void AddControlObserver (NSString key, Action<ObserverActionArgs> action, NSObject control = null)
		{
			base.AddControlObserver (key, action, control ?? Control);
		}
	}
}

