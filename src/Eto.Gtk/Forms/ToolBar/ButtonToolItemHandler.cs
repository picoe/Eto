using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.ToolBar
{

	public class ButtonToolItemHandler : ToolItemHandler<Gtk.ToolButton, ButtonToolItem>, ButtonToolItem.IHandler
	{

		#region IToolBarButton Members

		
		#endregion

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			Gtk.Toolbar tb = handler.Control;

			Control = new Gtk.ToolButton(GtkImage, Text);
			Control.IsImportant = true;
			Control.Sensitive = Enabled;
			Control.TooltipText = this.ToolTip;
			Control.ShowAll();
			Control.NoShowAll = true;
			Control.Visible = Visible;
			//control.CanFocus = false;			// why is this disabled and not true???
			tb.Insert(Control, index);
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
