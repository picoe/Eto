using Eto.Wpf.CustomControls;
using Eto.Wpf.Forms.Menu;
using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms
{
	public interface IWpfWindow
	{
		sw.Window Control { get; }

		void SetOwnerFor(sw.Window child);
	}

	public interface IWpfValidateBinding
	{
		void Validate();
	}

	static class WpfWindow
	{
		internal static readonly object MovableByWindowBackground_Key = new object();
		internal static EventInfo dpiChangedEvent = typeof(sw.Window).GetEvent("DpiChanged");
		internal static readonly object LastPixelSize_Key = new object();
		internal static readonly object IsClosing_Key = new object();
		internal static readonly object LocationSet_Key = new object();
		internal static readonly object Minimizable_Key = new object();
		internal static readonly object Maximizable_Key = new object();
		internal static readonly object Closeable_Key = new object();
		internal static readonly object Resizable_Key = new object();
		internal static readonly object Icon_Key = new object();
		internal static readonly object ShowSystemMenu_Key = new object();
	}
	
	public class EtoWindowContent : swc.DockPanel
	{
		
	}

	public abstract class WpfWindow<TControl, TWidget, TCallback> : WpfPanel<TControl, TWidget, TCallback>, Window.IHandler, IWpfWindow, IInputBindingHost
		where TControl : sw.Window
		where TWidget : Window
		where TCallback : Window.ICallback
	{
		MenuBar menu;
		Eto.Forms.ToolBar toolBar;
		swc.DockPanel main;
		swc.ContentControl menuHolder;
		swc.ContentControl toolBarHolder;
		protected swc.DockPanel content;
		Size? initialClientSize;
		PerMonitorDpiHelper dpiHelper;
		Point? initialLocation;
		bool isSourceInitialized;

		protected virtual bool IsAttached => false;

		sw.Interop.WindowInteropHelper windowInterop;
		
		public override IntPtr NativeHandle
		{
			get
			{
				if (windowInterop == null)
				{
					windowInterop = new sw.Interop.WindowInteropHelper(Control);
					windowInterop.EnsureHandle();
				}
				return windowInterop.Handle;
			}
		}
		
		public static bool EnablePerMonitorDpiSupport { get; set; } = true;

		public swc.DockPanel ContentPanel { get { return content; } }

		public swc.DockPanel MainPanel { get { return main; } }

		public bool MovableByWindowBackground
		{
			get => Widget.Properties.Get<bool>(WpfWindow.MovableByWindowBackground_Key);
			set
			{
				if (Widget.Properties.TrySet(WpfWindow.MovableByWindowBackground_Key, value))
				{
					if (value)
						content.MouseLeftButtonDown += Content_MouseLeftButtonDown;
					else
						content.MouseLeftButtonDown -= Content_MouseLeftButtonDown;
				}
			}
		}

		void Content_MouseLeftButtonDown(object sender, swi.MouseButtonEventArgs e)
		{
			// mouse could be captured by something else, so we release it here to ensure the DragMove works.
			// we only get here if no control has handled the mouse down event.
			swi.Mouse.Captured?.ReleaseMouseCapture();

			Control.DragMove();
			e.Handled = true;
		}

		protected override void Initialize()
		{
			if (IsAttached)
				return;
			content = new EtoWindowContent();
			UseShellDropManager = true;

			base.Initialize();
			Control.SizeToContent = sw.SizeToContent.WidthAndHeight;
			Control.SnapsToDevicePixels = true;
			Control.UseLayoutRounding = true;
			main = new swc.DockPanel();
			menuHolder = new swc.ContentControl { IsTabStop = false };
			toolBarHolder = new swc.ContentControl { IsTabStop = false };
			swc.DockPanel.SetDock(menuHolder, swc.Dock.Top);
			swc.DockPanel.SetDock(toolBarHolder, swc.Dock.Top);
			main.Children.Add(menuHolder);
			main.Children.Add(toolBarHolder);
			main.Children.Add(content);
			Control.Content = main;
			Control.SourceInitialized += Control_SourceInitialized;
			Control.Loaded += Control_Loaded;
			Control.PreviewKeyDown += (sender, e) =>
			{
				// need to call validate on the input bindings before trying to execute them
				foreach (var binding in Control.InputBindings.Cast<swi.InputBinding>().Select(r => r.Command).OfType<IWpfValidateBinding>())
				{
					binding.Validate();
				}
			};
			Control.SizeChanged += Control_SizeChanged;
			// needed to handle Application.Terminating event
			HandleEvent(Window.ClosingEvent);
		}

		private void Control_SourceInitialized(object sender, EventArgs e)
		{
			isSourceInitialized = true;

			if (Resizable && WindowStyle == WindowStyle.None)
			{
				SetWindowChrome();
			}
			
			if (!Minimizable || !Maximizable)
			{
				SetResizeMode();
			}
			
			SetSystemMenu();

			if (initialLocation != null)
			{
				SetLocation(initialLocation.Value);
				initialLocation = null;
			}
		}

		private IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 0x0084 /*WM_NCHITTEST*/ )
			{
				// This prevents a crash in WindowChromeWorker._HandleNCHitTest with some mouse drivers
				// See https://github.com/dotnet/wpf/issues/6777 for more information
				// This was fixed in .NET 8 but this is here to avoid the crash in .NET Framework 4.8 and .NET 7.
				try
				{
					lParam.ToInt32();
				}
				catch (OverflowException)
				{
					handled = true;
				}
			}
			return IntPtr.Zero;
		}

		private void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			SetMinimumSize();
			if (initialClientSize != null)
			{
				initialClientSize = null;
				SetContentSize();
			}

			// Set sizing mode - if it is set to manual before here it will be visible before it is loaded
			// which we do not want.
			SetSizeToContent();
			if (Control.ShowActivated)
				Control.MoveFocus(new swi.TraversalRequest(swi.FocusNavigationDirection.Next));

			((swin.HwndSource)sw.PresentationSource.FromVisual(Control))?.AddHook(HookProc);

			if (FireOnLoadComplete)
				Callback.OnLoadComplete(Widget, EventArgs.Empty);
		}

		protected bool FireOnLoadComplete { get; set; }

		private void Control_SizeChanged(object sender, sw.SizeChangedEventArgs e)
		{
			if (Widget.Loaded && Control.SizeToContent == sw.SizeToContent.Manual)
				Widget.Properties.Set(AutoSize_Key, false);
		}

		public bool UseShellDropManager
		{
			get => Widget.Properties.Get<WpfShellDropBehavior>(typeof(WpfShellDropBehavior)) != null;
			set
			{
				if (value != UseShellDropManager)
				{
					if (value)
						Widget.Properties.TrySet(typeof(WpfShellDropBehavior), new WpfShellDropBehavior(Control));
					else
					{
						var shellDrop = Widget.Properties.Get<WpfShellDropBehavior>(typeof(WpfShellDropBehavior));
						shellDrop.Detatch();
						Widget.Properties.Remove(typeof(WpfShellDropBehavior));
					}
				}
			}
		}

		void SetupPerMonitorDpi()
		{
			if (EnablePerMonitorDpiSupport && dpiHelper == null && Win32.PerMonitorDpiSupported)
			{
				dpiHelper = new PerMonitorDpiHelper(Control);

				if (!PerMonitorDpiHelper.BuiltInPerMonitorSupported)
					dpiHelper.ScaleChanged += (sender, e) => SetMinimumSize();
			}
		}

		protected override void SetContentScale(bool xscale, bool yscale)
		{
			base.SetContentScale(true, true);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.ShownEvent:
					Control.IsVisibleChanged += (sender, e) =>
					{
						if ((bool)e.NewValue)
						{
							// this is a trick to achieve similar behaviour as in WinForms
							// (IsVisibleChanged triggers too early, we want it after measure-lay-render)
							Control.Dispatcher.BeginInvoke(new Action(() =>
								Callback.OnShown(Widget, EventArgs.Empty)),
								sw.Threading.DispatcherPriority.ContextIdle, null);
						}
					};
					break;
				case Window.ClosedEvent:
					Control.Closed += delegate
					{
						Callback.OnClosed(Widget, EventArgs.Empty);
					};
					break;
				case Window.ClosingEvent:
					Control.Closing += Control_Closing;
					break;
				case Window.WindowStateChangedEvent:
					Control.StateChanged += (sender, e) => Callback.OnWindowStateChanged(Widget, EventArgs.Empty);
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.Activated += (sender, e) => Callback.OnGotFocus(Widget, EventArgs.Empty);
					break;
				case Eto.Forms.Control.LostFocusEvent:
					// Use AsyncInvoke here otherwise calling Close() during LostFocus does not work as expected
					// and can lead to the z-order of windows not being correct.
					Control.Deactivated += (sender, e) => Application.Instance.AsyncInvoke(() => Callback.OnLostFocus(Widget, EventArgs.Empty));
					break;
				case Window.LocationChangedEvent:
					Control.LocationChanged += (sender, e) => Callback.OnLocationChanged(Widget, EventArgs.Empty);
					break;
				case Window.LogicalPixelSizeChangedEvent:
					if (PerMonitorDpiHelper.BuiltInPerMonitorSupported && WpfWindow.dpiChangedEvent != null) // .NET 4.6.2 support!
					{
						var method = typeof(WpfWindow<TControl, TWidget, TCallback>).GetMethod(nameof(HandleLogicalPixelSizeChanged), BindingFlags.Instance | BindingFlags.NonPublic);
						WpfWindow.dpiChangedEvent.AddEventHandler(Control, Delegate.CreateDelegate(WpfWindow.dpiChangedEvent.EventHandlerType, this, method));
						break;
					}
					SetupPerMonitorDpi();
					if (dpiHelper != null)
						dpiHelper.ScaleChanged += HandleLogicalPixelSizeChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		bool IsClosing
		{
			get => Widget.Properties.Get<bool>(WpfWindow.IsClosing_Key);
			set => Widget.Properties.Set(WpfWindow.IsClosing_Key, value);
		}

		private void Control_Closing(object sender, CancelEventArgs e)
		{
			IsClosing = true;
			var args = new CancelEventArgs { Cancel = e.Cancel };
			Callback.OnClosing(Widget, args);
			var willShutDown =
				(
					sw.Application.Current.ShutdownMode == sw.ShutdownMode.OnLastWindowClose
					&& sw.Application.Current.Windows.Count == 1
				)
				|| (
					sw.Application.Current.ShutdownMode == sw.ShutdownMode.OnMainWindowClose
					&& sw.Application.Current.MainWindow == Control
				);

			if (!args.Cancel && willShutDown)
			{
				// last window closing, so call OnTerminating to let the app abort terminating
				var app = ((ApplicationHandler)Application.Instance.Handler);
				app.Callback.OnTerminating(app.Widget, args);
			}
			e.Cancel = args.Cancel;
			IsApplicationClosing = !e.Cancel && willShutDown;
			IsClosing = !e.Cancel;
			if (!e.Cancel)
			{
				InternalClosing();
			}
		}

		float LastPixelSize
		{
			get => Widget.Properties.Get<float>(WpfWindow.LastPixelSize_Key);
			set => Widget.Properties.Set(WpfWindow.LastPixelSize_Key, value);
		}

		void HandleLogicalPixelSizeChanged(object sender, EventArgs e)
		{
			if (LastPixelSize != LogicalPixelSize)
			{
				Callback.OnLogicalPixelSizeChanged(Widget, EventArgs.Empty);
				LastPixelSize = LogicalPixelSize;
			}
		}

		static bool IsApplicationClosing { get; set; }

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetScale(false, false);
			SetupPerMonitorDpi();
		}

		protected virtual void UpdateClientSize(Size size)
		{
			if (IsAttached)
				return;

			Control.SizeToContent = sw.SizeToContent.Manual;
			var xdiff = Control.ActualWidth - content.ActualWidth;
			var ydiff = Control.ActualHeight - content.ActualHeight;
			Control.Width = size.Width + xdiff;
			Control.Height = size.Height + ydiff;
		}

		protected override void SetSize()
		{
			if (IsAttached)
				return;

			SetSizeToContent();

			var size = UserPreferredSize;

			// don't set the minimum size of a window, just the preferred size
			ContainerControl.Width = size.Width;
			ContainerControl.Height = size.Height;
			SetMinimumSize();
		}

		private void SetSizeToContent()
		{
			sw.SizeToContent sizing;
			if (Control.IsLoaded && !AutoSize)
			{
				sizing = sw.SizeToContent.Manual;
			}
			else if (Control.WindowState == sw.WindowState.Maximized)
			{
				sizing = sw.SizeToContent.Manual;
			}
			else
			{
				var size = UserPreferredSize;
				if (double.IsNaN(size.Width) && double.IsNaN(size.Height))
					sizing = sw.SizeToContent.WidthAndHeight;
				else if (double.IsNaN(size.Width))
					sizing = sw.SizeToContent.Width;
				else if (double.IsNaN(size.Height))
					sizing = sw.SizeToContent.Height;
				else
				{
					Widget.Properties.Set(AutoSize_Key, false);
					sizing = sw.SizeToContent.Manual;
				}
			}

			// If we set it to manual before loaded, it gets shown before Loaded event fires,
			// which we do not want.
			if (sizing == sw.SizeToContent.Manual && !Control.IsLoaded)
				return;

			Control.SizeToContent = sizing;
		}

		static readonly object AutoSize_Key = new object();

		public virtual bool AutoSize
		{
			get => Widget.Properties.Get<bool>(AutoSize_Key);
			set
			{
				if (Widget.Properties.TrySet(AutoSize_Key, value))
				{
					SetSizeToContent();
				}
			}
		}

		void SetMinimumSize()
		{
			if (IsAttached)
				return;

			// can't use WpfScale reliably until it is loaded
			if (!Control.IsLoaded)
				return;
			ContainerControl.MinWidth = MinimumSize.Width * WpfScale;
			ContainerControl.MinHeight = MinimumSize.Height * WpfScale;
		}

		public Eto.Forms.ToolBar ToolBar
		{
			get { return toolBar; }
			set
			{
				if (IsAttached)
					throw new NotSupportedException();

				toolBar = value;
				toolBarHolder.Content = toolBar != null ? toolBar.ControlObject : null;
			}
		}
		
		protected virtual void InternalClosing()
		{
		}

		public void Close()
		{
			if (!IsApplicationClosing)
			{
				// prevent crash if we call this more than once..
				if (!IsClosing)
				{
					Control.Close();
				}
			}
			else
				Visible = false;

		}

		void CopyKeyBindings(swc.ItemCollection items)
		{
			foreach (var item in items.OfType<swc.MenuItem>())
			{
				Control.InputBindings.AddRange(item.InputBindings);
				if (item.HasItems)
					CopyKeyBindings(item.Items);
			}
		}

		public MenuBar Menu
		{
			get { return menu; }
			set
			{
				if (IsAttached)
					throw new NotSupportedException();
				menu = value;
				if (menu != null)
				{
					var handler = (MenuBarHandler)menu.Handler;
					menuHolder.Content = handler.Control;
					Control.InputBindings.AddKeyBindings(handler.Control.Items);
				}
				else
				{
					menuHolder.Content = null;
				}
			}
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			var preferred = UserPreferredSize;
			var available = preferred.IfNaN(availableSize.ToWpf());

			SizeF desired;
			if (!Control.IsLoaded)
			{
				var oldSizeToContent = Control.SizeToContent;
				var oldVisibility = Control.Visibility;
				var _ = NativeHandle; // Ensure SourceInitialized is triggered
				Control.SizeToContent = sw.SizeToContent.WidthAndHeight;
				Control.Visibility = sw.Visibility.Hidden; // Set to hidden (instead of Collapsed)
				Control.Measure(available);
				desired = Control.DesiredSize.ToEto();
				Control.Visibility = oldVisibility;
				Control.SizeToContent = oldSizeToContent;
			}
			else
			{
				Control.Measure(available);
				desired = Control.DesiredSize.ToEto();
			}
			desired = SizeF.Max(MinimumSize, desired);
			if (!double.IsNaN(preferred.Width))
				desired.Width = (float)preferred.Width;
			if (!double.IsNaN(preferred.Height))
				desired.Height = (float)preferred.Height;
			return desired;
		}
		
		public Icon Icon
		{
			get => Widget.Properties.Get<Icon>(WpfWindow.Icon_Key, () => {
				var icon = Control.Icon.ToEtoIcon();
				Widget.Properties.Set(WpfWindow.Icon_Key, icon);
				return icon;
			});
			set
			{
				if (Widget.Properties.TrySet<Icon>(WpfWindow.Icon_Key, value))
				{
					var style = SaveWindowStyle();
					Control.Icon = (swm.ImageSource)value?.ControlObject;
					RestoreWindowStyle(style);
				}
			}
		}

		public virtual bool Resizable
		{
			get => Widget.Properties.Get<bool>(WpfWindow.Resizable_Key, true);
			set
			{
				if (Widget.Properties.TrySet(WpfWindow.Resizable_Key, value, true))
				{
					SetResizeMode();
					SetWindowChrome();
				}
			}
		}

		public virtual bool Maximizable
		{
			get => Widget.Properties.Get<bool>(WpfWindow.Maximizable_Key, true);
			set
			{
				if (Widget.Properties.TrySet(WpfWindow.Maximizable_Key, value, true))
				{
					SetResizeMode();
					SetSystemMenu();
				}
			}
		}

		public virtual bool Minimizable
		{
			get => Widget.Properties.Get<bool>(WpfWindow.Minimizable_Key, true);
			set
			{
				if (Widget.Properties.TrySet(WpfWindow.Minimizable_Key, value, true))
				{
					SetResizeMode();
					SetSystemMenu();
				}
			}
		}
		
		public virtual bool Closeable
		{
			get => Widget.Properties.Get<bool>(WpfWindow.Closeable_Key, true);
			set
			{
				if (Widget.Properties.TrySet(WpfWindow.Closeable_Key, value, true))
					SetSystemMenu();
			}
		}
		
		public bool? ShowSystemMenu
		{
			get => Widget.Properties.Get<bool?>(WpfWindow.ShowSystemMenu_Key);
			set
			{
				if (Widget.Properties.TrySet(WpfWindow.ShowSystemMenu_Key, value))
					SetSystemMenu();
			}
		}
		
		
		void SetSystemMenu()
		{
			if (!isSourceInitialized)
				return;
				
			// hide system menu (and close button) if all commands are disabled
			var useSystemMenu = ShowSystemMenu ?? (Closeable || Minimizable || Maximizable);
			SetStyle(Win32.WS.SYSMENU, useSystemMenu);

			// enable/disable the close button if shown (does not disable Alt+F4)
			var sysMenu = Win32.GetSystemMenu(NativeHandle, false);
			if (sysMenu != IntPtr.Zero)
			{
				var closeFlags = Closeable ? Win32.MF.BYCOMMAND : Win32.MF.GRAYED;
				Win32.EnableMenuItem(sysMenu, Win32.SC.CLOSE, closeFlags);
			}
		}		

		internal void SetStyleEx(Win32.WS_EX style, bool value)
		{
			SetStyleEx(value ? style : 0, value ? 0 : style);
		}
		
		internal void SetStyleEx(Win32.WS_EX styleAdd, Win32.WS_EX styleRemove = 0)
		{
			var styleInt = Win32.GetWindowLong(NativeHandle, Win32.GWL.EXSTYLE);
			styleInt |= (uint)styleAdd;
			styleInt &= (uint)~styleRemove;
			Win32.SetWindowLong(NativeHandle, Win32.GWL.EXSTYLE, styleInt);
		}

		internal void SetStyle(Win32.WS style, bool value)
		{
			SetStyle(value ? style : 0, value ? 0 : style);
		}

		internal void SetStyle(Win32.WS styleAdd, Win32.WS styleRemove = 0)
		{
			var styleInt = Win32.GetWindowLong(NativeHandle, Win32.GWL.STYLE);
			styleInt |= (uint)styleAdd;
			styleInt &= (uint)~styleRemove;
			Win32.SetWindowLong(NativeHandle, Win32.GWL.STYLE, styleInt);
		}

		protected virtual void SetResizeMode()
		{
			if (Resizable)
				Control.ResizeMode = sw.ResizeMode.CanResize;
			else if (Minimizable)
				Control.ResizeMode = sw.ResizeMode.CanMinimize;
			else
				Control.ResizeMode = sw.ResizeMode.NoResize;

			if (isSourceInitialized)
			{
				SetStyle(Win32.WS.MAXIMIZEBOX, Maximizable);
				SetStyle(Win32.WS.MINIMIZEBOX, Minimizable);
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

		public void Minimize()
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
				if (IsAttached)
					throw new NotSupportedException();
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
			if (IsAttached)
				return;

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
			get
			{
				if (Control.IsLoaded && NativeHandle != IntPtr.Zero)
				{
					// WPF doesn't always report the correct size when maximized
					var rect = Win32.ExecuteInDpiAwarenessContext(() => Win32.GetWindowRect(NativeHandle, out var r) ? r : (Win32.RECT?)null);
					if (rect != null)
					{
						var scale = DpiScale;
						return new Size(
							(int)Math.Round(rect.Value.width * scale),
							(int)Math.Round(rect.Value.height * scale)
							);
					}
				}
				return base.Size;
			}
			set
			{
				if (IsAttached)
					throw new NotSupportedException();


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
			set
			{
				var style = SaveWindowStyle();
				Control.Title = value ?? string.Empty;
				RestoreWindowStyle(style);
			}
		}

		protected bool LocationSet
		{
			get { return Widget.Properties.Get<bool>(WpfWindow.LocationSet_Key); }
			set { Widget.Properties.Set(WpfWindow.LocationSet_Key, value); }
		}

		System.Windows.Forms.Screen SwfScreen => Win32.GetScreenFromWindow(NativeHandle);

		double DpiScale
		{
			get
			{
				var source = sw.PresentationSource.FromVisual(Control);
				double scale;
				if (source != null)
				{
					scale = source.CompositionTarget.TransformFromDevice.M22;
					if (Win32.IsSystemDpiAware)
					{
						scale = scale * Win32.SystemDpi / SwfScreen.GetLogicalPixelSize();	
					}
				}
				else scale = 1f / (Widget.Screen ?? Screen.PrimaryScreen).LogicalPixelSize;
				return scale;
			}
		}

		public new Point Location
		{
			get
			{
				if (initialLocation != null)
					return initialLocation.Value;
				var handle = NativeHandle;
				if (handle != IntPtr.Zero)
				{
					// Left/Top doesn't always report correct location when maximized, so use Win32 when we can.
					var rect = Win32.ExecuteInDpiAwarenessContext(() => Win32.GetWindowRect(handle, out var r) ? r : (Win32.RECT?)null);

					if (rect != null)
					{
						var location = new Point(rect.Value.left, rect.Value.top);
						return Point.Round(location.ScreenToLogical(SwfScreen));
					}
				}
				// in WPF, left/top of a window is transformed by the (current) screen dpi, which makes absolutely no sense.
				var left = Control.Left;
				var top = Control.Top;
				if (double.IsNaN(left) || double.IsNaN(top))
					return Point.Empty;
				// convert to screen co-ordinates
				var scale = DpiScale;
				var position = new Point((int)(left / scale), (int)(top / scale));
				// now, convert to "proper" logical co-ordinates, taking all screens into account
				return Point.Round(position.ScreenToLogical(SwfScreen));
			}
			set
			{
				if (IsAttached)
					throw new NotSupportedException();

				if (!isSourceInitialized)
				{
					// set location when the source is initialized and we have a Win32 handle to move about
					// using Left/Top doesn't work (properly) in a per-monitor dpi environment.
					initialLocation = value;
					LocationSet = true;
				}
				else
				{
					LocationSet = true;
					SetLocation(value);
				}
			}
		}

		void SetLocation(PointF location)
		{
			var loc = location.LogicalToScreen();
			Win32.ExecuteInDpiAwarenessContext(() => Win32.SetWindowPos(NativeHandle, IntPtr.Zero, loc.X, loc.Y, 0, 0, Win32.SWP.NOSIZE | Win32.SWP.NOACTIVATE));
		}

		public WindowState WindowState
		{
			get
			{
				switch (Control.WindowState)
				{
					case sw.WindowState.Maximized:
						return WindowState.Maximized;
					case sw.WindowState.Minimized:
						return WindowState.Minimized;
					case sw.WindowState.Normal:
						return WindowState.Normal;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case WindowState.Maximized:
						Control.WindowState = sw.WindowState.Maximized;
						break;
					case WindowState.Minimized:
						Control.WindowState = sw.WindowState.Minimized;
						break;
					case WindowState.Normal:
						Control.WindowState = sw.WindowState.Normal;
						break;
					default:
						throw new NotSupportedException();
				}
				SetSizeToContent();
			}
		}

		public Rectangle RestoreBounds
		{
			get
			{
				if (Control.WindowState == sw.WindowState.Normal || Control.RestoreBounds.IsEmpty)
					return Widget.Bounds;

				var restoreBounds = Control.RestoreBounds.ToEto();
				var scale = DpiScale;
				var position = new Point((int)(restoreBounds.X / scale), (int)(restoreBounds.Y / scale));
				restoreBounds.Location = Point.Truncate(position.ScreenToLogical(SwfScreen));
				return restoreBounds;
			}
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
						GlassHelper.BlurBehindWindow(Control);
						//GlassHelper.ExtendGlassFrame (Control);
						Control.Opacity = value;
					}
					else
					{
						Control.Loaded += delegate
						{
							GlassHelper.BlurBehindWindow(Control);
							//GlassHelper.ExtendGlassFrame (Control);
							Control.Opacity = value;
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
			get { return Control.WindowStyle.ToEto(); }
			set
			{
				if (WindowStyle != value)
				{
					Control.WindowStyle = value.ToWpf();
					SetWindowChrome();
				}
			}
		}
		
		uint? SaveWindowStyle()
		{
			if (!isSourceInitialized)
				return null;
				
			// Annoyingly, WPF gets an AritheticOverflow if the window style has WS_POPUP in it as it treats it as an int
			// So, we remove that style then re-apply it after.
			uint? oldStyle = null;
			var style = Win32.GetWindowLong(NativeHandle, Win32.GWL.STYLE);
			if (style > (uint)Int32.MaxValue)
			{
				// style will overflow, so remove the last bit
				oldStyle = style & (uint)(0x80000000);
				Win32.SetWindowLong(NativeHandle, Win32.GWL.STYLE, style & 0x7FFFFFFF);
			}
			return oldStyle;
		}
		
		void RestoreWindowStyle(uint? oldStyle)
		{
			if (oldStyle == null)
				return;
				
			// reapply the old style bit
			var style = Win32.GetWindowLong(NativeHandle, Win32.GWL.STYLE);
			style |= oldStyle.Value;
			Win32.SetWindowLong(NativeHandle, Win32.GWL.STYLE, style);
		}
		
		void SetWindowChrome()
		{
			if (!isSourceInitialized)
				return;
			var oldStyle = SaveWindowStyle();
			var needsCustomWindowChrome = Resizable && WindowStyle == WindowStyle.None;

			if (needsCustomWindowChrome)
			{
				var windowChrome = new sw.Shell.WindowChrome
				{
					CaptionHeight = 0,
					ResizeBorderThickness = new sw.Thickness(4)
				};
				sw.Shell.WindowChrome.SetWindowChrome(Control, windowChrome);
			}
			else
			{
				Control.ClearValue(sw.Shell.WindowChrome.WindowChromeProperty);
			}
			RestoreWindowStyle(oldStyle);
		}

		public override void Focus()
		{
			if (Control.WindowState == sw.WindowState.Minimized)
				Control.WindowState = sw.WindowState.Normal;
			base.Focus();
		}

		public void BringToFront()
		{
			if (Control.WindowState == sw.WindowState.Minimized)
				Control.WindowState = sw.WindowState.Normal;

			var hWnd = NativeHandle;
			if (hWnd != IntPtr.Zero)
				Win32.SetWindowPos(hWnd, Win32.HWND_TOP, 0, 0, 0, 0, Win32.SWP.NOSIZE | Win32.SWP.NOMOVE | Win32.SWP.NOACTIVATE);
		}

		public void SendToBack()
		{
			if (Topmost)
				return;
			var hWnd = NativeHandle;
			if (hWnd != IntPtr.Zero)
				Win32.SetWindowPos(hWnd, Win32.HWND_BOTTOM, 0, 0, 0, 0, Win32.SWP.NOSIZE | Win32.SWP.NOMOVE | Win32.SWP.NOACTIVATE);
			var window = sw.Application.Current.Windows.OfType<sw.Window>().FirstOrDefault(r => r != Control);
			window?.Focus();
		}

		public override Color BackgroundColor
		{
			get { return content.Background.ToEtoColor(); }
			set { content.Background = value.ToWpfBrush(content.Background); }
		}

		public Screen Screen
		{
			get { return new Screen(new ScreenHandler(Control, SwfScreen)); }
		}

		public swi.InputBindingCollection InputBindings
		{
			get { return Control.InputBindings; }
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			this.content.Children.Add(content);
		}


		public void SetOwnerFor(sw.Window child)
		{
			child.Owner = Control;
		}

		public void SetOwner(Window owner)
		{
			if (owner == null)
			{
				Control.Owner = null;
				return;
			}
			var wpfWindow = owner.Handler as IWpfWindow;
			if (wpfWindow != null)
				wpfWindow.SetOwnerFor(Control);
		}

		public double WpfScale
		{
			get { return dpiHelper?.WpfScale ?? 1f; }
		}

		public float LogicalPixelSize
		{
			get
			{
				var scale = (float)(dpiHelper?.Scale ?? sw.PresentationSource.FromVisual(Control)?.CompositionTarget.TransformToDevice.M11 ?? 1.0);
				// will be zero after the window is closed, but should always be a positive number
				if (scale <= 0)
					return 1f;
				return scale;
			}
		}

		protected override void Dispose(bool disposing)
		{
			// close the window when disposing from Eto explicitly
			if (disposing)
				Close();

			base.Dispose(disposing);
		}

		public override bool Visible
		{
			get => Control.IsVisible;
			set
			{
				if (value)
					Control.Show();
				else
					Control.Hide();
			}
		}
	}
}
