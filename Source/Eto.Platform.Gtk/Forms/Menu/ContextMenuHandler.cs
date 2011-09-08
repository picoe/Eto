using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class ContextMenuHandler : MenuHandler<Gtk.Menu, ContextMenu>, IContextMenu
	{

		public ContextMenuHandler()
		{
			Control = new Gtk.Menu();
		}

		public override void AddMenu(int index, MenuItem item)
		{
			Control.Insert((Gtk.Widget)item.ControlObject, index);
			
		}

		public override void RemoveMenu(MenuItem item)
		{
			Control.Remove((Gtk.Widget)item.ControlObject);
		}

		public override void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}

	}
}
