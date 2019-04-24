using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ToggleButtonHandler : ButtonHandler<Gtk.ToggleButton, ToggleButton, ToggleButton.ICallback>, ToggleButton.IHandler
	{
		int suppressClick;

		public class EtoToggleButton : Gtk.ToggleButton
		{
			WeakReference _reference;
			public ToggleButtonHandler Handler
			{
				get => _reference?.Target as ToggleButtonHandler;
				set => _reference = new WeakReference(value);
			}
#if GTK3
			protected override void OnAdjustSizeRequest(Gtk.Orientation orientation, out int minimum_size, out int natural_size)
			{
				base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
				var h = Handler;
				if (h == null)
					return;
				h.MinimumSize.AdjustMinimumSizeRequest(orientation, ref minimum_size, ref natural_size);
			}
#endif
		}

		protected override Gtk.ToggleButton CreateControl() => new EtoToggleButton { Handler = this };

		public bool Checked
		{
			get => Control.Active;
			set
			{
				suppressClick++;
				Control.Active = value;
				suppressClick--;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ToggleButton.CheckedChangedEvent:
					// handled by button click
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new ToggleButtonConnector Connector => (ToggleButtonConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new ToggleButtonConnector();

		protected class ToggleButtonConnector : ButtonConnector
		{
			new ToggleButtonHandler Handler => (ToggleButtonHandler)base.Handler;

			public override void HandleClicked(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				h.Callback.OnCheckedChanged(h.Widget, EventArgs.Empty);

				if (h.suppressClick == 0)
					base.HandleClicked(sender, e);
			}
		}
	}
}
