using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public interface IMenuActionItemHandler
	{
		void TriggerValidate();
	}
	public abstract class MenuActionItemHandler<T, W> : MenuHandler<T, W>, IMenuActionItemHandler
		where T: Gtk.MenuItem
		where W: MenuActionItem
	{
		public override void AttachEvent (string id)
		{
			switch (id) {
			case MenuActionItem.ValidateEvent:
				// handled by the contextmenu/menubar
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}
		
		public void TriggerValidate ()
		{
			if (Control.Submenu != null) {
				Widget.OnValidate (EventArgs.Empty);
			}
		}
	}
}

