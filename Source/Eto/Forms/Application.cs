using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Eto.Forms
{
	public partial interface IApplication : IInstanceWidget
	{
		void Attach(object context);

		void Run(string[] args);

		void Quit();

		IEnumerable<Command> GetSystemCommands();

		Keys CommonModifier { get; }

		Keys AlternateModifier { get; }

		void Open(string url);

		void Invoke(Action action);

		void AsyncInvoke(Action action);

		string BadgeLabel { get; set; }

		void OnMainFormChanged();
	}

	[Handler(typeof(IApplication))]
	public partial class Application : InstanceWidget
	{
		public static Application Instance { get; private set; }

		public event EventHandler<EventArgs> Initialized
		{
			add { Properties.AddEvent(InitializedKey, value); }
			remove { Properties.RemoveEvent(InitializedKey, value); }
		}

		static readonly object InitializedKey = new object();

		public virtual void OnInitialized(EventArgs e)
		{
			Properties.TriggerEvent(InitializedKey, this, e);
		}

		public const string TerminatingEvent = "Application.Terminating";

		public event EventHandler<CancelEventArgs> Terminating
		{
			add { Properties.AddHandlerEvent(TerminatingEvent, value); }
			remove { Properties.RemoveEvent(TerminatingEvent, value); }
		}

		public virtual void OnTerminating(CancelEventArgs e)
		{
			Properties.TriggerEvent(TerminatingEvent, this, e);
		}

		new IApplication Handler { get { return (IApplication)base.Handler; } }

		Form mainForm;
		public Form MainForm
		{
			get { return mainForm; }
			set
			{
				mainForm = value;
				Handler.OnMainFormChanged();
			}
		}

		public string Name { get; set; }

		static Application()
		{
			EventLookup.Register<Application>(c => c.OnTerminating(null), Application.TerminatingEvent);
		}

#if !PCL
		public Application()
			: this(Platform.Detect)
		{
		}

		[Obsolete("Use default constructor or Application(Platform) and HandlerAttribute instead")]
		protected Application(Generator generator, Type type, bool initialize = true)
			: base(generator ?? Platform.Detect, type, initialize)
		{
			Application.Instance = this;
			Platform.Initialize(generator as Platform ?? Platform.Detect); // make everything use this by default
		}
#else
		[Obsolete("Use Application(Platform) and HandlerAttribute instead")]
		protected Application(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Application.Instance = this;
			Platform.Initialize(generator as Platform);
		}
#endif

		[Obsolete("Use Application(Platform) instead")]
		public Application(Generator generator)
			: this((Platform)generator)
		{
		}

		public Application(string platformType)
			: this(Platform.Get(platformType))
		{
		}

		public Application(Platform platform)
			: this(InitializePlatform(platform))
		{
			Application.Instance = this;
		}

		Application(InitHelper init)
		{
		}

		class InitHelper { }

		/// <summary>
		/// Helper to call proper constructor for initializing the platform before base class constructor is called
		/// </summary>
		static InitHelper InitializePlatform(Platform platform)
		{
			Platform.Initialize(platform);
			return null;
		}

		public virtual void Run(params string[] args)
		{
			Handler.Run(args);
		}

		public virtual Application Attach(object context = null)
		{
			Handler.Attach(context);
			return this;
		}

		public virtual void Invoke(Action action)
		{
			Handler.Invoke(action);
		}

		public virtual void AsyncInvoke(Action action)
		{
			Handler.AsyncInvoke(action);
		}

		public void Quit()
		{
			Handler.Quit();
		}

		public void Open(string url)
		{
			Handler.Open(url);
		}

		public Keys CommonModifier
		{
			get { return Handler.CommonModifier; }
		}

		public Keys AlternateModifier
		{
			get { return Handler.AlternateModifier; }
		}

		public IEnumerable<Command> GetSystemCommands()
		{
			return Handler.GetSystemCommands();
		}

		public string BadgeLabel
		{
			get { return Handler.BadgeLabel; }
			set { Handler.BadgeLabel = value; }
		}
	}
}
