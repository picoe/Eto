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

		void BringToFront();

		void SendToBack();
	}

	public abstract partial class Window
	{
		public const string WindowStateChangedEvent = "Window.WindowStateChanged";

		public event EventHandler<EventArgs> WindowStateChanged
		{
			add { Properties.AddHandlerEvent(WindowStateChangedEvent, value); }
			remove { Properties.RemoveEvent(WindowStateChangedEvent, value); }
		}

		protected virtual void OnWindowStateChanged(EventArgs e)
		{
			Properties.TriggerEvent(WindowStateChangedEvent, this, e);
		}

		public virtual MenuBar Menu
		{
			get { return Handler.Menu; }
			set
			{
				var menu = Handler.Menu;
				if (menu != null)
					menu.OnUnLoad(EventArgs.Empty);
				if (value != null && value.AutoTrim)
				{
					value.Items.Trim();
				}
				Handler.Menu = value;
				if (value != null)
					value.OnLoad(EventArgs.Empty);
			}
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

		public void Minimize()
		{
			Handler.WindowState = WindowState.Minimized;
		}

		public void Maximize()
		{
			Handler.WindowState = WindowState.Maximized;
		}

		public WindowStyle WindowStyle
		{
			get { return Handler.WindowStyle; }
			set { Handler.WindowStyle = value; }
		}

		public void BringToFront()
		{
			Handler.BringToFront();
		}

		public void SendToBack()
		{
			Handler.SendToBack();
		}
	}
}

