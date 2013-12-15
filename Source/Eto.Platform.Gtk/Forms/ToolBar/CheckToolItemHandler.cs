using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	
	public class CheckToolItemHandler : ToolItemHandler<Gtk.ToggleToolButton, CheckToolItem>, ICheckToolItem
	{
		bool ischecked;
		bool isBeingChecked;
		
		#region ICheckToolBarButton Members
		
		
		public bool Checked
		{
			get { return (Control != null) ? Control.Active : ischecked; }
			set
			{
				if (value != ischecked)
				{
					isBeingChecked = true;
					if (Control != null) Control.Active = value;
					isBeingChecked = false;
					ischecked = value;
				}
			}
		}
		
		#endregion
		
		public override void CreateControl(ToolBarHandler handler)
		{
			Gtk.Toolbar tb = handler.Control;
			
			Control = new Gtk.ToggleToolButton();
			Control.Active = ischecked;
			Control.Label = Text;
			//Control.TooltipText = this.ToolTip;
			Control.IconWidget = GtkImage;
			Control.Toggled += button_Toggled;
			Control.Sensitive = Enabled;
			Control.CanFocus = false;
			Control.IsImportant = true;
			tb.Insert(Control, -1);
			if (tb.Visible) Control.ShowAll();
		}
		
		void button_Toggled(Object sender, EventArgs e)
		{
			if (!isBeingChecked)
			{
				isBeingChecked = true;
				Control.Active = ischecked;
				isBeingChecked = false;
				Widget.OnClick(EventArgs.Empty);
			}
		}
		
	}
}
