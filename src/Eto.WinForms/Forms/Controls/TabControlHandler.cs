using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class TabControlHandler : WindowsContainer<swf.TabControl, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		bool disableSelectedIndexChanged;
		public TabControlHandler ()
		{
			this.Control = new swf.TabControl ();
			this.Control.ImageList = new swf.ImageList{ ColorDepth = swf.ColorDepth.Depth32Bit };
			this.Control.SelectedIndexChanged += (sender, e) => {
				if (!disableSelectedIndexChanged)
					Callback.OnSelectedIndexChanged(Widget, e);
			};
		}

		public int SelectedIndex {
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		public DockPosition TabPosition
		{
			get { return Control.Alignment.ToEto(); }
			set { Control.Alignment = value.ToSWF(); }
		}

		public void InsertTab (int index, TabPage page)
		{
			var pageHandler = (TabPageHandler)page.Handler;
			if (index == -1 || index == Control.TabPages.Count)
				Control.TabPages.Add (pageHandler.Control);
			else
				Control.TabPages.Insert (index, pageHandler.Control);
			if (Widget.Loaded && Control.TabPages.Count == 1)
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
		}
		
		public void RemoveTab (int index, TabPage page)
		{
			disableSelectedIndexChanged = true;
			try {
				var tab = Control.TabPages[index];
				var isSelected = Control.SelectedIndex == index;
				Control.TabPages.Remove (tab);
				if (isSelected)
					Control.SelectedIndex = Math.Min (index, Control.TabPages.Count - 1);
				if (Widget.Loaded)
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			} finally {
				disableSelectedIndexChanged = false;
			}
		}
		
		public void ClearTabs ()
		{
			Control.TabPages.Clear ();
		}
	}
}
