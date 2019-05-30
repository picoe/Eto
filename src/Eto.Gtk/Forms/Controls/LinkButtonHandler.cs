using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.GtkSharp.Forms.Controls
{
	public class LinkButtonHandler : GtkControl<Gtk.LinkButton, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{

		Gtk.EventBox box;

		public override Gtk.Widget ContainerControl
		{
			get { return box; }
		}

		public LinkButtonHandler()
		{
			Control = new Gtk.LinkButton(string.Empty);
			Control.Xalign = 0f;
			Control.Yalign = .5f;
			Control.TooltipText = null;
			box = new Gtk.EventBox();
			box.Child = Control;
		}

		public Color TextColor
		{
			get { return Control.GetForeground(); }
			set
			{
				Control.SetForeground(value);
				Control.SetTextColor(value);
				Control.Child.SetForeground(value);
				Control.Child.SetTextColor(value);
			}
		}

		public Color DisabledTextColor
		{
			get { return Control.GetForeground(GtkStateFlags.Insensitive); }
			set
			{
				Control.SetForeground(value, GtkStateFlags.Insensitive);
				Control.Child.SetForeground(value, GtkStateFlags.Insensitive);
			}
		}

		public override string Text
		{
			get { return Control.Label; }
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