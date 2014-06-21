using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Starting point for any UI application
	/// </summary>
	/// <remarks>
	/// This class is used to start an application.
	/// 
	/// When you are using Eto.Forms within an existing application, you can use the <see cref="Attach"/> method.
	/// </remarks>
	[Handler(typeof(Application.IHandler))]
	public class Application : Widget
	{
		/// <summary>
		/// Gets the current application instance
		/// </summary>
		/// <value>The instance.</value>
		public static Application Instance { get; private set; }

		/// <summary>
		/// Occurs when the application is initialized
		/// </summary>
		/// <remarks>
		/// This is where any of your startup code should be placed, such as creating the main form and showing it.
		/// If subclassing Application, you can override <see cref="OnInitialized"/> instead.
		/// </remarks>
		public event EventHandler<EventArgs> Initialized
		{
			add { Properties.AddEvent(InitializedKey, value); }
			remove { Properties.RemoveEvent(InitializedKey, value); }
		}

		static readonly object InitializedKey = new object();

		/// <summary>
		/// Raises the <see cref="Initialized"/> event.
		/// </summary>
		/// <remarks>
		/// This is where any of your startup code should be placed, such as creating the main form and showing it.
		/// If you are not subclassing Application, you can handle the <see cref="Initialized"/> event instead.
		/// </remarks>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnInitialized(EventArgs e)
		{
			Properties.TriggerEvent(InitializedKey, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Terminating"/> event
		/// </summary>
		public const string TerminatingEvent = "Application.Terminating";

		/// <summary>
		/// Occurs when the application is terminating.
		/// </summary>
		public event EventHandler<CancelEventArgs> Terminating
		{
			add { Properties.AddHandlerEvent(TerminatingEvent, value); }
			remove { Properties.RemoveEvent(TerminatingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Terminating"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnTerminating(CancelEventArgs e)
		{
			Properties.TriggerEvent(TerminatingEvent, this, e);
		}

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		Form mainForm;
		/// <summary>
		/// Gets or sets the main form for your application.
		/// </summary>
		/// <remarks>
		/// When you set this to your main application form, it will get standard platform behaviour applied, such as
		/// quitting the application when it is closed (on windows, linux), or showing the main form on OS X when clicking
		/// the application icon if it has been hidden/closed.
		/// 
		/// Setting this is optional, however you must then manually call <see cref="Quit"/> when you want your application
		/// to quit in that case.
		/// </remarks>
		/// <value>The main form for your application.</value>
		public Form MainForm
		{
			get { return mainForm; }
			set
			{
				mainForm = value;
				Handler.OnMainFormChanged();
			}
		}

		/// <summary>
		/// Gets or sets the name of your application
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		static Application()
		{
			EventLookup.Register<Application>(c => c.OnTerminating(null), Application.TerminatingEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Application"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor or Application(Platform) and HandlerAttribute instead")]
		protected Application(Generator generator, Type type, bool initialize = true)
			: base(generator ?? Platform.Detect, type, initialize)
		{
			Application.Instance = this;
			Platform.Initialize(generator as Platform ?? Platform.Detect); // make everything use this by default
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Application"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use Application(Platform) instead")]
		public Application(Generator generator)
			: this((Platform)generator)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Application"/> class.
		/// </summary>
		public Application()
			: this(Platform.Detect)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Application"/> class with the specified platform type
		/// </summary>
		/// <seealso cref="Platforms"/>
		/// <param name="platformType">Platform type to initialize this application with</param>
		public Application(string platformType)
			: this(Platform.Get(platformType))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Application"/> class with the specified platform
		/// </summary>
		/// <param name="platform">Platform to run the application</param>
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

		/// <summary>
		/// Runs the application with the specified arguments
		/// </summary>
		/// <param name="args">Arguments to run the application</param>
		public virtual void Run(params string[] args)
		{
			Handler.Run(args);
		}

		/// <summary>
		/// Attach the application to an already-running native application with the same platform.
		/// </summary>
		/// <param name="context">Context of the application</param>
		public virtual Application Attach(object context = null)
		{
			Handler.Attach(context);
			return this;
		}

		/// <summary>
		/// Invoke the specified action on the UI thread, blocking the current execution until it is complete.
		/// </summary>
		/// <param name="action">Action to invoke</param>
		public virtual void Invoke(Action action)
		{
			Handler.Invoke(action);
		}

		/// <summary>
		/// Invoke the action asynchronously on the UI thread
		/// </summary>
		/// <remarks>
		/// This will return immediately and queue the action to be executed on the UI thread, regardless on whether
		/// the current thread is the UI thread.
		/// </remarks>
		/// <param name="action">Action to queue on the UI thread.</param>
		public virtual void AsyncInvoke(Action action)
		{
			Handler.AsyncInvoke(action);
		}

		/// <summary>
		/// Quits the application
		/// </summary>
		/// <remarks>
		/// This will call the <see cref="Terminating"/> event before terminating the application.
		/// </remarks>
		public void Quit()
		{
			Handler.Quit();
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Eto.Forms.Application"/> supports the <see cref="Quit"/> operation.
		/// </summary>
		/// <value><c>true</c> if quit is supported; otherwise, <c>false</c>.</value>
		public bool QuitIsSupported
		{
			get { return Handler.QuitIsSupported; }
		}

		/// <summary>
		/// Open the specified file or url with its associated application.
		/// </summary>
		/// <param name="url">url or file path to open</param>
		public void Open(string url)
		{
			Handler.Open(url);
		}

		/// <summary>
		/// Gets the common modifier for shortcuts.
		/// </summary>
		/// <remarks>
		/// On Windows/Linux, this will typically return <see cref="Keys.Control"/>, and on OS X this will be <see cref="Keys.Application"/> (the command key).
		/// </remarks>
		/// <value>The common modifier.</value>
		public Keys CommonModifier
		{
			get { return Handler.CommonModifier; }
		}

		/// <summary>
		/// Gets the alternate modifier for shortcuts.
		/// </summary>
		/// <remarks>
		/// This is usually the <see cref="Keys.Alt"/> key.
		/// </remarks>
		/// <value>The alternate modifier.</value>
		public Keys AlternateModifier
		{
			get { return Handler.AlternateModifier; }
		}

		/// <summary>
		/// Gets the system commands used for standard menu items
		/// </summary>
		/// <remarks>
		/// The system commands are used for the <see cref="MenuBar.CreateStandardMenu"/>.
		/// This is useful to be able to access the system command list to build your own menu instead of using the standard
		/// menu.
		/// </remarks>
		/// <returns>The system commands.</returns>
		public IEnumerable<Command> GetSystemCommands()
		{
			return Handler.GetSystemCommands();
		}

		/// <summary>
		/// Gets or sets the badge label on the application icon in the dock, taskbar, etc.
		/// </summary>
		/// <remarks>
		/// This allows you to specify the text to show as a label to notify the state of your application to the user.
		/// Note that some platforms (iOS) only support numeric badge labels.
		/// </remarks>
		/// <value>The badge label.</value>
		public string BadgeLabel
		{
			get { return Handler.BadgeLabel; }
			set { Handler.BadgeLabel = value; }
		}

		/// <summary>
		/// Advanced. Runs an iteration of the main UI loop when you are blocking the UI thread with logic.
		/// </summary>
		/// <remarks>
		/// This is not recommended to use and you should use asynchronous calls instead via Task.Run or threads.
		/// </remarks>
		public void RunIteration()
		{
			Handler.RunIteration();
		}

		/// <summary>
		/// Restarts the application
		/// </summary>
		public void Restart()
		{
			Handler.Restart();
		}

		internal void InternalCreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands = null)
		{
			Handler.CreateStandardMenu(menuItems, commands ?? GetSystemCommands());
		}

		/// <summary>
		/// Creates the standard menu.
		/// </summary>
		/// <param name="menuItems">Menu items.</param>
		/// <param name="commands">Commands.</param>
		[Obsolete("Use MenuBar.CreateStandardMenu() instead")]
		public void CreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands = null)
		{
			Handler.CreateStandardMenu(menuItems, commands ?? GetSystemCommands());
		}


		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Interface for callbacks to the <see cref="Application"/> class
		/// </summary>
		public new interface ICallback : Widget.ICallback
		{
			/// <summary>
			/// Raises the initialized event.
			/// </summary>
			void OnInitialized(Application widget, EventArgs e);
			/// <summary>
			/// Raises the terminating event.
			/// </summary>
			void OnTerminating(Application widget, CancelEventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="Application"/> class
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the initialized event.
			/// </summary>
			public void OnInitialized(Application widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnInitialized(e));
			}
			/// <summary>
			/// Raises the terminating event.
			/// </summary>
			public void OnTerminating(Application widget, CancelEventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnTerminating(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="Application"/> class
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Attach the application to an already-running native application with the same platform.
			/// </summary>
			/// <param name="context">Context of the application</param>
			void Attach(object context);

			/// <summary>
			/// Runs the application with the specified arguments
			/// </summary>
			/// <param name="args">Arguments to run the application</param>
			void Run(string[] args);

			/// <summary>
			/// Quits the application
			/// </summary>
			/// <remarks>
			/// This should call the <see cref="ICallback.OnTerminating"/> callback to allow user-defined code to cancel
			/// the operation.
			/// </remarks>
			void Quit();

			/// <summary>
			/// Gets a value indicating whether the application supports the <see cref="Quit"/> operation.
			/// </summary>
			/// <value><c>true</c> if quit is supported; otherwise, <c>false</c>.</value>
			bool QuitIsSupported { get ; }

			/// <summary>
			/// Gets the system commands used for standard menu items
			/// </summary>
			/// <remarks>
			/// The system commands are used for the <see cref="MenuBar.CreateStandardMenu"/>.
			/// This is useful to be able to access the system command list to build your own menu instead of using the standard
			/// menu.
			/// </remarks>
			/// <returns>The system commands.</returns>
			IEnumerable<Command> GetSystemCommands();

			/// <summary>
			/// Gets the common modifier for shortcuts.
			/// </summary>
			/// <remarks>
			/// On Windows/Linux, this will typically return <see cref="Keys.Control"/>, and on OS X this will be <see cref="Keys.Application"/> (the command key).
			/// </remarks>
			/// <value>The common modifier.</value>
			Keys CommonModifier { get; }

			/// <summary>
			/// Gets the alternate modifier for shortcuts.
			/// </summary>
			/// <remarks>
			/// This is usually the <see cref="Keys.Alt"/> key.
			/// </remarks>
			/// <value>The alternate modifier.</value>
			Keys AlternateModifier { get; }

			/// <summary>
			/// Open the specified file or url with its associated application.
			/// </summary>
			/// <param name="url">url or file path to open</param>
			void Open(string url);

			/// <summary>
			/// Invoke the specified action on the UI thread, blocking the current execution until it is complete.
			/// </summary>
			/// <remarks>
			/// Implementors should be careful to execute the action directly if the current thread is the UI thread.
			/// </remarks>
			/// <param name="action">Action to invoke</param>
			void Invoke(Action action);

			/// <summary>
			/// Invoke the action asynchronously on the UI thread
			/// </summary>
			/// <remarks>
			/// This will return immediately and queue the action to be executed on the UI thread, regardless on whether
			/// the current thread is the UI thread.
			/// </remarks>
			/// <param name="action">Action to queue on the UI thread.</param>
			void AsyncInvoke(Action action);

			/// <summary>
			/// Gets or sets the badge label on the application icon in the dock, taskbar, etc.
			/// </summary>
			/// <remarks>
			/// This allows you to specify the text to show as a label to notify the state of your application to the user.
			/// Note that some platforms (iOS) only support numeric badge labels.
			/// </remarks>
			/// <value>The badge label.</value>
			string BadgeLabel { get; set; }

			/// <summary>
			/// Called by the widget when the <see cref="Application.MainForm"/> is changed
			/// </summary>
			void OnMainFormChanged();

			/// <summary>
			/// Restarts the application
			/// </summary>
			void Restart();

			/// <summary>
			/// Advanced. Runs an iteration of the main UI loop when you are blocking the UI thread with logic.
			/// </summary>
			/// <remarks>
			/// This is not recommended to use and you should use asynchronous calls instead via Task.Run or threads.
			/// </remarks>
			void RunIteration();

			/// <summary>
			/// Creates a standard menu for the platform.
			/// </summary>
			/// <remarks>
			/// This should only create menu items that are required for the platform.  For example, on OS X cut/copy/paste
			/// will not work via shortcuts unless they are added to the menu.  All other platforms have no standard menu
			/// defined.
			/// </remarks>
			/// <param name="menuItems">Menu item collection to add the standard menu to</param>
			/// <param name="commands">Commands for the standard menu, usually returned from <see cref="GetSystemCommands"/></param>
			void CreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands);
		}
	}
}
