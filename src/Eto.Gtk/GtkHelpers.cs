using System;
using Eto.Forms;
using Eto.GtkSharp;
using Eto.GtkSharp.Forms.Controls;
using Eto.GtkSharp.Forms;

namespace Eto.Forms
{
	public static class
	#if GTK2
		Gtk2Helpers
	#elif GTK3
		Gtk3Helpers
	#endif
	{
		/// <summary>
		/// Gets the native Gtk widget that contains the Eto.Forms control.
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
		public static Gtk.Widget ToNative(this Control control, bool attach = false)
		{
			if (control == null)
				return null;
			if (attach && !control.Loaded)
			{
				control.AttachNative();
				control.GetContainerWidget().ShowAll();
			}
			return control.GetContainerWidget();
		}

		/// <summary>
		/// Wraps the specified <paramref name="nativeWidget"/> to an Eto control that can be used directly in Eto.Forms code.
		/// </summary>
		/// <returns>The eto control wrapper around the native control.</returns>
		/// <param name="nativeWidget">Native control to wrap.</param>
		public static Control ToEto(this Gtk.Widget nativeWidget)
		{
			if (nativeWidget == null)
				return null;
			return new Control(new NativeControlHandler(nativeWidget));
		}

		/// <summary>
		/// Wraps the specified Gtk <paramref name="window"/> in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <returns>The eto window wrapper around the native Gtk window.</returns>
		/// <param name="window">Gtk Window to wrap.</param>
		public static Window ToEtoWindow(this Gtk.Window window)
		{
			if (window == null)
				return null;
			return new Form(new FormHandler(window));
		}

	}
}

