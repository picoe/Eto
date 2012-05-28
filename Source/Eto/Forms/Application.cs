using System;
using System.ComponentModel;

namespace Eto.Forms
{
	public partial interface IApplication : IInstanceWidget
	{
		void Run (string[] args);
		void Quit();
		
		void GetSystemActions(GenerateActionArgs args);
		
		Key CommonModifier { get; }
		Key AlternateModifier { get; }
		
		void Open(string url);

		void Invoke (Action action);
		void AsyncInvoke (Action action);

	}

	public partial class Application : InstanceWidget
	{
		public static Application Instance { get; private set; }
		
		public event EventHandler<EventArgs> Initialized;
		public virtual void OnInitialized(EventArgs e)
		{
			if (Initialized != null) Initialized(this, e);
		}

		public const string TerminatingEvent = "Application.Terminating";

		event EventHandler<CancelEventArgs> terminating;

		public event EventHandler<CancelEventArgs> Terminating {
			add {
				HandleEvent (TerminatingEvent);
				terminating += value;
			}
			remove { terminating -= value; }
		}

		public virtual void OnTerminating (CancelEventArgs e)
		{
			if (terminating != null)
				terminating (this, e);
		}
		
		IApplication inner;

		public Form MainForm { get; set; }
		
		public string Name { get; set; }

		public Application() : this(Generator.Detect) { }
		
		public Application(Generator g) : base(g, typeof(IApplication))
		{
			Application.Instance = this;
			inner = (IApplication)base.Handler;
			Generator.Initialize(g); // make everything use this by default
		}

		public virtual void Run(params string[] args)
		{
			inner.Run(args);
		}

		[Obsolete("Use Invoke instead")]
		public virtual void InvokeOnMainThread (Action action)
		{
			Invoke (action);
		}

		public virtual void Invoke (Action action)
		{
			inner.Invoke (action);
		}

		public virtual void AsyncInvoke (Action action)
		{
			inner.AsyncInvoke (action);
		}
		
		public void Quit()
		{
			inner.Quit();
		}
		
		public void Open(string url)
		{
			inner.Open(url);
		}
		
		public Key CommonModifier
		{
			get { return inner.CommonModifier; }
		}
		
		public Key AlternateModifier
		{
			get { return inner.AlternateModifier; }
		}
		
		
		public virtual void GetSystemActions(GenerateActionArgs args)
		{
			inner.GetSystemActions(args);
		}
	}
}
