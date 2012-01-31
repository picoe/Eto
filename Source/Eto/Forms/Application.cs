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

		void InvokeOnMainThread (Action action);

	}

	public partial class Application : InstanceWidget, IApplication
	{
		public static Application Instance { get; private set; }
		
		public event EventHandler<EventArgs> Initialized;
		public virtual void OnInitialized(EventArgs e)
		{
			if (Initialized != null) Initialized(this, e);
		}

		public event EventHandler<CancelEventArgs> Terminating;
		public virtual void OnTerminating(CancelEventArgs e)
		{
			if (Terminating != null) Terminating(this, e);
		}
		
		IApplication inner;

		public Form MainForm { get; set; }
		
		public string Name { get; set; }

		public Application() : this(Generator.Current) { }
		
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

		public virtual void InvokeOnMainThread (Action action)
		{
			inner.InvokeOnMainThread (action);
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
