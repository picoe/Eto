using System;
using System.Collections.Generic;
using System.Linq;
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

		public double Scale { get; private set; } = 1;

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
			var content = window.Content as Visual;
			if (content != null)
			{
				var wpfScale = WpfScale;
				content.SetValue(FrameworkElement.LayoutTransformProperty, new ScaleTransform(wpfScale, wpfScale));
			}
			OnScaleChanged(EventArgs.Empty);
		}

		static bool setProcessDpi;

		public PerMonitorDpiHelper(Window window)
		{
			this.window = window;
			if (!Win32.PerMonitorDpiSupported)
				return;

			if (!setProcessDpi)
			{
				Win32.SetProcessDpiAwareness(Win32.PROCESS_DPI_AWARENESS.PER_MONITOR_DPI_AWARE);
				setProcessDpi = true;
			}

			if (this.window.IsLoaded)
				AddHook();
			else
				this.window.Loaded += (o, e) => AddHook();
			this.window.Closing += (o, e) => RemoveHook();
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
			if (hwnd != IntPtr.Zero)
				SetScale(Win32.GetWindowDpi(hwnd));
		}

		void RemoveHook()
		{
			window.SourceInitialized -= Window_SourceInitialized;
			HwndSource.RemoveHook(HwndHook);
		}

		IntPtr HwndHook(IntPtr hwnd, int message, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			if (message == (int)Win32.WM.DPICHANGED)
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