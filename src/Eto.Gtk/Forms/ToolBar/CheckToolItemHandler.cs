using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.ToolBar
{
	public class CheckToolItemHandler : ToolItemHandler<Gtk.ToggleToolButton, CheckToolItem>, CheckToolItem.IHandler
	{
		bool ischecked;

		public bool Checked
		{
			get { return (Control != null) ? Control.Active : ischecked; }
			set
			{
				if (value != ischecked)
				{
					if (Control != null)
						Control.Active = value;
					ischecked = value;
				}
			}
		}

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			Gtk.Toolbar tb = handler.Control;
			
			Control = new Gtk.ToggleToolButton();
			Control.Active = ischecked;
			Control.Label = Text;
			Control.TooltipText = this.ToolTip;
			Control.IconWidget = GtkImage;
			Control.Sensitive = Enabled;
			Control.CanFocus = false;
			Control.IsImportant = true;
			Control.ShowAll();
			Control.NoShowAll = true;
			Control.Visible = Visible;
			tb.Insert(Control, index);
			Control.Toggled += Control_CheckedChanged;
			Control.Clicked += Control_Click;
		}

		private void Control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		private void Control_CheckedChanged(object sender, EventArgs e)
		{
			Widget.OnCheckedChanged(EventArgs.Empty);
		}
	}
}
