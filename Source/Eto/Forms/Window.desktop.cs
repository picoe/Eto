#if DESKTOP
using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public enum WindowState
	{
		Normal,
		Maximized,
		Minimized
	}

	public enum WindowStyle
	{
		Default,
		None
	}

	public partial interface IWindow
	{
		MenuBar Menu { get; set; }

		Icon Icon { get; set; }

		bool Resizable { get; set; }

		bool Maximizable { get; set; }

		bool Minimizable { get; set; }

		bool ShowInTaskbar { get; set; }

		bool Topmost { get; set; }

		WindowState WindowState { get; set; }

		Rectangle? RestoreBounds { get; }

		WindowStyle WindowStyle { get; set; }

		void BringToFront ();

		void SendToBack ();
	}

	public abstract partial class Window
	{

		public const string WindowStateChangedEvent = "Window.WindowStateChanged";
		
		event EventHandler<EventArgs> windowStateChanged;

		public event EventHandler<EventArgs> WindowStateChanged {
			add {
				HandleEvent (WindowStateChangedEvent);
				windowStateChanged += value;
			}
			remove
			{
				windowStateChanged -= value;
			}
		}

		public virtual void OnWindowStateChanged (EventArgs e)
		{
			if (windowStateChanged != null)
				windowStateChanged (this, e);
#pragma warning disable 612, 618
			if (WindowState == WindowState.Maximized)
				OnMaximized (e);
			if (WindowState == WindowState.Minimized)
				OnMinimized (e);
#pragma warning restore 612, 618
		}

		public virtual MenuBar Menu
		{
			get { return Handler.Menu; }
			set { Handler.Menu = value; }
		}

		public Icon Icon
		{
			get { return Handler.Icon; }
			set { Handler.Icon = value; }
		}

		public bool Resizable
		{
			get { return Handler.Resizable; }
			set { Handler.Resizable = value; }
		}

		public bool Maximizable
		{
			get { return Handler.Maximizable; }
			set { Handler.Maximizable = value; }
		}

		public bool Minimizable
		{
			get { return Handler.Minimizable; }
			set { Handler.Minimizable = value; }
		}

		public bool ShowInTaskbar
		{
			get { return Handler.ShowInTaskbar; }
			set { Handler.ShowInTaskbar = value; }
		}

		[Obsolete("Use Topmost")]
		public bool TopMost { get { return Topmost; } set { Topmost = value; } }

		public bool Topmost
		{
			get { return Handler.Topmost; }
			set { Handler.Topmost = value; }
		}

		public WindowState WindowState
		{
			get { return Handler.WindowState; }
			set { Handler.WindowState = value; }
		}

		public Rectangle? RestoreBounds
		{
			get { return Handler.RestoreBounds; }
		}

		public void Minimize ()
		{
			Handler.WindowState = WindowState.Minimized;
		}

		public void Maximize ()
		{
			Handler.WindowState = WindowState.Maximized;
		}

		public WindowStyle WindowStyle
		{
			get { return Handler.WindowStyle; }
			set { Handler.WindowStyle = value; }
		}

		public void BringToFront ()
		{
			Handler.BringToFront ();
		}

		public void SendToBack ()
		{
			Handler.SendToBack ();
		}

		#region Obsolete

		[Obsolete ("Use StateChanged event instead")]
		public const string MinimizedEvent = WindowStateChangedEvent;

		event EventHandler<EventArgs> minimized;

		[Obsolete("Use StateChanged event instead")]
		public event EventHandler<EventArgs> Minimized
		{
			add
			{
				HandleEvent (WindowStateChangedEvent);
				minimized += value;
			}
			remove { minimized -= value; }
		}
		
		[Obsolete("Use OnStateChanged instead")]
		protected virtual void OnMinimized (EventArgs e)
		{
			if (minimized != null)
				minimized (this, e);
		}

		[Obsolete ("Use StateChanged event instead")]
		public const string MaximizedEvent = WindowStateChangedEvent;
		
		event EventHandler<EventArgs> maximized;
		
		[Obsolete("Use StateChanged event instead")]
		public event EventHandler<EventArgs> Maximized
		{
			add
			{
				HandleEvent (WindowStateChangedEvent);
				maximized += value;
			}
			remove { maximized -= value; }
		}
		
		[Obsolete("Use OnStateChanged instead")]
		protected virtual void OnMaximized (EventArgs e)
		{
			if (maximized != null)
				maximized (this, e);
		}

		[Obsolete ("Use WindowState instead")]
		public WindowState State
		{
			get { return Handler.WindowState; }
			set { Handler.WindowState = value; }
		}
		

		#endregion
	}
}
#endif
