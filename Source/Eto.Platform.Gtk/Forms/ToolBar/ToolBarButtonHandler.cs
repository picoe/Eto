using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{

	public class ToolBarButtonHandler : ToolBarItemHandler<Gtk.ToolButton, ToolBarButton>, IToolBarButton
	{

		#region IToolBarButton Members

		
		#endregion

		public override void CreateControl(ToolBarHandler handler)
		{
			Gtk.Toolbar tb = (Gtk.Toolbar)handler.ControlObject;

			Control = new Gtk.ToolButton(Image, this.Text);
			Control.Clicked += button_Clicked;
			Control.IsImportant = true;
			Control.Sensitive = this.Enabled;
			//Control.TooltipText = this.ToolTip;
			//control.CanFocus = false;
			tb.Insert(Control, -1);
			if (tb.Visible) Control.ShowAll();
		}

		void button_Clicked(object sender, EventArgs e)
		{
			Widget.OnClick(e);
		}
	}
}
