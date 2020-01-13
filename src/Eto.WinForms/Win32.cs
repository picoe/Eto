using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

namespace Eto
{
	static partial class Win32
	{
		#pragma warning disable 0649
		// Analysis disable InconsistentNaming
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
			public int width => right - left;
			public int height => bottom - top;
		}
		#pragma warning restore 0649

		[Flags]
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

		public enum SW
		{
			HIDE = 0,
			SHOWNORMAL = 1,
			SHOWMINIMIZED = 2,
			SHOWMAXIMIZED = 3,
			MAXIMIZE = 3,
			SHOWNOACTIVATE = 4,
			SHOW = 5,
			MINIMIZE = 6,
			SHOWMINNOACTIVE = 7,
			SHOWNA = 8,
			RESTORE = 9,
			SHOWDEFAULT = 10,
			FORCEMINIMIZE = 11
		}


		public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		public static readonly IntPtr HWND_TOP = new IntPtr(0);
		public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

		public enum GWL
		{
			EXSTYLE = -20,
			HINSTANCE = -6,
			ID = -12,
			STYLE = -16,
			USERDATA = -21,
			WNDPROC = -4
		}

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
			TOPMOST = 0x00000008,
			TOOLWINDOW = 0x00000080,
			APPWINDOW = 0x00040000,
			NOACTIVATE = 0x08000000
		}

		public enum WM
		{
			SETREDRAW = 0xB,

			GETDLGCODE = 0x0087,

			KEYDOWN = 0x0100,
			KEYUP = 0x0101,
			CHAR = 0x0102,
			SYSKEYDOWN = 0x0104,
			SYSKEYUP = 0x0105,
			SYSCHAR = 0x0106,
			IME_CHAR = 0x0286,

			MOUSEMOVE = 0x0200,
			LBUTTONDOWN = 0x0201,
			LBUTTONUP = 0x0202,
			LBUTTONDBLCLK = 0x0203,
			RBUTTONDOWN = 0x0204,
			RBUTTONUP = 0x0205,
			RBUTTONDBLCLK = 0x0206,
			MBUTTONDOWN = 0x0207,
			MBUTTONUP = 0x0208,
			MBUTTONDBLCLK = 0x0209,
			MOUSEWHEEL = 0x20A,

			CUT = 0x0300,
			COPY = 0x0301,
			PASTE = 0x0302,
			CLEAR = 0x0303,

			ERASEBKGND = 0x14,

			TV_FIRST = 0x1100,
			TVM_SETBKCOLOR = TV_FIRST + 29,
			TVM_SETEXTENDEDSTYLE = TV_FIRST + 44,

			ECM_FIRST = 0x1500,
			EM_SETCUEBANNER = ECM_FIRST + 1,

			DPICHANGED = 0x02E0,
			NCCREATE = 0x0081,
			NCLBUTTONDOWN = 0x00A1
		}

		public enum HT
		{
			CAPTION = 0x2
		}

		public static ushort LOWORD(IntPtr word)
		{
			return (ushort)(((long)word) & 0xffff);
		}

		public static ushort LOWORD(int word)
		{
			return (ushort)(word & 0xFFFF);
		}

		public static ushort HIWORD(IntPtr dwValue)
		{
			return (ushort)((((long)dwValue) >> 0x10) & 0xffff);
		}

		public static ushort HIWORD(uint dwValue)
		{
			return (ushort)(dwValue >> 0x10);
		}

		public static int SignedHIWORD(IntPtr n)
		{
			return SignedHIWORD((int)((long)n));
		}

		public static int SignedLOWORD(IntPtr n)
		{
			return SignedLOWORD((int)((long)n));
		}

		public static int SignedHIWORD(int n)
		{
			return (short)((n >> 16) & 0xFFFF);
		}

		public static int SignedLOWORD(int n)
		{
			return (short)(n & 0xFFFF);
		}

		public static int GetWheelDeltaWParam(IntPtr wParam)
		{
			return SignedHIWORD(wParam);
		}

		[Flags]
		public enum MK
		{
			NONE = 0x0000,
			LBUTTON = 0x0001,
			RBUTTON = 0x0002,
			SHIFT = 0x0004,
			CONTROL = 0x0008,
			MBUTTON = 0x0010,
			XBUTTON1 = 0x0020,
			XBUTTON2 = 0x0040
		}

		public static MouseButtons GetMouseButtonWParam(IntPtr wParam)
		{
			var mask = (MK)LOWORD(wParam);
			var buttons = MouseButtons.None;

			if (mask.HasFlag(MK.LBUTTON))
				buttons |= MouseButtons.Left;
			if (mask.HasFlag(MK.RBUTTON))
				buttons |= MouseButtons.Right;
			if (mask.HasFlag(MK.MBUTTON))
				buttons |= MouseButtons.Middle;
			if (mask.HasFlag(MK.XBUTTON1))
				buttons |= MouseButtons.XButton1;
			if (mask.HasFlag(MK.XBUTTON2))
				buttons |= MouseButtons.XButton2;
			return buttons;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsZoomed(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, SW nCmdShow);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnableWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bEnable);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowEnabled(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowLong(IntPtr hWnd, GWL nIndex);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, uint dwNewLong);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, WM wMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		public static extern IntPtr SendMessage(IntPtr hWnd, WM msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool PeekMessage(ref Message wMsg, IntPtr hwnd, int msgMin, int msgMax, int remove);


		public static Message? GetNextMessage(Control ctl, params WM[] wMsg)
		{
			Message? msg = null;
			Message pmsg = default(Message);
			var ret = false;
			do
			{
				ret = false;
				foreach (var wm in wMsg)
				{
					ret |= PeekMessage(ref pmsg, ctl.Handle, (int)wm, (int)wm, 0);
					if (ret)
						msg = pmsg;
				}
			} while (ret);
			return msg;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern IntPtr LoadLibrary(string library);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern bool FreeLibrary(IntPtr moduleHandle);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true, ExactSpelling = true)]
		static extern IntPtr GetProcAddress(IntPtr moduleHandle, string method);

		public enum GA : uint
		{
			GA_PARENT = 1,
			GA_ROOT = 2,
			GA_ROOTOWNER = 3
		}

		[DllImport("user32.dll")]
		public static extern IntPtr GetAncestor(IntPtr hwnd, GA gaFlags);

		public static bool MethodExists(string module, string method)
		{
			var moduleHandle = LoadLibrary(module);
			if (moduleHandle == IntPtr.Zero)
				return false;
			try
			{
				return GetProcAddress(moduleHandle, method) != IntPtr.Zero;
			}
			finally
			{
				FreeLibrary(moduleHandle);
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern int GetWindowTextLength(IntPtr hWnd);

		public static string GetWindowText(IntPtr hwnd)
		{
			try
			{
				var len = GetWindowTextLength(hwnd);
				var sb = new StringBuilder(len + 1);
				GetWindowText(hwnd, sb, sb.Capacity);
				return sb.ToString();
			}
			catch
			{
				return null;
			}
		}

		  // for tray indicator

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(int hookId, int code, int param, IntPtr dataPointer);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int SetWindowsHookEx(int hookId, HookProc function, IntPtr instance, int threadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int UnhookWindowsHookEx(int hookId);

		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		public delegate int HookProc(int code, int wParam, IntPtr structPointer);

		[StructLayout(LayoutKind.Sequential)]
		public struct MouseLowLevelHook
		{
			internal readonly int X;
			internal readonly int Y;
			internal readonly int MouseData;
			internal readonly int Flags;
			internal readonly int Time;
			internal readonly int ExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KeyboardLowLevelHook
		{
			internal readonly int VirtualKeyCode;
			internal readonly int ScanCode;
			internal readonly int Flags;
			internal readonly int Time;
			internal readonly int ExtraInfo;
		}

	 }
}
