namespace Eto.GirCore.Forms.Controls
{
	public class PanelHandler : GirPanel<Gtk.Box, Panel, Panel.ICallback>, Panel.IHandler
	{
		public PanelHandler()
		{
			Control = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.Append(content);
		}

		protected override void RemoveContainerContent(Gtk.Widget content)
		{
			Control.Remove(content);
		}
	}
}
