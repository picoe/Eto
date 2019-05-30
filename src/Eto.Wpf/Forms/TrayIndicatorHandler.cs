using swf = System.Windows.Forms;
using swc = System.Windows.Controls;

namespace Eto.Wpf.Forms
{
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Windows;
	using Eto.Drawing;
	using Eto.Forms;
	using Menu;
	using Point = System.Windows.Point;

	public class TrayIndicatorHandler : WidgetHandler<swf.NotifyIcon, TrayIndicator, TrayIndicator.ICallback>,
		TrayIndicator.IHandler
	{
		private const int KeyEscape = 27;
		private const int WhKeyboardLowLevel = 13;
		private const int WhMouseLowLevel = 14;
		private const int WmKeydown = 0x100;
		private const int WmLeftButtonDown = 0x201;
		private const int WmRightButtonDown = 0x204;
		private Image _image;
		private int _keyboardHookHandle;
		private Win32.HookProc _keyboardHookProcRef;
		private int _mouseHookHandle;
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
			if (_mouseHookHandle != 0)
			{
				Win32.UnhookWindowsHookEx(_mouseHookHandle);
			}

			if (_keyboardHookHandle != 0)
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
			return keyboardHook.VirtualKeyCode;
		}

		private void InitializeNativeHooks()
		{
			_mouseHookProcRef = MouseEventProc;
			_keyboardHookProcRef = KeyboardEventProc;
		}

		private void InstallHooks()
		{
			using (var process = Process.GetCurrentProcess())
			using (var module = process.MainModule)
			{
				_mouseHookHandle = Win32.SetWindowsHookEx(WhMouseLowLevel, _mouseHookProcRef, Win32.GetModuleHandle(module.ModuleName), 0);
				if (_mouseHookHandle == 0)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}

				_keyboardHookHandle =
					Win32.SetWindowsHookEx(WhKeyboardLowLevel, _keyboardHookProcRef, Win32.GetModuleHandle(module.ModuleName), 0);
				if (_keyboardHookHandle == 0)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
		}

		private int KeyboardEventProc(int code, int wParam, IntPtr lParam)
		{
			if (code == 0 && wParam == WmKeydown)
			{
				var key = GetKeyCode(lParam);
				if (key == KeyEscape)
				{
					var menu = ContextMenuHandler.GetControl(Menu);
					menu.IsOpen = false;
				}
			}

			return Win32.CallNextHookEx(_keyboardHookHandle, code, wParam, lParam);
		}

		private int MouseEventProc(int code, int wParam, IntPtr lParam)
		{
			var menu = ContextMenuHandler.GetControl(Menu);
			if (menu.IsVisible && code == 0 && (wParam == WmLeftButtonDown || wParam == WmRightButtonDown))
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