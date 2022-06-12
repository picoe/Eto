using System;
using Eto.Forms;
using Eto.WinForms;
using swf = System.Windows.Forms;
using Eto.WinForms.Forms.Controls;
using Eto.WinForms.Forms;

namespace Eto.Forms
{
	public static class WinFormsHelpers
	{
		/// <summary>
		/// Gets the native WinForms control that contains the Eto.Forms control.
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
		public static swf.Control ToNative(this Control control, bool attach = false)
		{
			if (control == null)
				return null;
			if (attach && !control.Loaded)
				control.AttachNative();
			return control.GetContainerControl();
		}

		/// <summary>
		/// Gets the native WinForms window of the specified Eto window
		/// </summary>
		/// <param name="window">Eto window to get the native control for</param>
		/// <returns>The native WinForms window object.</returns>
		public static swf.Form ToNative(this Window window)
		{
			return window.ToSWF();
		}

		/// <summary>
		/// Wraps the specified <paramref name="nativeControl"/> to an Eto control that can be used directly in Eto.Forms code.
		/// </summary>
		/// <returns>The eto control wrapper around the native control.</returns>
		/// <param name="nativeControl">Native control to wrap.</param>
		public static Control ToEto(this swf.Control nativeControl)
		{
			if (nativeControl == null)
				return null;
			return new Control(new NativeControlHandler(nativeControl));
		}

		/// <summary>
		/// Wraps the specified WinForms <paramref name="window"/> in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <returns>The eto window wrapper around the native WinForms window.</returns>
		/// <param name="window">WinForms Window to wrap.</param>
		public static Window ToEtoWindow(this swf.Form window)
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
	}
}