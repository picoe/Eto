using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Forms.Controls;

namespace Eto.GtkSharp.Forms.Menu
{
	public class ButtonMenuItemHandler : MenuActionItemHandler<Gtk.ImageMenuItem, ButtonMenuItem, ButtonMenuItem.ICallback>, ButtonMenuItem.IHandler
	{
		string tooltip;
		Keys shortcut;
		Image image;
		readonly Gtk.AccelLabel label;

		public ButtonMenuItemHandler()
		{
			Control = new Gtk.ImageMenuItem();
			label = new Gtk.AccelLabel(string.Empty);
			label.Xalign = 0;
			label.UseUnderline = true;
			label.AccelWidget = Control;
			Control.Add(label);
			Control.ShowAll();
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Activated += Connector.HandleActivated;
		}

		protected new ButtonMenuItemConnector Connector { get { return (ButtonMenuItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ButtonMenuItemConnector();
		}

		protected class ButtonMenuItemConnector : WeakConnector
		{
			public new ButtonMenuItemHandler Handler { get { return (ButtonMenuItemHandler)base.Handler; } }

			public void HandleActivated(object sender, EventArgs e)
			{
				if (Handler.Control.Submenu != null)
					Handler.ValidateItems();
				
				Handler.Callback.OnClick (Handler.Widget, e);
			}
		}

		public bool Enabled
		{
			get { return Control.Sensitive; }
			set { Control.Sensitive = value; }
		}

		public string Text
		{
			get { return label.Text; }
			set
			{
				label.Text = value.ToPlatformMnemonic();
				label.UseUnderline = true;
			}
		}

		public string ToolTip
		{
			get { return tooltip; }
			set
			{
				tooltip = value;
				//label.TooltipText = value;
			}
		}


		public Keys Shortcut
		{
			get { return shortcut; }
			set
			{
				shortcut = value;
				SetAccelerator();
			}
		}

		protected override Keys GetShortcut()
		{
			return Shortcut;
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Image = image.ToGtk(Gtk.IconSize.Menu);
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			if (Control.Submenu == null) Control.Submenu = new Gtk.Menu();
			((Gtk.Menu)Control.Submenu).Insert((Gtk.Widget)item.ControlObject, index);
			SetChildAccelGroup(item);
		}

		public void RemoveMenu(MenuItem item)
		{
			if (Control.Submenu == null) return;
			var menu = (Gtk.Menu)Control.Submenu;
			menu.Remove((Gtk.Widget)item.ControlObject);
			if (menu.Children.Length == 0)
			{
				Control.Submenu = null;
			}
		}

		public void Clear()
		{
			Control.Submenu = null;
		}
	}
}
