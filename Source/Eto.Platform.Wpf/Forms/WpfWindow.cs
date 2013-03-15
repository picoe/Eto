using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swm = System.Windows.Media;
using swc = System.Windows.Controls;
using System.Runtime.InteropServices;
using Eto.Platform.Wpf.CustomControls;
using Eto.Platform.Wpf.Forms.Menu;

namespace Eto.Platform.Wpf.Forms
{
	public interface IWpfWindow
	{
		sw.Window Control { get; }
	}

	public abstract class WpfWindow<T, W> : WpfContainer<T, W>, IWindow, IWpfWindow
		where T : sw.Window
		where W : Window
	{
		Icon icon;
		MenuBar menu;
		ToolBar toolBar;
		swc.DockPanel main;
		swc.ContentControl menuHolder;
		swc.ContentControl toolBarHolder;
		swc.DockPanel content;
		Size? initialClientSize;
		bool resizable = true;
		bool maximizable = true;
		bool minimizable = true;

		public swc.DockPanel Content
		{
			get { return content; }
		}

		protected override void Initialize ()
		{
			base.Initialize ();

			Control.SizeToContent = sw.SizeToContent.WidthAndHeight;
			Control.UseLayoutRounding = true;
			main = new swc.DockPanel ();
			content = new swc.DockPanel ();
			menuHolder = new swc.ContentControl { IsTabStop = false };
			toolBarHolder = new swc.ContentControl { IsTabStop = false };
			content.Background = System.Windows.SystemColors.ControlBrush;
			swc.DockPanel.SetDock (menuHolder, swc.Dock.Top);
			swc.DockPanel.SetDock (toolBarHolder, swc.Dock.Top);
			main.Children.Add (menuHolder);
			main.Children.Add (toolBarHolder);
			main.Children.Add (content);
			Control.Content = main;
			Control.Loaded += delegate {
				if (initialClientSize != null) {
					UpdateClientSize (initialClientSize.Value);
					initialClientSize = null;
				}
			};
			// needed to handle Application.Terminating event
			HandleEvent (Window.ClosingEvent);
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Window.ClosedEvent:
				Control.Closed += delegate {
					Widget.OnClosed (EventArgs.Empty);
				};
				break;
			case Window.ClosingEvent:
				Control.Closing += (sender, e) => {
					Widget.OnClosing (e);
					if (!e.Cancel && sw.Application.Current.Windows.Count == 1) {
						// last window closing, so call OnTerminating to let the app abort terminating
						Application.Instance.OnTerminating (e);
					}
				};
				break;
			case Window.WindowStateChangedEvent:
				Control.StateChanged += (sender, e) => {
					Widget.OnWindowStateChanged (EventArgs.Empty);
				};
				break;
			case Window.GotFocusEvent:
				Control.Activated += (sender, e) => {
					Widget.OnGotFocus (EventArgs.Empty);
				};
				break;
			case Window.LostFocusEvent:
				Control.Deactivated += (sender, e) => {
					Widget.OnLostFocus (EventArgs.Empty);
				};
				break;
			case Window.LocationChangedEvent:
				Control.LocationChanged += (sender, e) => {
					Widget.OnLocationChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		protected virtual void UpdateClientSize (Size size)
		{
			var xdiff = Control.ActualWidth - content.ActualWidth;
			var ydiff = Control.ActualHeight - content.ActualHeight;
			Control.Width = size.Width + xdiff;
			Control.Height = size.Height + ydiff;
			Control.SizeToContent = sw.SizeToContent.Manual;
		}

		public ToolBar ToolBar
		{
			get { return toolBar; }
			set
			{
				toolBar = value;
				if (toolBar != null) {
					toolBarHolder.Content = toolBar.ControlObject;
				}
				else
					toolBarHolder.Content = null;
			}
		}

		public void Close ()
		{
			Control.Close ();
		}

		void CopyKeyBindings (swc.ItemCollection items)
		{
			foreach (var item in items.OfType<swc.MenuItem>()) {
				this.Control.InputBindings.AddRange (item.InputBindings);
				if (item.HasItems)
					CopyKeyBindings (item.Items);
			}
		}

		public MenuBar Menu
		{
			get { return menu; }
			set
			{
				menu = value;
				if (menu != null) {
					var handler = (MenuBarHandler)menu.Handler;
					menuHolder.Content = handler.Control;
					this.Control.InputBindings.Clear ();
					CopyKeyBindings (handler.Control.Items);
				}
				else {
					menuHolder.Content = null;
				}
			}
		}

		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				if (value != null) {
					Control.Icon = (swm.ImageSource)icon.ControlObject;
				}
			}
		}

		public virtual bool Resizable
		{
			get { return resizable; }
			set
			{
				if (resizable != value) {
					resizable = value;
					SetResizeMode ();
				}
			}
		}

		public virtual bool Maximizable
		{
			get { return maximizable; }
			set
			{
				if (maximizable != value) {
					maximizable = value;
					SetResizeMode ();
				}
			}
		}

		public virtual bool Minimizable
		{
			get { return minimizable; }
			set
			{
				if (minimizable != value) {
					minimizable = value;
					SetResizeMode ();
				}
			}
		}

		void SetResizeMode ()
		{
			if (resizable) Control.ResizeMode = sw.ResizeMode.CanResizeWithGrip;
			else if (minimizable) Control.ResizeMode = sw.ResizeMode.CanMinimize;
			else Control.ResizeMode = sw.ResizeMode.NoResize;

			var hwnd = new sw.Interop.WindowInteropHelper (Control).Handle;
			if (hwnd != IntPtr.Zero) {
				var val = Win32.GetWindowLong(hwnd, Win32.GWL.STYLE);
				if (maximizable)
					val |= (uint)Win32.WS.MAXIMIZEBOX;
				else
					val &= ~(uint)Win32.WS.MAXIMIZEBOX;
				if (minimizable)
					val |= (uint)Win32.WS.MINIMIZEBOX;
				else
					val &= ~(uint)Win32.WS.MINIMIZEBOX;
				Win32.SetWindowLong (hwnd, Win32.GWL.STYLE, val);
			}
		}

		public virtual bool ShowInTaskbar
		{
			get { return Control.ShowInTaskbar; }
			set { Control.ShowInTaskbar = value; }
		}

		public virtual bool TopMost
		{
			get { return Control.Topmost; }
			set { Control.Topmost = value; }
		}

		public void Minimize ()
		{
			Control.WindowState = sw.WindowState.Minimized;
		}

		public override Size ClientSize
		{
			get
			{
				if (Control.IsLoaded)
					return new Size ((int)content.ActualWidth, (int)content.ActualHeight);
				else
					return initialClientSize ?? Size.Empty;
			}
			set
			{
				if (Control.IsLoaded)
					UpdateClientSize (value);
				else
					initialClientSize = value;
			}
		}

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				Control.SizeToContent = sw.SizeToContent.Manual;
				base.Size = value;
				if (!Control.IsLoaded)
					initialClientSize = null;
			}
		}

		public override object ContainerObject
		{
			get { return Control; }
		}

		public override void SetLayout (Layout layout)
		{
			content.Children.Clear ();
			content.Children.Add ((sw.UIElement)layout.ControlObject);
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}


		public new Point Location
		{
			get
			{
				return new Point ((int)Control.Left, (int)Control.Top);
			}
			set
			{
				Control.Left = value.X;
				Control.Top = value.Y;
			}
		}

		public WindowState WindowState
		{
			get
			{
				switch (Control.WindowState) {
					case sw.WindowState.Maximized:
						return WindowState.Maximized;
					case sw.WindowState.Minimized:
						return WindowState.Minimized;
					case sw.WindowState.Normal:
						return WindowState.Normal;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
				case WindowState.Maximized:
					Control.WindowState = sw.WindowState.Maximized;
					if (!Control.IsLoaded)
						Control.SizeToContent = sw.SizeToContent.Manual;
					break;
				case WindowState.Minimized:
					Control.WindowState = sw.WindowState.Minimized;
					if (!Control.IsLoaded)
						Control.SizeToContent = sw.SizeToContent.WidthAndHeight;
					break;
				case WindowState.Normal:
					Control.WindowState = sw.WindowState.Normal;
					if (!Control.IsLoaded)
						Control.SizeToContent = sw.SizeToContent.WidthAndHeight;
					break;
				default:
					throw new NotSupportedException ();
				}
			}
		}

		public Rectangle? RestoreBounds
		{
			get { return Control.RestoreBounds.ToEto (); }
		}


		public override Size? MinimumSize
		{
			get
			{
				if (Control.MinWidth > 0 && Control.MinHeight > 0)
					return new Size ((int)Control.MinWidth, (int)Control.MinHeight);
				else
					return null;
			}
			set
			{
				if (value != null) {
					Control.MinWidth = value.Value.Width;
					Control.MinHeight = value.Value.Height;
				}
				else {
					Control.MinHeight = 0;
					Control.MinWidth = 0;
				}
			}
		}

		sw.Window IWpfWindow.Control
		{
			get { return this.Control; }
		}

		public double Opacity
		{
			get { return Control.Opacity; }
			set
			{
				if (value != 1.0) {
					if (Control.IsLoaded) {
						GlassHelper.BlurBehindWindow (Control);
						//GlassHelper.ExtendGlassFrame (Control);
						Control.Opacity = value;
					}
					else {
						Control.Loaded += delegate {
							GlassHelper.BlurBehindWindow (Control);
							//GlassHelper.ExtendGlassFrame (Control);
							Control.Opacity = value;
						};
					}
				}
				else {
					Control.Opacity = value;
				}
			}
		}

		public override bool HasFocus
		{
			get { return Control.IsActive && ((ApplicationHandler)Application.Instance.Handler).IsActive; }
		}

        public WindowStyle WindowStyle
        {
			get { return Control.WindowStyle.ToEto (); }
			set { Control.WindowStyle = value.ToWpf (); }
        }

		public void BringToFront ()
		{
			if (Control.WindowState == sw.WindowState.Minimized)
				Control.WindowState = sw.WindowState.Normal;
			Control.Activate ();
		}

		public void SendToBack ()
		{
			if (TopMost)
				return;
			var hWnd = new sw.Interop.WindowInteropHelper (Control).Handle;
			if (hWnd != IntPtr.Zero)
				Win32.SetWindowPos (hWnd, Win32.HWND_BOTTOM, 0, 0, 0, 0, Win32.SWP.NOSIZE | Win32.SWP.NOMOVE);
			var window = sw.Application.Current.Windows.OfType<sw.Window> ().FirstOrDefault (r => r != Control);
			if (window != null)
				window.Focus ();
		}

		public override Color BackgroundColor
		{
			get
			{
				var brush = Control.Background as System.Windows.Media.SolidColorBrush;
				if (brush != null) return brush.Color.ToEto ();
				else return Colors.Black;
			}
			set
			{
				Control.Background = new swm.SolidColorBrush (value.ToWpf ());
            }
		}


		public Screen Screen
		{
			get { return new Screen (Generator, new ScreenHandler (Control)); }
		}
    }
}
