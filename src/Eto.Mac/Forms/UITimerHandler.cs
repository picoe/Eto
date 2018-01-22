using System;
using Eto.Forms;
#if IOS
using Foundation;
#else
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif
#endif

#if IOS
namespace Eto.iOS.Forms

#elif OSX
namespace Eto.Mac.Forms
#endif
{
	public class UITimerHandler : WidgetHandler<NSTimer, UITimer, UITimer.ICallback>, UITimer.IHandler
	{
		double interval = 1f;
		
		class Helper
		{
			WeakReference handler;
			public UITimerHandler Handler { get { return (UITimerHandler)handler.Target; } set { handler = new WeakReference(value); } }

			#if XAMMAC2 || IOS
			public void Elapsed(NSTimer timer)
			#else
			public void Elapsed()
			#endif
			{
				var h = Handler;
				if (h != null)
				{
					h.Callback.OnElapsed(h.Widget, EventArgs.Empty);
				}
			}
		}

		protected override NSTimer CreateControl()
		{
			return NSTimer.CreateRepeatingTimer(interval, new Helper { Handler = this }.Elapsed);
		}

		public void Start ()
		{
			Stop();
			NSRunLoop.Current.AddTimer(Control, NSRunLoopMode.Default);
		}

		public void Stop ()
		{
			if (HasControl)
			{
				Control.Invalidate();
				Control.Dispose ();
				Control = null;
			}
		}

		public double Interval
		{
			get { return interval; }
			set { 
				interval = value;
				if (Widget.Started)
					Start();
			}
		}
	}
}

