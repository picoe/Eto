using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{

	public class ButtonToolItemHandler : ToolItemHandler<Gtk.ToolButton, ButtonToolItem>, IButtonToolItem
	{

		#region IToolBarButton Members

		
		#endregion

		public override void CreateControl(ToolBarHandler handler)
		{
			Gtk.Toolbar tb = handler.Control;

			Control = new Gtk.ToolButton(GtkImage, Text);
			Control.Clicked += button_Clicked;
			Control.IsImportant = true;
			Control.Sensitive = Enabled;
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
