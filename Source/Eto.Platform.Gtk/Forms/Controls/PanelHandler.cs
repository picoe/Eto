using System;
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

		protected override void SetContent(Control content)
		{
			if (box.Children.Length > 0)
				foreach (Gtk.Widget child in box.Children)
					box.Remove(child);
			var containerWidget = content.GetContainerWidget();
			if (containerWidget != null)
			{
				if (containerWidget.Parent != null)
					containerWidget.Reparent(box);
				box.Add(containerWidget);
				containerWidget.ShowAll();
			}
		}
	}
}
