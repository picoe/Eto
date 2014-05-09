#if TODO_XAML

using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using sw = Windows.UI.Xaml;
using swm = Windows.UI.Xaml.Media;
using swc = Windows.UI.Xaml.Controls;
using swi = Windows.UI.Xaml.Input;
using Eto.WinRT.CustomControls;
//using Eto.WinRT.Forms.Menu;

namespace Eto.WinRT.Forms
{
	public interface IWpfWindow
	{
		sw.Window Control { get; }
	}

	/// <summary>
	/// Window handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class WpfWindow<TControl, TWidget> : WpfPanel<TControl, TWidget>, IWindow, IWpfWindow
		where TControl : sw.Window
		where TWidget : Window
	{
		Icon icon;
		MenuBar menu;
		ToolBar toolBar;
#if TODO_XAML
		swc.DockPanel main;
#endif
		swc.ContentControl menuHolder;
		swc.ContentControl toolBarHolder;
#if TODO_XAML
		swc.DockPanel content;
#else
		swc.Panel content;
#endif
		Size? initialClientSize;
		bool resizable = true;
		bool maximizable = true;
		bool minimizable = true;

		protected override void Initialize ()
		{
#if TODO_XAML
			content = new swc.DockPanel();
#else 
			content = new Panel();
#endif			
			base.Initialize();
			Control.SizeToContent = sw.SizeToContent.WidthAndHeight;
			Control.SnapsToDevicePixels = true;
			Control.UseLayoutRounding = true;
#if TODO_XAML
			main = new swc.DockPanel();
#endif
			menuHolder = new swc.ContentControl { IsTabStop = false };
			toolBarHolder = new swc.ContentControl { IsTabStop = false };
			content.Background = Windows.UI.Xaml.SystemColors.ControlBrush;
			swc.DockPanel.SetDock (menuHolder, swc.Dock.Top);
			swc.DockPanel.SetDock (toolBarHolder, swc.Dock.Top);
#if TODO_XAML
			main.Children.Add (menuHolder);
			main.Children.Add (toolBarHolder);
			main.Children.Add (content);
			Control.Content = main;
#endif
			Control.Loaded += delegate
			{
				SetResizeMode();
				if (initialClientSize != null)
				{
					initialClientSize = null;
					SetContentSize();
				}
				// stop form from auto-sizing after it is shown
				Control.SizeToContent = sw.SizeToContent.Manual;
				Control.MoveFocus(new swi.TraversalRequest(swi.FocusNavigationDirection.Next));
			};
			// needed to handle Application.Terminating event
			HandleEvent (Window.ClosingEvent);
		}

		protected override void SetContentScale(bool xscale, bool yscale)
		{
			base.SetContentScale(true, true);
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
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
				Control.StateChanged += (sender, e) => Widget.OnWindowStateChanged(EventArgs.Empty);
				break;
			case Eto.Forms.Control.GotFocusEvent:
				Control.Activated += (sender, e) => Widget.OnGotFocus(EventArgs.Empty);
				break;
			case Eto.Forms.Control.LostFocusEvent:
				Control.Deactivated += (sender, e) => Widget.OnLostFocus(EventArgs.Empty);
				break;
			case Window.LocationChangedEvent:
				Control.LocationChanged += (sender, e) => Widget.OnLocationChanged(EventArgs.Empty);
				break;
			default:
				base.AttachEvent (id);
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

		protected override void SetSize()
		{
			// don't set the minimum size of a window, just the preferred size
			ContainerControl.Width = PreferredSize.Width;
			ContainerControl.Height = PreferredSize.Height;
			ContainerControl.MinWidth = MinimumSize.Width;
			ContainerControl.MinHeight = MinimumSize.Height;
		}

		public ToolBar ToolBar
		{
			get { return toolBar; }
			set
			{
				toolBar = value;
				toolBarHolder.Content = toolBar != null ? toolBar.ControlObject : null;
			}
		}

		public void Close ()
		{
			Control.Close ();
		}

		void CopyKeyBindings (swc.ItemCollection items)
		{
			foreach (var item in items.OfType<swc.MenuItem>()) {
				Control.InputBindings.AddRange (item.InputBindings);
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
					Control.InputBindings.Clear ();
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

		protected virtual void SetResizeMode ()
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

		public virtual bool Topmost
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
					return new Size((int)content.ActualWidth, (int)content.ActualHeight);
				return initialClientSize ?? Size.Empty;
			}
			set
			{
				if (Control.IsLoaded)
					UpdateClientSize(value);
				else
				{
					initialClientSize = value;
					SetContentSize();
				}
			}
		}

		void SetContentSize()
		{
			if (initialClientSize != null)
			{
				var value = initialClientSize.Value;
				content.MinWidth = value.Width >= 0 ? value.Width : 0;
				content.MinHeight = value.Height >= 0 ? value.Height : 0;
				content.MaxWidth = value.Width >= 0 ? value.Width : double.PositiveInfinity;
				content.MaxHeight = value.Height >= 0 ? value.Height : double.PositiveInfinity;
			}
			else
			{
				content.MinWidth = content.MinHeight = 0;
				content.MaxWidth = content.MaxHeight = double.PositiveInfinity;
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
				{
					initialClientSize = null;
					SetContentSize();
				}
			}
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

		sw.Window IWpfWindow.Control
		{
			get { return Control; }
		}

		public double Opacity
		{
			get { return Control.Opacity; }
			set
			{
				if (Math.Abs(value - 1.0) > 0.01f)
				{
					if (Control.IsLoaded)
					{
#if TODO_XAML
						GlassHelper.BlurBehindWindow(Control);
						//GlassHelper.ExtendGlassFrame (Control);
						Control.Opacity = value;
#endif
					}
					else
					{
						Control.Loaded += delegate
						{
#if TODO_XAML
							GlassHelper.BlurBehindWindow(Control);
							//GlassHelper.ExtendGlassFrame (Control);
							Control.Opacity = value;
#endif
						};
					}
				}
				else
				{
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
			if (Topmost)
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
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public Screen Screen
		{
			get { return new Screen (Generator, new ScreenHandler (Control)); }
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			this.content.Children.Add(content);
		}
    }
}
#endif