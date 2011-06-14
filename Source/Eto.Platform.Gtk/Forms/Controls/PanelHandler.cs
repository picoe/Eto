using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class PanelHandler : GtkContainer<Gtk.EventBox, Panel>, IPanel
	{
		Gtk.VBox box;
		
		public PanelHandler()
		{
			Control = new Gtk.EventBox();
			box = new Gtk.VBox();
			Control.Add (box);
		}

		public override object ContainerObject
		{
			get { return box; }
		}
		
		public override void SetLayout(Layout inner)
		{
			if (box.Children.Length > 0)
				foreach (Gtk.Widget child in box.Children)
					box.Remove(child);
			IGtkLayout gtklayout = (IGtkLayout)inner.Handler;
			var containerWidget = (Gtk.Widget)gtklayout.ContainerObject;
			box.Add(containerWidget);
			containerWidget.ShowAll ();
		}
		

	}
}
