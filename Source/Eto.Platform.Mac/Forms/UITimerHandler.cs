using System;
using Eto.Forms;
#if IOS
using MonoTouch.Foundation;
#else
using MonoMac.Foundation;
#endif

namespace Eto.Platform.Mac.Forms
{
	public class UITimerHandler : WidgetHandler<NSTimer, UITimer>, IUITimer
	{
		double interval = UITimer.DefaultInterval;
		
		public UITimerHandler ()
		{
		}

		public void Start ()
		{
			Stop();
			Control = NSTimer.CreateRepeatingTimer(interval, delegate{ 
				Widget.OnElapsed(EventArgs.Empty);
			});
			NSRunLoop.Current.AddTimer(Control, NSRunLoopMode.Default);
		}

		public void Stop ()
		{
			if (Control != null)
			{
				Control.Invalidate();
				Control.Dispose ();
				Control = null;
			}
		}

		public double Interval
		{
			get { return interval; }
			set { interval = value; }
		}
	}
}

