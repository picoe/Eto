using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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
		LocalizeEventArgs localizeArgs;
		readonly object localizeLock = new object();
		static readonly object ApplicationKey = new object();

		/// <summary>
		/// Gets the current application instance
		/// </summary>
		/// <value>The instance.</value>
		public static Application Instance
		{
			get
			{ 
				var platform = Platform.Instance;
				return platform != null ? platform.GetSharedProperty<Application>(ApplicationKey, () => null) : null;
			}
			private set { Platform.Instance.SetSharedProperty(ApplicationKey, value); }
		}

		/// <summary>
		/// Event to handle when a string needs to be localized
		/// </summary>
		/// <remarks>
		/// This can be used by some controls (e.g. <see cref="AboutDialog"/> and <see cref="MenuBar"/> on some platforms.
		/// You can use this event or override <see cref="OnLocalizeString(LocalizeEventArgs)"/> to provide your own implementation
		/// for localizing system-supplied strings to other languages.
		/// </remarks>
		public event EventHandler<LocalizeEventArgs> LocalizeString;

		/// <summary>
		/// Triggers the <see cref="LocalizeString"/> event.
		/// </summary>
		/// <param name="e">Event arguments for localization</param>
		protected internal virtual void OnLocalizeString(LocalizeEventArgs e) => LocalizeString?.Invoke(this, e);

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

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="UnhandledException"/> event
		/// </summary>
		public const string UnhandledExceptionEvent = "Application.UnhandledException";

		/// <summary>
		/// Occurs when an unhandled exception occcurs.
		/// </summary>
		public event EventHandler<UnhandledExceptionEventArgs> UnhandledException
		{
			add { Properties.AddHandlerEvent(UnhandledExceptionEvent, value); }
			remove { Properties.RemoveEvent(UnhandledExceptionEvent, value); }
		}

		/// <summary>
		/// Raises the unhandled exception event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnUnhandledException(UnhandledExceptionEventArgs e)
		{
			Properties.TriggerEvent(UnhandledExceptionEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="NotificationActivated"/> event
		/// </summary>
		public const string NotificationActivatedEvent = "Application.NotificationActivated";

		/// <summary>
		/// Occurs when a notification is clicked by the user that was previously displayed.
		/// </summary>
		/// <remarks>
		/// To send a notification, use <see cref="Notification"/>.
		/// 
		/// The <see cref="NotificationEventArgs.ID"/> and <see cref="NotificationEventArgs.UserData"/>
		/// should be used to determine what action to perform when the user clicks on the notification.
		/// These parameters are set when creating the <see cref="Notification"/>
		/// </remarks>
		public event EventHandler<NotificationEventArgs> NotificationActivated
		{
			add { Properties.AddHandlerEvent(NotificationActivatedEvent, value); }
			remove { Properties.RemoveEvent(NotificationActivatedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="NotificationActivated"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnNotificationActivated(NotificationEventArgs e)
		{
			Properties.TriggerEvent(NotificationActivatedEvent, this, e);
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

		List<Window> windows = new List<Window>();

		internal void AddWindow(Window window)
		{
			window.Closed += HandleClosed;
			windows.Add(window);
		}

		void HandleClosed(object sender, EventArgs e)
		{
			var window = (Window)sender;
			window.Closed -= HandleClosed;
			if (windows.Contains(window))
				windows.Remove(window);
		}

		/// <summary>
		/// Gets an enumeration of windows currently open in the application.
		/// </summary>
		/// <value>The enumeration of open windows.</value>
		public IEnumerable<Window> Windows { get { return windows; } }

		/// <summary>
		/// Gets or sets the name of your application
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		static Application()
		{
			EventLookup.Register<Application>(c => c.OnTerminating(null), Application.TerminatingEvent);
			EventLookup.Register<Application>(c => c.OnUnhandledException(null), Application.UnhandledExceptionEvent);
			EventLookup.Register<Application>(c => c.OnNotificationActivated(null), Application.NotificationActivatedEvent);
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
			: this(Platform.Get(platformType, false, null))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Application"/> class with the specified platform
		/// </summary>
		/// <param name="platform">Platform to run the application</param>
		public Application(Platform platform)
			: this(InitializePlatform(platform))
		{
			Instance = this;
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

			if (!Platform.AllowReinitialize && Instance != null)
				throw new InvalidOperationException("The Eto.Forms Application is already created.");

			return null;
		}

		/// <summary>
		/// Runs the application and begins the main loop.
		/// </summary>
		public virtual void Run()
		{
			Handler.Run();
		}

		/// <summary>
		/// Runs the application with the specified <paramref name="mainForm"/> and begins the main loop.
		/// </summary>
		/// <seealso cref="MainForm"/>
		/// <param name="mainForm">Main form for the application.</param>
		public virtual void Run(Form mainForm)
		{
			Initialized += (sender, e) =>
			{
				MainForm = mainForm;
				MainForm.Show();
			};
			Handler.Run();
		}

		/// <summary>
		/// Runs the application with the specified <paramref name="dialog"/> and begins the main loop.
		/// </summary>
		/// <remarks>
		/// When the dialog is closed, the application will exit.
		/// </remarks>
		/// <param name="dialog">Dialog to show for the application.</param>
		public virtual void Run(Dialog dialog)
		{
			Initialized += (sender, e) =>
			{
				dialog.ShowModal();
				Quit();
			};
			Handler.Run();
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
		/// <remarks>
		/// Use this method when you want to perform changes to the UI from a worker thread, and return when
		/// the changes are complete.
		/// </remarks>
		/// <param name="action">Action to invoke</param>
		public virtual void Invoke(Action action)
		{
			Handler.Invoke(action);
		}

		/// <summary>
		/// Invoke the specified function on the UI thread returning its value after the execution is complete.
		/// </summary>
		/// <remarks>
		/// Use this method when you want to return values from the UI in a worker thread.
		/// </remarks>
		/// <param name="func">Function to execute and return the value on the UI thread.</param>
		/// <typeparam name="T">The type of the return value.</typeparam>
		public T Invoke<T>(Func<T> func)
		{
			T value = default(T);
			Invoke(new Action(() => value = func()));
			return value;
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
		/// Invokes the specified function on the UI thread asynchronously and return the result in a Task.
		/// </summary>
		/// <returns>The task that returns the result of the function.</returns>
		/// <param name="func">Function to execute and return the value.</param>
		/// <typeparam name="T">The type of the result.</typeparam>
		public Task<T> InvokeAsync<T>(Func<T> func)
		{
			var tcs = new TaskCompletionSource<T>();
			InvokeAsync(() =>
			{
				try
				{
					var result = func();
					tcs.SetResult(result);
				}
				catch (Exception ex)
				{
					tcs.SetException(ex);
				}
			});
			return tcs.Task;
		}

		/// <summary>
		/// Invokes the specified action on the UI thread asynchronously with a Task.
		/// </summary>
		/// <returns>The task that is used to await when the action is completed.</returns>
		/// <param name="action">Action to execute.</param>
		public Task InvokeAsync(Action action)
		{
			var tcs = new TaskCompletionSource<bool>();
			AsyncInvoke(() =>
			{
				try
				{
					action();
					tcs.SetResult(true);
				}
				catch (Exception ex)
				{
					tcs.SetException(ex);
				}
			});
			return tcs.Task;
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

		/// <summary>
		/// Localizes the specified text for the current locale, or provide alternative text for system supplied strings.
		/// </summary>
		/// <remarks>
		/// This depends on your custom implementation for the <see cref="LocalizeString"/> event.
		/// You can provide your own localization for system-supplied strings or change the strings to your own liking.
		/// </remarks>
		/// <returns>The localized text.</returns>
		/// <param name="source">Source widget to localize for.</param>
		/// <param name="text">English text to localize.</param>
		public string Localize(object source, string text)
		{
			lock (localizeLock)
			{
				localizeArgs = localizeArgs ?? new LocalizeEventArgs();
				localizeArgs.Initialize(source, text);
				OnLocalizeString(localizeArgs);
				return localizeArgs.GetResultAndReset();
			}
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

			/// <summary>
			/// Raises the unhandled exception event.
			/// </summary>
			void OnUnhandledException(Application widget, UnhandledExceptionEventArgs e);

			/// <summary>
			/// Raises the notification activated event.
			/// </summary>
			void OnNotificationActivated(Application wiget, NotificationEventArgs e);
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
				using (widget.Platform.Context)
					widget.OnInitialized(e);
			}
			/// <summary>
			/// Raises the terminating event.
			/// </summary>
			public void OnTerminating(Application widget, CancelEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnTerminating(e);
			}

			/// <summary>
			/// Raises the unhandled exception event.
			/// </summary>
			public void OnUnhandledException(Application widget, UnhandledExceptionEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnUnhandledException(e);
			}

			/// <summary>
			/// Raises the notification activated event.
			/// </summary>
			public void OnNotificationActivated(Application widget, NotificationEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnNotificationActivated(e);
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
			/// Runs the application and starts a main loop.
			/// </summary>
			void Run();

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
		}
	}
}
