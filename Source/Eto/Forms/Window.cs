using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IWindow : IDockContainer
	{
		ToolBar ToolBar { get; set; }
		
		void Close ();
		
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

		EventHandler<EventArgs> closed;

		public event EventHandler<EventArgs> Closed {
			add {
				HandleEvent (ClosedEvent);
				closed += value;
			}
			remove { closed -= value; }
		}

		public virtual void OnClosed (EventArgs e)
		{
			if (closed != null)
				closed (this, e);
		}
		

		public const string ClosingEvent = "Window.Closing";

		EventHandler<CancelEventArgs> closing;

		public event EventHandler<CancelEventArgs> Closing {
			add {
				HandleEvent (ClosingEvent);
				closing += value;
			}
			remove { closing -= value; }
		}

		public virtual void OnClosing (CancelEventArgs e)
		{
			if (closing != null)
				closing (this, e);
		}

		public const string LocationChangedEvent = "Window.LocationChanged";
		
		EventHandler<EventArgs> locationChanged;
		
		public event EventHandler<EventArgs> LocationChanged {
			add {
				HandleEvent (LocationChangedEvent);
				locationChanged += value;
			}
			remove { locationChanged -= value; }
		}
		
		public virtual void OnLocationChanged (EventArgs e)
		{
			if (locationChanged != null)
				locationChanged (this, e);
		}

		#endregion

		protected Window (Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
			//toolBars = new ToolBarCollection(this);
			if (initialize) Initialize (); 
		}

		public string Title {
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		[Obsolete("Use Title instead")]
		public string Text {
			get { return Title; }
			set { Title = value; }
		}
		
		public new Point Location {
			get { return Handler.Location; }
			set { Handler.Location = value; }
		}
		
		public Rectangle Bounds {
			get { return new Rectangle (Handler.Location, Handler.Size); }
			set { Handler.Location = value.Location;
				Handler.Size = value.Size; }
		}
		
		public ToolBar ToolBar {
			get { return Handler.ToolBar; }
			set { Handler.ToolBar = value; }
		}

		public double Opacity {
			get { return Handler.Opacity; }
			set { Handler.Opacity = value; }
		}
		
		public virtual void Close ()
		{
			Handler.Close ();
		}
		
		public Screen Screen
		{
			get { return Handler.Screen; }
		}
		
	}
}
