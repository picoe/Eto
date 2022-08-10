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

		Size UserPreferredSize { get; }
	}

	public class GtkShrinkableVBox : Gtk.VBox
	{
		public bool Resizable { get; set; }

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
			if (Resizable)
				minimum_width = 0;
		}

		protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeight(out minimum_height, out natural_height);

			if (Resizable)
				minimum_height = 0;
		}

#if GTKCORE
		protected override void OnGetPreferredHeightAndBaselineForWidth(int width, out int minimum_height, out int natural_height, out int minimum_baseline, out int natural_baseline)
		{
			base.OnGetPreferredHeightAndBaselineForWidth(width, out minimum_height, out natural_height, out minimum_baseline, out natural_baseline);
			if (Resizable)
				minimum_height = 0;
		}
#endif
#endif
	}

	static class GtkWindow
	{
		internal static readonly object MovableByWindowBackground_Key = new object();
		internal static readonly object DisableAutoSizeUpdate_Key = new object();
		internal static readonly object AutoSizePerformed_Key = new object();
		internal static readonly object AutoSize_Key = new object();
		internal static readonly object Minimizable_Key = new object();
		internal static readonly object Maximizable_Key = new object();
	}

	public abstract class GtkWindow<TControl, TWidget, TCallback> : GtkPanel<TControl, TWidget, TCallback>, Window.IHandler, IGtkWindow
		where TControl : Gtk.Window
		where TWidget : Window
		where TCallback : Window.ICallback
	{
		Gtk.VBox vbox;
		readonly Gtk.VBox actionvbox;
		readonly Gtk.Box topToolbarBox;
		Gtk.Box menuBox;
		GtkShrinkableVBox containerBox;
		readonly Gtk.Box bottomToolbarBox;
		MenuBar menuBar;
		Icon icon;
		Eto.Forms.ToolBar toolBar;
		Gtk.AccelGroup accelGroup;
		Rectangle? restoreBounds;
		Point? currentLocation;
		Size minimumSize;
		WindowState state;
		WindowStyle style;
		bool topmost;
		bool resizable;
		Size? clientSize;

		protected GtkWindow()
		{
			vbox = new Gtk.VBox();
			actionvbox = new Gtk.VBox();

			menuBox = new Gtk.HBox();
			topToolbarBox = new Gtk.VBox();

			containerBox = new GtkShrinkableVBox();
			containerBox.Resizable = true;
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
			get { return minimumSize; }
			set
			{
				minimumSize = value;
				SetMinMax(null);
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
			get { return resizable; }
			set
			{
				containerBox.Resizable = value;
				resizable = value;
#if GTK3
				Control.Resizable = value;
#endif
				SetMinMax(null);
			}
		}

		private void SetMinMax(Size? size)
		{
			var geom = new Gdk.Geometry();
			geom.MinWidth = minimumSize.Width;
			geom.MinHeight = minimumSize.Height;

#if GTK2
			if (!resizable)
			{
				if (size != null)
				{
					geom.MinWidth = geom.MaxWidth = size.Value.Width;
					geom.MinHeight = geom.MaxHeight = size.Value.Height;
				}
				else if (Control.IsRealized)
				{
					geom.MinWidth = geom.MaxWidth = Control.Allocation.Width;
					geom.MinHeight = geom.MaxHeight = Control.Allocation.Height;
				}
				else
				{
					geom.MinWidth = geom.MaxWidth = Control.DefaultWidth;
					geom.MinHeight = geom.MaxHeight = Control.DefaultHeight;
				}
			}
#endif
			Control.SetGeometryHints(Control, geom, Gdk.WindowHints.MinSize);
		}
		

		public bool Minimizable
		{
			get => Widget.Properties.Get<bool>(GtkWindow.Minimizable_Key, true);
			set
			{
				if (Widget.Properties.TrySet(GtkWindow.Minimizable_Key, value, true))
					SetTypeHint();
			}
		}

		public bool Maximizable
		{
			get => Widget.Properties.Get<bool>(GtkWindow.Maximizable_Key, true);
			set
			{
				if (Widget.Properties.TrySet(GtkWindow.Maximizable_Key, value, true))
					SetTypeHint();
			}
		}

		public bool ShowInTaskbar
		{
			get { return !Control.SkipTaskbarHint; }
			set { Control.SkipTaskbarHint = !value; }
		}
		
		public bool Closeable
		{
			get => Control.Deletable;
			set => Control.Deletable = value;
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
						case WindowStyle.Utility:
							Control.Decorated = true;
							break;
						default:
							throw new NotSupportedException();
					}
					SetTypeHint();
				}
			}
		}
		
		protected virtual Gdk.WindowTypeHint DefaultTypeHint => Gdk.WindowTypeHint.Normal;
		
		void SetTypeHint()
		{
			if (WindowStyle == WindowStyle.Default && (Minimizable || Maximizable))
			{
				Control.TypeHint = DefaultTypeHint;
			}
			else
			{
				Control.TypeHint = Gdk.WindowTypeHint.Utility;
			}
		}

		public override Size Size
		{
			get => Widget.Loaded ? Control.GetWindow()?.FrameExtents.Size.ToEto() ?? UserPreferredSize : UserPreferredSize;
			set
			{
				DisableAutoSizeUpdate++;
				UserPreferredSize = value;
					
				if (Widget.Loaded)
				{
					var diff = WindowDecorationSize;
					Control.Resize(value.Width - diff.Width, value.Height - diff.Height);
				}
				else
				{
					clientSize = null;
					// this doesn't take into account window decoration size
					// there doesn't seem to be an (easy) way to do that currently..
					Control.SetDefaultSize(value.Width, value.Height);
				}
				SetMinMax(value);
				DisableAutoSizeUpdate--;
			}
		}

		public override Size ClientSize
		{
			get
			{
				return containerBox.IsRealized ? containerBox.Allocation.Size.ToEto() : containerBox.GetPreferredSize().ToEto();
			}
			set
			{
				DisableAutoSizeUpdate++;
				if (Control.IsRealized)
				{
					var diff = vbox.Allocation.Size.ToEto() - containerBox.Allocation.Size.ToEto();
					Control.Resize(value.Width + diff.Width, value.Height + diff.Height);
					SetMinMax(value + diff);
				}
				else
				{
					clientSize = value;
					Control.SetDefaultSize(value.Width, value.Height);
					SetMinMax(value);
				}
				DisableAutoSizeUpdate--;
			}
		}
		public bool MovableByWindowBackground
		{
			get => Widget.Properties.Get<bool>(GtkWindow.MovableByWindowBackground_Key);
			set
			{
				if (Widget.Properties.TrySet(GtkWindow.MovableByWindowBackground_Key, value))
				{
					if (value)
						Control.ButtonPressEvent += Connector.ButtonPressEvent_Movable;
					else
						Control.ButtonPressEvent -= Connector.ButtonPressEvent_Movable;
				}
			}
		}

		private void Control_Realized(object sender, EventArgs e)
		{
			Control.Realized -= Control_Realized;
			Size size;
			if (clientSize.HasValue)
			{
				size = clientSize.Value;
			}
			else
			{
				size = UserPreferredSize - WindowDecorationSize;
			}

			if (size.Width > 0 || size.Height > 0)
			{
				var allocated = Control.Allocation.Size;
				if (size.Width < 0)
					size.Width = allocated.Width;
				if (size.Height < 0)
					size.Height = allocated.Height;
				Control.Resize(size.Width, size.Height);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			DisableAutoSizeUpdate++;
			HandleEvent(Window.WindowStateChangedEvent); // to set restore bounds properly
			HandleEvent(Window.ClosingEvent); // to chain application termination events
			HandleEvent(Eto.Forms.Control.SizeChangedEvent); // for RestoreBounds
			HandleEvent(Window.LocationChangedEvent); // for RestoreBounds
			Control.SetSizeRequest(-1, -1);
			Control.Realized += Connector.Control_Realized;

			ApplicationHandler.Instance.RegisterIsActiveChanged(Control);
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			DisableAutoSizeUpdate--;
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
				case Window.LogicalPixelSizeChangedEvent:
					// not supported on GTK, yet.
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
				var handler = Handler;
				if (handler == null)
					return;
				args.RetVal = !handler.CloseWindow();
			}

			public void HandleShownEvent(object sender, EventArgs e)
			{
				Handler?.Callback.OnShown(Handler.Widget, EventArgs.Empty);
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
				var handler = Handler;
				if (handler == null)
					return;
				var e = args.Event.ToEto();
				if (e != null)
				{
					handler.Callback.OnKeyDown(handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			public void HandleWindowSizeAllocated(object o, Gtk.SizeAllocatedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				var newSize = handler.Control.Allocation.Size.ToEto();
				if (handler.Control.IsRealized && oldSize != newSize)
				{
					handler.Callback.OnSizeChanged(Handler.Widget, EventArgs.Empty);

					// annoyingly, there's no way to tell if the user is resizing things
					if (handler.AutoSizePerformed == 0 && handler.DisableAutoSizeUpdate == 0)
						handler.AutoSize = false;

					if (handler.WindowState == WindowState.Normal)
						handler.restoreBounds = handler.Widget.Bounds;
					oldSize = newSize;
				}
				if (handler.AutoSizePerformed > 0)
					handler.AutoSizePerformed--;
			}

			Point? oldLocation;

			[GLib.ConnectBefore]
			public void HandleConfigureEvent(object o, Gtk.ConfigureEventArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
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

			internal void Control_Realized(object sender, EventArgs e) => Handler?.Control_Realized(sender, e);

			internal void ButtonPressEvent_Movable(object o, Gtk.ButtonPressEventArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				var evt = args.Event;
				if (handler != null && evt.Type == Gdk.EventType.ButtonPress && evt.Button == 1)
				{
					handler.Control.BeginMoveDrag((int)evt.Button, (int)evt.XRoot, (int)evt.YRoot, evt.Time);
				}
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
					var windows = Gdk.Screen.Default.ToplevelWindows.Where(r => r.State != Gdk.WindowState.Withdrawn && r.WindowType != Gdk.WindowType.Temp).ToList();
					if (windows.Count == 1 && ReferenceEquals(windows[0], Control.GetWindow()))
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
			if (Widget.Loaded && CloseWindow())
			{
				Control.Hide();
				Control.Unrealize();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
#if !GTKCORE
				Control.Destroy();
#endif
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
				if (WindowState != value)
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
#if GTKCORE
					var monitor = screen.Display.GetMonitorAtWindow(gdkWindow);
					return new Screen(new ScreenHandler(monitor));
#else
					var monitor = screen.GetMonitorAtWindow(gdkWindow);
					return new Screen(new ScreenHandler(screen, monitor));
#endif
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

		public virtual void SetOwner(Window owner)
		{
			Control.TransientFor = owner.ToGtk();
		}

		public override bool Visible
		{
			get { return base.Visible; }
			set
			{
				base.Visible = value;
				Control.NoShowAll = false; // always show all.
			}
		}

		public float LogicalPixelSize
		{
			get
			{
#if GTKCORE
				var screen = Control.Screen;
				var gdkWindow = Control.GetWindow();
				if (screen != null && gdkWindow != null)
				{
					var monitor = screen.Display.GetMonitorAtWindow(gdkWindow);
					return monitor?.ScaleFactor ?? 1f;
				}
#endif
				return 1f;
			}
		}

		internal int DisableAutoSizeUpdate
		{
			get => Widget.Properties.Get<int>(GtkWindow.DisableAutoSizeUpdate_Key);
			set => Widget.Properties.Set(GtkWindow.DisableAutoSizeUpdate_Key, value);
		}
		internal int AutoSizePerformed
		{
			get => Widget.Properties.Get<int>(GtkWindow.AutoSizePerformed_Key);
			set => Widget.Properties.Set(GtkWindow.AutoSizePerformed_Key, value);
		}

		public bool AutoSize
		{
			get => Widget.Properties.Get<bool>(GtkWindow.AutoSize_Key) || !Resizable; // gtk always auto sizes when the window is not resizable
			set
			{
				if (Widget.Properties.TrySet(GtkWindow.AutoSize_Key, value))
				{
					InvalidateMeasure();
				}
			}
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			var size = base.GetPreferredSize(availableSize);
			return size + WindowDecorationSize;
		}

		Size WindowDecorationSize
		{
			get
			{
				var window = Control.GetWindow();
				if (window == null)
					return Size.Empty;
				return window.FrameExtents.Size.ToEto() - Control.Allocation.Size.ToEto();
			}
		}

		bool isInvalidated;

		internal void PerformResize()
		{
			Control.GetSize(out var width, out var height);
			var availableSize = Screen?.WorkingArea.Size ?? SizeF.PositiveInfinity;
			var preferred = Size.Round(SizeF.Min(base.GetPreferredSize(availableSize), availableSize));
			if (preferred.Width != width || preferred.Height != height)
			{
				// signal that we are auto sizing ourselves so don't turn off AutoSize.
				AutoSizePerformed++;
				Control.Resize(preferred.Width, preferred.Height);
			}
			isInvalidated = false;
			DisableAutoSizeUpdate--;
		}

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			if (!isInvalidated && Resizable && Widget.Loaded && !Widget.IsSuspended && AutoSize && DisableAutoSizeUpdate == 0 && AutoSizePerformed == 0)
			{
				isInvalidated = true;
				DisableAutoSizeUpdate++;
				Application.Instance.AsyncInvoke(PerformResize);
			}
		}
	}
}
