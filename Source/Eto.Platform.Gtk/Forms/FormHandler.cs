using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class FormHandler : GtkWindow<Gtk.Window, Form>, IForm
	{
		public FormHandler()
		{
			Control = new Gtk.Window(Gtk.WindowType.Toplevel);
			Control.AllowShrink = true;
			Control.SetSizeRequest(100,100);
			Control.SetPosition(Gtk.WindowPosition.Center);
			Control.Add(vbox);
		}

		public void Show()
		{
			Control.ShowAll();
		}
	}
}
