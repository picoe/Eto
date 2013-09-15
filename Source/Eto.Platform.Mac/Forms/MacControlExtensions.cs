using System;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms
{
	public static class MacControlExtensions
	{
		public static Size GetPreferredSize (this Control control, Size availableSize)
		{
			if (control == null)
				return Size.Empty;
			var mh = control.GetMacAutoSizing();
			if (mh != null) {
				return mh.GetPreferredSize (availableSize);
			}
			
			var c = control.ControlObject as NSControl;
			if (c != null) {
				c.SizeToFit ();
				return c.Frame.Size.ToEtoSize ();
			}
			var child = control.ControlObject as Control;
			if (child != null)
				return child.GetPreferredSize(availableSize);

			return Size.Empty;
		}

		public static IMacContainer GetMacContainer(this Control control)
		{
			if (control == null)
				return null;
			var container = control.Handler as IMacContainer;
			if (container != null)
				return container;
			var child = control.ControlObject as Control;
			if (child != null)
				return child.GetMacContainer();
			return null;
		}

		public static IMacAutoSizing GetMacAutoSizing(this Control control)
		{
			if (control == null)
				return null;
			var container = control.Handler as IMacAutoSizing;
			if (container != null)
				return container;
			var child = control.ControlObject as Control;
			if (child != null)
				return child.GetMacAutoSizing();
			return null;
		}

		public static NSView GetContainerView(this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IMacContainerControl;
			if (containerHandler != null)
				return containerHandler.ContainerControl;
			var childControl = control.ControlObject as Control;
			if (childControl != null)
				return childControl.GetContainerView();
			return control.ControlObject as NSView;
		}

		public static NSView GetContentView(this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IMacContainerControl;
			if (containerHandler != null)
				return containerHandler.ContentControl;
			var childControl = control.ControlObject as Control;
			if (childControl != null)
				return childControl.GetContentView();
			return control.ControlObject as NSView;
		}

	}
}

