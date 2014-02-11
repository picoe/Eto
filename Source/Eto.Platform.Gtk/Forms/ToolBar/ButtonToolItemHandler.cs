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
			Control.IsImportant = true;
			Control.Sensitive = Enabled;
			//Control.TooltipText = this.ToolTip;
			//control.CanFocus = false;
			tb.Insert(Control, -1);
			if (tb.Visible) Control.ShowAll();
			Control.Clicked += Connector.HandleClicked;
		}

		protected new ButtonToolItemConnector Connector { get { return (ButtonToolItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ButtonToolItemConnector();
		}

		protected class ButtonToolItemConnector : WeakConnector
		{
			public new ButtonToolItemHandler Handler { get { return (ButtonToolItemHandler)base.Handler; } }

			public void HandleClicked(object sender, EventArgs e)
			{
				Handler.Widget.OnClick(e);
			}
		}
	}
}
