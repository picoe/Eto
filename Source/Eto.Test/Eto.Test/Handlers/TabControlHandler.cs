using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Handlers
{
	public class TabControlHandler : ThemedContainerHandler<Container, TabControl>, ITabControl
	{
		/// <summary>
		/// Contains the tabs
		/// </summary>
		Tabs Tabs { get; set; }

		/// <summary>
		/// The content for the currently selected tab
		/// </summary>
		public Panel ContentPanel { get; private set; }

		public Action NewTabClicked { get; set; }

		public Action<Panel> SetContentPanelDelegate { get; set; }

		public TabControlHandler()
		{
			this.Tabs = new Tabs
			{
				SelectionChanged = tab => {
					Panel tabContentPanel;
					TabPageHandler h;
					if (tab != null &&
					    (h = tab.Tag as TabPageHandler) != null &&
					    this.ContentPanel != null)
					{
						tabContentPanel = h.Control;
						if (SetContentPanelDelegate != null)
							SetContentPanelDelegate(tabContentPanel);
						else
							this.ContentPanel.Content = tabContentPanel; // can be null
					}
				},
			};
			ClearTabs();

			this.ContentPanel = new Panel { BackgroundColor = Colors.White };
			var layout = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
			layout.BeginHorizontal();
			layout.Add(Tabs);
			layout.Add(ContentPanel, xscale: true);
			layout.EndHorizontal();
			Control = layout;
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
			var p = page != null ? page.Handler as TabPageHandler : null;

			if (Tabs != null && p != null)
				Tabs.Insert(p.Tab, index); 
		}

		public void ClearTabs()
		{
			if (Tabs != null)
				Tabs.RemoveAllTabs();
		}

		public void RemoveTab(int index, TabPage page)
		{
			if (Tabs != null)
				Tabs.Remove(index);
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

		Tab selectedTab;
		public Tab SelectedTab {
			get { return selectedTab; }
			private set {
				if (!object.ReferenceEquals(selectedTab, value))
				{
					if (selectedTab != null)
						selectedTab.Selected = false;
					selectedTab = value;
					if (selectedTab != null)
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
			Items[index].Click -= HandleClick;
			Items.RemoveAt(index);
			LayoutItems();
		}

		internal void RemoveAllTabs()
		{
			foreach (var r in Items) r.Click -= HandleClick;
			Items.Clear();
			LayoutItems();
		}

		internal void Insert(Tab tab, int index)
		{
			Items.Insert(index, tab);
			tab.Click += HandleClick;
			LayoutItems();
			if (SelectedTab == null)
				SelectedTab = tab;
		}

		void LayoutItems()
		{
			var layout = new DynamicLayout(padding: Padding.Empty, spacing: Size.Empty);
			layout.BeginVertical();
			foreach (var tab in Items)
				layout.Add(tab);
			layout.Add(null);
			layout.EndVertical();
			Content = layout;
		}

		void HandleClick (object sender, EventArgs e)
		{
			SelectedTab = (Tab)sender;
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
