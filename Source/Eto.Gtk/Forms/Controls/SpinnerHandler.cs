using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class SpinnerHandler : GtkControl<Gtk.Spinner, Spinner>, ISpinner
	{
		bool enabled;
		public SpinnerHandler()
		{
			Control = new Gtk.Spinner();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if (enabled)
				Control.Start();
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			if (enabled)
				Control.Stop();
		}

		public override bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					enabled = value;
					if (Widget.Loaded)
					{
						if (enabled)
							Control.Start();
						else
							Control.Stop();
					}
				}
			}
		}
	}
}

