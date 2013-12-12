using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IWindow : IDockContainer
	{
		ToolBar ToolBar { get; set; }

		void Close();

		new Point Location { get; set; }

		double Opacity { get; set; }

		string Title { get; set; }

		Screen Screen { get; }
		//void AddToolbar(ToolBar toolBar);
		//void RemoveToolbar(ToolBar toolBar);
		//void ClearToolbars();
	}

	public abstract partial class Window : DockContainer
	{
		new IWindow Handler { get { return (IWindow)base.Handler; } }
		//ToolBarCollection toolBars;

		#region Events

		public const string ClosedEvent = "Window.Closed";

		public event EventHandler<EventArgs> Closed
		{
			add { Properties.AddHandlerEvent(ClosedEvent, value); }
			remove { Properties.RemoveEvent(ClosedEvent, value); }
		}

		public virtual void OnClosed(EventArgs e)
		{
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
			#if DESKTOP
			EventLookup.Register<Window>(c => c.OnWindowStateChanged(null), WindowStateChangedEvent);
			#endif
		}

		protected Window(Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
			//toolBars = new ToolBarCollection(this);
			if (initialize)
				Initialize(); 
		}

		public string Title
		{
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		[Obsolete("Use Title instead")]
		public string Text
		{
			get { return Title; }
			set { Title = value; }
		}

		public new Point Location
		{
			get { return Handler.Location; }
			set { Handler.Location = value; }
		}

		public Rectangle Bounds
		{
			get { return new Rectangle(Handler.Location, Handler.Size); }
			set
			{
				Handler.Location = value.Location;
				Handler.Size = value.Size;
			}
		}

		public ToolBar ToolBar
		{
			get { return Handler.ToolBar; }
			set { Handler.ToolBar = value; }
		}

		public double Opacity
		{
			get { return Handler.Opacity; }
			set { Handler.Opacity = value; }
		}

		public virtual void Close()
		{
			Handler.Close();
		}

		public Screen Screen
		{
			get { return Handler.Screen; }
		}
	}
}
