using System;
using Foundation;
using Eto.Threading;
using ObjCRuntime;

namespace Eto.iOS.Threading
{
	public class ThreadHandler : WidgetHandler<NSThread, Thread, Thread.ICallback>, Thread.IHandler
	{
		public ThreadHandler ()
		{
		}
		
		class Delegate : NSObject
		{
			public ThreadHandler Handler { get; set; }
			[Export("execute")]
			public void Execute()
			{
				using (var pool = new NSAutoreleasePool())
				{
					Handler.Callback.OnExecuted(Handler.Widget);
				}
			}
		}
		
		public void Create()
		{
			Control = new NSThread(new Delegate { Handler = this }, new Selector("execute"), NSNull.Null);
		}

		public void CreateMain ()
		{
			Control = NSThread.MainThread;
		}

		public void CreateCurrent ()
		{
			Control = NSThread.Current;
		}

		public void Start ()
		{
			Control.Start();
		}

		public void Abort ()
		{
			Control.Cancel();
		}
		
		public bool IsAlive
		{
			get { return Control.IsExecuting; }
		}

		public bool IsMain
		{
			get { return Control.IsMainThread; }
		}
	}
}

