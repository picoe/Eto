using System;
using Eto.Forms;
using System.Linq;

namespace Eto.GtkSharp.Forms.ToolBar
{
	public class RadioToolItemHandler : ToolItemHandler<Gtk.RadioToolButton, RadioToolItem>, RadioToolItem.IHandler
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

			Control = new Gtk.RadioToolButton(handler.RadioGroup);
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
			Control.Toggled += Connector.HandleToggled;
		}

		protected new RadioToolItemConnector Connector { get { return (RadioToolItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new RadioToolItemConnector();
		}

		protected class RadioToolItemConnector : WeakConnector
		{
			public new RadioToolItemHandler Handler { get { return (RadioToolItemHandler)base.Handler; } }

			public void HandleToggled(object sender, EventArgs e)
			{
				Handler.Widget.OnClick(EventArgs.Empty);
			}
		}
	}
}
