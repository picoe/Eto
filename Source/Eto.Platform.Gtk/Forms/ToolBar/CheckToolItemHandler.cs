using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class CheckToolItemHandler : ToolItemHandler<Gtk.ToggleToolButton, CheckToolItem>, ICheckToolItem
	{
		bool ischecked;
		bool isBeingChecked;

		public bool Checked
		{
			get { return (Control != null) ? Control.Active : ischecked; }
			set
			{
				if (value != ischecked)
				{
					isBeingChecked = true;
					if (Control != null)
						Control.Active = value;
					isBeingChecked = false;
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
			//Control.TooltipText = this.ToolTip;
			Control.IconWidget = GtkImage;
			Control.Sensitive = Enabled;
			Control.CanFocus = false;
			Control.IsImportant = true;
			tb.Insert(Control, index);
			if (tb.Visible)
				Control.ShowAll();
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Toggled += Connector.HandleToggled;
		}

		protected new CheckToolItemConnector Connector { get { return (CheckToolItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new CheckToolItemConnector();
		}

		protected class CheckToolItemConnector : WeakConnector
		{
			public new CheckToolItemHandler Handler { get { return (CheckToolItemHandler)base.Handler; } }

			public void HandleToggled(object sender, EventArgs e)
			{
				var handler = Handler;
				if (!handler.isBeingChecked)
				{
					handler.isBeingChecked = true;
					handler.Control.Active = handler.ischecked;
					handler.isBeingChecked = false;
					handler.Widget.OnClick(EventArgs.Empty);
				}
			}
		}
	}
}
