using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac.Forms.Menu
{
	public class ContextMenuHandler : WidgetHandler<NSMenu, ContextMenu>, ContextMenu.IHandler
	{
		public ContextMenuHandler ()
		{
			Control = new NSMenu ();
			Control.AutoEnablesItems = false;
			Control.ShowsStateColumn = true;
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.InsertItem ((NSMenuItem)item.ControlObject, index);
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.RemoveItem ((NSMenuItem)item.ControlObject);
		}

		public void Clear ()
		{
			Control.RemoveAllItems ();
		}
		
		public void Show (Control relativeTo)
		{
			NSEvent nsevent = NSApplication.SharedApplication.CurrentEvent;
			if (nsevent == null) {
				var location = NSEvent.CurrentMouseLocation;
				location = NSApplication.SharedApplication.KeyWindow.ConvertScreenToBase (location);
				
				var time = DateTime.Now.ToOADate();
				var windowNumber = NSApplication.SharedApplication.KeyWindow.WindowNumber;
				
				nsevent = NSEvent.MouseEvent(NSEventType.RightMouseDown, location, 0, time, windowNumber, null, 0, 0, 0.1f);
			}
			var view = relativeTo != null ? relativeTo.ControlObject as NSView : null;
			NSMenu.PopUpContextMenu(Control, nsevent, view);
		}

	}
}

