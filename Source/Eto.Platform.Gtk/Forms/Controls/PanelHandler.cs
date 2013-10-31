using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class PanelHandler : GtkDockContainer<Gtk.EventBox, Panel>, IPanel
	{
		Gtk.VBox box;

		public PanelHandler()
		{
			Control = new Gtk.EventBox();
			//Control.VisibleWindow = false; // can't use this as it causes overlapping widgets
			box = new Gtk.VBox();
			Control.Add(box);
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			box.Add(content);
		}
	}
}
