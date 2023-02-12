using System;
using Eto.Drawing;
using Eto.Forms;
using Gdk;

namespace Eto.GtkSharp.Forms.ToolBar
{

	public class DropDownToolItemHandler : ToolItemHandler<Gtk.ToggleToolButton, DropDownToolItem>, DropDownToolItem.IHandler
	{
		// GTK3 MenuToolButton looks kind of horrible with a huge visually separate drop button, and forces
		// the main button and drop button to function separately. So, use a normal ToolButton with a Menu, but
		// add a drop arrow to the button text.

		private Gtk.Menu dropMenu;
		bool showDropArrow = true;

		public DropDownToolItemHandler()
		{
			dropMenu = new Gtk.Menu();
		}

		#region IToolBarButton Members


		#endregion

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			Gtk.Toolbar tb = handler.Control;

			Control = new Gtk.ToggleToolButton();
			Control.IconWidget = GtkImage;
			Control.IsImportant = true;
			Control.Sensitive = Enabled;
			Control.TooltipText = this.ToolTip;
			Control.ShowAll();
			Control.NoShowAll = true;
			Control.Visible = Visible;
			//control.CanFocus = false;			// why is this disabled and not true???
			tb.Insert(Control, index);
			Control.Clicked += HandleClicked;
			dropMenu.Hidden += HandleMenuClosed;
			SetText();
		}

		protected override void SetText()
		{
			if (Control != null)
				Control.Label = ShowDropArrow ? Text + "  ▾" : Text;
		}

		/// <summary>
		/// Gets or sets whether the drop arrow is shown on the button.
		/// </summary>
		public bool ShowDropArrow
		{
			get => showDropArrow;
			set
			{
				if (showDropArrow != value)
				{
					showDropArrow = value;
					SetText();
				}
			}
		}

#if GTKCORE
		private void HandleClicked(object sender, EventArgs e)
		{
			if (!Control.Active)
				return;

			dropMenu.PopupAtWidget(Control, Gravity.SouthWest, Gravity.NorthWest, null);
			Connector.HandleClicked(sender, e);
		}
#else
		private void HandleClicked(object sender, EventArgs e)
		{
			if (!Control.Active)
				return;
				
			var buttonRect = Control.Allocation;
			var pt = new PointF(buttonRect.Left, buttonRect.Bottom);
			var parentWindow = (Widget.Parent as Eto.Forms.ToolBar).Parent as Eto.Forms.Window;
			showLocation = parentWindow.PointToScreen(pt);
			dropMenu.Popup(null, null, PopupMenuPosition, 3u, Gtk.Global.CurrentEventTime);
		
			Connector.HandleClicked(sender, e);
		}

		PointF showLocation;

		void PopupMenuPosition(Gtk.Menu menu, out int x, out int y, out bool push_in)
		{
			x = (int)showLocation.X;
			y = (int)showLocation.Y;
			push_in = false;
		}
#endif

		private void HandleMenuClosed(Object sender, EventArgs e)
		{
			Control.Active = false;
		}

		protected new DropDownToolItemConnector Connector { get { return (DropDownToolItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new DropDownToolItemConnector();
		}

		protected class DropDownToolItemConnector : WeakConnector
		{
			public new DropDownToolItemHandler Handler { get { return (DropDownToolItemHandler)base.Handler; } }

			public void HandleClicked(object sender, EventArgs e)
			{
				Handler.Widget.OnClick(e);
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			dropMenu.Insert((Gtk.Widget)item.ControlObject, index);
			var handler = item.Handler as Menu.IMenuHandler;
			//SetChildAccelGroup(item);
		}

		public void RemoveMenu(MenuItem item)
		{
			dropMenu.Remove((Gtk.Widget)item.ControlObject);
			/*
			var handler = item.Handler as Menu.IMenuHandler;
			if (handler != null)
				handler.SetAccelGroup(null);
			*/
		}

		public void Clear()
		{
			foreach (Gtk.Widget w in dropMenu.Children)
			{
				dropMenu.Remove(w);
			}
		}
	}
}
