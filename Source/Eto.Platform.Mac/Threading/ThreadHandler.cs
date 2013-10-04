using System;
using Eto.Threading;

#if OSX
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Threading
#elif IOS

using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace Eto.Platform.iOS.Threading
#endif
{
	public class ThreadHandler : WidgetHandler<NSThread, Thread>, IThread
	{
		class Delegate : NSObject
		{
			WeakReference handler;
			public ThreadHandler Handler { get { return (ThreadHandler)handler.Target; } set { handler = new WeakReference(value); } }

			[Export("execute")]
			public void Execute()
			{
				using (var pool = new NSAutoreleasePool())
				{
					Handler.Widget.OnExecuted();
				}
			}
		}

		static Selector selExecute = new Selector("execute");
		
		public void Create()
		{
			Control = new NSThread(new Delegate { Handler = this }, selExecute, null);
		}

		public void CreateCurrent()
		{
			Control = NSThread.Current;
		}

		public void CreateMain()
		{
			Control = NSThread.MainThread;
		}

		public void Start()
		{
			Control.Start();
		}

		public void Abort()
		{
			Control.Cancel();
		}
		
		public bool IsAlive { get { return Control.IsExecuting; } }

		public bool IsMain { get { return Control.IsMainThread; } }
	}
}

