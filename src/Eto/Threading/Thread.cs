using System;

namespace Eto.Threading
{
	/// <summary>
	/// Provides access to platform-specific threading.
	/// Not all platforms implement this as you can use System.Threading.Thread instead in most cases.
	/// </summary>
	/// <remarks>
	/// Most platforms have a concept of threads, though some (e.g. WinRT) do not.
	/// This may be removed in a future version in favour of using Task.Run(), Task.Delay(), which works across all
	/// platforms on .net 4.5+ and PCL.
	/// </remarks>
	[Handler(typeof(Thread.IHandler))]
	public class Thread : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		readonly Action action;

		/// <summary>
		/// Gets the current thread instance
		/// </summary>
		/// <value>The current thread.</value>
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

		/// <summary>
		/// Gets the main thread.
		/// </summary>
		/// <value>The main thread.</value>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Threading.Thread"/> class for a specific ation
		/// </summary>
		/// <param name="action">Action to execute in a separate thread</param>
		public Thread(Action action)
		{
			this.action = action;
			Handler.Create();
			Initialize();
		}

		/// <summary>
		/// Called when the thread is executed, for custom thread instances
		/// </summary>
		protected virtual void OnExecuted()
		{
			if (action != null)
				action();
		}

		/// <summary>
		/// Start the thread, for custom thread instances
		/// </summary>
		public void Start()
		{
			Handler.Start();
		}

		/// <summary>
		/// Abort this instance, for custom thread instances
		/// </summary>
		public void Abort()
		{
			Handler.Abort();
		}

		/// <summary>
		/// Gets a value indicating whether this thread is alive.
		/// </summary>
		/// <value><c>true</c> if this thread is alive; otherwise, <c>false</c>.</value>
		public bool IsAlive
		{
			get { return Handler.IsAlive; }
		}

		/// <summary>
		/// Gets a value indicating whether this thread instance is the main UI thread.
		/// </summary>
		/// <value><c>true</c> if this thread instance is the main UI thread; otherwise, <c>false</c>.</value>
		public bool IsMain
		{
			get { return Handler.IsMain; }
		}

		/// <summary>
		/// Gets a value indicating if the current thread is the main UI thread.
		/// </summary>
		/// <value><c>true</c> if the current thread is the main UI thread; otherwise, <c>false</c>.</value>
		public static bool IsMainThread
		{
			get { return CurrentThread.IsMain; }
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Interface for callback methods of this class
		/// </summary>
		public new interface ICallback : Widget.ICallback
		{
			/// <summary>
			/// Raises the executed event.
			/// </summary>
			void OnExecuted(Thread widget);
		}

		/// <summary>
		/// Callbacks for the <see cref="Thread"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the executed event.
			/// </summary>
			public void OnExecuted(Thread widget)
			{
				widget.Platform.Invoke(widget.OnExecuted);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="Thread"/> class
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Creates a custom thread that will execute the <see cref="ICallback.OnExecuted"/> method when started
			/// </summary>
			void Create();

			/// <summary>
			/// Creates an instance for the current thread
			/// </summary>
			void CreateCurrent();

			/// <summary>
			/// Creates an instance for the main UI thread
			/// </summary>
			void CreateMain();

			/// <summary>
			/// Starts a custom thread when created using <see cref="Create"/>
			/// </summary>
			void Start();

			/// <summary>
			/// Aborts a custom thread when created using <see cref="Create"/>
			/// </summary>
			void Abort();

			/// <summary>
			/// Gets a value indicating whether this thread is alive.
			/// </summary>
			/// <value><c>true</c> if this thread is alive; otherwise, <c>false</c>.</value>
			bool IsAlive { get; }

			/// <summary>
			/// Gets a value indicating whether this thread instance is the main UI thread.
			/// </summary>
			/// <value><c>true</c> if this thread instance is the main UI thread; otherwise, <c>false</c>.</value>
			bool IsMain { get; }
		}
	}
}
