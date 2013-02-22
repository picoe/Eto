using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Eto.Platform
{
	static class Win32
	{
		[Flags ()]
		public enum SWP : uint
		{
			ASYNCWINDOWPOS = 0x4000,
			DEFERERASE = 0x2000,
			DRAWFRAME = 0x0020,
			FRAMECHANGED = 0x0020,
			HIDEWINDOW = 0x0080,
			NOACTIVATE = 0x0010,
			NOCOPYBITS = 0x0100,
			NOMOVE = 0x0002,
			NOOWNERZORDER = 0x0200,
			NOREDRAW = 0x0008,
			NOREPOSITION = 0x0200,
			NOSENDCHANGING = 0x0400,
			NOSIZE = 0x0001,
			NOZORDER = 0x0004,
			SHOWWINDOW = 0x0040,
		}

		public static readonly IntPtr HWND_TOPMOST = new IntPtr (-1);
		public static readonly IntPtr HWND_NOTOPMOST = new IntPtr (-2);
		public static readonly IntPtr HWND_TOP = new IntPtr (0);
		public static readonly IntPtr HWND_BOTTOM = new IntPtr (1);

		public enum GWL : int
		{
			EXSTYLE = -20,
			HINSTANCE = -6,
			ID = -12,
			STYLE = -16,
			USERDATA = -21,
			WNDPROC = -4
		};

		[Flags]
		public enum WS : uint
		{
			BORDER = 0x00800000,
			CAPTION = 0x00C00000,
			CHILD = 0x40000000,
			CHILDWINDOW = 0x40000000,
			CLIPCHILDREN = 0x02000000,
			CLIPSIBLINGS = 0x04000000,
			DISABLED = 0x08000000,
			DLGFRAME = 0x00400000,
			GROUP = 0x00020000,
			HSCROLL = 0x00100000,
			ICONIC = 0x20000000,
			MAXIMIZE = 0x01000000,
			MAXIMIZEBOX = 0x00010000,
			MINIMIZE = 0x20000000,
			MINIMIZEBOX = 0x00020000,
			OVERLAPPED = 0x00000000,
			OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
			POPUP = 0x80000000,
			POPUPWINDOW = POPUP | BORDER | SYSMENU,
			SIZEBOX = 0x00040000,
			SYSMENU = 0x00080000,
			TABSTOP = 0x00010000,
			THICKFRAME = 0x00040000,
			TILED = 0x00000000,
			TILEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
			VISIBLE = 0x10000000,
			VSCROLL = 0x00200000
		}

		[Flags]
		public enum WS_EX : uint
		{
			TOOLWINDOW = 0x80
		}

		public enum WM : uint
		{
			SETREDRAW = 0xB
		}

		[DllImport ("user32.dll")]
		private static extern int ShowWindow (IntPtr hWnd, uint Msg);

		[DllImport ("user32.dll")]
		[return: MarshalAs (UnmanagedType.Bool)]
		public static extern bool SetWindowPos (IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SWP uFlags);

		[DllImport ("user32.dll", SetLastError = true)]
		public static extern uint GetWindowLong (IntPtr hWnd, GWL nIndex);

		[DllImport ("user32.dll")]
		public static extern int SetWindowLong (IntPtr hWnd, GWL nIndex, uint dwNewLong);

		[DllImport("user32.dll")]
	    public static extern int SendMessage(IntPtr hWnd, WM wMsg, IntPtr wParam, IntPtr lParam);

	}
}
