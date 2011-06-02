using System;

namespace Eto.Threading
{
	
	
	public interface IThread : IWidget
	{
		void Create();
		void CreateCurrent();

		void Start();
		void Abort();
		
		bool IsAlive { get; }
	}
	
	public class Thread : Widget
	{
		IThread inner;
		Action action;
		
		public static Thread CurrentThread
		{
			get
			{
				var thread = new Thread();
				thread.inner.CreateCurrent();
				return thread;
			}
		}

		private Thread ()
			: base(Generator.Current, typeof(IThread))
		{
			inner = (IThread)Handler;
		}
		
		public Thread (Action action)
			: this(Generator.Current, action)
		{
		}
		
		public Thread (Generator generator, Action action)
			: base(generator, typeof(IThread))
		{
			inner = (IThread)Handler;
			this.action = action;
			inner.Create();
		}
		
		public virtual void OnExecuted()
		{
			if (action != null) action();
		}
		
		public void Start()
		{
			inner.Start();
		}
		
		public void Abort()
		{
			inner.Abort();
		}
		
		public bool IsAlive
		{
			get { return inner.IsAlive; }
		}
	}
}

