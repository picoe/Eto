namespace Eto
{
	static partial class Win32
	{
#pragma warning disable 0649
		// Analysis disable InconsistentNaming
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
			public int width => right - left;
			public int height => bottom - top;

			public System.Drawing.Rectangle ToSD() => new System.Drawing.Rectangle(left, top, width, height);
			public Eto.Drawing.Rectangle ToEto() => new Eto.Drawing.Rectangle(left, top, width, height);
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
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

		[Flags]
		public enum PRF
		{
			CHECKVISIBLE = 0x00000001,
			NONCLIENT = 0x00000002,
			CLIENT = 0x00000004,
			ERASEBKGND = 0x00000008,
			CHILDREN = 0x00000010
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

		public enum SB
		{
			THUMBTRACK = 5,
			TOP = 6,
			LEFT = 6,
			BOTTOM = 7,
			RIGHT = 7
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
			EM_SETMARGINS = 0xd3,

			DPICHANGED = 0x02E0,
			NCCREATE = 0x0081,
			NCLBUTTONDOWN = 0x00A1,
			PRINT = 0x0317,
			SHOWWINDOW = 0x00000018,
			HSCROLL = 0x114,
			VSCROLL = 0x115,
			USER = 0x400,
			EM_GETSCROLLPOS = USER + 221,
			EM_SETSCROLLPOS = USER + 222,
		}

		public enum VK : long
		{
			SHIFT = 0x10,
			CONTROL = 0x11,
			MENU = 0x12,
			CAPSLOCK = 0x14,
			ESCAPE = 0x1B,
			NUMLOCK = 0x90,
			SCROLL = 0x91,
			LSHIFT = 0xA0,
			RSHIFT = 0xA1,
			LCONTROL = 0xA2,
			RCONTROL = 0xA3,
			LMENU = 0xA4,
			RMENU = 0xA5,
			LWIN = 0x5B,
			RWIN = 0x5C
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

		public static swf.MouseButtons GetMouseButtonWParam(IntPtr wParam)
		{
			var mask = (MK)LOWORD(wParam);
			var buttons = swf.MouseButtons.None;

			if (mask.HasFlag(MK.LBUTTON))
				buttons |= swf.MouseButtons.Left;
			if (mask.HasFlag(MK.RBUTTON))
				buttons |= swf.MouseButtons.Right;
			if (mask.HasFlag(MK.MBUTTON))
				buttons |= swf.MouseButtons.Middle;
			if (mask.HasFlag(MK.XBUTTON1))
				buttons |= swf.MouseButtons.XButton1;
			if (mask.HasFlag(MK.XBUTTON2))
				buttons |= swf.MouseButtons.XButton2;
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
		public static extern IntPtr GetActiveWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);

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
		public static extern IntPtr SendMessage(IntPtr hWnd, WM wMsg, IntPtr wParam, ref sd.Point lParam);


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool PeekMessage(ref swf.Message wMsg, IntPtr hwnd, int msgMin, int msgMax, int remove);


		public static swf.Message? GetNextMessage(swf.Control ctl, params WM[] wMsg)
		{
			swf.Message? msg = null;
			swf.Message pmsg = default(swf.Message);
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

		public enum WH
		{
			KEYBOARD = 2,
			KEYBOARD_LL = 13,
			MOUSE_LL = 14
		}


		public static IntPtr SetHook(WH hookId, HookProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx((IntPtr)hookId, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr CallNextHookEx(IntPtr hookId, int code, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr SetWindowsHookEx(IntPtr hookId, HookProc function, IntPtr instance, int threadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hookId);

		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

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

		public static bool ApplicationIsActivated()
		{
			var activatedHandle = GetForegroundWindow();
			if (activatedHandle == IntPtr.Zero)
			{
				return false;       // No window is currently activated
			}

			var procId = Process.GetCurrentProcess().Id;
			int activeProcId;
			GetWindowThreadProcessId(activatedHandle, out activeProcId);

			return activeProcId == procId;
		}


		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

		[StructLayout(LayoutKind.Sequential)]
		public struct GUITHREADINFO
		{
			public int cbSize;
			public uint flags;
			public IntPtr hwndActive;
			public IntPtr hwndFocus;
			public IntPtr hwndCapture;
			public IntPtr hwndMenuOwner;
			public IntPtr hwndMoveSize;
			public IntPtr hwndCaret;
			public RECT rcCaret;
		}

		[DllImport("kernel32.dll")]
		static extern uint GetCurrentThreadId();

		public static bool GetInfo(out GUITHREADINFO lpgui, uint? threadId = null)
		{
			lpgui = new GUITHREADINFO();
			lpgui.cbSize = Marshal.SizeOf(lpgui);

			return GetGUIThreadInfo(threadId ?? GetCurrentThreadId(), ref lpgui);
		}

		public static IntPtr GetThreadFocusWindow(uint? threadId = null)
		{
			if (!GetInfo(out var info, threadId))
				return IntPtr.Zero;

			return info.hwndFocus;
		}

		[DllImport("gdi32.dll")]
		public static extern bool OffsetWindowOrgEx(IntPtr hdc, int nXOffset, int nYOffset, ref POINT lpPoint);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nheightRect, int nweightRect);


		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT lpPoint);


		[DllImport("user32.dll")]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
		[DllImport("user32.dll")]
		public static extern bool EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable);

		[Flags]
		public enum MF : uint
		{
			BYCOMMAND = 0x00000000,
			GRAYED = 0x00000001
		}

		public enum SC : uint
		{
			CLOSE = 0xF060
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GlobalLock(IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern bool GlobalUnlock(IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GlobalSize(IntPtr handle);


		public enum SBB
		{
			HORZ = 0,
			VERT = 1
		}

		public enum ScrollInfoMask : uint
		{
			SIF_RANGE = 0x1,
			SIF_PAGE = 0x2,
			SIF_POS = 0x4,
			SIF_DISABLENOSCROLL = 0x8,
			SIF_TRACKPOS = 0x10,
			SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
		}
		public enum SBOrientation : int
		{
			SB_HORZ = 0x0,
			SB_VERT = 0x1,
			SB_CTL = 0x2,
			SB_BOTH = 0x3
		}
		
		[Serializable, StructLayout(LayoutKind.Sequential)]
		public struct SCROLLINFO
		{
			public int cbSize; // (uint) int is because of Marshal.SizeOf
			public uint fMask;
			public int nMin;
			public int nMax;
			public int nPage;
			public int nPos;
			public int nTrackPos;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

		[DllImport("user32.dll")]
		public static extern bool DestroyWindow(IntPtr hWnd);
		
		[DllImport("User32.dll", SetLastError = true)]
    	public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
	}
}
