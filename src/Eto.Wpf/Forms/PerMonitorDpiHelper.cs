using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Eto.Wpf.Forms
{
	public class PerMonitorDpiHelper
	{
		Window window;
		IntPtr hwnd;
		HwndSource hwndSource;

		public event EventHandler<EventArgs> ScaleChanged;

		protected virtual void OnScaleChanged(EventArgs e)
		{
			ScaleChanged?.Invoke(this, e);
		}

		static Lazy<bool> builtInPerMonitorSupported = new Lazy<bool>(() =>
		{
			Win32.PROCESS_DPI_AWARENESS awareness;
			if (!Win32.PerMonitorDpiSupported)
				return false;
			if (Win32.GetProcessDpiAwareness(IntPtr.Zero, out awareness) != 0)
				return false;
			if (awareness != Win32.PROCESS_DPI_AWARENESS.PER_MONITOR_DPI_AWARE)
				return false;
			if (typeof(Window).GetEvent("DpiChanged") == null) // .NET 4.6.2
				return false;

			// now check if it was disabled specifically (more .NET 4.6 apis)
			var contextType = typeof(object).Assembly.GetType("System.AppContext");
			if (contextType == null)
				return false;
			var method = contextType.GetMethod("TryGetSwitch", BindingFlags.Static | BindingFlags.Public, null, new [] { typeof(string), typeof(bool).MakeByRefType() }, null);
			if (method == null)
				return false;
			var args = new object[] { "Switch.System.Windows.DoNotScaleForDpiChanges", null };
			method.Invoke(null, args);
			var doNotScaleForDpiChanges = (bool)args[1];
			return !doNotScaleForDpiChanges;
		});

		public static bool BuiltInPerMonitorSupported => builtInPerMonitorSupported.Value;

		double? _scale;
		public double Scale
		{
			get
			{
				if (_scale != null)
					return _scale.Value;
				if (hwnd == IntPtr.Zero)
					hwnd = new WindowInteropHelper(window).Handle;

				if (hwnd != IntPtr.Zero)
					return Win32.GetWindowDpi(hwnd) / 96.0;

				return 1;
			}
			set { _scale = value; }
		}

		public double WpfScale
		{
			get
			{
				return Scale / (PresentationSource.FromVisual(window)?.CompositionTarget.TransformToDevice.M11 ?? 1.0);
			}
		}

		HwndSource HwndSource
		{
			get { return hwndSource ?? (hwndSource = PresentationSource.FromVisual(window) as HwndSource); }
		}

		void SetScale(uint dpi)
		{
			var scale = dpi / 96.0;
			if (Scale == scale)
				return;
			Scale = scale;

			// set the scale for the window content
			var content = VisualTreeHelper.GetChild(window, 0);
			if (content != null)
			{
				var wpfScale = WpfScale;
				var val = content.GetValue(FrameworkElement.LayoutTransformProperty);
				content.SetValue(FrameworkElement.LayoutTransformProperty, new ScaleTransform(wpfScale, wpfScale));
			}
			OnScaleChanged(EventArgs.Empty);
		}

		static bool? dpiEventEnabled;

		public PerMonitorDpiHelper(Window window)
		{
			this.window = window;
			if (!Win32.PerMonitorDpiSupported)
				return;

			if (!BuiltInPerMonitorSupported && dpiEventEnabled == null)
			{
				dpiEventEnabled = false;
				Win32.PROCESS_DPI_AWARENESS awareness;
				var ret = Win32.GetProcessDpiAwareness(IntPtr.Zero, out awareness);
				if (ret == 0 && awareness != Win32.PROCESS_DPI_AWARENESS.PER_MONITOR_DPI_AWARE)
				{
					//dpiEventEnabled |= awareness == Win32.PROCESS_DPI_AWARENESS.SYSTEM_DPI_AWARE;

					ret = Win32.SetProcessDpiAwareness(Win32.PROCESS_DPI_AWARENESS.PER_MONITOR_DPI_AWARE);
					dpiEventEnabled |= ret == 0;
				}
			}
			else
				dpiEventEnabled = false;

			if (dpiEventEnabled.Value)
			{
				if (this.window.IsLoaded)
					AddHook();
				else
					this.window.Loaded += (o, e) => AddHook();
				this.window.Closed += (o, e) => RemoveHook();
			}
		}

		void Window_SourceInitialized(object sender, EventArgs e)
		{
			AddHook();
		}

		void AddHook()
		{
			if (!window.IsInitialized)
			{
				window.SourceInitialized += Window_SourceInitialized;
				return;
			}
			HwndSource.AddHook(HwndHook);
			hwnd = new WindowInteropHelper(window).Handle;
			if (!BuiltInPerMonitorSupported && hwnd != IntPtr.Zero)
				SetScale(Win32.GetWindowDpi(hwnd));
		}

		void RemoveHook()
		{
			window.SourceInitialized -= Window_SourceInitialized;
			HwndSource?.RemoveHook(HwndHook);
		}

		IntPtr HwndHook(IntPtr hwnd, int message, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			/* Doesn't work
			if (message == (int)Win32.WM.NCCREATE)
			{
				Win32.EnableNonClientDpiScaling(hwnd);
			}*/
			if (!BuiltInPerMonitorSupported && message == (int)Win32.WM.DPICHANGED)
			{
				var rect = (Win32.RECT)Marshal.PtrToStructure(lparam, typeof(Win32.RECT));
				SetScale(Win32.HIWORD(wparam));

				Win32.SetWindowPos(hwnd, IntPtr.Zero,
					rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top,
					Win32.SWP.NOOWNERZORDER
					| Win32.SWP.NOACTIVATE
					| Win32.SWP.NOZORDER);
			}

			return IntPtr.Zero;
		}

	}
}