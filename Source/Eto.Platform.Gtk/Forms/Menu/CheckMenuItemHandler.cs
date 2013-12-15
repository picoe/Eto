using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class CheckMenuItemHandler : MenuActionItemHandler<Gtk.CheckMenuItem, CheckMenuItem>, ICheckMenuItem
	{
		string text;
		string tooltip;
		Keys shortcut;
		readonly Gtk.AccelLabel label;

		public CheckMenuItemHandler()
		{
			Control = new Gtk.CheckMenuItem();
			Control.Toggled += control_Activated;
			label = new Gtk.AccelLabel(string.Empty);
			label.Xalign = 0;
			label.UseUnderline = true;
			label.AccelWidget = Control;
			Control.Add(label);
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				string val = text;
				label.Text = GtkControl<Gtk.Widget, Control>.StringToMnuemonic(val);
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
			set { shortcut = value; }
		}

		bool isBeingChecked;

		public bool Checked
		{
			get { return Control.Active; }
			set
			{
				isBeingChecked = true;
				Control.Active = value;
				isBeingChecked = false;
			}
		}
		
		public bool Enabled
		{
			get { return Control.Sensitive; }
			set { Control.Sensitive = value; }
		}

		void control_Activated(object sender, EventArgs e)
		{
			if (!isBeingChecked)
			{
				isBeingChecked = true;
				Control.Active = !Control.Active; // don't let Gtk turn it on/off
				isBeingChecked = false;
				Widget.OnClick(e);
			}
		}
	}
}
