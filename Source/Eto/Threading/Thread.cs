using System;

namespace Eto.Threading
{
	public interface IThread : IInstanceWidget
	{
		void Create();
		
		void CreateCurrent();
		
		void CreateMain();
		
		void Start();
		
		void Abort();
		
		bool IsAlive { get; }
		
		bool IsMain { get; }
	}
	
	public class Thread : InstanceWidget
	{
		new IThread Handler { get { return (IThread)base.Handler; } }
		
		readonly Action action;
		
		public static Thread CurrentThread(Generator generator = null)
		{
			var thread = new Thread(generator);
			thread.Handler.CreateCurrent();
			return thread;
		}
		
		public static Thread MainThread(Generator generator = null)
		{
			var thread = new Thread(generator);
			thread.Handler.CreateMain();
			return thread;
		}
		
		Thread(Generator generator)
			: base(generator, typeof(IThread))
		{
		}
		
		public Thread(Action action, Generator generator = null)
			: base(generator, typeof(IThread), false)
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
		
		public static bool IsMainThread(Generator generator = null)
		{
			return CurrentThread(generator).IsMain;
		}
		
	}
}
