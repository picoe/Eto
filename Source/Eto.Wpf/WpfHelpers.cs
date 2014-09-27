using System;
using Eto.Forms;
using Eto.Wpf.Forms;
using sw = System.Windows;

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
			if (attach)
				control.AttachNative();
			return control.GetContainerControl();
		}
	}
}