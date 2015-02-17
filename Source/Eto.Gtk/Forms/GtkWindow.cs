using System;
using System.ComponentModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;
using Eto.GtkSharp.Forms;
using Eto.GtkSharp.Forms.Menu;

namespace Eto.GtkSharp.Forms
{
	public interface IGtkWindow
	{
		bool CloseWindow(Action<CancelEventArgs> closing = null);

		Gtk.Window Control { get; }
	}

	public abstract class GtkWindow<TControl, TWidget, TCallback> : GtkPanel<TControl, TWidget, TCallback>, Window.IHandler, IGtkWindow
		where TControl: Gtk.Window
		where TWidget: Window
		where TCallback: Window.ICallback
	{
		Gtk.VBox vbox;
		readonly Gtk.VBox actionvbox;
		readonly Gtk.Box topToolbarBox;
		Gtk.Box menuBox;
		Gtk.Box containerBox;
		readonly Gtk.Box bottomToolbarBox;
		MenuBar menuBar;
		Icon icon;
		Eto.Forms.ToolBar toolBar;
		Gtk.AccelGroup accelGroup;
		Rectangle? restoreBounds;
		Point? currentLocation;
		Size? initialSize;
		WindowState state;
		WindowStyle style;
		bool topmost;

		protected GtkWindow()
		{
			vbox = new Gtk.VBox();
			actionvbox = new Gtk.VBox();

			menuBox = new Gtk.HBox();
			topToolbarBox = new Gtk.VBox();

			containerBox = new Gtk.VBox();
			containerBox.Visible = true;

			bottomToolbarBox = new Gtk.VBox();

			actionvbox.PackStart(menuBox, false, false, 0);
			actionvbox.PackStart(topToolbarBox, false, false, 0);
			vbox.PackStart(containerBox, true, true, 0);
			vbox.PackStart(bottomToolbarBox, false, false, 0);
		}

		protected override Color DefaultBackgroundColor
		{
			get { return Control.Style.Background(Gtk.StateType.Normal).ToEto(); }
		}

		protected override bool UseMinimumSizeRequested
		{
			get { return false; }
		}

		public override Size MinimumSize
		{
			get { return base.MinimumSize; }
			set
			{
				base.MinimumSize = value;
				#if GTK2
				Control.AllowShrink = value.Width <= 0 && value.Height <= 0;
				#endif
				if (Widget.Loaded)
				{
					Control.SetSizeRequest(MinimumSize.Width, MinimumSize.Height);
				}
			}
		}

		public Gtk.Widget WindowContentControl
		{
			get { return vbox; }
		}

		public Gtk.Widget WindowActionControl
		{
			get { return actionvbox; }
		}

		public override Gtk.Widget ContainerContentControl
		{
			get { return containerBox; }
		}
		#if GTK2
		public bool Resizable
		{
			get { return Control.Resizable; }
			set { Control.Resizable = value; }
		}
		#else
		public bool Resizable
		{
			get { return Control.Resizable; }
			set { Control.Resizable = Control.HasResizeGrip = value; }
		}
#endif
		public bool Minimizable { get; set; }

		public bool Maximizable { get; set; }

		public bool ShowInTaskbar
		{
			get { return !Control.SkipTaskbarHint; }
			set { Control.SkipTaskbarHint = !value; }
		}

		public bool Topmost
		{
			get { return topmost; }
			set
			{ 
				if (topmost != value)
				{
					topmost = value;
					Control.KeepAbove = topmost;
				}
			}
		}

		public WindowStyle WindowStyle
		{
			get { return style; }
			set
			{ 
				if (style != value)
				{
					style = value;

					switch (style)
					{
						case WindowStyle.Default:
							Control.Decorated = true;
							break;
						case WindowStyle.None:
							Control.Decorated = false;
							break;
						default:
							throw new NotSupportedException();
					}
				}
			}
		}

		public override Size Size
		{
			get
			{
				return Control.GdkWindow != null ? Control.GdkWindow.FrameExtents.Size.ToEto() : initialSize ?? Control.DefaultSize.ToEto();
			}
			set
			{
				if (Control.GdkWindow != null)
				{
					var diff = Control.GdkWindow.FrameExtents.Size.ToEto() - Control.Allocation.Size.ToEto();
					Control.Resize(value.Width - diff.Width, value.Height - diff.Height);
				}
				else
				{
					Control.Resize(value.Width, value.Height);
					initialSize = value;
				}
			}
		}

		void HandleControlShown(object sender, EventArgs e)
		{
			//Control.Shown -= HandleControlShown;
			if (initialSize != null)
			{
				var frameExtents = Control.GdkWindow.FrameExtents.Size.ToEto();
				// HACK: get twice to get 'real' size? Ubuntu 14.04 returns inflated size the first call.
				frameExtents = Control.GdkWindow.FrameExtents.Size.ToEto();

				var diff = frameExtents - Control.Allocation.Size.ToEto();
				Control.Resize(initialSize.Value.Width - diff.Width, initialSize.Value.Height - diff.Height);
				initialSize = null;
			}
			// set initial minimum size
			Control.SetSizeRequest(MinimumSize.Width, MinimumSize.Height);
		}

		public override Size ClientSize
		{
			get
			{
				return containerBox.IsRealized ? containerBox.Allocation.Size.ToEto() : containerBox.SizeRequest().ToEto();
			}
			set
			{
				if (Control.IsRealized)
				{
					var diff = vbox.Allocation.Size.ToEto() - containerBox.Allocation.Size.ToEto();
					Control.Resize(value.Width + diff.Width, value.Height + diff.Height);
				}
				else
				{
					Control.SetSizeRequest(-1, -1);
					containerBox.SetSizeRequest(value.Width, value.Height);
					containerBox.Realized += HandleContainerRealized;
				}
			}
		}

		void HandleContainerRealized(object sender, EventArgs e)
		{
			containerBox.SetSizeRequest(-1, -1);
			containerBox.Realized -= HandleContainerRealized;
		}

		protected override void Initialize()
		{
			base.Initialize();
			
			HandleEvent(Window.WindowStateChangedEvent); // to set restore bounds properly
			HandleEvent(Window.ClosingEvent); // to chain application termination events
			Control.Shown += HandleControlShown;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.KeyDownEvent:
					EventControl.AddEvents((int)Gdk.EventMask.KeyPressMask);
					EventControl.KeyPressEvent += Connector.HandleWindowKeyPressEvent;
					break;
				case Window.ClosedEvent:
					HandleEvent(Window.ClosingEvent);
					break;
				case Window.ClosingEvent:
					Control.DeleteEvent += Connector.HandleDeleteEvent;
					break;
				case Eto.Forms.Control.ShownEvent:
					Control.Shown += Connector.HandleShown;
					break;
				case Window.WindowStateChangedEvent:
					Connector.OldState = WindowState;
					Control.WindowStateEvent += Connector.HandleWindowStateEvent;
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					Control.SizeAllocated += Connector.HandleWindowSizeAllocated;
					break;
				case Window.LocationChangedEvent:
					Control.ConfigureEvent += Connector.HandleConfigureEvent;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new GtkWindowConnector Connector { get { return (GtkWindowConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new GtkWindowConnector();
		}

		protected class GtkWindowConnector : GtkPanelEventConnector
		{
			Size? oldSize;

			public WindowState OldState { get; set; }

			public new GtkWindow<TControl, TWidget, TCallback> Handler { get { return (GtkWindow<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleDeleteEvent(object o, Gtk.DeleteEventArgs args)
			{
				args.RetVal = !Handler.CloseWindow();
			}

			public void HandleShown(object sender, EventArgs e)
			{
				Handler.Callback.OnShown(Handler.Widget, EventArgs.Empty);
			}

			public void HandleWindowStateEvent(object o, Gtk.WindowStateEventArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				var windowState = handler.WindowState;
				if (windowState == WindowState.Normal)
				{
					if ((args.Event.ChangedMask & Gdk.WindowState.Maximized) != 0 && (args.Event.NewWindowState & Gdk.WindowState.Maximized) != 0)
					{
						handler.restoreBounds = handler.Widget.Bounds;
					}
					else if ((args.Event.ChangedMask & Gdk.WindowState.Iconified) != 0 && (args.Event.NewWindowState & Gdk.WindowState.Iconified) != 0)
					{
						handler.restoreBounds = handler.Widget.Bounds;
					}
					else if ((args.Event.ChangedMask & Gdk.WindowState.Fullscreen) != 0 && (args.Event.NewWindowState & Gdk.WindowState.Fullscreen) != 0)
					{
						handler.restoreBounds = handler.Widget.Bounds;
					}
				}

				if (windowState != OldState)
				{
					OldState = windowState;
					Handler.Callback.OnWindowStateChanged(Handler.Widget, EventArgs.Empty);
				}
			}

			// do not connect before, otherwise it is sent before sending to child
			public void HandleWindowKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
			{
				var e = args.Event.ToEto();
				if (e != null)
				{
					Handler.Callback.OnKeyDown(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			public void HandleWindowSizeAllocated(object o, Gtk.SizeAllocatedArgs args)
			{
				var handler = Handler;
				var newSize = handler.Size;
				if (handler.Control.IsRealized && oldSize != newSize)
				{
					Handler.Callback.OnSizeChanged(Handler.Widget, EventArgs.Empty);
					oldSize = newSize;
				}
			}

			Point? oldLocation;

			[GLib.ConnectBefore]
			public void HandleConfigureEvent(object o, Gtk.ConfigureEventArgs args)
			{
				var handler = Handler;
				handler.currentLocation = new Point(args.Event.X, args.Event.Y);
				if (handler.Control.IsRealized && handler.Widget.Loaded && oldLocation != handler.currentLocation)
				{
					handler.Callback.OnLocationChanged(handler.Widget, EventArgs.Empty);
					oldLocation = handler.currentLocation;
				}
				handler.currentLocation = null;
			}
		}

		public MenuBar Menu
		{
			get { return menuBar; }
			set
			{
				if (menuBar != null)
					menuBox.Remove((Gtk.Widget)menuBar.ControlObject);
				if (accelGroup != null)
					Control.RemoveAccelGroup(accelGroup);
				accelGroup = new Gtk.AccelGroup();
				Control.AddAccelGroup(accelGroup);
				// set accelerators
				menuBar = value;
				var handler = menuBar != null ? menuBar.Handler as IMenuHandler : null;
				if (handler != null)
					handler.SetAccelGroup(accelGroup);

				menuBox.PackStart((Gtk.Widget)value.ControlObject, true, true, 0);
				((Gtk.Widget)value.ControlObject).ShowAll();
			}
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			containerBox.PackStart(content, true, true, 0);
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public bool CloseWindow(Action<CancelEventArgs> closing = null)
		{
			var args = new CancelEventArgs();
			Callback.OnClosing(Widget, args);
			var shouldQuit = false;
			if (!args.Cancel)
			{
				if (closing != null)
					closing(args);
				else
				{
					var windows = Gdk.Screen.Default.ToplevelWindows;
					if (windows.Count(r => r.IsViewable) == 1 && ReferenceEquals(windows.First(r => r.IsViewable), Control.GdkWindow))
					{
						var app = ((ApplicationHandler)Application.Instance.Handler);
						app.Callback.OnTerminating(app.Widget, args);
						shouldQuit = !args.Cancel;
					}
				}
			}
			if (!args.Cancel)
			{
				Callback.OnClosed(Widget, EventArgs.Empty);
				if (shouldQuit)
					Gtk.Application.Quit();

			}
			return !args.Cancel;
		}

		public void Close()
		{
			if (CloseWindow())
			{
				Control.Hide();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Control.Destroy();
				if (menuBox != null)
				{
					menuBox.Dispose();
					menuBox = null;
				}
				if (vbox != null)
				{
					vbox.Dispose();
					vbox = null;
				}
				if (containerBox != null)
				{
					containerBox.Dispose();
					containerBox = null;
				}
			}
			base.Dispose(disposing);
		}

		public Eto.Forms.ToolBar ToolBar
		{
			get
			{
				return toolBar;
			}
			set
			{
				if (toolBar != null)
					topToolbarBox.Remove((Gtk.Widget)toolBar.ControlObject);
				toolBar = value;
				if (toolBar != null)
					topToolbarBox.Add((Gtk.Widget)toolBar.ControlObject);
				topToolbarBox.ShowAll();
			}
		}

		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				Control.Icon = ((IconHandler)icon.Handler).Pixbuf;
			}
		}

		public new Point Location
		{
			get
			{
				if (currentLocation != null)
					return currentLocation.Value;
				int x, y;
				Control.GetPosition(out x, out y);
				return new Point(x, y);
			}
			set
			{
				Control.Move(value.X, value.Y);
			}
		}

		public WindowState WindowState
		{
			get
			{
				if (Control.GdkWindow == null)
					return state;	

				if (Control.GdkWindow.State.HasFlag(Gdk.WindowState.Iconified))
					return WindowState.Minimized;
				if (Control.GdkWindow.State.HasFlag(Gdk.WindowState.Maximized))
					return WindowState.Maximized;
				if (Control.GdkWindow.State.HasFlag(Gdk.WindowState.Fullscreen))
					return WindowState.Maximized;
				return WindowState.Normal;
			}
			set
			{
				if (state != value)
				{
					state = value;
				
					switch (value)
					{
						case WindowState.Maximized:
							if (Control.GdkWindow != null)
							{
								if (Control.GdkWindow.State.HasFlag(Gdk.WindowState.Fullscreen))
									Control.Unfullscreen();
								if (Control.GdkWindow.State.HasFlag(Gdk.WindowState.Iconified))
									Control.Deiconify();
							}
							Control.Maximize();
							break;
						case WindowState.Minimized:
							Control.Iconify();
							break;
						case WindowState.Normal:
							if (Control.GdkWindow != null)
							{
								if (Control.GdkWindow.State.HasFlag(Gdk.WindowState.Fullscreen))
									Control.Unfullscreen();
								if (Control.GdkWindow.State.HasFlag(Gdk.WindowState.Maximized))
									Control.Unmaximize();
								if (Control.GdkWindow.State.HasFlag(Gdk.WindowState.Iconified))
									Control.Deiconify();
							}
							break;
					}
				}
			}
		}

		public Rectangle? RestoreBounds
		{
			get
			{
				return WindowState == WindowState.Normal ? null : restoreBounds;
			}
		}

		public double Opacity
		{
			get { return Control.Opacity; }
			set { Control.Opacity = value; }
		}

		Gtk.Window IGtkWindow.Control { get { return Control; } }

		public Screen Screen
		{
			get
			{
				var screen = Control.Screen;
				var gdkWindow = Control.GdkWindow;
				if (screen != null && gdkWindow != null)
				{
					var monitor = screen.GetMonitorAtWindow(gdkWindow);
					return new Screen(new ScreenHandler(screen, monitor));
				}
				return null;
			}
		}

		public void BringToFront()
		{
			Control.Present();
		}

		public void SendToBack()
		{
			if (Control.GdkWindow != null)
				Control.GdkWindow.Lower();
		}
	}
}
