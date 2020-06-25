using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// State of a <see cref="Window"/>
	/// </summary>
	public enum WindowState
	{
		/// <summary>
		/// Normal, windowed state
		/// </summary>
		Normal,
		/// <summary>
		/// Window is maximized, taking the entire screen space
		/// </summary>
		Maximized,
		/// <summary>
		/// Window is minimized to the dock/taskbar/etc.
		/// </summary>
		Minimized
	}

	/// <summary>
	/// Style of a <see cref="Window"/>
	/// </summary>
	public enum WindowStyle
	{
		/// <summary>
		/// Default, bordered style
		/// </summary>
		Default,
		/// <summary>
		/// Window with no border
		/// </summary>
		None,
		/// <summary>
		/// Utility window, usually with a smaller border
		/// </summary>
		/// <remarks>
		/// Note that this is only a hint; some platforms may show it as a default window.
		/// E.g. on macOS, only a <see cref="FloatingForm"/> supports this mode.
		/// </remarks>
		Utility
	}

	/// <summary>
	/// Base window
	/// </summary>
	public abstract class Window : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Closed"/> event.
		/// </summary>
		public const string ClosedEvent = "Window.Closed";

		/// <summary>
		/// Occurs when the window is closed.
		/// </summary>
		public event EventHandler<EventArgs> Closed
		{
			add { Properties.AddHandlerEvent(ClosedEvent, value); }
			remove { Properties.RemoveEvent(ClosedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Closed"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnClosed(EventArgs e)
		{
			OnUnLoad(EventArgs.Empty);
			Properties.TriggerEvent(ClosedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="Closing"/> event.
		/// </summary>
		public const string ClosingEvent = "Window.Closing";

		/// <summary>
		/// Occurs before the window is closed, giving an opportunity to cancel the close operation.
		/// </summary>
		public event EventHandler<CancelEventArgs> Closing
		{
			add { Properties.AddHandlerEvent(ClosingEvent, value); }
			remove { Properties.RemoveEvent(ClosingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Closing"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnClosing(CancelEventArgs e)
		{
			Properties.TriggerEvent(ClosingEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="LocationChanged"/> event.
		/// </summary>
		public const string LocationChangedEvent = "Window.LocationChanged";

		/// <summary>
		/// Occurs when the <see cref="Location"/> of the window is changed.
		/// </summary>
		public event EventHandler<EventArgs> LocationChanged
		{
			add { Properties.AddHandlerEvent(LocationChangedEvent, value); }
			remove { Properties.RemoveEvent(LocationChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="LocationChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnLocationChanged(EventArgs e)
		{
			Properties.TriggerEvent(LocationChangedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="OwnerChanged"/> event.
		/// </summary>
		private const string OwnerChangedEvent = "Window.OwnerChanged";

		/// <summary>
		/// Occurs when the <see cref="Owner"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> OwnerChanged
		{
			add { Properties.AddEvent(OwnerChangedEvent, value); }
			remove { Properties.RemoveEvent(OwnerChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="OwnerChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnOwnerChanged(EventArgs e)
		{
			Properties.TriggerEvent(OwnerChangedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="WindowStateChanged"/> event.
		/// </summary>
		public const string WindowStateChangedEvent = "Window.WindowStateChanged";

		/// <summary>
		/// Occurs when the <see cref="WindowState"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> WindowStateChanged
		{
			add { Properties.AddHandlerEvent(WindowStateChangedEvent, value); }
			remove { Properties.RemoveEvent(WindowStateChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="WindowStateChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnWindowStateChanged(EventArgs e)
		{
			Properties.TriggerEvent(WindowStateChangedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="LogicalPixelSizeChanged"/> event.
		/// </summary>
		public const string LogicalPixelSizeChangedEvent = "Window.LogicalPixelSizeChanged";

		/// <summary>
		/// Occurs when the <see cref="LogicalPixelSize"/> of the window is changed.
		/// </summary>
		public event EventHandler<EventArgs> LogicalPixelSizeChanged
		{
			add { Properties.AddHandlerEvent(LogicalPixelSizeChangedEvent, value); }
			remove { Properties.RemoveEvent(LogicalPixelSizeChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="LogicalPixelSizeChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnLogicalPixelSizeChanged(EventArgs e)
		{
			Properties.TriggerEvent(LogicalPixelSizeChangedEvent, this, e);
		}

		#endregion

		static Window()
		{
			EventLookup.Register<Window>(c => c.OnClosed(null), ClosedEvent);
			EventLookup.Register<Window>(c => c.OnClosing(null), ClosingEvent);
			EventLookup.Register<Window>(c => c.OnLocationChanged(null), LocationChangedEvent);
			EventLookup.Register<Window>(c => c.OnWindowStateChanged(null), WindowStateChangedEvent);
			EventLookup.Register<Window>(c => c.OnLogicalPixelSizeChanged(null), LogicalPixelSizeChangedEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Window"/> class.
		/// </summary>
		protected Window()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Window"/> with the specified handler
		/// </summary>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		protected Window(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets or sets the title of the window
		/// </summary>
		/// <remarks>
		/// The title of the window is displayed to the user usually at the top of the window, but in cases where
		/// you show a window in a mobile environment, this may be the title shown in a navigation controller.
		/// </remarks>
		/// <value>The title of the window</value>
		public string Title
		{
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		/// <summary>
		/// Gets or sets the location of the window
		/// </summary>
		/// <remarks>
		/// Note that in multi-monitor setups, the origin of the location is at the upper-left of <see cref="Eto.Forms.Screen.PrimaryScreen"/>
		/// </remarks>
		public new Point Location
		{
			get { return Handler.Location; }
			set { Handler.Location = value; }
		}

		/// <summary>
		/// Gets or sets the size and location of the window
		/// </summary>
		/// <value>The bounding rectangle of the window</value>
		public new Rectangle Bounds
		{
			get { return new Rectangle(Handler.Location, Handler.Size); }
			set
			{
				Handler.Location = value.Location;
				Handler.Size = value.Size;
			}
		}

		/// <summary>
		/// Gets or sets the tool bar for the window.
		/// </summary>
		/// <remarks>
		/// Note that each window can only have a single tool bar
		/// </remarks>
		/// <value>The tool bar for the window</value>
		public ToolBar ToolBar
		{
			get { return Handler.ToolBar; }
			set
			{ 
				var toolbar = Handler.ToolBar;
				if (toolbar != null)
				{
					toolbar.TriggerUnLoad(EventArgs.Empty);
					toolbar.Parent = null;
				}
				if (value != null)
				{
					value.Parent = this;
					value.TriggerPreLoad(EventArgs.Empty);
				}
				Handler.ToolBar = value;
				if (value != null)
					value.TriggerLoad(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the opacity of the window
		/// </summary>
		/// <value>The window opacity.</value>
		public double Opacity
		{
			get { return Handler.Opacity; }
			set { Handler.Opacity = value; }
		}

		/// <summary>
		/// Closes the window
		/// </summary>
		/// <remarks>
		/// Note that once a window is closed, it cannot be shown again in most platforms.
		/// </remarks>
		public virtual void Close()
		{
			// if we're already disposed, don't bother crashing.
			if (IsDisposed)
				return;
			Handler.Close();
		}

		static readonly object OwnerKey = new object();

		/// <summary>
		/// Gets or sets the owner of this window.
		/// </summary>
		/// <remarks>
		/// This sets the parent window that has ownership over this window.
		/// For a <see cref="Dialog"/>, this will be the window that will be disabled while the modal dialog is shown.
		/// With a  <see cref="Form"/>, the specified owner will always be below the current window when shown, and will 
		/// still be responsive to user input.  Typically, but not always, the window will move along with the owner.
		/// </remarks>
		/// <value>The owner of this window.</value>
		public Window Owner
		{
			get => Properties.Get<Window>(OwnerKey);
			set
			{
				if (Properties.TrySet(OwnerKey, value))
				{
					Handler.SetOwner(value);
					OnOwnerChanged(EventArgs.Empty);
				};
			}
		}

		/// <summary>
		/// Gets the screen this window is mostly contained in. Typically defined by the screen center of the window is visible.
		/// </summary>
		/// <value>The window's current screen.</value>
		public Screen Screen
		{
			get { return Handler.Screen; }
		}

		/// <summary>
		/// Gets or sets the menu bar for this window
		/// </summary>
		/// <remarks>
		/// Some platforms have a global menu bar (e.g. ubuntu, OS X).
		/// When the winow is in focus, the global menu bar will be changed to reflect the menu assigned.
		/// </remarks>
		/// <value>The menu.</value>
		public virtual MenuBar Menu
		{
			get { return Handler.Menu; }
			set
			{
				var menu = Handler.Menu;
				if (menu != null)
				{
					menu.OnUnLoad(EventArgs.Empty);
					menu.Parent = null;
				}
				if (value != null)
				{
					value.Parent = this;
					value.OnPreLoad(EventArgs.Empty);
				}
				Handler.Menu = value;
				if (value != null)
					value.OnLoad(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the icon for the window to show in the menu bar.
		/// </summary>
		/// <remarks>
		/// The icon should have many variations, such as 16x16, 24x24, 32x32, 48x48, 64x64, etc. This ensures that
		/// the many places it is used (title bar, task bar, switch window, etc) all have optimized icon sizes.
		/// 
		/// For OS X, the application icon is specified in the .app bundle, not by this value.
		/// </remarks>
		/// <value>The icon for this window.</value>
		public Icon Icon
		{
			get { return Handler.Icon; }
			set { Handler.Icon = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> is resizable.
		/// </summary>
		/// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
		public bool Resizable
		{
			get { return Handler.Resizable; }
			set { Handler.Resizable = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> can be maximized.
		/// </summary>
		/// <remarks>
		/// This may hide or disable the minimize button on the title bar.
		/// </remarks>
		/// <value><c>true</c> if maximizable; otherwise, <c>false</c>.</value>
		public bool Maximizable
		{
			get { return Handler.Maximizable; }
			set { Handler.Maximizable = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> can be minimized.
		/// </summary>
		/// <remarks>
		/// This may hide or disable the maximize button on the title bar.
		/// </remarks>
		/// <value><c>true</c> if minimizable; otherwise, <c>false</c>.</value>
		public bool Minimizable
		{
			get { return Handler.Minimizable; }
			set { Handler.Minimizable = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> will show in the taskbar.
		/// </summary>
		/// <remarks>
		/// Some platforms, e.g. macOS do not show a separate icon for each running window.  You may also have to add 
		/// the LSUIElement key to your app's Info.plist to make your app hidden in the dock.  
		/// See https://developer.apple.com/library/archive/documentation/General/Reference/InfoPlistKeyReference/Articles/LaunchServicesKeys.html#//apple_ref/doc/uid/TP40009250-108256-TPXREF136
		/// </remarks>
		/// <value><c>true</c> if the window will show in taskbar; otherwise, <c>false</c>.</value>
		public bool ShowInTaskbar
		{
			get { return Handler.ShowInTaskbar; }
			set { Handler.ShowInTaskbar = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> is above all other windows.
		/// </summary>
		/// <remarks>
		/// The window should be above all other windows when this is true.  In some platforms, this will show above all other windows only
		/// when the application has focus.
		/// </remarks>
		/// <value><c>true</c> if the window should be topmost; otherwise, <c>false</c>.</value>
		public bool Topmost
		{
			get { return Handler.Topmost; }
			set { Handler.Topmost = value; }
		}

		/// <summary>
		/// Gets or sets the state of the window.
		/// </summary>
		/// <value>The state of the window.</value>
		public WindowState WindowState
		{
			get { return Handler.WindowState; }
			set { Handler.WindowState = value; }
		}

		/// <summary>
		/// Gets the bounds of the window before it was minimized or maximized, or the current bounds if <see cref="WindowState"/> is Normal.
		/// </summary>
		/// <remarks>
		/// This is useful to retrieve the desired size and position of the window even though it is currently maximized or minimized.
		/// </remarks>
		/// <value>The restore bounds.</value>
		public Rectangle RestoreBounds
		{
			get { return Handler.RestoreBounds; }
		}

		/// <summary>
		/// Sets <see cref="WindowState"/> to <see cref="Eto.Forms.WindowState.Minimized"/>
		/// </summary>
		public void Minimize()
		{
			Handler.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Sets <see cref="WindowState"/> to <see cref="Eto.Forms.WindowState.Maximized"/>
		/// </summary>
		public void Maximize()
		{
			Handler.WindowState = WindowState.Maximized;
		}

		/// <summary>
		/// Gets or sets the style of this window.
		/// </summary>
		/// <value>The window style.</value>
		public WindowStyle WindowStyle
		{
			get { return Handler.WindowStyle; }
			set { Handler.WindowStyle = value; }
		}

		/// <summary>
		/// Brings the window in front of all other windows in the z-order.
		/// </summary>
		public void BringToFront()
		{
			Handler.BringToFront();
		}

		/// <summary>
		/// Sends the window behind all other windows in the z-order.
		/// </summary>
		public void SendToBack()
		{
			Handler.SendToBack();
		}

		/// <summary>
		/// Raises the <see cref="BindableWidget.DataContextChanged"/> event
		/// </summary>
		/// <remarks>
		/// Implementors may override this to fire this event on child widgets in a heirarchy. 
		/// This allows a control to be bound to its own <see cref="BindableWidget.DataContext"/>, which would be set
		/// on one of the parent control(s).
		/// </remarks>
		/// <param name="e">Event arguments</param>
		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			ToolBar?.TriggerDataContextChanged();
			Menu?.TriggerDataContextChanged();
		}

		/// <summary>
		/// Gets the number of pixels per logical pixel when on a high DPI display.
		/// </summary>
		/// <remarks>
		/// This indicates the number of pixels per logical pixel.  
		/// All units in Eto.Forms such as control size, drawing operations, etc are in logical pixels.
		/// When not in high DPI, this will be 1.0; 
		/// Retina displays in OS X will return 2; and
		/// in windows this matches the scale set in the monitor settings.
		/// 
		/// Use the <see cref="LogicalPixelSizeChanged"/> to detect when the window is moved to 
		/// a display with a different DPI.
		/// </remarks>
		public float LogicalPixelSize => Handler.LogicalPixelSize;

		/// <summary>
		/// Gets or sets a value indicating that the window can be moved by click+dragging the window background
		/// </summary>
		public bool MovableByWindowBackground
		{
			get => Handler.MovableByWindowBackground;
			set => Handler.MovableByWindowBackground = value;
		}

		#region Callback

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for instances of <see cref="Window"/>
		/// </summary>
		public new interface ICallback : Panel.ICallback
		{
			/// <summary>
			/// Raises the closed event.
			/// </summary>
			void OnClosed(Window widget, EventArgs e);
			/// <summary>
			/// Raises the closing event.
			/// </summary>
			void OnClosing(Window widget, CancelEventArgs e);
			/// <summary>
			/// Raises the location changed event.
			/// </summary>
			void OnLocationChanged(Window widget, EventArgs e);
			/// <summary>
			/// Raises the window state changed event.
			/// </summary>
			void OnWindowStateChanged(Window widget, EventArgs e);
			/// <summary>
			/// Raises the logical pixel size changed event.
			/// </summary>
			void OnLogicalPixelSizeChanged(Window widget, EventArgs e);
		}

		/// <summary>
		/// Callback methods for handlers of <see cref="Control"/>
		/// </summary>
		protected new class Callback : Panel.Callback, ICallback
		{
			/// <summary>
			/// Raises the closed event.
			/// </summary>
			public void OnClosed(Window widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnClosed(e);
			}
			/// <summary>
			/// Raises the closing event.
			/// </summary>
			public void OnClosing(Window widget, CancelEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnClosing(e);
			}
			/// <summary>
			/// Raises the location changed event.
			/// </summary>
			public void OnLocationChanged(Window widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnLocationChanged(e);
			}
			/// <summary>
			/// Raises the window state changed event.
			/// </summary>
			public void OnWindowStateChanged(Window widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnWindowStateChanged(e);
			}
			/// <summary>
			/// Raises the logical pixel size changed event.
			/// </summary>
			public void OnLogicalPixelSizeChanged(Window widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnLogicalPixelSizeChanged(e);
			}
		}

		#endregion

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="Window"/>
		/// </summary>
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Gets or sets the tool bar for the window.
			/// </summary>
			/// <remarks>
			/// Note that each window can only have a single tool bar
			/// </remarks>
			/// <value>The tool bar for the window</value>
			ToolBar ToolBar { get; set; }

			/// <summary>
			/// Closes the window
			/// </summary>
			/// <remarks>
			/// Note that once a window is closed, it cannot be shown again in most platforms.
			/// </remarks>
			void Close();

			/// <summary>
			/// Gets or sets the location of the window
			/// </summary>
			/// <remarks>
			/// Note that in multi-monitor setups, the origin of the location is at the upper-left of <see cref="Eto.Forms.Screen.PrimaryScreen"/>
			/// </remarks>
			new Point Location { get; set; }

			/// <summary>
			/// Gets or sets the opacity of the window
			/// </summary>
			/// <value>The window opacity.</value>
			double Opacity { get; set; }

			/// <summary>
			/// Gets or sets the title of the window
			/// </summary>
			/// <remarks>
			/// The title of the window is displayed to the user usually at the top of the window, but in cases where
			/// you show a window in a mobile environment, this may be the title shown in a navigation controller.
			/// </remarks>
			/// <value>The title of the window</value>
			string Title { get; set; }

			/// <summary>
			/// Gets the screen this window is mostly contained in. Typically defined by the screen center of the window is visible.
			/// </summary>
			/// <value>The window's current screen.</value>
			Screen Screen { get; }

			/// <summary>
			/// Gets or sets the menu bar for this window
			/// </summary>
			/// <remarks>
			/// Some platforms have a global menu bar (e.g. ubuntu, OS X).
			/// When the winow is in focus, the global menu bar will be changed to reflect the menu assigned.
			/// </remarks>
			/// <value>The menu.</value>
			MenuBar Menu { get; set; }

			/// <summary>
			/// Gets or sets the icon for the window to show in the menu bar.
			/// </summary>
			/// <remarks>
			/// The icon should have many variations, such as 16x16, 24x24, 32x32, 48x48, 64x64, etc. This ensures that
			/// the many places it is used (title bar, task bar, switch window, etc) all have optimized icon sizes.
			/// 
			/// For OS X, the application icon is specified in the .app bundle, not by this value.
			/// </remarks>
			/// <value>The icon for this window.</value>
			Icon Icon { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> is resizable.
			/// </summary>
			/// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
			bool Resizable { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> can be maximized.
			/// </summary>
			/// <remarks>
			/// This may hide or disable the minimize button on the title bar.
			/// </remarks>
			/// <value><c>true</c> if maximizable; otherwise, <c>false</c>.</value>
			bool Maximizable { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> can be minimized.
			/// </summary>
			/// <remarks>
			/// This may hide or disable the maximize button on the title bar.
			/// </remarks>
			/// <value><c>true</c> if minimizable; otherwise, <c>false</c>.</value>
			bool Minimizable { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> will show in the taskbar.
			/// </summary>
			/// <remarks>
			/// Some platforms, e.g. OS X do not show a separate icon for each running window.
			/// </remarks>
			/// <value><c>true</c> if the window will show in taskbar; otherwise, <c>false</c>.</value>
			bool ShowInTaskbar { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Window"/> is above all other windows.
			/// </summary>
			/// <remarks>
			/// The window should be above all other windows when this is true.  In some platforms, this will show above all other windows only
			/// when the application has focus.
			/// </remarks>
			/// <value><c>true</c> if the window should be topmost; otherwise, <c>false</c>.</value>
			bool Topmost { get; set; }

			/// <summary>
			/// Gets or sets the state of the window.
			/// </summary>
			/// <value>The state of the window.</value>
			WindowState WindowState { get; set; }

			/// <summary>
			/// Gets the bounds of the window before it was minimized or maximized.
			/// </summary>
			/// <remarks>
			/// This is useful to retrieve the desired size and position of the window even though it is currently maximized or minimized.
			/// </remarks>
			/// <value>The restore bounds.</value>
			Rectangle RestoreBounds { get; }

			/// <summary>
			/// Gets or sets the style of this window.
			/// </summary>
			/// <value>The window style.</value>
			WindowStyle WindowStyle { get; set; }

			/// <summary>
			/// Brings the window in front of all other windows in the z-order.
			/// </summary>
			void BringToFront();

			/// <summary>
			/// Sends the window behind all other windows in the z-order.
			/// </summary>
			void SendToBack();

			/// <summary>
			/// Sets the owner of the window
			/// </summary>
			/// <param name="owner">Owner of the window</param>
			void SetOwner(Window owner);

			/// <summary>
			/// Gets the number of pixels per logical pixel when on a high DPI display.
			/// </summary>
			/// <remarks>
			/// This indicates the number of pixels per logical pixel.  
			/// All units in Eto.Forms such as control size, drawing operations, etc are in logical pixels.
			/// When not in high DPI, this will be 1.0; 
			/// Retina displays in OS X will return 2; and
			/// in windows this matches the scale set in the monitor settings.
			/// 
			/// Use the <see cref="LogicalPixelSizeChanged"/> to detect when the window is moved to 
			/// a display with a different DPI.
			/// </remarks>
			float LogicalPixelSize { get; }

			/// <summary>
			/// Gets or sets a value indicating that the window can be moved by click+dragging the window background
			/// </summary>
			bool MovableByWindowBackground { get; set; }
		}

		#endregion
	}
}
