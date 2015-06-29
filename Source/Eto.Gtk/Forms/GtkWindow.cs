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

	public class GtkShrinkableVBox : Gtk.VBox
	{
		public GtkShrinkableVBox()
		{
		}

		public GtkShrinkableVBox(Gtk.Widget child)
		{
			if (child != null)
				PackStart(child, true, true, 0);
		}

#if GTK3
		protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
		{
			base.OnGetPreferredWidth(out minimum_width, out natural_width);
			minimum_width = 0;
		}

		protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeight(out minimum_height, out natural_height);
			minimum_height = 0;
		}
#endif
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

			containerBox = new GtkShrinkableVBox();
			containerBox.Visible = true;

			bottomToolbarBox = new Gtk.VBox();

			actionvbox.PackStart(menuBox, false, false, 0);
			actionvbox.PackStart(topToolbarBox, false, false, 0);
			vbox.PackStart(containerBox, true, true, 0);
			vbox.PackStart(bottomToolbarBox, false, false, 0);
		}

		protected override Color DefaultBackgroundColor
		{
			get { return Control.GetBackground(); }
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

		public bool Resizable
		{
			get { return Control.Resizable; }
			set
			{
				Control.Resizable = value; 
				#if GTK2
				Control.AllowGrow = value;
				#else
				Control.HasResizeGrip = value;
				#endif
			}
		}

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
				var window = Control.GetWindow();
				return window != null ? window.FrameExtents.Size.ToEto() : initialSize ?? Control.DefaultSize.ToEto();
			}
			set
			{
				var window = Control.GetWindow();
				if (window != null)
				{
					var diff = window.FrameExtents.Size.ToEto() - Control.Allocation.Size.ToEto();
					Control.Resize(value.Width - diff.Width, value.Height - diff.Height);
				}
				else
				{
					Control.Resize(value.Width, value.Height);
					initialSize = value;
				}
			}
		}

		void HandleControlRealized(object sender, EventArgs e)
		{
			var allocation = Control.Allocation.Size;
			var minSize = MinimumSize;

			if (initialSize != null)
			{
				var gdkWindow = Control.GetWindow();
				var frameExtents = gdkWindow.FrameExtents.Size.ToEto();
				// HACK: get twice to get 'real' size? Ubuntu 14.04 returns inflated size the first call.
				frameExtents = gdkWindow.FrameExtents.Size.ToEto();

				var diff = frameExtents - Control.Allocation.Size.ToEto();
				allocation.Width = initialSize.Value.Width - diff.Width;
				allocation.Height = initialSize.Value.Height - diff.Height;
				initialSize = null;
			}

			if (Resizable)
			{
				Control.Resize(allocation.Width, allocation.Height);
			}
			else
			{
				// when not resizable, Control.Resize doesn't work
				minSize.Width = Math.Max(minSize.Width, allocation.Width);
				minSize.Height = Math.Max(minSize.Height, allocation.Height);
			}

			// set initial minimum size
			Control.SetSizeRequest(minSize.Width, minSize.Height);

			containerBox.SetSizeRequest(-1, -1);

			// only do this the first time
			Control.Realized -= HandleControlRealized;
		}

		public override Size ClientSize
		{
			get
			{
				return containerBox.IsRealized ? containerBox.Allocation.Size.ToEto() : containerBox.GetPreferredSize().ToEto();
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
					containerBox.SetSizeRequest(value.Width, value.Height);
				}
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			
			HandleEvent(Window.WindowStateChangedEvent); // to set restore bounds properly
			HandleEvent(Window.ClosingEvent); // to chain application termination events
			HandleEvent(Eto.Forms.Control.SizeChangedEvent); // for RestoreBounds
			HandleEvent(Window.LocationChangedEvent); // for RestoreBounds
			Control.SetSizeRequest(-1, -1);
			Control.Realized += HandleControlRealized;
			#if GTK2
			Control.AllowShrink = false;
			#endif
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
					Control.Shown += Connector.HandleShownEvent;
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

			public void HandleShownEvent(object sender, EventArgs e)
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
					if (args.Event.ChangedMask.HasFlag(Gdk.WindowState.Maximized) && !args.Event.NewWindowState.HasFlag(Gdk.WindowState.Maximized))
					{
						handler.restoreBounds = handler.Widget.Bounds;
					}
					else if (args.Event.ChangedMask.HasFlag(Gdk.WindowState.Iconified) && !args.Event.NewWindowState.HasFlag(Gdk.WindowState.Iconified))
					{
						handler.restoreBounds = handler.Widget.Bounds;
					}
					else if (args.Event.ChangedMask.HasFlag(Gdk.WindowState.Fullscreen) && !args.Event.NewWindowState.HasFlag(Gdk.WindowState.Fullscreen))
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
					handler.Callback.OnSizeChanged(Handler.Widget, EventArgs.Empty);
					if (handler.WindowState == WindowState.Normal)
						handler.restoreBounds = handler.Widget.Bounds;
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
					if (handler.WindowState == WindowState.Normal)
						handler.restoreBounds = handler.Widget.Bounds;
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

				if (value != null)
				{
					menuBox.PackStart((Gtk.Widget)value.ControlObject, true, true, 0);
					((Gtk.Widget)value.ControlObject).ShowAll();
				}
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
					if (windows.Count(r => r.IsViewable) == 1 && ReferenceEquals(windows.First(r => r.IsViewable), Control.GetWindow()))
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

		public virtual void Close()
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
				var gdkWindow = Control.GetWindow();
				if (gdkWindow == null)
					return state;	

				if (gdkWindow.State.HasFlag(Gdk.WindowState.Iconified))
					return WindowState.Minimized;
				if (gdkWindow.State.HasFlag(Gdk.WindowState.Maximized))
					return WindowState.Maximized;
				if (gdkWindow.State.HasFlag(Gdk.WindowState.Fullscreen))
					return WindowState.Maximized;
				return WindowState.Normal;
			}
			set
			{
				if (state != value)
				{
					state = value;
					var gdkWindow = Control.GetWindow();				
					switch (value)
					{
						case WindowState.Maximized:
							if (gdkWindow != null)
							{
								if (gdkWindow.State.HasFlag(Gdk.WindowState.Fullscreen))
									Control.Unfullscreen();
								if (gdkWindow.State.HasFlag(Gdk.WindowState.Iconified))
									Control.Deiconify();
							}
							Control.Maximize();
							break;
						case WindowState.Minimized:
							Control.Iconify();
							break;
						case WindowState.Normal:
							if (gdkWindow != null)
							{
								if (gdkWindow.State.HasFlag(Gdk.WindowState.Fullscreen))
									Control.Unfullscreen();
								if (gdkWindow.State.HasFlag(Gdk.WindowState.Maximized))
									Control.Unmaximize();
								if (gdkWindow.State.HasFlag(Gdk.WindowState.Iconified))
									Control.Deiconify();
							}
							break;
					}
				}
			}
		}

		public Rectangle RestoreBounds
		{
			get
			{
				return WindowState == WindowState.Normal ? Widget.Bounds : restoreBounds ?? Widget.Bounds;
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
				var gdkWindow = Control.GetWindow();
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
			var gdkWindow = Control.GetWindow();
			if (gdkWindow != null)
				gdkWindow.Lower();
		}
	}
}
