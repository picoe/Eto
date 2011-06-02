using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class PanelHandler : GtkContainer<Gtk.VBox, Panel>, IPanel
	{
		
		public PanelHandler()
		{
			Control = new Gtk.VBox();
		}

		public override object ContainerObject
		{
			get { return Control; }
		}
		
		public override void SetLayout(Layout inner)
		{
			if (Control.Children.Length > 0)
				foreach (Gtk.Widget child in Control.Children)
					Control.Remove(child);
			IGtkLayout gtklayout = (IGtkLayout)inner.Handler;
			Control.Add((Gtk.Widget)gtklayout.ContainerObject);
		}
		

	}
}
