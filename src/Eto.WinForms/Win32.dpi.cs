using System.Windows;
#if WPF
using Eto.Wpf.Forms;
#elif WINFORMS
using Eto.WinForms.Forms;
#endif

namespace Eto
{
	static partial class Win32
	{
		static Lazy<bool> perMonitorThreadDpiSupported = new Lazy<bool>(() => MethodExists("User32.dll", "SetThreadDpiAwarenessContext"));
		static Lazy<bool> perMonitorDpiSupported = new Lazy<bool>(() => MethodExists("shcore.dll", "SetProcessDpiAwareness"));
		static Lazy<bool> monitorDpiSupported = new Lazy<bool>(() => MethodExists("shcore.dll", "GetDpiForMonitor"));

		public static bool PerMontiorThreadDpiSupported => perMonitorThreadDpiSupported.Value;
		public static bool PerMonitorDpiSupported => perMonitorDpiSupported.Value;

		public static bool MonitorDpiSupported => monitorDpiSupported.Value;

		public enum PROCESS_DPI_AWARENESS : uint
		{
			UNAWARE = 0,
			SYSTEM_DPI_AWARE = 1,
			PER_MONITOR_DPI_AWARE = 2
		}

		public enum MONITOR : uint
		{
			DEFAULTTONULL = 0,
			DEFAULTTOPRIMARY = 1,
			DEFAULTTONEAREST = 2
		}

		/// <summary>
		/// Monitor Display Type
		/// </summary>
		public enum MDT : uint
		{
			EFFECTIVE_DPI = 0,
			ANGULAR_DPI = 1,
			RAW_DPI = 2
		}

		public static uint GetWindowDpi(IntPtr hwnd, bool onlyIfSupported = true)
		{
			// use system DPI if per-monitor DPI is not supported.
			if (onlyIfSupported && !PerMonitorDpiSupported)
				return 1;

			var monitor = MonitorFromWindow(hwnd, MONITOR.DEFAULTTONEAREST);
			uint dpiX; uint dpiY;
			GetDpiForMonitor(monitor, MDT.EFFECTIVE_DPI, out dpiX, out dpiY);

			return dpiX;
		}

		public static Eto.Drawing.Point LogicalToScreen(this Eto.Drawing.PointF point, Eto.Forms.Screen screen = null, bool usePerMonitor = true)
		{
			screen = screen ?? Eto.Forms.Screen.FromPoint(point);
			var sdscreen = ScreenHandler.GetControl(screen);
			var location = sdscreen.GetLogicalLocation();
			var pixelSize = sdscreen.GetLogicalPixelSize(usePerMonitor);
			var sdscreenBounds = usePerMonitor ? sdscreen.GetBounds() : sdscreen.Bounds.ToEto();

			var x = sdscreenBounds.X + (point.X - location.X) * pixelSize;
			var y = sdscreenBounds.Y + (point.Y - location.Y) * pixelSize;

			return Drawing.Point.Round(new Drawing.PointF(x, y));
		}

		public static Eto.Drawing.PointF ScreenToLogical(this Eto.Drawing.Point point, swf.Screen sdscreen = null, bool usePerMonitor = true)
		{
			return ScreenToLogical(point.ToSD(), sdscreen, usePerMonitor);
		}

		public static Eto.Drawing.PointF ScreenToLogical(this sd.Point point, swf.Screen sdscreen = null, bool usePerMonitor = true)
		{
			sdscreen ??= swf.Screen.FromPoint(point);
			var location = sdscreen.GetLogicalLocation();
			var pixelSize = sdscreen.GetLogicalPixelSize(usePerMonitor);
			var sdscreenBounds = usePerMonitor ? sdscreen.GetBounds() : sdscreen.Bounds.ToEto();

			var x = location.X + (point.X - sdscreenBounds.X) / pixelSize;
			var y = location.Y + (point.Y - sdscreenBounds.Y) / pixelSize;

			// Console.WriteLine($"In: {point}, out: {x},{y}");
			return new Drawing.PointF(x, y);
		}

		public static Eto.Drawing.RectangleF ScreenToLogical(this Eto.Drawing.Rectangle rect, swf.Screen sdscreen, bool usePerMonitor = true)
		{
			sdscreen = sdscreen ?? swf.Screen.FromPoint(rect.Location.ToSD());
			var location = sdscreen.GetLogicalLocation();
			var pixelSize = sdscreen.GetLogicalPixelSize(usePerMonitor);
			var sdscreenBounds = usePerMonitor ? sdscreen.GetBounds() : sdscreen.Bounds.ToEto();
			return new Eto.Drawing.RectangleF(
				location.X + (rect.X - sdscreenBounds.X) / pixelSize,
				location.Y + (rect.Y - sdscreenBounds.Y) / pixelSize,
				rect.Width / pixelSize,
				rect.Height / pixelSize
				);
		}

		public static float GetMaxLogicalPixelSize() => locationHelper.GetMaxLogicalPixelSize();

		public static Eto.Drawing.RectangleF GetLogicalBounds(this swf.Screen screen)
		{
			return new Eto.Drawing.RectangleF(GetLogicalLocation(screen), GetLogicalSize(screen));
		}
		
		public static bool IsSystemDpiAware => PerMonitorDpiSupported ?
			(Win32.GetProcessDpiAwareness(IntPtr.Zero, out var awareness) == 0 && awareness == Win32.PROCESS_DPI_AWARENESS.SYSTEM_DPI_AWARE) :
			Win32.IsProcessDPIAware();

		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		public static float SystemDpi => (PerMontiorThreadDpiSupported ? Win32.GetDpiForSystem() : (uint)Win32.GetDeviceCaps(IntPtr.Zero, 88 /*LOGPIXELSX*/)) / 96f;

		class ScreenHelper : LogicalScreenHelper<swf.Screen>
		{
			public override IEnumerable<swf.Screen> AllScreens => swf.Screen.AllScreens;

			public override swf.Screen PrimaryScreen => swf.Screen.PrimaryScreen;

			public override sd.Rectangle GetBounds(swf.Screen screen)
			{
				if (!PerMonitorDpiSupported)
					return screen.Bounds;

				var info = new MONITORINFOEX();
				GetMonitorInfo(screen, ref info);

				return info.rcMonitor.ToSD();
			}

			public sd.Rectangle GetWorkingArea(swf.Screen screen)
			{
				if (!PerMonitorDpiSupported)
					return screen.WorkingArea;

				var info = new MONITORINFOEX();
				GetMonitorInfo(screen, ref info);
				
				return info.rcWork.ToSD();
			}


			public override float GetLogicalPixelSize(swf.Screen screen, bool usePerMonitor = true)
			{
				if (!MonitorDpiSupported)
				{
					// fallback to slow method if we can't get the dpi from the system
					using (var form = new System.Windows.Forms.Form { Bounds = screen.Bounds })
					using (var graphics = form.CreateGraphics())
					{
						return (uint)graphics.DpiY / 96f;
					}
				}
				var mon = MonitorFromPoint(screen.Bounds.Location, MONITOR.DEFAULTTONEAREST);

				// use per-monitor aware dpi awareness to get ACTUAL dpi here
				var oldDpiAwareness = usePerMonitor ? SetThreadDpiAwarenessContextSafe(DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_v2) : DPI_AWARENESS_CONTEXT.NONE;

				uint dpiX, dpiY;
				GetDpiForMonitor(mon, MDT.EFFECTIVE_DPI, out dpiX, out dpiY);

				if (oldDpiAwareness != DPI_AWARENESS_CONTEXT.NONE)
					SetThreadDpiAwarenessContextSafe(oldDpiAwareness);
				return dpiX / 96f;
			}

			public override SizeF GetLogicalSize(swf.Screen screen) => (SizeF)GetBounds(screen).Size.ToEto() / screen.GetLogicalPixelSize();
		}

		static ScreenHelper locationHelper = new ScreenHelper();

		public static Eto.Drawing.Rectangle GetBounds(this swf.Screen screen) => locationHelper.GetBounds(screen).ToEto();
		public static Eto.Drawing.Rectangle GetWorkingArea(this swf.Screen screen) => locationHelper.GetWorkingArea(screen).ToEto();

		public static Eto.Drawing.PointF GetLogicalLocation(this swf.Screen screen) => locationHelper.GetLogicalLocation(screen);

		public static Eto.Drawing.SizeF GetLogicalSize(this swf.Screen screen) => locationHelper.GetLogicalSize(screen);

		public static float GetLogicalPixelSize(this swf.Screen screen, bool usePerMonitor = true) => locationHelper.GetLogicalPixelSize(screen, usePerMonitor);

		public static void GetMonitorInfo(this swf.Screen screen, ref MONITORINFOEX info)
		{
			var hmonitor = MonitorFromPoint(screen.Bounds.Location, 0);

			var oldDpiAwareness = SetThreadDpiAwarenessContextSafe(DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_v2);

			GetMonitorInfo(new HandleRef(null, hmonitor), info);

			if (oldDpiAwareness != DPI_AWARENESS_CONTEXT.NONE)
				SetThreadDpiAwarenessContextSafe(oldDpiAwareness);
		}


		[DllImport("User32.dll")]
		public static extern IntPtr MonitorFromPoint(System.Drawing.Point pt, MONITOR dwFlags);

		[DllImport("user32.dll")]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MONITOR flags);

		[DllImport("user32.dll")]
		public static extern uint GetDpiForWindow(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern uint GetDpiForSystem();

		[DllImport("shcore.dll")]
		public static extern uint GetDpiForMonitor(IntPtr hmonitor, MDT dpiType, out uint dpiX, out uint dpiY);

		[DllImport("shcore.dll")]
		public static extern uint SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

		[DllImport("shcore.dll")]
		public static extern uint GetProcessDpiAwareness(IntPtr handle, out PROCESS_DPI_AWARENESS awareness);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsProcessDPIAware();

		[DllImport("User32.dll")]
		static extern DPI_AWARENESS_CONTEXT SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT dpiContext);

		public static DPI_AWARENESS_CONTEXT SetThreadDpiAwarenessContextSafe(DPI_AWARENESS_CONTEXT dpiContext)
		{
			if (!PerMontiorThreadDpiSupported)
				return DPI_AWARENESS_CONTEXT.NONE;
			return SetThreadDpiAwarenessContext(dpiContext);
		}
		
		public static swf.Screen GetScreenFromWindow(IntPtr nativeHandle)
		{
			if (nativeHandle == IntPtr.Zero)
				return swf.Screen.PrimaryScreen;

			return swf.Screen.FromHandle(nativeHandle);

			// var monitorPtr = Win32.MonitorFromWindow(nativeHandle, MONITOR.DEFAULTTONEAREST);
			// var info = new MONITORINFOEX();
			// Win32.GetMonitorInfo(new HandleRef(null, monitorPtr), info);
			// var monitorBounds = info.rcMonitor.ToSD();
			// foreach (var screen in swf.Screen.AllScreens)
			// {
			// 	if (screen.Bounds == monitorBounds)
			// 		return screen;
			// }
			// return swf.Screen.PrimaryScreen;
		}

		
		public static T ExecuteInDpiAwarenessContext<T>(Func<T> func)
		{
			var oldDpiAwareness = Win32.SetThreadDpiAwarenessContextSafe(Win32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_v2);
			try
			{
				return func();
			}
			finally
			{
				if (oldDpiAwareness != Win32.DPI_AWARENESS_CONTEXT.NONE)
					Win32.SetThreadDpiAwarenessContextSafe(oldDpiAwareness);
			}
		}

		[DllImport("User32.dll")]
		public static extern DPI_AWARENESS_CONTEXT GetThreadDpiAwarenessContext();

		[DllImport("User32.dll")]
		public static extern bool EnableNonClientDpiScaling(IntPtr hwnd);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);
		[DllImport("User32.dll", ExactSpelling = true)]
		public static extern IntPtr MonitorFromPoint(POINT pt, int flags);

		[DllImport("user32.dll")]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
		public class MONITORINFOEX
		{
			public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
			public RECT rcMonitor = new RECT();
			public RECT rcWork = new RECT();
			public int dwFlags = 0;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szDevice = new char[32];
		}

		public enum DPI_AWARENESS_CONTEXT
		{
			NONE = 0,
			UNAWARE = -1,
			SYSTEM_AWARE = -2,
			PER_MONITOR_AWARE = -3,
			PER_MONITOR_AWARE_v2 = -4,
			UNAWARE_GDISCALED = -5
		}
	}
}
