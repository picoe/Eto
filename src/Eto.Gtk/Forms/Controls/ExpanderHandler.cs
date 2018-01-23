using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ExpanderHandler : GtkPanel<Gtk.Expander, Expander, Expander.ICallback>, Expander.IHandler
	{
		public ExpanderHandler()
		{
			Control = new Gtk.Expander(string.Empty);
		}

		protected new ExpanderConnector Connector { get { return (ExpanderConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ExpanderConnector();
		}

		protected class ExpanderConnector : GtkPanelEventConnector
		{
			public new ExpanderHandler Handler { get { return (ExpanderHandler)base.Handler; } }

			public void HandleActivated(object sender, EventArgs e)
			{
				Handler.Callback.OnExpandedChanged(Handler.Widget, EventArgs.Empty);
			}
		}

		public bool Expanded
		{
			get { return Control.Expanded; }
			set
			{ 
				if (Control.Expanded != value)
				{
					Control.Expanded = value;
					Callback.OnExpandedChanged(Widget, EventArgs.Empty);
				}
			}
		}

		static readonly object Header_Key = new object();

		public Eto.Forms.Control Header
		{
			get { return Widget.Properties.Get<Control>(Header_Key); }
			set
			{
				Widget.Properties.Set(Header_Key, value, () =>
				{
					Control.LabelWidget = value.GetContainerWidget();
				});
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Expander.ExpandedChangedEvent:
					Control.Activated += Connector.HandleActivated;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.Child = content;
		}
	}
}
