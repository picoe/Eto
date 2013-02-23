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
		
		public void InsertTab (int index, TabPage page)
		{
			var pageHandler = (TabPageHandler)page.Handler;

			if (Widget.Loaded) {
				pageHandler.Control.ShowAll ();
				pageHandler.LabelControl.ShowAll ();
			}
			
			if (index == -1)
				Control.AppendPage(pageHandler.Control, pageHandler.LabelControl);
			else
				Control.InsertPage(pageHandler.Control, pageHandler.LabelControl, index);
		}
		
		public void ClearTabs ()
		{
			while (Control.NPages > 0)
				Control.RemovePage (0);
		}

		public void RemoveTab (int index, TabPage page)
		{
			Control.RemovePage (index);
		}
	}
}
