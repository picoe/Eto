using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Menu
{
	public class ContextMenuHandler : MenuHandler<Gtk.Menu, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler
	{
		public ContextMenuHandler()
		{
			Control = new Gtk.Menu();
			Control.ShowAll();
		}

		internal event EventHandler Changed;

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
					HandleEvent(ContextMenu.ClosingEvent);
					break;
				case ContextMenu.ClosingEvent:
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
				var h = Handler;
				// before menuitem click is processed
				h.Callback.OnClosing(h.Widget, EventArgs.Empty);
				// call OnClosed after menuitem click is processed
				Application.Instance.AsyncInvoke(() => h.Callback.OnClosed(h.Widget, EventArgs.Empty));
			}

			[GLib.ConnectBefore]
			public void HandleKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
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
			Changed?.Invoke(this, EventArgs.Empty);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.Remove((Gtk.Widget)item.ControlObject);
			Changed?.Invoke(this, EventArgs.Empty);
		}

		public void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
			Changed?.Invoke(this, EventArgs.Empty);
		}

		protected override Keys GetShortcut()
		{
			return Keys.None;
		}

		List<Gtk.Menu> attachedSubmenus;

		void Prepare()
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
		}

		PointF? showLocation;

		public void Show(Control relativeTo, PointF? location)
		{
			Prepare();
			if (location != null)
			{
				showLocation = relativeTo?.PointToScreen(location.Value) ?? location;
				Control.Popup(null, null, PopupMenuPosition, 3u, Gtk.Global.CurrentEventTime);
			}
			else
			{
				showLocation = null;
				Control.Popup();
			}
		}

		void PopupMenuPosition(Gtk.Menu menu, out int x, out int y, out bool push_in)
		{
			var location = showLocation ?? Mouse.Position;
			x = (int)location.X;
			y = (int)location.Y;
			push_in = false;
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
