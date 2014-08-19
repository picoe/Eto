using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class LinkButtonHandler : GtkControl<Gtk.LinkButton, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{
		public LinkButtonHandler()
		{
			Control = new Gtk.LinkButton(string.Empty);
			Control.TooltipText = null;
		}

		public Color TextColor
		{
			get { return Control.Style.Foreground(Gtk.StateType.Normal).ToEto(); }
			set { Control.ModifyFg(Gtk.StateType.Normal, value.ToGdk()); }
		}

		public override string Text
		{
			get { return Control.Label;  }
			set { Control.Label = value; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case LinkButton.ClickEvent:
					Control.Clicked += Connector.HandleClicked;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new LinkButtonConnector Connector { get { return (LinkButtonConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new LinkButtonConnector();
		}

		protected class LinkButtonConnector : GtkControlConnector
		{
			public new LinkButtonHandler Handler { get { return (LinkButtonHandler)base.Handler; } }

			public void HandleClicked(object sender, EventArgs e)
			{
				Handler.Callback.OnClick(Handler.Widget, EventArgs.Empty);
			}
		}

	}
}