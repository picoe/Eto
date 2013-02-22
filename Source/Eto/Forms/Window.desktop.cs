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

	public enum WindowStyle
	{
		Default,
		None
	}

	public partial interface IWindow : IContainer
	{
		MenuBar Menu { get; set; }

		Icon Icon { get; set; }

		bool Resizable { get; set; }

		bool Maximizable { get; set; }

		bool Minimizable { get; set; }

		bool ShowInTaskbar { get; set; }

		bool TopMost { get; set; }

		WindowState WindowState { get; set; }

		Rectangle? RestoreBounds { get; }

		WindowStyle WindowStyle { get; set; }

		void BringToFront ();

		void SendToBack ();
	}

	public abstract partial class Window : Container
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
			if (this.WindowState == WindowState.Maximized)
				OnMaximized (e);
			if (this.WindowState == WindowState.Minimized)
				OnMinimized (e);
#pragma warning restore 612, 618
		}

		public virtual MenuBar Menu
		{
			get { return handler.Menu; }
			set { handler.Menu = value; }
		}

		public Icon Icon
		{
			get { return handler.Icon; }
			set { handler.Icon = value; }
		}

		public bool Resizable
		{
			get { return handler.Resizable; }
			set { handler.Resizable = value; }
		}

		public bool Maximizable
		{
			get { return handler.Maximizable; }
			set { handler.Maximizable = value; }
		}

		public bool Minimizable
		{
			get { return handler.Minimizable; }
			set { handler.Minimizable = value; }
		}

		public bool ShowInTaskbar
		{
			get { return handler.ShowInTaskbar; }
			set { handler.ShowInTaskbar = value; }
		}

		public bool TopMost
		{
			get { return handler.TopMost; }
			set { handler.TopMost = value; }
		}

		public WindowState WindowState
		{
			get { return handler.WindowState; }
			set { handler.WindowState = value; }
		}

		public Rectangle? RestoreBounds
		{
			get { return handler.RestoreBounds; }
		}

		public void Minimize ()
		{
			handler.WindowState = WindowState.Minimized;
		}

		public void Maximize ()
		{
			handler.WindowState = WindowState.Maximized;
		}

		public WindowStyle WindowStyle
		{
			get { return handler.WindowStyle; }
			set { handler.WindowStyle = value; }
		}

		public void BringToFront ()
		{
			handler.BringToFront ();
		}

		public void SendToBack ()
		{
			handler.SendToBack ();
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
			get { return handler.WindowState; }
			set { handler.WindowState = value; }
		}
		

		#endregion
	}
}
#endif
