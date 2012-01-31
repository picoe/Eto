using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IWindow : IContainer, ITextControl
	{
		ToolBar ToolBar { get; set; }
		
		void Close ();
		
		Point Location { get; set; }

		//void AddToolbar(ToolBar toolBar);
		//void RemoveToolbar(ToolBar toolBar);
		//void ClearToolbars();
	}
	
	public abstract partial class Window : Container
	{
		IWindow inner;
		//ToolBarCollection toolBars;
		
		
		#region Events
		
		public event EventHandler<EventArgs> Closed;

		public virtual void OnClosed (EventArgs e)
		{
			if (Closed != null)
				Closed (this, e);
		}

		public event EventHandler<CancelEventArgs> Closing;

		public virtual void OnClosing (CancelEventArgs e)
		{
			if (Closing != null)
				Closing (this, e);
		}
		
		#endregion
		

		protected Window (Generator g, Type type) : base(g, type, false)
		{
			inner = (IWindow)this.Handler;
			//toolBars = new ToolBarCollection(this);
			Initialize (); 
		}
	
		public string Text {
			get { return inner.Text; }
			set { inner.Text = value; }
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
		
		public virtual void Close ()
		{
			inner.Close ();
		}
		
	}
}
