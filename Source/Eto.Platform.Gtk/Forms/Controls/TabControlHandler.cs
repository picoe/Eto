using System;
using System.Collections;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class TabControlHandler : GtkControl<Gtk.Notebook, TabControl>, ITabControl
	{
		public TabControlHandler()
		{
			Control = new Gtk.Notebook();
			Control.SwitchPage += delegate {
				Widget.OnSelectedIndexChanged (EventArgs.Empty);
				if (SelectedIndex >= 0)
					Widget.TabPages[SelectedIndex].OnClick (EventArgs.Empty);
			};
		}

		public int SelectedIndex
		{
			get { return Control.CurrentPage; }
			set { Control.CurrentPage = value; }
		}

		public void AddTab(TabPage page)
		{
			Control.AppendPage((Gtk.Widget)page.ContainerObject, (Gtk.Widget)((TabPageHandler)page.Handler).LabelControl);
		}

		public void RemoveTab(TabPage page)
		{
			Control.RemovePage(Control.PageNum((Gtk.Widget)page.ContainerObject));
		}
	}
}
