using swf = System.Windows.Forms;
using swc = System.Windows.Controls;

namespace Eto.Wpf.Forms
{
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
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
		private HookProc _keyboardHookProcRef;
		private int _mouseHookHandle;
		private HookProc _mouseHookProcRef;


		public TrayIndicatorHandler()
		{
			Control = new swf.NotifyIcon();
			Control.MouseClick += Control_MouseClick;
			InitializeNativeHooks();
		}

		public string Title { get => Control.Text; set => Control.Text = value; }

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
				UnhookWindowsHookEx(_mouseHookHandle);
			}

			if (_keyboardHookHandle != 0)
			{
				UnhookWindowsHookEx(_keyboardHookHandle);
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

		private static Rect GetContextMenuRect(swc.ContextMenu menu)
		{
			var begin = menu.PointToScreen(new Point(0, 0));
			var end = menu.PointToScreen(new Point(menu.ActualWidth, menu.ActualHeight));
			return new Rect(begin, end);
		}

		private static Point GetHitPoint(IntPtr structPointer)
		{
			var mouseHook = (MouseLowLevelHook) Marshal.PtrToStructure(structPointer, typeof(MouseLowLevelHook));
			return new Point(mouseHook.X, mouseHook.Y);
		}

		private static int GetKeyCode(IntPtr structPointer)
		{
			var keyboardHook = (KeyboardLowLevelHook) Marshal.PtrToStructure(structPointer, typeof(KeyboardLowLevelHook));
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
				_mouseHookHandle = SetWindowsHookEx(WhMouseLowLevel, _mouseHookProcRef, GetModuleHandle(module.ModuleName), 0);
				if (_mouseHookHandle == 0)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}

				_keyboardHookHandle =
					SetWindowsHookEx(WhKeyboardLowLevel, _keyboardHookProcRef, GetModuleHandle(module.ModuleName), 0);
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

			return CallNextHookEx(_keyboardHookHandle, code, wParam, lParam);
		}

		private int MouseEventProc(int code, int wParam, IntPtr lParam)
		{
			var menu = ContextMenuHandler.GetControl(Menu);
			if (menu.IsVisible && code == 0 && (wParam == WmLeftButtonDown || wParam == WmRightButtonDown))
			{
				var contextMenuRect = GetContextMenuRect(menu);
				var hitPoint = GetHitPoint(lParam);

				if (!contextMenuRect.Contains(hitPoint))
				{
					menu.IsOpen = false;
				}
			}

			return CallNextHookEx(_mouseHookHandle, code, wParam, lParam);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		private static extern int CallNextHookEx(int hookId, int code, int param, IntPtr dataPointer);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		private static extern int SetWindowsHookEx(int hookId, HookProc function, IntPtr instance, int threadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		private static extern int UnhookWindowsHookEx(int hookId);

		private delegate int HookProc(int code, int wParam, IntPtr structPointer);

		[StructLayout(LayoutKind.Sequential)]
		private struct MouseLowLevelHook
		{
			internal readonly int X;
			internal readonly int Y;
			internal readonly int MouseData;
			internal readonly int Flags;
			internal readonly int Time;
			internal readonly int ExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct KeyboardLowLevelHook
		{
			internal readonly int VirtualKeyCode;
			internal readonly int ScanCode;
			internal readonly int Flags;
			internal readonly int Time;
			internal readonly int ExtraInfo;
		}
	}
}