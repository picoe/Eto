using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IWindow : IPanel
	{
		ToolBar ToolBar { get; set; }

		void Close();

		new Point Location { get; set; }

		double Opacity { get; set; }

		string Title { get; set; }

		Screen Screen { get; }
	}

	public abstract partial class Window : Panel
	{
		new IWindow Handler { get { return (IWindow)base.Handler; } }

		#region Events

		public const string ClosedEvent = "Window.Closed";

		public event EventHandler<EventArgs> Closed
		{
			add { Properties.AddHandlerEvent(ClosedEvent, value); }
			remove { Properties.RemoveEvent(ClosedEvent, value); }
		}

		public virtual void OnClosed(EventArgs e)
		{
			OnUnLoad(EventArgs.Empty);
			Properties.TriggerEvent(ClosedEvent, this, e);
		}

		public const string ClosingEvent = "Window.Closing";

		public event EventHandler<CancelEventArgs> Closing
		{
			add { Properties.AddHandlerEvent(ClosingEvent, value); }
			remove { Properties.RemoveEvent(ClosingEvent, value); }
		}

		public virtual void OnClosing(CancelEventArgs e)
		{
			Properties.TriggerEvent(ClosingEvent, this, e);
		}

		public const string LocationChangedEvent = "Window.LocationChanged";

		public event EventHandler<EventArgs> LocationChanged
		{
			add { Properties.AddHandlerEvent(LocationChangedEvent, value); }
			remove { Properties.RemoveEvent(LocationChangedEvent, value); }
		}

		public virtual void OnLocationChanged(EventArgs e)
		{
			Properties.TriggerEvent(LocationChangedEvent, this, e);
		}

		#endregion

		static Window()
		{
			EventLookup.Register<Window>(c => c.OnClosed(null), ClosedEvent);
			EventLookup.Register<Window>(c => c.OnClosing(null), ClosingEvent);
			EventLookup.Register<Window>(c => c.OnLocationChanged(null), LocationChangedEvent);
			EventLookup.Register<Window>(c => c.OnWindowStateChanged(null), WindowStateChangedEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Window"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler instance</param>
		/// <param name="type">Type of interface to create for the handler, must implement <see cref="IWindow"/></param>
		/// <param name="initialize"><c>true</c> to initialize the handler, false if the subclass will initialize</param>
		protected Window(Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
			if (initialize)
				Initialize();
			HandleEvent(ClosedEvent);
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
		/// Note that in multi-monitor setups, the origin of the location is at the upper-left of <see cref="Screen.PrimaryScreen"/>
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
					toolbar.OnUnLoad(EventArgs.Empty);
				Handler.ToolBar = value;
				if (value != null)
					value.OnLoad(EventArgs.Empty);
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
			Handler.Close();
		}

		/// <summary>
		/// Gets the screen this window is mostly contained in. Typically defined by the screen center of the window is visible.
		/// </summary>
		/// <value>The window's current screen.</value>
		public Screen Screen
		{
			get { return Handler.Screen; }
		}
	}
}
