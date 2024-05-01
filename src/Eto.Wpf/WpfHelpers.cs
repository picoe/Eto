using Eto.Wpf.Forms;
using Eto.Wpf.Forms.Controls;

namespace Eto.Forms
{
	public static class WpfHelpers
	{
		/// <summary>
		/// Gets the native Wpf framework element that contains the Eto.Forms control.
		/// </summary>
		/// <remarks>
		/// Note for some controls, this will not be the 'main' native control.
		/// For example, a GridView on OS X will return a NSScrollView instead of a NSTableView, since the table view
		/// itself does not scroll.
		/// 
		/// When you intend on using the control inside an existing native application, set <paramref name="attach"/> to
		/// true so that it can prepare for attaching to the native application by sending OnPreLoad/Load/LoadComplete events. 
		/// </remarks>
		/// <returns>The native control that can be used to add this control to an existing application.</returns>
		/// <param name="control">Control to get the native control for.</param>
		/// <param name="attach">If set to <c>true</c> the control is to be attached to an existing application, or <c>false</c> to get the native control directly.</param>
		public static sw.FrameworkElement ToNative(this Control control, bool attach = false)
		{
			if (control == null)
				return null;
            if (attach && !control.Loaded)
            {
                control.AttachNative();
                var handler = control.GetWpfFrameworkElement();
                if (handler != null)
                    handler.SetScale(false, false);
            }
			return control.GetContainerControl();
		}

		/// <summary>
		/// Gets the native WPF window of the specified Eto window
		/// </summary>
		/// <param name="window">Eto window to get the native control for</param>
		/// <returns>The native WPF window object.</returns>
		public static sw.Window ToNative(this Window window)
		{
			if (window == null)
				return null;
			return (window.Handler as IWpfWindow)?.Control;
		}

		/// <summary>
		/// Wraps the specified <paramref name="nativeControl"/> to an Eto control that can be used directly in Eto.Forms code.
		/// </summary>
		/// <returns>The eto control wrapper around the native control.</returns>
		/// <param name="nativeControl">Native control to wrap.</param>
		public static Control ToEto(this sw.FrameworkElement nativeControl)
		{
			if (nativeControl == null)
				return null;
			return new NativeControlHost(nativeControl);
		}

		/// <summary>
		/// Wraps the specified WPF <paramref name="window"/> in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <returns>The eto window wrapper around the native WPF window.</returns>
		/// <param name="window">WPF Window to wrap.</param>
		public static Window ToEtoWindow(this sw.Window window)
		{
			if (window == null)
				return null;
			return new Form(new NativeFormHandler(window));
		}

		/// <summary>
		/// Wraps a native win32 window in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <remarks>
		/// This is useful when your application is fully native and does not use WinForms or Wpf.
		/// </remarks>
		/// <returns>The eto window wrapper around the win32 window with the specified handle.</returns>
		/// <param name="windowHandle">Handle of the win32 window.</param>
		public static Window ToEtoWindow(IntPtr windowHandle)
		{
			if (windowHandle == IntPtr.Zero)
				return null;
			return new Form(new HwndFormHandler(windowHandle));
		}

		/// <summary>
		/// Converts a point in native (Windows Forms/Win32) screen coordinates to a point in Eto logical screen coordinates.
		/// </summary>
		/// <param name="point">A point in Windows Forms/win32 screen coordinates.</param>
		/// <param name="screen">The screen to translate to, if known.</param>
		/// <param name="perMonitor">True to get screen bounds/location in per monitor mode, false to remain in current mode.</param>
		/// <returns>A point in Eto screen coordinates.</returns>
		public static PointF ToEtoScreen(this Point point, Screen screen = null, bool perMonitor = false) => Win32.ScreenToLogical(point, ScreenHandler.GetControl(screen), perMonitor);

		/// <summary>
		/// Converts a rectangle in native (Windows Forms/Win32) screen coordinates to an rectangle in Eto logical screen coordinates.
		/// </summary>
		/// <param name="rect">A rectangle in Windows Forms/win32 screen coordinates.</param>
		/// <param name="screen">The screen to translate to, if known.</param>
		/// <param name="perMonitor">True to get screen bounds/location in per monitor mode, false to remain in current mode.</param>
		/// <returns>A point in Eto screen coordinates.</returns>
		public static RectangleF ToEtoScreen(this Rectangle rect, Screen screen = null, bool perMonitor = false)
		{
			var topLeft = ToEtoScreen(rect.Location, screen, perMonitor);
			var bottomRight = ToEtoScreen(new Point(rect.X + rect.Width, rect.Y + rect.Height), screen, perMonitor);
			return new RectangleF(topLeft, new SizeF(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));
		}

		/// <summary>
		/// Converts a point in Eto logical screen coordinates to a point in native (Windows Forms/win32) screen coordinates.
		/// </summary>
		/// <param name="point">A point in Eto screen coordinates.</param>
		/// <param name="screen">The screen to translate to, if known.</param>
		/// <param name="perMonitor">True to get screen bounds/location in per monitor mode, false to remain in current mode.</param>
		/// <returns>A point in Windows Forms/win32 screen coordinates.</returns>
		public static Point ToNativeScreen(this PointF point, Screen screen = null, bool perMonitor = false) => Win32.LogicalToScreen(point, screen, perMonitor);

		/// <summary>
		/// Converts a rectangle in Eto logical screen coordinates to a rectangle in native (Windows Forms/win32) screen coordinates.
		/// </summary>
		/// <param name="rectangle">A rectangle in Eto screen coordinates.</param>
		/// <param name="screen">The screen to translate to, if known.</param>
		/// <param name="perMonitor">True to get screen bounds/location in per monitor mode, false to remain in current mode.</param>
		/// <returns>A rectangle in Windows Forms/win32 screen coordinates.</returns>
		public static Rectangle ToNativeScreen(this RectangleF rect, Screen screen = null, bool perMonitor = false)
		{
			var topLeft = rect.TopLeft.ToNativeScreen(screen, perMonitor);
			var bottomRight = rect.BottomRight.ToNativeScreen(screen, perMonitor);
			return new Rectangle(topLeft, new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));
		}

	}
}