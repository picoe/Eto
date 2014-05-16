using System;
using Eto.Forms;

namespace Eto.GtkSharp
{
	public interface IMenuActionItemHandler
	{
		void TriggerValidate();
	}

	public abstract class MenuActionItemHandler<TControl, TWidget, TCallback> : MenuHandler<TControl, TWidget, TCallback>, IMenuActionItemHandler
		where TControl: Gtk.MenuItem
		where TWidget: MenuItem
		where TCallback: MenuItem.ICallback
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
			Callback.OnValidate(Widget, EventArgs.Empty);
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}

