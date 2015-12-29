using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms.Menu
{
	public class ContextMenuHandler : MenuHandler<Gtk.Menu, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler
	{
		public ContextMenuHandler()
		{
			Control = new Gtk.Menu();
			Control.ShowAll();
		}

		protected override void Initialize()
		{
			Control.KeyPressEvent += Connector.HandleKeyPressEvent;
			Control.AccelGroup = new Gtk.AccelGroup();
			SetAccelGroup(Control.AccelGroup);
			base.Initialize();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ContextMenu.OpeningEvent:
					Control.Shown += Connector.HandleMenuOpening;
					break;
				case ContextMenu.ClosedEvent:
					Control.Hidden += Connector.HandleMenuClosed;
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new ContextMenuConnector Connector
		{
			get { return (ContextMenuConnector)base.Connector; }
		}

		protected override WeakConnector CreateConnector()
		{
			return new ContextMenuConnector();
		}

		protected class ContextMenuConnector : WeakConnector
		{
			public new ContextMenuHandler Handler
			{
				get { return (ContextMenuHandler)base.Handler; }
			}

			public void HandleMenuOpening(object sender, EventArgs e)
			{
				Handler.Callback.OnOpening(Handler.Widget, EventArgs.Empty);
			}

			public void HandleMenuClosed(object sender, EventArgs e)
			{
				Handler.Callback.OnClosed(Handler.Widget, EventArgs.Empty);
			}

			[GLib.ConnectBefore]
			public void HandleKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
			{
				// Handle pressing shortcut keys when the context menu is open
				var shortcut = args.Event.Key.ToEto() | args.Event.State.ToEtoKey();
				if (shortcut == Keys.None)
					return;
				var item = Handler.Widget.GetChildren().FirstOrDefault(r => r.Shortcut == shortcut);
				if (item != null)
				{
					Handler.Control.Hide();
					item.PerformClick();
				}
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.Insert((Gtk.Widget)item.ControlObject, index);
			SetChildAccelGroup(item);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.Remove((Gtk.Widget)item.ControlObject);
		}

		public void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}

		protected override Keys GetShortcut()
		{
			return Keys.None;
		}

		List<Gtk.Menu> attachedSubmenus;

		public void Show(Control relativeTo)
		{
			// trap key events to handle shortcut keys
			attachedSubmenus = Widget.GetChildren()
				.Select(r => r.Handler)
				.OfType<ButtonMenuItemHandler>()
				.Select(r => r.Control.Submenu)
				.OfType<Gtk.Menu>()
				.ToList();
			attachedSubmenus.ForEach(r => r.KeyPressEvent += Connector.HandleKeyPressEvent);
			Widget.Closed += HandleMenuClosed;

			ValidateItems();
			Control.ShowAll();
			Control.Popup();
		}

		void HandleMenuClosed (object sender, EventArgs e)
		{
			// clean up attached key events
			if (attachedSubmenus != null)
			{
				attachedSubmenus.ForEach(r => r.KeyPressEvent -= Connector.HandleKeyPressEvent);
				attachedSubmenus = null;
			}
			Widget.Closed -= HandleMenuClosed;
		}
	}
}
