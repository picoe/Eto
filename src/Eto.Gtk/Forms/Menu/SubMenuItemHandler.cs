using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Menu
{
	public class SubMenuItemHandler : ButtonMenuItemHandler<SubMenuItem, SubMenuItem.ICallback>, SubMenuItem.IHandler
	{
		public SubMenuItemHandler()
		{
			Control.Submenu = new Gtk.Menu();
		}

		public override void RemoveMenu(MenuItem item) => Submenu.Remove((Gtk.Widget)item.ControlObject);

		public override void Clear() => Submenu.RemoveAllChildren();

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case SubMenuItem.OpeningEvent:
					Submenu.Shown += Connector.HandleMenuOpening;
					break;
				case SubMenuItem.ClosingEvent:
					HandleEvent(SubMenuItem.ClosedEvent);
					break;
				case SubMenuItem.ClosedEvent:
					Control.Deselected += Connector.HandleMenuClosed;
					Control.Hidden += Connector.HandleMenuClosed;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new SubMenuItemConnector Connector => (SubMenuItemConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new SubMenuItemConnector();

		protected class SubMenuItemConnector : ButtonMenuItemConnector
		{
			public new SubMenuItemHandler Handler => (SubMenuItemHandler)base.Handler;

			public void HandleMenuOpening(object sender, EventArgs e)
			{
				Handler?.Callback.OnOpening(Handler.Widget, EventArgs.Empty);
			}

			public void HandleMenuClosed(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				// before menuitem click is processed
				h.Callback.OnClosing(h.Widget, EventArgs.Empty);
				// call OnClosed after menuitem click is processed
				Application.Instance.AsyncInvoke(() => h.Callback.OnClosed(h.Widget, EventArgs.Empty));
			}
		}
	}
}
