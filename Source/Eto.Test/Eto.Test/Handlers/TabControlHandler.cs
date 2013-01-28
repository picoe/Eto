using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Handlers
{
    public class TabControlHandler : ThemedContainerHandler<Panel, TabControl>, ITabControl
    {
        /// <summary>
        /// Contains the tabs
        /// </summary>
		private Tabs Tabs { get; set; }
		/// <summary>
		/// The content for the currently selected tab
		/// </summary>
        public Panel ContentPanel {get; private set;}
		public Action NewTabClicked { get; set; }

		public Action<Panel> SetContentPanelDelegate { get; set; }

        public TabControlHandler()
        {
            this.Control = new Panel { };
			this.Tabs = new Tabs
			{
				SelectionChanged = e => {
					var tab = e as Tab; 
					Panel tabContentPanel = null;
					TabPageHandler h = null;
					if (tab != null &&
						(h = tab.Tag as TabPageHandler) != null &&
						this.ContentPanel != null)
					{
						tabContentPanel = h.Control;
						if (SetContentPanelDelegate != null)
							SetContentPanelDelegate(tabContentPanel);
						else
							this.ContentPanel.AddDockedControl(tabContentPanel); // can be null
					}
				},
			};
 			ClearTabs();
			this.ContentPanel = new Panel { BackgroundColor = Colors.White };
			Control.LoadComplete += (s, e) =>
				{
					var tableLayout = new TableLayout(Control, 2, 1) { Padding = Padding.Empty, Spacing = Size.Empty };
					tableLayout.SetRowScale(0, scale: false);
					tableLayout.Add(Tabs, 0, 0);
					tableLayout.Add(ContentPanel, 1, 0);
				};
        }

        public int SelectedIndex
        {
            get
            {
                return Tabs != null ? Tabs.SelectedIndex : -1;
            }
            set
            {
                if (Tabs != null)
                    Tabs.SelectedIndex = value;
            }
        }

        public void InsertTab(int index, TabPage page)
        {
            var p = page != null
                ? page.Handler as TabPageHandler
                : null;

            if (this.Tabs != null &&
                p != null)
                this.Tabs.Insert(p.Tab, index); 
        }

        public void ClearTabs()
        {
			if (this.Tabs != null)
				this.Tabs.RemoveAll();
        }

        public void RemoveTab(int index, TabPage page)
        {
            if (this.Tabs != null)
                this.Tabs.Remove(index);
        }
    }

	class Tab : Button
	{
		bool selected;
		public bool Selected
		{
			get { return selected; }
			set
			{
				selected = value;
				BackgroundColor = selected ? Colors.LightCyan : Colors.Transparent;
			}
		}
	}

	class Tabs : Panel
	{
		public Action<Tab> SelectionChanged { get; set; }

		private Tab selectedTab;
		public Tab SelectedTab {
			get { return selectedTab; }
			private set {
				if (!object.ReferenceEquals(selectedTab, value))
				{
					if (selectedTab != null)
						selectedTab.Selected = false;
					if ((selectedTab = value) != null) // note: assignment
						selectedTab.Selected = true;
					if (SelectionChanged != null) SelectionChanged(value);
				}
			}
		}

		List<Tab> Items { get; set; }

		public Tabs()
		{
			this.Items = new List<Tab>();
		}

		internal void Remove(int index)
		{
			Items.RemoveAt(index);
			this.LayoutItems();
		}

		internal void RemoveAll()
		{
			Items.Clear();
			LayoutItems();
		}

		internal void Insert(Tab tab, int index)
		{
			Items.Insert(index, tab);
			tab.MouseUp += (s, e) => SelectedTab = tab;
			LayoutItems();
			if (SelectedTab == null)
				SelectedTab = tab;
		}

		private void LayoutItems()
		{
			var layout = new DynamicLayout(padding: Padding.Empty, spacing: Size.Empty);
			this.Layout = layout;
			layout.BeginVertical();
			foreach (var tab in Items)
				layout.Add(tab, xscale: true, yscale:false);
		}

		public int SelectedIndex
		{
			get
			{
				for (var i = 0; i < Items.Count; ++i)
					if (object.ReferenceEquals(Items[i], SelectedTab))
						return i;
				return -1;
			}
			set
			{
				if (value > 0 && value < Items.Count)
					SelectedTab = Items[value];
			}
		}
	}
}
