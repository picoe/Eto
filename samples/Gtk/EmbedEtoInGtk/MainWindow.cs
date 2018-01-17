using System;
using Gtk;
using Eto.Forms;

namespace EmbedEtoInGtk
{
	public partial class MainWindow: Gtk.Window
	{
		public MainWindow() : base(Gtk.WindowType.Toplevel)
		{
			Build();

			var nativeWidget = new MyEtoPanel().ToNative(true);

			this.Child = nativeWidget;
		}

		protected void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			Gtk.Application.Quit();
			a.RetVal = true;
		}
	}

}