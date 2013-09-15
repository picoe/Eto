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
		IWindow handler;
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
			handler = (IWindow)this.Handler;
			//toolBars = new ToolBarCollection(this);
			if (initialize) Initialize (); 
		}

		public string Title {
			get { return handler.Title; }
			set { handler.Title = value; }
		}

		[Obsolete("Use Title instead")]
		public string Text {
			get { return Title; }
			set { Title = value; }
		}
		
		public new Point Location {
			get { return handler.Location; }
			set { handler.Location = value; }
		}
		
		public Rectangle Bounds {
			get { return new Rectangle (handler.Location, handler.Size); }
			set { handler.Location = value.Location;
				handler.Size = value.Size; }
		}
		
		public ToolBar ToolBar {
			get { return handler.ToolBar; }
			set { handler.ToolBar = value; }
		}

		public double Opacity {
			get { return handler.Opacity; }
			set { handler.Opacity = value; }
		}
		
		public virtual void Close ()
		{
			handler.Close ();
		}
		
		public Screen Screen
		{
			get { return handler.Screen; }
		}
		
	}
}
