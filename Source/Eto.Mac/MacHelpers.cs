using System;
using Eto.Forms;
using Eto.Mac.Forms;
using Eto.Mac.Forms.Controls;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
#else
using MonoMac.AppKit;
#endif

namespace Eto.Forms
{
	public static class
	#if XAMMAC2
	XamMac2Helpers
	#elif XAMMAC
	XamMacHelpers
	#elif Mac64
	MonoMac64Helpers
	#elif MONOMAC
	MonoMacHelpers
	#endif
	{
		/// <summary>
		/// Gets the native Mac NSView that contains the Eto.Forms control.
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
		public static NSView ToNative(this Control control, bool attach = false)
		{
			if (attach && !control.Loaded)
			{
				control.AttachNative();
				var macControl = control.GetMacControl();
				if (macControl != null && macControl.AutoSize)
					macControl.ContainerControl.SetFrameSize(macControl.GetPreferredSize(SizeF.MaxValue).ToSD());
			}
			return control.GetContainerView();
		}

		/// <summary>
		/// Wraps the specified <paramref name="view"/> to an Eto control that can be used directly in Eto.Forms code.
		/// </summary>
		/// <returns>The eto control wrapper around the native control.</returns>
		/// <param name="view">Native control to wrap.</param>
		public static Control ToEto(this NSView view)
		{
			return new Control(new NativeControlHandler(view));
		}

		/// <summary>
		/// Wraps the specified <paramref name="viewController"/> to an Eto control that can be used directly in Eto.Forms code.
		/// </summary>
		/// <returns>The eto control wrapper around the native control.</returns>
		/// <param name="viewController">Native control to wrap.</param>
		public static Control ToEto(this NSViewController viewController)
		{
			return new Control(new NativeControlHandler(viewController));
		}

		/// <summary>
		/// Wraps the specified Cocoa <paramref name="window"/> in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <returns>The eto window wrapper around the native Cocoa window.</returns>
		/// <param name="window">Cocoa Window to wrap.</param>
		public static Window ToEtoWindow(this NSWindow window)
		{
			return new Form(new FormHandler(window));
		}

		/// <summary>
		/// Wraps the specified Cocoa <paramref name="windowController"/> in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <returns>The eto window wrapper around the native Cocoa window.</returns>
		/// <param name="windowController">Cocoa Window to wrap.</param>
		public static Window ToEtoWindow(this NSWindowController windowController)
		{
			return new Form(new FormHandler(windowController));
		}
	}
}

