using System;
using System.Linq;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class TabControlHandler : MacView<NSTabView, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		bool disableSelectedIndexChanged;

		public bool RecurseToChildren { get { return true; } }

		public override NSView ContainerControl { get { return Control; } }

		public class EtoTabView : NSTabView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		// CWEN: should have some form of implementation here
		public virtual Size ClientSize { get { return Size; } set { Size = value; } }

		public TabControlHandler ()
		{
			Enabled = true;
			Control = new EtoTabView { Handler = this };
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			Control.ShouldSelectTabViewItem += HandleShouldSelectTabViewItem;
			Control.DidSelect += HandleDidSelect;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			Control.ShouldSelectTabViewItem -= HandleShouldSelectTabViewItem;
			Control.DidSelect -= HandleDidSelect;
		}

		static bool HandleShouldSelectTabViewItem(NSTabView tabView, NSTabViewItem item)
		{
			var handler = ((EtoTabView)tabView).WeakHandler.Target as TabControlHandler;
			var tab = handler.Widget.Pages.FirstOrDefault (r => ((TabPageHandler)r.Handler).TabViewItem == item);
			return tab == null || tab.Enabled;
		}

		static void HandleDidSelect (object sender, NSTabViewItemEventArgs e)
		{
			var handler = GetHandler(sender) as TabControlHandler;
			if (handler != null)
			{
				if (!handler.disableSelectedIndexChanged)
					handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
			}
		}

		public int SelectedIndex
		{
			get { return Control.Selected == null ? -1 : Control.IndexOf(Control.Selected); }
			set { Control.SelectAt (value); }
		}

		public override bool Enabled { get; set; }
		
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
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			} finally {
				disableSelectedIndexChanged = false;
			}
		}

		protected override SizeF GetNaturalSize (SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			foreach (var tab in Widget.Pages.Where(r => r.Visible)) {
				size = SizeF.Max (size, tab.GetPreferredSize(availableSize));
			}
			return size;
		}
	}
}
