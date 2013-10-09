using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Handlers
{
	public class TabControlHandler : ThemedContainerHandler<Container, TabControl>, ITabControl
	{
		/// <summary>
		/// Contains the tabs
		/// </summary>
		private Tabs Tabs { get; set; }

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

			if (this.Tabs != null && p != null)
				this.Tabs.Insert(p.Tab, index); 
		}

		public void ClearTabs()
		{
			if (this.Tabs != null)
				this.Tabs.RemoveAllTabs();
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
			this.LayoutItems();
		}

		internal void RemoveAllTabs()
		{
			Items.ForEach(r => r.Click -= HandleClick);
			Items.Clear();
			LayoutItems();
		}

		internal void Insert(Tab tab, int index)
		{
			Items.Insert(index, tab);
			tab.Click += HandleClick;;
			LayoutItems();
			if (SelectedTab == null)
				SelectedTab = tab;
		}

		private void LayoutItems()
		{
			var layout = new DynamicLayout(padding: Padding.Empty, spacing: Size.Empty);
			layout.BeginVertical();
			foreach (var tab in Items)
				layout.Add(tab);
			layout.Add(null);
			layout.EndVertical();
			this.Content = layout;
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
