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

	
	public class MacObject<T, W> : MacBase<T, W>
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
	}
}

