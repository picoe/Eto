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

		// Pages index; hidden pages aren't in the native tab control, so indexes differ from it.
		public int SelectedIndex {
			get {
				var selected = Control.SelectedTab;
				if (selected == null)
					return -1;
				var page = Widget.Pages.FirstOrDefault(r => ((TabPageHandler)r.Handler).Control == selected);
				return page != null ? Widget.Pages.IndexOf(page) : -1;
			}
			set {
				if (value < 0 || value >= Widget.Pages.Count)
					return;
				var tab = ((TabPageHandler)Widget.Pages[value].Handler).Control;
				if (tab.Parent == Control)
					Control.SelectedTab = tab;
			}
		}

		public DockPosition TabPosition
		{
			get { return Control.Alignment.ToEto(); }
			set { Control.Alignment = value.ToSWF(); }
		}

		public void InsertTab (int index, TabPage page)
		{
			if (!page.Visible)
				return;
			AddItem (page);
			if (Widget.Loaded && Control.TabPages.Count == 1)
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
		}
		
		public void RemoveTab (int index, TabPage page)
		{
			RemoveItem (page, true);
		}

		internal void SetPageVisible (TabPage page, bool visible)
		{
			if (visible) {
				var wasEmpty = Control.TabPages.Count == 0;
				AddItem (page);
				if (Widget.Loaded && wasEmpty)
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			}
			else
				RemoveItem (page, false);
		}

		void AddItem (TabPage page)
		{
			var tab = ((TabPageHandler)page.Handler).Control;
			if (tab.Parent == Control)
				return;
			disableSelectedIndexChanged = true;
			try {
				var selected = Control.SelectedTab;
				// swf.TabPage.Visible doesn't hide its tab, so only visible pages are added.
				var nativeIndex = Widget.Pages.TakeWhile(r => r != page).Count(r => r.Visible);
				if (nativeIndex >= Control.TabPages.Count)
					Control.TabPages.Add (tab);
				else
					Control.TabPages.Insert (nativeIndex, tab);
				// inserting before the selected tab keeps the native index, not the tab
				if (selected != null && Control.SelectedTab != selected)
					Control.SelectedTab = selected;
			} finally {
				disableSelectedIndexChanged = false;
			}
		}

		void RemoveItem (TabPage page, bool alwaysNotify)
		{
			var tab = ((TabPageHandler)page.Handler).Control;
			if (tab.Parent != Control)
				return;
			disableSelectedIndexChanged = true;
			try {
				var nativeIndex = Control.TabPages.IndexOf (tab);
				var isSelected = Control.SelectedTab == tab;
				Control.TabPages.Remove (tab);
				if (isSelected)
					Control.SelectedIndex = Math.Min (nativeIndex, Control.TabPages.Count - 1);
				if (Widget.Loaded && (alwaysNotify || isSelected))
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
