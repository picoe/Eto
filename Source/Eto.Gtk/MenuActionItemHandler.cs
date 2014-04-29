using System;
using Eto.Forms;

namespace Eto.GtkSharp
{
	public interface IMenuActionItemHandler
	{
		void TriggerValidate();
	}

	public abstract class MenuActionItemHandler<TControl, TWidget> : MenuHandler<TControl, TWidget>, IMenuActionItemHandler
		where TControl: Gtk.MenuItem
		where TWidget: MenuItem
	{
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case MenuItem.ValidateEvent:
				// handled by the contextmenu/menubar
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void TriggerValidate()
		{
			if (Control.Submenu != null)
			{
				Widget.OnValidate(EventArgs.Empty);
			}
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}

