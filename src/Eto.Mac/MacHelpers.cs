using System;
using Eto.Forms;
using Eto.Mac;
using Eto.Mac.Forms;
using Eto.Mac.Forms.Controls;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
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
			if (control == null)
				return null;
			if (attach && !control.Loaded)
			{
				control.AttachNative();
				var macView = control.GetMacViewHandler();

				macView?.SetAlignmentFrameSize(macView.GetPreferredSize(SizeF.PositiveInfinity).ToNS());
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
			if (view == null)
				return null;
			return new Control(new NativeControlHandler(view));
		}

		/// <summary>
		/// Wraps the specified <paramref name="viewController"/> to an Eto control that can be used directly in Eto.Forms code.
		/// </summary>
		/// <returns>The eto control wrapper around the native control.</returns>
		/// <param name="viewController">Native control to wrap.</param>
		public static Control ToEto(this NSViewController viewController)
		{
			if (viewController == null)
				return null;
			return new Control(new NativeControlHandler(viewController));
		}

		/// <summary>
		/// Wraps the specified Cocoa <paramref name="window"/> in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <returns>The eto window wrapper around the native Cocoa window.</returns>
		/// <param name="window">Cocoa Window to wrap.</param>
		public static Window ToEtoWindow(this NSWindow window)
		{
			if (window == null)
				return null;

			if (window is IMacControl macControl && macControl.WeakHandler?.Target is IMacWindow macWindow)
				return macWindow.Widget;

			return new Form(new NativeFormHandler(window));
		}

		/// <summary>
		/// Wraps the specified Cocoa <paramref name="windowController"/> in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <returns>The eto window wrapper around the native Cocoa window.</returns>
		/// <param name="windowController">Cocoa Window to wrap.</param>
		public static Window ToEtoWindow(this NSWindowController windowController)
		{
			if (windowController == null)
				return null;
			return new Form(new NativeFormHandler(windowController));
		}

		/// <summary>
		/// Gets the native NSWindow from a Window
		/// </summary>
		/// <returns>The native NSWindow that represents the Eto Window.</returns>
		/// <param name="window">Eto window object to get the native window handle from.</param>
		public static NSWindow ToNative(this Window window)
		{
			if (window == null)
				return null;
			return window.ControlObject as NSWindow;
		}


		/// <summary>
		/// Gets a value indicating that a field editor is required for the specified client
		/// </summary>
		/// <remarks>
		/// When you are embedding an Eto control inside a native NSWindow, certain events for the TextBox and similar controls
		/// may not fire as they are handled through a custom field editor.
		/// 
		/// You must wire up your native NSWindowDelegate to handle windowWillReturnFieldEditor:toObject: and call this method
		/// with the handle of the client object.
		/// 
		/// If this is true, you should call <see cref="GetFieldEditor"/> to return an instance of a field editor that can be used.
		/// </remarks>
		/// <returns><c>true</c>, if field editor is needed, <c>false</c> otherwise.</returns>
		/// <param name="clientHandle">Handle to the client object from the 2nd parameter of windowWillReturnFieldEditor:toObject:</param>
		public static bool NeedsFieldEditor(IntPtr clientHandle)
		{
			var obj = Runtime.TryGetNSObject(clientHandle) as IMacControl;
			return obj != null;
		}

		static readonly object FieldEditor_Key = new object();

		/// <summary>
		/// Gets the field editor required for the specified control.
		/// </summary>
		/// <remarks>
		/// When you are embedding an Eto control inside a native NSWindow, certain events for the TextBox and similar controls
		/// may not fire as they are handled through a custom field editor.
		/// 
		/// You must wire up your native NSWindowDelegate to handle windowWillReturnFieldEditor:toObject: and call this method
		/// with the handle of the client object.
		/// </remarks>
		/// <returns>The field editor for the specified client control, or null if none is required.</returns>
		/// <param name="clientHandle">Handle to the client object from the 2nd parameter of windowWillReturnFieldEditor:toObject:</param>
		public static NSObject GetFieldEditor(IntPtr clientHandle)
		{
			var client = Runtime.TryGetNSObject(clientHandle);
			var control = client as IMacControl;
			if (control != null)
			{
				var weakHandler = control.WeakHandler;
				if (weakHandler != null)
				{

					var childHandler = weakHandler.Target as IMacViewHandler;
					if (childHandler != null && childHandler.Widget != null)
					{
						var fieldEditor = childHandler.Widget.Properties.Get<NSObject>(FieldEditor_Key);
						if (fieldEditor == null)
						{
							fieldEditor = new MacFieldEditor();
							childHandler.Widget.Properties.Set(FieldEditor_Key, fieldEditor);
						}
						return fieldEditor;
					}
				}
			}
			return null;
		}
	}
}

