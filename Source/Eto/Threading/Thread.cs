using System;

namespace Eto.Threading
{
	[AutoInitialize(false)]
	public interface IThread : IWidget
	{
		void Create();
		
		void CreateCurrent();
		
		void CreateMain();
		
		void Start();
		
		void Abort();
		
		bool IsAlive { get; }
		
		bool IsMain { get; }
	}

	[Handler(typeof(IThread))]
	public class Thread : Widget
	{
		new IThread Handler { get { return (IThread)base.Handler; } }
		
		readonly Action action;

		public static Thread CurrentThread
		{
			get
			{
				var thread = new Thread();
				thread.Handler.CreateCurrent();
				thread.Initialize();
				return thread;
			}
		}

		public static Thread MainThread
		{
			get
			{
				var thread = new Thread();
				thread.Handler.CreateMain();
				thread.Initialize();
				return thread;
			}
		}

		Thread()
		{
		}

		public Thread(Action action)
		{
			this.action = action;
			Handler.Create();
			Initialize();
		}

		public virtual void OnExecuted()
		{
			if (action != null)
				action();
		}
		
		public void Start()
		{
			Handler.Start();
		}
		
		public void Abort()
		{
			Handler.Abort();
		}
		
		public bool IsAlive
		{
			get { return Handler.IsAlive; }
		}
		
		public bool IsMain
		{
			get { return Handler.IsMain; }
		}
		
		public static bool IsMainThread
		{
			get { return CurrentThread.IsMain; }
		}

		#pragma warning disable 612,618

		[Obsolete("Use constructor without generator instead")]
		public Thread(Action action, Generator generator = null)
			: base(generator, typeof(IThread), false)
		{
			this.action = action;
			Handler.Create();
			Initialize();
		}

		#pragma warning restore 612,618
	}
}
