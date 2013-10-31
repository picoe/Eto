using System;
using MonoMac.Foundation;
using MonoTouch.Foundation;

namespace Eto.Platform.Mac.Forms
{
	public class MacObject<TControl, TWidget> : MacBase<TControl, TWidget>
		where TControl: NSObject 
		where TWidget: InstanceWidget
	{
		public virtual object EventObject
		{
			get { return Control; }
		}

		public new void AddMethod (IntPtr selector, Delegate action, string arguments, object control = null)
		{
			base.AddMethod (selector, action, arguments, control ?? EventObject);
		}

		public new NSObject AddObserver (NSString key, Action<ObserverActionEventArgs> action, NSObject control = null)
		{
			return base.AddObserver (key, action, control ?? Control);
		}
		
		public new void AddControlObserver (NSString key, Action<ObserverActionEventArgs> action, NSObject control = null)
		{
			base.AddControlObserver (key, action, control ?? Control);
		}
		
	}
}

