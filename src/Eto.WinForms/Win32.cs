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

		[DllImport("kernel32.dll")]
		public static extern void SetLastError(uint dwErrCode);

		public const int MAX_PATH = 260;

		[Flags]
		public enum SHGSI : uint
		{
			SHGSI_ICONLOCATION = 0,
			SHGSI_ICON = 0x000000100,
			SHGSI_SYSICONINDEX = 0x000004000,
			SHGSI_LINKOVERLAY = 0x000008000,
			SHGSI_SELECTED = 0x000010000,
			SHGSI_LARGEICON = 0x000000000,
			SHGSI_SMALLICON = 0x000000001,
			SHGSI_SHELLICONSIZE = 0x000000004
		}
		public enum SHSTOCKICONID : uint
		{
			SIID_DOCNOASSOC = 0,          //Blank document icon (Document of a type with no associated application).
			SIID_DOCASSOC = 1,            //Application-associated document icon (Document of a type with an associated application).
			SIID_APPLICATION = 2,         //Generic application with no custom icon.
			SIID_FOLDER = 3,              //Folder (generic, unspecified state).
			SIID_FOLDEROPEN = 4,          //Folder (open).
			SIID_DRIVE525 = 5,            //5.25-inch disk drive.
			SIID_DRIVE35 = 6,             //3.5-inch disk drive.
			SIID_DRIVEREMOVE = 7,         //Removable drive.
			SIID_DRIVEFIXED = 8,          //Fixed drive (hard disk).
			SIID_DRIVENET = 9,            //Network drive (connected).
			SIID_DRIVENETDISABLED = 10,   //Network drive (disconnected).
			SIID_DRIVECD = 11,            //CD drive.
			SIID_DRIVERAM = 12,           //RAM disk drive.
			SIID_WORLD = 13,              //The entire network.
			SIID_SERVER = 15,             //A computer on the network.
			SIID_PRINTER = 16,            //A local printer or print destination.
			SIID_MYNETWORK = 17,          //The Network virtual folder (FOLDERID_NetworkFolder/CSIDL_NETWORK).
			SIID_FIND = 22,               //The Search feature.
			SIID_HELP = 23,               //The Help and Support feature.
			SIID_SHARE = 28,              //Overlay for a shared item.
			SIID_LINK = 29,               //Overlay for a shortcut.
			SIID_SLOWFILE = 30,           //Overlay for items that are expected to be slow to access.
			SIID_RECYCLER = 31,           //The Recycle Bin (empty).
			SIID_RECYCLERFULL = 32,       //The Recycle Bin (not empty).
			SIID_MEDIACDAUDIO = 40,       //Audio CD media.
			SIID_LOCK = 47,               //Security lock.
			SIID_AUTOLIST = 49,           //A virtual folder that contains the results of a search.
			SIID_PRINTERNET = 50,         //A network printer.
			SIID_SERVERSHARE = 51,        //A server shared on a network.
			SIID_PRINTERFAX = 52,         //A local fax printer.
			SIID_PRINTERFAXNET = 53,      //A network fax printer.
			SIID_PRINTERFILE = 54,        //A file that receives the output of a Print to file operation.
			SIID_STACK = 55,              //A category that results from a Stack by command to organize the contents of a folder.
			SIID_MEDIASVCD = 56,          //Super Video CD (SVCD) media.
			SIID_STUFFEDFOLDER = 57,      //A folder that contains only subfolders as child items.
			SIID_DRIVEUNKNOWN = 58,       //Unknown drive type.
			SIID_DRIVEDVD = 59,           //DVD drive.
			SIID_MEDIADVD = 60,           //DVD media.
			SIID_MEDIADVDRAM = 61,        //DVD-RAM media.
			SIID_MEDIADVDRW = 62,         //DVD-RW media.
			SIID_MEDIADVDR = 63,          //DVD-R media.
			SIID_MEDIADVDROM = 64,        //DVD-ROM media.
			SIID_MEDIACDAUDIOPLUS = 65,   //CD+ (enhanced audio CD) media.
			SIID_MEDIACDRW = 66,          //CD-RW media.
			SIID_MEDIACDR = 67,           //CD-R media.
			SIID_MEDIACDBURN = 68,        //A writeable CD in the process of being burned.
			SIID_MEDIABLANKCD = 69,       //Blank writable CD media.
			SIID_MEDIACDROM = 70,         //CD-ROM media.
			SIID_AUDIOFILES = 71,         //An audio file.
			SIID_IMAGEFILES = 72,         //An image file.
			SIID_VIDEOFILES = 73,         //A video file.
			SIID_MIXEDFILES = 74,         //A mixed file.
			SIID_FOLDERBACK = 75,         //Folder back.
			SIID_FOLDERFRONT = 76,        //Folder front.
			SIID_SHIELD = 77,             //Security shield. Use for UAC prompts only.
			SIID_WARNING = 78,            //Warning.
			SIID_INFO = 79,               //Informational.
			SIID_ERROR = 80,              //Error.
			SIID_KEY = 81,                //Key.
			SIID_SOFTWARE = 82,           //Software.
			SIID_RENAME = 83,             //A UI item, such as a button, that issues a rename command.
			SIID_DELETE = 84,             //A UI item, such as a button, that issues a delete command.
			SIID_MEDIAAUDIODVD = 85,      //Audio DVD media.
			SIID_MEDIAMOVIEDVD = 86,      //Movie DVD media.
			SIID_MEDIAENHANCEDCD = 87,    //Enhanced CD media.
			SIID_MEDIAENHANCEDDVD = 88,   //Enhanced DVD media.
			SIID_MEDIAHDDVD = 89,         //High definition DVD media in the HD DVD format.
			SIID_MEDIABLURAY = 90,        //High definition DVD media in the Blu-ray Disc™ format.
			SIID_MEDIAVCD = 91,           //Video CD (VCD) media.
			SIID_MEDIADVDPLUSR = 92,      //DVD+R media.
			SIID_MEDIADVDPLUSRW = 93,     //DVD+RW media.
			SIID_DESKTOPPC = 94,          //A desktop computer.
			SIID_MOBILEPC = 95,           //A mobile computer (laptop).
			SIID_USERS = 96,              //The User Accounts Control Panel item.
			SIID_MEDIASMARTMEDIA = 97,    //Smart media.
			SIID_MEDIACOMPACTFLASH = 98,  //CompactFlash media.
			SIID_DEVICECELLPHONE = 99,    //A cell phone.
			SIID_DEVICECAMERA = 100,      //A digital camera.
			SIID_DEVICEVIDEOCAMERA = 101, //A digital video camera.
			SIID_DEVICEAUDIOPLAYER = 102, //An audio player.
			SIID_NETWORKCONNECT = 103,    //Connect to network.
			SIID_INTERNET = 104,          //The Network and Internet Control Panel item.
			SIID_ZIPFILE = 105,           //A compressed file with a .zip file name extension.
			SIID_SETTINGS = 106,          //The Additional Options Control Panel item.
			SIID_DRIVEHDDVD = 132,        //Windows Vista with Service Pack 1 (SP1) and later. High definition DVD drive (any type - HD DVD-ROM, HD DVD-R, HD-DVD-RAM) that uses the HD DVD format.
			SIID_DRIVEBD = 133,           //Windows Vista with SP1 and later. High definition DVD drive (any type - BD-ROM, BD-R, BD-RE) that uses the Blu-ray Disc format.
			SIID_MEDIAHDDVDROM = 134,     //Windows Vista with SP1 and later. High definition DVD-ROM media in the HD DVD-ROM format.
			SIID_MEDIAHDDVDR = 135,       //Windows Vista with SP1 and later. High definition DVD-R media in the HD DVD-R format.
			SIID_MEDIAHDDVDRAM = 136,     //Windows Vista with SP1 and later. High definition DVD-RAM media in the HD DVD-RAM format.
			SIID_MEDIABDROM = 137,        //Windows Vista with SP1 and later. High definition DVD-ROM media in the Blu-ray Disc BD-ROM format.
			SIID_MEDIABDR = 138,          //Windows Vista with SP1 and later. High definition write-once media in the Blu-ray Disc BD-R format.
			SIID_MEDIABDRE = 139,         //Windows Vista with SP1 and later. High definition read/write media in the Blu-ray Disc BD-RE format.
			SIID_CLUSTEREDDRIVE = 140,    //Windows Vista with SP1 and later. A cluster disk array.
			SIID_MAX_ICONS = 174,         //The highest valid value in the enumeration. Values over 160 are Windows 7-only icons.
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SHSTOCKICONINFO
		{
			public UInt32 cbSize;
			public IntPtr hIcon;
			public Int32 iSysIconIndex;
			public Int32 iIcon;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
			public string szPath;
		}
		[DllImport("Shell32.dll", SetLastError = false)]
		public static extern Int32 SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);

		/// <summary>
		/// Provides access to function required to delete handle. This method is used internally
		/// and is not required to be called separately.
		/// </summary>
		/// <param name="hIcon">Pointer to icon handle.</param>
		/// <returns>N/A</returns>
		[DllImport("User32.dll")]
		public static extern int DestroyIcon(IntPtr hIcon);
	}
}
