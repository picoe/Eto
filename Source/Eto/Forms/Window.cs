using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IWindow : IContainer
	{
		ToolBar ToolBar { get; set; }
		
		void Close ();
		
		Point Location { get; set; }

		double Opacity { get; set; }

		string Title { get; set; }

		//void AddToolbar(ToolBar toolBar);
		//void RemoveToolbar(ToolBar toolBar);
		//void ClearToolbars();
	}
	
	public abstract partial class Window : Container
	{
		IWindow inner;
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
		
		#endregion
		

		protected Window (Generator g, Type type) : base(g, type, false)
		{
			inner = (IWindow)this.Handler;
			//toolBars = new ToolBarCollection(this);
			Initialize (); 
		}

		public string Title {
			get { return inner.Title; }
			set { inner.Title = value; }
		}

		[Obsolete("Use Title instead")]
		public string Text {
			get { return Title; }
			set { Title = value; }
		}
		
		public Point Location {
			get { return inner.Location; }
			set { inner.Location = value; }
		}
		
		public Rectangle Bounds {
			get { return new Rectangle (inner.Location, inner.Size); }
			set { inner.Location = value.Location;
				inner.Size = value.Size; }
		}
		
		public ToolBar ToolBar {
			get { return inner.ToolBar; }
			set { inner.ToolBar = value; }
		}

		public double Opacity {
			get { return inner.Opacity; }
			set { inner.Opacity = value; }
		}
		
		public virtual void Close ()
		{
			inner.Close ();
		}
		
	}
}
