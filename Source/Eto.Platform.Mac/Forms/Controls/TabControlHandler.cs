using System;
using System.Reflection;
using System.Linq;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TabControlHandler : MacView<NSTabView, TabControl>, ITabControl
	{
		bool disableSelectedIndexChanged;
		public class EtoTabView : NSTabView, IMacControl
		{
			public object Handler { get; set; }
		}
		
		public TabControlHandler ()
		{
			Enabled = true;
			Control = new EtoTabView { Handler = this };
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			Control.DidSelect += delegate {
				if (!disableSelectedIndexChanged)
					this.Widget.OnSelectedIndexChanged (EventArgs.Empty);
			};
			Control.ShouldSelectTabViewItem += (tabView, item) => {
				var tab = this.Widget.TabPages.FirstOrDefault (r => ((TabPageHandler)r.Handler).TabViewItem == item);
				if (tab != null)
					return tab.Enabled;
				else
					return true;
			}; 
		}

		public int SelectedIndex
		{
			get { return Control.Selected != null ? Control.IndexOf(Control.Selected) : -1; }
			set { Control.SelectAt (value); }
		}

		public override bool Enabled {
			get; set;
		}
		
		public void InsertTab (int index, TabPage page)
		{
			if (index == -1)
				Control.Add (((TabPageHandler)page.Handler).TabViewItem);
			else
				Control.Insert (((TabPageHandler)page.Handler).TabViewItem, index);
		}
		
		public void ClearTabs ()
		{
			foreach (var tab in Control.Items)
				Control.Remove (tab);
		}
		
		public void RemoveTab (int index, TabPage page)
		{
			disableSelectedIndexChanged = true;
			try {
				var isSelected = SelectedIndex == index;
				Control.Remove (((TabPageHandler)page.Handler).TabViewItem);
				if (isSelected && Control.Items.Length > 0)
					SelectedIndex = Math.Min (index, Control.Items.Length - 1);
				if (Widget.Loaded)
					Widget.OnSelectedIndexChanged (EventArgs.Empty);
			} finally {
				disableSelectedIndexChanged = false;
			}
		}

		protected override Size GetNaturalSize (Size availableSize)
		{
			Size size = base.GetNaturalSize(availableSize);
			foreach (var tab in Widget.TabPages) {
				size = Size.Max (size, tab.GetPreferredSize(availableSize));
			}
			return size;
		}
	}
}
