using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class TabControlHandler : MacView<NSTabView, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		bool disableSelectedIndexChanged;

		public bool RecurseToChildren { get { return true; } }

		public override NSView ContainerControl { get { return Control; } }

		public override IEnumerable<Control> VisualControls => Widget.Controls;

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

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSTabView CreateControl() => new EtoTabView();

		protected override void Initialize()
		{
			Enabled = true;
			base.Initialize();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
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
			var tab = handler.Widget.Pages.FirstOrDefault(r => ((TabPageHandler)r.Handler).Control == item);
			return tab == null || tab.Enabled;
		}

		static void HandleDidSelect(object sender, NSTabViewItemEventArgs e)
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
			get { return (int)(Control.Selected == null ? -1 : Control.IndexOf(Control.Selected)); }
			set { Control.SelectAt(value); }
		}

		protected override bool ControlEnabled
		{
			get => base.ControlEnabled;
			set
			{
				base.ControlEnabled = value;
				foreach (var child in Widget.Controls)
				{
					child.GetMacViewHandler()?.SetEnabled(value);
				}
			}
		}

		public void InsertTab(int index, TabPage page)
		{
			var tabViewItem = ((TabPageHandler)page.Handler).Control;
			if (index == -1)
				Control.Add(tabViewItem);
			else
				Control.Insert(tabViewItem, index);
		}

		public void ClearTabs()
		{
			foreach (var tab in Control.Items)
				Control.Remove(tab);
		}

		public void RemoveTab(int index, TabPage page)
		{
			disableSelectedIndexChanged = true;
			try
			{
				var isSelected = SelectedIndex == index;
				Control.Remove(((TabPageHandler)page.Handler).Control);
				if (isSelected && Control.Items.Length > 0)
					SelectedIndex = Math.Min(index, Control.Items.Length - 1);
				if (Widget.Loaded)
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			}
			finally
			{
				disableSelectedIndexChanged = false;
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var naturalSize = NaturalSize;
			if (naturalSize != null)
				return naturalSize.Value;

			var size = base.GetNaturalSize(availableSize);
			var borderSize = Control.Frame.Size.ToEto() - Control.ContentRect.Size.ToEto();

			foreach (var tab in Widget.Pages.Where(r => r.Visible))
			{
				size = SizeF.Max(size, tab.GetPreferredSize(availableSize));
			}
			naturalSize = size + borderSize;
			NaturalSize = naturalSize;
			return naturalSize.Value;
		}

		public DockPosition TabPosition
		{
			get { return Control.TabViewType.ToEto(); }
			set { Control.TabViewType = value.ToNS(); }
		}
	}
}
