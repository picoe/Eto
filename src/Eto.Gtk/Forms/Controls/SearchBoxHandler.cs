using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
#if GTKCORE
	public class SearchBoxHandler : TextBoxHandler<Gtk.SearchEntry, SearchBox, SearchBox.ICallback>, SearchBox.IHandler
	{
		public SearchBoxHandler()
		{
			Control = new Gtk.SearchEntry();
			Control.WidthRequest = 100;
		}
	}
#else
	public class SearchBoxHandler : TextBoxHandler, SearchBox.IHandler
	{
	}
#endif
}

