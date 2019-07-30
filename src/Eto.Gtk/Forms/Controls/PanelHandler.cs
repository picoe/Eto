using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class PanelHandler : GtkPanel<Gtk.EventBox, Panel, Panel.ICallback>, Panel.IHandler
	{
#if GTK3
		public PanelHandler()
		{
			Control = new EtoEventBox { Handler = this };
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.Add(content);
		}
#else
		readonly Gtk.VBox box;

		public PanelHandler()
		{
			Control = new Gtk.EventBox();
			box = new Gtk.VBox();
			Control.Add(box);
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			box.Add(content);
		}
#endif
	}
}
