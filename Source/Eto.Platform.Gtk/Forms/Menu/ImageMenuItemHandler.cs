using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class ImageMenuItemHandler : MenuActionItemHandler<Gtk.ImageMenuItem, ImageMenuItem>, IImageMenuItem
	{
		string tooltip;
		string text;
		Key shortcut;
		Image image;
		readonly Gtk.AccelLabel label;
		
		public ImageMenuItemHandler()
		{
			Control = new Gtk.ImageMenuItem();
			Control.Activated += control_Activated;
			label = new Gtk.AccelLabel(string.Empty);
			label.Xalign = 0;
			label.UseUnderline = true;
			label.AccelWidget = Control;
			Control.Add(label);
		}
		
		public bool Enabled
		{
			get { return Control.Sensitive; }
			set { Control.Sensitive = value; }
		}
		

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				//string val = (shortcutText.Length > 0) ? text+"\t"+shortcutText : text;
				label.Text = GtkControl<Gtk.Widget, Control>.StringToMnuemonic(text);
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
		

		public Key Shortcut
		{
			get { return shortcut; }
			set
			{
				shortcut = value;
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Image = image.ToGtk (Gtk.IconSize.Menu);
			}
		}

		public override void AddMenu(int index, MenuItem item)
		{
			if (Control.Submenu == null) Control.Submenu = new Gtk.Menu();
			((Gtk.Menu)Control.Submenu).Insert((Gtk.Widget)item.ControlObject, index);
		}

		public override void RemoveMenu(MenuItem item)
		{
			if (Control.Submenu == null) return;
			var menu = (Gtk.Menu)Control.Submenu;
			menu.Remove((Gtk.Widget)item.ControlObject);
			if (menu.Children.Length == 0)
			{
				Control.Submenu = null;
			}
		}

		public override void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}
		void control_Activated(object sender, EventArgs e)
		{
			if (Control.Submenu != null)
				ValidateItems ();
			Widget.OnClick(e);
		}
	}
}
