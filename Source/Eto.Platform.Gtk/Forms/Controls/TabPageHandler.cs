using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class TabPageHandler : GtkContainer<Gtk.VBox, TabPage>, ITabPage
	{
		Gtk.Label label;
		//Gtk.VBox vbox;

		public TabPageHandler()
		{
			Control = new Gtk.VBox();
			label = new Gtk.Label();
			label.ButtonPressEvent += new Gtk.ButtonPressEventHandler(label_ButtonPressEvent);
		}
		
		public override object ContainerObject {
			get {
				return Control;
			}
		}

		public override void SetLayout(Layout inner)
		{
			if (Control.Children.Length > 0)
				foreach (Gtk.Widget child in Control.Children)
					Control.Remove(child);
			IGtkLayout gtklayout = (IGtkLayout)inner.Handler;
			Control.PackStart((Gtk.Widget)gtklayout.ContainerObject, false, false, 0);
		}

		public override string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		private void label_ButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			//base.OnClick(EventArgs.Empty);
		}
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing) {
				if (label != null) { label.Dispose(); label = null; }
			}
		}
	}
}
