using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IWindow : IContainer, ITextControl
	{
		ToolBar ToolBar { get; set; }
		
		void Close ();

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
		
		public const string ShownEvent = "Window.Shown";
		
		event EventHandler<EventArgs> shown;
		
		public event EventHandler<EventArgs> Shown {
			add {
				HandleEvent (ShownEvent);
				shown += value;
			}
			remove { shown -= value; }
		}
		
		public virtual void OnShown (EventArgs e)
		{
			if (shown != null)
				shown (this, e);
		}
		
		public const string MinimizedEvent = "Window.Minimized";
		
		event EventHandler<EventArgs> minimized;
		
		public event EventHandler<EventArgs> Minimized {
			add {
				HandleEvent (MinimizedEvent);
				minimized += value;
			}
			remove { minimized -= value; }
		}
		
		public virtual void OnMinimized (EventArgs e)
		{
			if (minimized != null)
				minimized (this, e);
		}

		public const string MaximizedEvent = "Window.Maximized";
		
		event EventHandler<EventArgs> maximized;
		
		public event EventHandler<EventArgs> Maximized {
			add {
				HandleEvent (MaximizedEvent);
				maximized += value;
			}
			remove { maximized -= value; }
		}
		
		public virtual void OnMaximized (EventArgs e)
		{
			if (maximized != null)
				maximized (this, e);
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
