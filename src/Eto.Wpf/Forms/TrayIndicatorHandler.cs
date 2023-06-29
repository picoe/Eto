using System.Windows;
using Eto.Wpf.Forms.Menu;
using Point = System.Windows.Point;

namespace Eto.Wpf.Forms
{

	public class TrayIndicatorHandler : WidgetHandler<swf.NotifyIcon, TrayIndicator, TrayIndicator.ICallback>,
		TrayIndicator.IHandler
	{
		private Image _image;
		private IntPtr _keyboardHookHandle;
		private Win32.HookProc _keyboardHookProcRef;
		private IntPtr _mouseHookHandle;
		private Win32.HookProc _mouseHookProcRef;

		public TrayIndicatorHandler()
		{
			Control = new swf.NotifyIcon();
			Control.MouseClick += Control_MouseClick;
			InitializeNativeHooks();
		}

		public string Title
		{
			get => Control.Text;
			set
			{
				if (value.Length >= 128)
					throw new ArgumentOutOfRangeException("Text limited to 127 characters");

				// increase size limitation from 63 to 127 characters
				// see https://stackoverflow.com/questions/579665/how-can-i-show-a-systray-tooltip-longer-than-63-chars
				var t = typeof(swf.NotifyIcon);
				var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
				var textField = t.GetField("text", bindingFlags);
				textField?.SetValue(Control, value);
				var addedField = t.GetField("added", bindingFlags);
				if (addedField != null && (bool)addedField.GetValue(Control))
				{
					t.GetMethod("UpdateIcon", bindingFlags)?.Invoke(Control, new object[] { true });
				}
			}
		}

		  public bool Visible { get => Control.Visible; set => Control.Visible = value; }

		public Image Image
		{
			get => _image;
			set
			{
				_image = value;
				Control.Icon = _image.ToSDIcon();
			}
		}

		public ContextMenu Menu { get; set; }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TrayIndicator.ActivatedEvent:
					Control.MouseClick += (sender, e) =>
					{
						if (e.Button.HasFlag(swf.MouseButtons.Left))
						{
							Callback.OnActivated(Widget, EventArgs.Empty);
						}
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void ContextMenuClosed(object sender, RoutedEventArgs e)
		{
			if (_mouseHookHandle != IntPtr.Zero)
			{
				Win32.UnhookWindowsHookEx(_mouseHookHandle);
			}

			if (_keyboardHookHandle != IntPtr.Zero)
			{
				Win32.UnhookWindowsHookEx(_keyboardHookHandle);
			}

			var menu = ContextMenuHandler.GetControl(Menu);
			menu.Opened -= ContextMenuOpened;
			menu.Closed -= ContextMenuClosed;
		}

		private void ContextMenuOpened(object sender, RoutedEventArgs e)
		{
			InstallHooks();
		}

		private void Control_MouseClick(object sender, swf.MouseEventArgs e)
		{
			var menu = ContextMenuHandler.GetControl(Menu);

			if (menu != null && e.Button.HasFlag(swf.MouseButtons.Right))
			{
				// Close tray menu when clicked outside, see
				// https://weblogs.asp.net/marianor/a-wpf-wrapper-around-windows-form-notifyicon
				menu.Opened += ContextMenuOpened;
				menu.Closed += ContextMenuClosed;
				menu.IsOpen = true;
			}
		}

		private static Rect GetElementRect(FrameworkElement menu)
		{
			var begin = menu.PointToScreen(new Point(0, 0));
			var end = menu.PointToScreen(new Point(menu.ActualWidth, menu.ActualHeight));
			return new Rect(begin, end);
		}

		private static Point GetHitPoint(IntPtr structPointer)
		{
			var mouseHook = (Win32.MouseLowLevelHook) Marshal.PtrToStructure(structPointer, typeof(Win32.MouseLowLevelHook));
			return new Point(mouseHook.X, mouseHook.Y);
		}

		private static int GetKeyCode(IntPtr structPointer)
		{
			var keyboardHook = (Win32.KeyboardLowLevelHook) Marshal.PtrToStructure(structPointer, typeof(Win32.KeyboardLowLevelHook));
			return (int)keyboardHook.VirtualKeyCode;
		}

		private void InitializeNativeHooks()
		{
			_mouseHookProcRef = MouseEventProc;
			_keyboardHookProcRef = KeyboardEventProc;
		}

		private void InstallHooks()
		{
			_mouseHookHandle = Win32.SetHook(Win32.WH.MOUSE_LL, _mouseHookProcRef);
			if (_mouseHookHandle == IntPtr.Zero)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			_keyboardHookHandle = Win32.SetHook(Win32.WH.KEYBOARD_LL, _keyboardHookProcRef);
			if (_keyboardHookHandle == IntPtr.Zero)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		private IntPtr KeyboardEventProc(int code, IntPtr wParam, IntPtr lParam)
		{
			if (code == 0 && wParam == (IntPtr)Win32.WM.KEYDOWN)
			{
				var key = GetKeyCode(lParam);
				if (key == (int)Win32.VK.ESCAPE)
				{
					var menu = ContextMenuHandler.GetControl(Menu);
					menu.IsOpen = false;
				}
			}

			return Win32.CallNextHookEx(_keyboardHookHandle, code, wParam, lParam);
		}

		private IntPtr MouseEventProc(int code, IntPtr wParam, IntPtr lParam)
		{
			var menu = ContextMenuHandler.GetControl(Menu);
			if (menu.IsVisible && code == 0 && (wParam == (IntPtr)Win32.WM.LBUTTONDOWN || wParam == (IntPtr)Win32.WM.RBUTTONDOWN))
			{
				swc.MenuItem subMenuItem = GetCurrentSubMenuItem(menu.Items);
				var hitPoint = GetHitPoint(lParam);
				Rect menuRect = subMenuItem != null ? GetElementRect(subMenuItem) : GetElementRect(menu);

				if (!menuRect.Contains(hitPoint))
				{
					menu.IsOpen = false;
				}
			}

			return Win32.CallNextHookEx(_mouseHookHandle, code, wParam, lParam);
		}

		private swc.MenuItem GetCurrentSubMenuItem(swc.ItemCollection items)
		{
			foreach (var item in items)
			{
				if (item is swc.MenuItem menuItem)
				{
					if (menuItem.IsSubmenuOpen)
					{
						var subItem = GetCurrentSubMenuItem(menuItem.Items);
						if (subItem != null)
							return subItem;
					}

					if (menuItem.IsMouseOver)
					{
						return menuItem;
					}
				}
			}

			return null;
		}
	}
}