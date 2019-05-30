using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Handlers
{
	public class TabControlHandler : ThemedContainerHandler<Container, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		/// <summary>
		/// Contains the tabs
		/// </summary>
		readonly Tabs tabs;

		/// <summary>
		/// The content for the currently selected tab
		/// </summary>
		public Panel ContentPanel { get; private set; }

		public TabControlHandler()
		{
			tabs = new Tabs
			{
				SelectionChanged = tab =>
				{
					Panel tabContentPanel = null;
					if (tab != null)
					{
						var tabHandler = tab.Tag as TabPageHandler;
						if (tabHandler != null)
							tabContentPanel = tabHandler.Control;
					}
					ContentPanel.Content = tabContentPanel;
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				},
			};

			ContentPanel = new Panel { BackgroundColor = Colors.White };
			var layout = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
			layout.BeginHorizontal();
			layout.Add(tabs);
			layout.Add(ContentPanel, xscale: true);
			layout.EndHorizontal();
			Control = layout;
		}

		public int SelectedIndex
		{
			get { return tabs.SelectedIndex; }
			set { tabs.SelectedIndex = value; }
		}

		public void InsertTab(int index, TabPage page)
		{
			var p = page != null ? page.Handler as TabPageHandler : null;

			if (p != null)
				tabs.Insert(p.Tab, index);
		}

		public void ClearTabs()
		{
			tabs.RemoveAllTabs();
		}

		public void RemoveTab(int index, TabPage page)
		{
			tabs.Remove(index);
		}

		public DockPosition TabPosition
		{
			get;
			set;
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
		public Tab SelectedTab
		{
			get { return selectedTab; }
			private set
			{
				if (!ReferenceEquals(selectedTab, value))
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

		readonly List<Tab> items;

		public Tabs()
		{
			items = new List<Tab>();
		}

		internal void Remove(int index)
		{
			var isSelected = SelectedIndex == index;
			items[index].Click -= HandleClick;
			items.RemoveAt(index);
			if (isSelected)
				SelectedIndex = Math.Min(items.Count - 1, index);
			LayoutItems();
		}

		internal void RemoveAllTabs()
		{
			foreach (var r in items) r.Click -= HandleClick;
			items.Clear();
			LayoutItems();
			SelectedIndex = -1;
		}

		internal void Insert(Tab tab, int index)
		{
			items.Insert(index, tab);
			tab.Click += HandleClick;
			LayoutItems();
			if (SelectedTab == null)
				SelectedTab = tab;
		}

		void LayoutItems()
		{
			var layout = new DynamicLayout { Padding = Padding.Empty, Spacing =  Size.Empty };
			layout.BeginVertical();
			foreach (var tab in items)
				layout.Add(tab);
			layout.Add(null);
			layout.EndVertical();
			Content = layout;
		}

		void HandleClick(object sender, EventArgs e)
		{
			SelectedTab = (Tab)sender;
		}

		public int SelectedIndex
		{
			get
			{
				for (var i = 0; i < items.Count; ++i)
					if (ReferenceEquals(items[i], SelectedTab))
						return i;
				return -1;
			}
			set
			{
				if (value >= 0 && value < items.Count)
					SelectedTab = items[value];
				else
					SelectedTab = null;
			}
		}
	}
}
