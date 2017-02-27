using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using swf = System.Windows.Forms;
using sd = System.Drawing;
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

		public static uint GetWindowDpi(IntPtr hwnd)
		{
			// use system DPI if per-monitor DPI is not supported.
			if (!PerMonitorDpiSupported)
				return 1;

			var monitor = MonitorFromWindow(hwnd, MONITOR.DEFAULTTONEAREST);
			uint dpiX; uint dpiY;
			GetDpiForMonitor(monitor, MDT.EFFECTIVE_DPI, out dpiX, out dpiY);

			return dpiX;
		}

		static swf.Screen FindLeftScreen(this swf.Screen screen) =>
			swf.Screen.AllScreens
			.Where(r => r.Bounds.X >= 0 && r.Bounds.Right == screen.Bounds.X)
			.OrderBy(r => r.GetDpi())
			.FirstOrDefault();

		static swf.Screen FindRightScreen(this swf.Screen screen) => 
			swf.Screen.AllScreens
			.Where(r => r.Bounds.X < 0 && r.Bounds.X == screen.Bounds.Right)
			.OrderBy(r => r.GetDpi())
			.FirstOrDefault();

		static swf.Screen FindTopScreen(this swf.Screen screen) =>
			swf.Screen.AllScreens
			.Where(r => r.Bounds.Y >= 0 && r.Bounds.Bottom == screen.Bounds.Y)
			.OrderBy(r => r.GetDpi())
			.FirstOrDefault();

		static swf.Screen FindBottomScreen(this swf.Screen screen) =>
			swf.Screen.AllScreens
			.Where(r => r.Bounds.Y < 0 && r.Bounds.Y == screen.Bounds.Bottom)
			.OrderBy(r => r.GetDpi())
			.FirstOrDefault();

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

		public static float GetMaxLogicalPixelSize() => swf.Screen.AllScreens.Max(r => r.GetLogicalPixelSize());

		public static Eto.Drawing.RectangleF GetLogicalBounds(this swf.Screen screen)
		{
			return new Eto.Drawing.RectangleF(GetLogicalLocation(screen), GetLogicalSize(screen));
		}

		public static Eto.Drawing.PointF GetLogicalLocation(this swf.Screen screen)
		{
			var bounds = screen.Bounds;
			float x, y;
			if (bounds.X > 0)
			{
				var left = screen.FindLeftScreen();
				if (left != null)
					x = GetLogicalLocation(left).X + GetLogicalSize(left).Width;
				else
					x = bounds.X / GetMaxLogicalPixelSize();
			}
			else if (bounds.X < 0)
			{
				var right = screen.FindRightScreen();
				if (right != null)
					x = GetLogicalLocation(right).X - GetLogicalSize(screen).Width;
				else
					x = bounds.X / screen.GetLogicalPixelSize();
			}
			else x = bounds.X;
			if (bounds.Y > 0)
			{
				var top = screen.FindTopScreen();
				if (top != null)
					y = GetLogicalLocation(top).Y + GetLogicalSize(top).Height;
				else
					y = bounds.Y / GetMaxLogicalPixelSize();
			}
			else if (bounds.Y < 0)
			{
				var bottom = screen.FindBottomScreen();
				if (bottom != null)
					y = GetLogicalLocation(bottom).Y - GetLogicalSize(screen).Height;
				else
					y = bounds.Y / screen.GetLogicalPixelSize();
			}
			else y = bounds.Y;
			return new Eto.Drawing.PointF(x, y);
		}

		public static Eto.Drawing.SizeF GetLogicalSize(this swf.Screen screen) => (Eto.Drawing.SizeF)screen.Bounds.Size.ToEto() / screen.GetLogicalPixelSize();

		public static float GetLogicalPixelSize(this swf.Screen screen) => GetDpi(screen) / 96f;

		public static uint GetDpi(this System.Windows.Forms.Screen screen)
		{
			if (!MonitorDpiSupported)
			{
				// fallback to slow method if we can't get the dpi from the system
				using (var form = new System.Windows.Forms.Form { Bounds = screen.Bounds })
				using (var graphics = form.CreateGraphics())
				{
					return (uint)graphics.DpiY;
				}
			}

			var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
			var mon = MonitorFromPoint(pnt, MONITOR.DEFAULTTONEAREST);
			uint dpiX, dpiY;
			GetDpiForMonitor(mon, MDT.EFFECTIVE_DPI, out dpiX, out dpiY);
			return dpiX;
		}

		[DllImport("User32.dll")]
		public static extern IntPtr MonitorFromPoint(System.Drawing.Point pt, MONITOR dwFlags);

		[DllImport("user32.dll")]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MONITOR flags);

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
