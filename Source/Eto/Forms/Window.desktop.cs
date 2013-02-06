#if DESKTOP
using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public enum WindowState
	{
		Normal,
		Maximized,
		Minimized
	}
	
	public partial interface IWindow : IContainer
	{
		MenuBar Menu { get; set; }
		
		Icon Icon { get; set; }
		
		bool Resizable { get; set; }
		
		WindowState State { get; set; }
		
		Rectangle? RestoreBounds { get; }

        void RemoveBorder();

        void BringToFront();
    }
	
	public abstract partial class Window : Container
	{
		
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

		
		public virtual MenuBar Menu {
			get { return handler.Menu; }
			set { handler.Menu = value; }
		}

		public Icon Icon {
			get { return handler.Icon; }
			set { handler.Icon = value; }
		}
		
		public bool Resizable {
			get { return handler.Resizable; }
			set { handler.Resizable = value; }
		}
		
		public WindowState State {
			get { return handler.State; }
			set { handler.State = value; }
		}
		
		public Rectangle? RestoreBounds {
			get { return handler.RestoreBounds; }
		}
		
		public void Minimize ()
		{
			handler.State = WindowState.Minimized;
		}
		
		public void Maximize ()
		{
			handler.State = WindowState.Maximized;
		}

        public void RemoveBorder()
        {
            handler.RemoveBorder();
        }

        public void BringToFront()
        {
            handler.BringToFront();
        }
	}
}
#endif
