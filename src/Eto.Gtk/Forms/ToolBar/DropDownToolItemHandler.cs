using Eto.Drawing;
using Eto.Forms;
using System;

namespace Eto.GtkSharp.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<Gtk.ToolButton, DropDownToolItem>, DropDownToolItem.IHandler
	{
		ContextMenu contextMenu;

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			Gtk.Toolbar tb = handler.Control;

			Control = new Gtk.ToolButton(GtkImage, Text);
			Control.IsImportant = true;
			Control.Sensitive = Enabled;
			Control.TooltipText = this.ToolTip;
			Control.ShowAll();
			Control.NoShowAll = true;
			Control.Visible = Visible;
			//control.CanFocus = false;			// why is this disabled and not true???
			tb.Insert(Control, index);
			Control.Clicked += Connector.HandleClicked;
		}

		protected new DropDownToolItemConnector Connector { get { return (DropDownToolItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new DropDownToolItemConnector();
		}

		protected class DropDownToolItemConnector : WeakConnector
		{
			public new DropDownToolItemHandler Handler { get { return (DropDownToolItemHandler)base.Handler; } }

			PointF showLocation;

			public void HandleClicked(object sender, EventArgs e)
			{
				Handler?.Widget.OnClick(e);

				var ctxMenu = Handler.ContextMenu.ControlObject as Gtk.Menu;
				if (ctxMenu != null)
				{
					var buttonRect = Handler.Control.Clip;
					var pt = new PointF(buttonRect.Left, buttonRect.Bottom);

					var parentWindow = (Handler.Widget.Parent as Eto.Forms.ToolBar).Parent as Window;
					showLocation = parentWindow.PointToScreen(pt);

					ctxMenu.Popup(null, null, PopupMenuPosition, 3u, Gtk.Global.CurrentEventTime);
				}
			}

			void PopupMenuPosition(Gtk.Menu menu, out int x, out int y, out bool push_in)
			{
				x = (int)showLocation.X;
				y = (int)showLocation.Y;
				push_in = false;
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; }
		}
	}
}
