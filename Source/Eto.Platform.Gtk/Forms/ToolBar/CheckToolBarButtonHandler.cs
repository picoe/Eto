using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	
	public class CheckToolBarButtonHandler : ToolBarItemHandler<Gtk.ToggleToolButton, CheckToolBarButton>, ICheckToolBarButton
	{
		bool ischecked;
		bool isBeingChecked;
		
		#region ICheckToolBarButton Members
		
		
		public bool Checked
		{
			get { return (Control != null) ? Control.Active : this.ischecked; }
			set
			{
				if (value != ischecked)
				{
					isBeingChecked = true;
					if (Control != null) Control.Active = value;
					isBeingChecked = false;
					this.ischecked = value;
				}
			}
		}
		
		#endregion
		
		public override void CreateControl(ToolBarHandler handler)
		{
			Gtk.Toolbar tb = (Gtk.Toolbar)handler.ControlObject;
			
			Control = new Gtk.ToggleToolButton();
			Control.Active = ischecked;
			Control.Label = this.Text;
			//Control.TooltipText = this.ToolTip;
			Control.IconWidget = Image;
			Control.Toggled += button_Toggled;
			Control.Sensitive = this.Enabled;
			Control.CanFocus = false;
			Control.IsImportant = true;
			tb.Insert(Control, -1);
			if (tb.Visible) Control.ShowAll();
		}
		
		private void button_Toggled(Object sender, EventArgs e)
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
