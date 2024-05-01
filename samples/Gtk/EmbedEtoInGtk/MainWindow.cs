using System;
using Gtk;
using Eto.Forms;

namespace EmbedEtoInGtk
{

	public class MainWindow: Gtk.Window
	{
		public MainWindow() : base(Gtk.WindowType.Toplevel)
		{
			Title = "EmbedEtoInGtk.MainWindow";
			WindowPosition = Gtk.WindowPosition.CenterOnParent;
			DefaultWidth = 423;
			DefaultHeight = 312;
			DeleteEvent += OnDeleteEvent;

			var nativeWidget = new MyEtoPanel().ToNative(true);

			Child = nativeWidget;
		}

		protected void OnDeleteEvent(object sender, DeleteEventArgs a)
		{
			Gtk.Application.Quit();
			a.RetVal = true;
		}
	}

}