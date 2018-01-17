using System;
using System.Globalization;
using System.Runtime.InteropServices;
using sw = System.Windows;
using swm = System.Windows.Media;

namespace Eto.Wpf.CustomControls
{
	/// <summary>
	/// Aero Glass helper class
	/// </summary>
	public class GlassHelper
	{
		/// <summary>
		/// Structure to hold margins for the aero glass frame
		/// </summary>
		[StructLayout (LayoutKind.Sequential)]
		public struct MARGINS
		{
			#region Fields

			public int Left;

			public int Right;

			public int Top;

			public int Bottom;

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of the MARGINS struct
			/// </summary>
			/// <param name="t">
			/// The supplied thickness
			/// </param>
			public MARGINS (sw.Thickness t)
			{
				this.Left = (int)t.Left;
				this.Right = (int)t.Right;
				this.Top = (int)t.Top;
				this.Bottom = (int)t.Bottom;
			}

			#endregion
		}

		[Flags]
		enum DwmBlurBehindFlags : uint
		{
			// Analysis disable InconsistentNaming

			/// <summary>
			/// Indicates a value for fEnable has been specified.
			/// </summary>
			DWM_BB_ENABLE = 0x00000001,

			/// <summary>
			/// Indicates a value for hRgnBlur has been specified.
			/// </summary>
			DWM_BB_BLURREGION = 0x00000002,

			/// <summary>
			/// Indicates a value for fTransitionOnMaximized has been specified.
			/// </summary>
			DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004
		}

		enum DwmWindowAttribute : uint
		{
			DWMWA_NCRENDERING_ENABLED = 1,
			DWMWA_NCRENDERING_POLICY,
			DWMWA_TRANSITIONS_FORCEDISABLED,
			DWMWA_ALLOW_NCPAINT,
			DWMWA_CAPTION_BUTTON_BOUNDS,
			DWMWA_NONCLIENT_RTL_LAYOUT,
			DWMWA_FORCE_ICONIC_REPRESENTATION,
			DWMWA_FLIP3D_POLICY,
			DWMWA_EXTENDED_FRAME_BOUNDS,
			DWMWA_HAS_ICONIC_BITMAP,
			DWMWA_DISALLOW_PEEK,
			DWMWA_EXCLUDED_FROM_PEEK,
			DWMWA_LAST
		}

		[StructLayout (LayoutKind.Sequential)]
		struct DWM_BLURBEHIND
		{
			public DwmBlurBehindFlags dwFlags;
			public bool fEnable;
			public IntPtr hRgnBlur;
			public bool fTransitionOnMaximized;
		}

		[DllImport ("dwmapi.dll")]
		static extern IntPtr DwmEnableBlurBehindWindow (IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);


		public static bool BlurBehindWindow (sw.Window window)
		{
			if (!DwmIsCompositionEnabled ())
				return false;

			var windowInteropHelper = new sw.Interop.WindowInteropHelper (window);
			IntPtr myHwnd = windowInteropHelper.Handle;
			var mainWindowSrc = System.Windows.Interop.HwndSource.FromHwnd (myHwnd);

			window.Background = swm.Brushes.Transparent;
			mainWindowSrc.CompositionTarget.BackgroundColor = swm.Colors.Transparent;

			var blurBehindParameters = new DWM_BLURBEHIND ();
			blurBehindParameters.dwFlags = DwmBlurBehindFlags.DWM_BB_ENABLE;
			blurBehindParameters.fEnable = true;
			blurBehindParameters.hRgnBlur = IntPtr.Zero;

			DwmEnableBlurBehindWindow (myHwnd, ref blurBehindParameters);
			int val = 1;
			DwmSetWindowAttribute (myHwnd, DwmWindowAttribute.DWMWA_TRANSITIONS_FORCEDISABLED, ref val, sizeof (int));
			return true;
		}

		public static bool ExtendGlassFrame (sw.Window window)
		{
			return ExtendGlassFrame (window, new sw.Thickness (-1));
		}

		/// <summary>
		/// Extends the aero glass to the edge of the window
		/// </summary>
		/// <param name="window">
		/// The window to be affected
		/// </param>
		/// <param name="margin">
		/// The window margins
		/// </param>
		/// <returns>
		/// Returns true/false depending on success
		/// </returns>
		public static bool ExtendGlassFrame (sw.Window window, sw.Thickness margin)
		{
			try {
				// desktop window manader must be enabled if it isn't don't bother trying to add glass
				if (!DwmIsCompositionEnabled ())
					return false;

				IntPtr hwnd = new sw.Interop.WindowInteropHelper (window).Handle;

				if (hwnd == IntPtr.Zero) {
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The Window must be shown before extending glass."));
				}

				// Set the background to transparent from both the WPF and Win32 perspectives
				window.Background = swm.Brushes.Transparent;
				sw.Interop.HwndSource.FromHwnd (hwnd).CompositionTarget.BackgroundColor = swm.Colors.Transparent;

				var margins = new MARGINS (margin);
				DwmExtendFrameIntoClientArea (hwnd, ref margins);
				return true;
			}
			catch {
				return false;
			}
		}

		/// <summary>
		/// Extend the frame into the client area
		/// </summary>
		/// <param name="hwnd">
		/// The handle to be used
		/// </param>
		/// <param name="margins">
		/// The window margins
		/// </param>
		[DllImport ("dwmapi.dll", PreserveSig = false)]
		static extern void DwmExtendFrameIntoClientArea (IntPtr hwnd, ref MARGINS margins);

		/// <summary>
		/// See if the desktop window manager composition is enabled
		/// </summary>
		/// <returns>
		/// Returns true/false based on whether DWM is endabled
		/// </returns>
		[DllImport ("dwmapi.dll", PreserveSig = false)]
		static extern bool DwmIsCompositionEnabled ();

		[DllImport ("dwmapi.dll", PreserveSig = false)]
		static extern void DwmSetWindowAttribute (IntPtr hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize);

	}
}
