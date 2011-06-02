using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class TabPageHandler : GtkContainer<Gtk.Label, TabPage>, ITabPage
	{
		Gtk.VBox vbox;

		public TabPageHandler()
		{
			vbox = new Gtk.VBox();
			Control = new Gtk.Label();
			Control.ButtonPressEvent += new Gtk.ButtonPressEventHandler(label_ButtonPressEvent);
			//control.Click += control_Click;
		}


		public override object ContainerObject
		{
			get { return vbox; }
		}

		public override void SetLayout(Layout inner)
		{
			if (vbox.Children.Length > 0)
				foreach (Gtk.Widget child in vbox.Children)
					vbox.Remove(child);
			IGtkLayout gtklayout = (IGtkLayout)inner.Handler;
			vbox.PackStart((Gtk.Widget)gtklayout.ContainerObject, false, false, 0);
		}

		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		private void label_ButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			//base.OnClick(EventArgs.Empty);
		}
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing) {
				if (vbox != null) { vbox.Dispose(); vbox = null; }
			}
		}
	}
}
