using System;
using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp
{
	public interface IGtkWindow
	{
		bool CloseWindow ();
	}

	public abstract class GtkWindow<T, W> : GtkContainer<T, W>, IWindow, IGtkWindow
		where T: Gtk.Window
		where W: Window
	{
		protected Gtk.VBox vbox;
		Gtk.Box topToolbarBox;
		Gtk.Box menuBox;
		Gtk.Box containerBox;
		Gtk.Box bottomToolbarBox;
		MenuBar menuBar;
		Icon icon;
		ToolBar toolBar;
		Gtk.AccelGroup accelGroup;
		Rectangle? restoreBounds;
		WindowState state;
		
		public GtkWindow ()
		{
			vbox = new Gtk.VBox ();

			menuBox = new Gtk.HBox ();
			topToolbarBox = new Gtk.VBox ();

			containerBox = new Gtk.VBox ();
			containerBox.Visible = true;

			bottomToolbarBox = new Gtk.VBox ();
		}
		
		public override object ContainerObject {
			get { return containerBox; }
		}

		public bool Resizable {
			get {
				return Control.AllowGrow/* && Control.AllowShrink*/;
			}
			set {
				Control.AllowGrow = value;
				//Control.AllowShrink = value;
			}
		}
		
		public override Size Size {
			get {
				if (Control.Visible)
					return Generator.Convert (Control.Allocation.Size);
				else
					return Generator.Convert (Control.DefaultSize);
			}
			set {
				if (Control.Visible)
					Control.Allocation = new Gdk.Rectangle (Control.Allocation.Location, Generator.Convert (value));
				else
					Control.SetDefaultSize (value.Width, value.Height);
			}
		}

		public override Size ClientSize
		{
			get
			{
				int width, height;
				containerBox.GetSizeRequest (out width, out height);
				return new Size(width, height);
			}
			set
			{
				if (Control.IsRealized) {
					int width, height;
					Control.GetSize (out width, out height);

					var size = new Size(width, height);
					containerBox.GetSizeRequest (out width, out height);
					size -= new Size(width, height);
					size += value;
					Control.Resize (size.Width, size.Height);
				}
				else {
					Control.SetSizeRequest (-1, -1);
					containerBox.SetSizeRequest (value.Width, value.Height);
				}
			}
		}

		public override void Initialize ()
		{
			base.Initialize ();
			vbox.PackStart (menuBox, false, false, 0);
			vbox.PackStart (topToolbarBox, false, false, 0);
			vbox.PackStart (containerBox, true, true, 0);
			vbox.PackStart (bottomToolbarBox, false, false, 0);
			
			Control.WindowStateEvent += delegate(object o, Gtk.WindowStateEventArgs args) {
				if (this.State == WindowState.Normal) {
					if ((args.Event.ChangedMask & Gdk.WindowState.Maximized) != 0 && (args.Event.NewWindowState & Gdk.WindowState.Maximized) != 0) {
						restoreBounds = Widget.Bounds;
					} else if ((args.Event.ChangedMask & Gdk.WindowState.Iconified) != 0 && (args.Event.NewWindowState & Gdk.WindowState.Iconified) != 0) {
						restoreBounds = Widget.Bounds;
					} else if ((args.Event.ChangedMask & Gdk.WindowState.Fullscreen) != 0 && (args.Event.NewWindowState & Gdk.WindowState.Fullscreen) != 0) {
						restoreBounds = Widget.Bounds;
					}
				}
			};
			
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Window.ClosedEvent:
				HandleEvent (Window.ClosingEvent);
				break;
			case Window.ClosingEvent:
				Control.DeleteEvent += delegate(object o, Gtk.DeleteEventArgs args) {
					args.RetVal = !CloseWindow ();
				};
				break;
			case Window.ShownEvent:
				Control.Shown += delegate {
					Widget.OnShown (EventArgs.Empty);
				};
				break;
			case Window.HiddenEvent:
				Control.Hidden += delegate {
					Widget.OnHidden (EventArgs.Empty);
				};
				break;
			case Window.MaximizedEvent:
				Control.WindowStateEvent += delegate(object o, Gtk.WindowStateEventArgs args) {
					if ((args.Event.ChangedMask & (Gdk.WindowState.Maximized | Gdk.WindowState.Fullscreen)) != 0 
						&&
						(args.Event.NewWindowState & (Gdk.WindowState.Maximized | Gdk.WindowState.Fullscreen)) != 0) {
						Widget.OnMaximized (EventArgs.Empty);
					}
				};
				break;
			case Window.MinimizedEvent:
				Control.WindowStateEvent += delegate(object o, Gtk.WindowStateEventArgs args) {
					if ((args.Event.ChangedMask & Gdk.WindowState.Iconified) != 0 
						&&
						(args.Event.NewWindowState & Gdk.WindowState.Iconified) != 0) {
						Widget.OnMinimized (EventArgs.Empty);
					}
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public MenuBar Menu {
			get {
				return menuBar;
			}
			set {
				if (menuBar != null)
					menuBox.Remove ((Gtk.Widget)menuBar.ControlObject);
				if (accelGroup != null)
					Control.RemoveAccelGroup (accelGroup);
				accelGroup = new Gtk.AccelGroup ();
				Control.AddAccelGroup (accelGroup);
				// set accelerators
				menuBar = value;
				SetAccelerators (menuBar);
				menuBox.PackStart ((Gtk.Widget)value.ControlObject); //, false, false, 0);
				((Gtk.Widget)value.ControlObject).ShowAll ();
			}
		}
		
		void SetAccelerators (ISubMenuWidget item)
		{
			if (item != null && item.MenuItems != null)
				foreach (var child in item.MenuItems) {
					var actionItem = child as MenuActionItem;
					if (actionItem != null && actionItem.Shortcut != Key.None) {
						var widget = actionItem.ControlObject as Gtk.Widget;
						var key = new Gtk.AccelKey (KeyMap.ConvertToKey (actionItem.Shortcut), KeyMap.ConvertToModifier (actionItem.Shortcut), Gtk.AccelFlags.Visible | Gtk.AccelFlags.Locked);
						widget.AddAccelerator ("activate", accelGroup, key);
					}
					SetAccelerators (child as ISubMenuWidget);
				}
			
		}

		public override void SetLayout (Layout inner)
		{
			IGtkLayout gtklayout = (IGtkLayout)inner.Handler;
			if (containerBox.Children.Length > 0)
				foreach (Gtk.Widget child in containerBox.Children)
					containerBox.Remove (child);
			var containerWidget = (Gtk.Widget)gtklayout.ContainerObject;
			containerBox.PackStart (containerWidget, true, true, 0);
			containerWidget.ShowAll ();
		}

		public string Title {
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public bool CloseWindow ()
		{
			var cancelArgs = new CancelEventArgs ();
			Widget.OnClosing (cancelArgs);
			if (!cancelArgs.Cancel) {
				Widget.OnClosed (EventArgs.Empty);
			}
			return !cancelArgs.Cancel;
		}

		public void Close ()
		{
			if (CloseWindow ()) {
				Control.Hide ();
			}
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				Control.Destroy ();
				if (menuBox != null) {
					menuBox.Dispose ();
					menuBox = null;
				}
				if (vbox != null) {
					vbox.Dispose ();
					vbox = null;
				}
				if (containerBox != null) {
					containerBox.Dispose ();
					containerBox = null;
				}
			}
			base.Dispose (disposing);
		}

		public ToolBar ToolBar {
			get {
				return toolBar;
			}
			set {
				if (toolBar != null)
					topToolbarBox.Remove ((Gtk.Widget)toolBar.ControlObject);
				toolBar = value;
				if (toolBar != null)
					topToolbarBox.Add ((Gtk.Widget)toolBar.ControlObject);
				topToolbarBox.ShowAll ();
			}
		}

		public Icon Icon {
			get { return icon; }
			set {
				icon = value;
				Control.Icon = ((IconHandler)icon.Handler).Pixbuf;
			}
		}
		
		public override Point Location {
			get {
				int x, y;
				Control.GetPosition (out x, out y);
				return new Point (x, y);
			}
			set {
				Control.Move (value.X, value.Y);
			}
		}
		
		public WindowState State {
			get {
				if (Control.GdkWindow == null)
					return state;	

				switch (Control.GdkWindow.State & (Gdk.WindowState.Maximized | Gdk.WindowState.Iconified | Gdk.WindowState.Fullscreen)) {
				case Gdk.WindowState.Maximized:
				case Gdk.WindowState.Fullscreen:
					return WindowState.Maximized;
				case Gdk.WindowState.Iconified:
					return WindowState.Minimized;
				default:
					return WindowState.Normal;
				}
			}
			set {
				if (state != value) {
					state = value;
				
					switch (value) {
					case WindowState.Maximized:
						Control.Maximize ();
						break;
					case WindowState.Minimized:
						Control.Iconify ();
						break;
					case WindowState.Normal:
						if (Control.GdkWindow != null) {
							switch (Control.GdkWindow.State & (Gdk.WindowState.Maximized | Gdk.WindowState.Iconified | Gdk.WindowState.Fullscreen)) {
							case Gdk.WindowState.Maximized:
								Control.Unmaximize ();
								break;
							case Gdk.WindowState.Fullscreen:
								Control.Unfullscreen ();
								break;
							case Gdk.WindowState.Iconified:
								Control.Deiconify ();
								break;
							}
						}
						break;
					}
				}
			}
		}
		
		public Rectangle? RestoreBounds {
			get {
				return State == WindowState.Normal ? null : restoreBounds;
			}
		}

		public double Opacity
		{
			get { return Control.Opacity; }
			set { Control.Opacity = value; }
		}


        public void RemoveBorder()
        {
            throw new NotImplementedException();
        }


        public void BringToFront()
        {
            // TODO
        }
    }
}
