using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Drawing;
#if WPF
using Eto.Wpf.Forms;
#elif WINFORMS
using Eto.WinForms.Forms;
#endif

namespace Eto
{
	static partial class Win32
	{
		static Lazy<bool> perMonitorDpiSupported = new Lazy<bool>(() => MethodExists("shcore.dll", "SetProcessDpiAwareness"));
		static Lazy<bool> monitorDpiSupported = new Lazy<bool>(() => MethodExists("shcore.dll", "GetDpiForMonitor"));

		public static bool PerMonitorDpiSupported
		{
			get { return perMonitorDpiSupported.Value; }
		}

		public static bool MonitorDpiSupported
		{
			get { return monitorDpiSupported.Value; }
		}

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

		public static Eto.Drawing.Point LogicalToScreen(this Eto.Drawing.PointF point)
		{
			var screen = Eto.Forms.Screen.FromPoint(point);
			var sdscreen = ScreenHandler.GetControl(screen);
			var pixelSize = sdscreen.GetLogicalPixelSize();
			var location = sdscreen.Bounds.Location;
			var screenBounds = screen.Bounds;

			var x = location.X + (point.X - screenBounds.X) * pixelSize;
			var y = location.Y + (point.Y - screenBounds.Y) * pixelSize;

			return Drawing.Point.Round(new Drawing.PointF(x, y));
		}

		public static Eto.Drawing.PointF ScreenToLogical(this Eto.Drawing.Point point, swf.Screen sdscreen = null)
		{
			return ScreenToLogical(point.ToSD(), sdscreen);
		}

		public static Eto.Drawing.PointF ScreenToLogical(this sd.Point point, swf.Screen sdscreen = null)
		{
			sdscreen = sdscreen ?? swf.Screen.FromPoint(point);
			var location = sdscreen.GetLogicalLocation();
			var pixelSize = sdscreen.GetLogicalPixelSize();

			var x = location.X + (point.X - sdscreen.Bounds.X) / pixelSize;
			var y = location.Y + (point.Y - sdscreen.Bounds.Y) / pixelSize;

			return new Drawing.PointF(x, y);
		}

		public static Eto.Drawing.RectangleF ScreenToLogical(this Eto.Drawing.Rectangle rect) => ScreenToLogical(rect.ToSD());

		public static Eto.Drawing.RectangleF ScreenToLogical(this sd.Rectangle rect)
		{
			var screen = swf.Screen.FromPoint(new sd.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2));
			var location = screen.GetLogicalLocation();
			var pixelSize = screen.GetLogicalPixelSize();
			return new Eto.Drawing.RectangleF(
				location.X + (rect.X - screen.Bounds.X) / pixelSize,
				location.Y + (rect.Y - screen.Bounds.Y) / pixelSize,
				rect.Width / pixelSize,
				rect.Height / pixelSize
				);
		}

		public static float GetMaxLogicalPixelSize() => locationHelper.GetMaxLogicalPixelSize();

		public static Eto.Drawing.RectangleF GetLogicalBounds(this swf.Screen screen)
		{
			return new Eto.Drawing.RectangleF(GetLogicalLocation(screen), GetLogicalSize(screen));
		}

		class ScreenHelper : LogicalScreenHelper<swf.Screen>
		{
			public override IEnumerable<swf.Screen> AllScreens => swf.Screen.AllScreens;

			public override swf.Screen PrimaryScreen => swf.Screen.PrimaryScreen;

			public override sd.Rectangle GetBounds(swf.Screen screen) => screen.Bounds;

			public override float GetLogicalPixelSize(swf.Screen screen)
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

				var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
				var mon = MonitorFromPoint(pnt, MONITOR.DEFAULTTONEAREST);
				uint dpiX, dpiY;
				GetDpiForMonitor(mon, MDT.EFFECTIVE_DPI, out dpiX, out dpiY);
				return dpiX / 96f;
			}

			public override SizeF GetLogicalSize(swf.Screen screen) => (SizeF)screen.Bounds.Size.ToEto() / screen.GetLogicalPixelSize();
		}

		static ScreenHelper locationHelper = new ScreenHelper();

		public static Eto.Drawing.PointF GetLogicalLocation(this swf.Screen screen) => locationHelper.GetLogicalLocation(screen);

		public static Eto.Drawing.SizeF GetLogicalSize(this swf.Screen screen) => locationHelper.GetLogicalSize(screen);

		public static float GetLogicalPixelSize(this swf.Screen screen) => locationHelper.GetLogicalPixelSize(screen);

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

		[DllImport("User32.dll")]
		public static extern bool EnableNonClientDpiScaling(IntPtr hwnd);
	}
}
