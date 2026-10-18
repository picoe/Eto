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

		// Pages index; hidden pages aren't in the native tab view, so indexes differ from it.
		public int SelectedIndex
		{
			get
			{
				var selected = Control.Selected;
				if (selected == null)
					return -1;
				var page = Widget.Pages.FirstOrDefault(r => ((TabPageHandler)r.Handler).Control == selected);
				return page != null ? Widget.Pages.IndexOf(page) : -1;
			}
			set
			{
				if (value < 0 || value >= Widget.Pages.Count)
					return;
				var item = ((TabPageHandler)Widget.Pages[value].Handler).Control;
				if (item.TabView != null)
					Control.Select(item);
			}
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
			if (page.Visible)
				AddItem(page);
		}

		public void ClearTabs()
		{
			foreach (var tab in Control.Items)
				Control.Remove(tab);
		}

		public void RemoveTab(int index, TabPage page)
		{
			RemoveItem(page, true);
		}

		internal void SetPageVisible(TabPage page, bool visible)
		{
			if (visible)
				AddItem(page);
			else
				RemoveItem(page, false);
		}

		void AddItem(TabPage page)
		{
			var item = ((TabPageHandler)page.Handler).Control;
			if (item.TabView != null)
				return;
			// NSTabView can't hide a tab, so only visible pages are added to it.
			var nativeIndex = Widget.Pages.TakeWhile(r => r != page).Count(r => r.Visible);
			Control.Insert(item, nativeIndex);
		}

		void RemoveItem(TabPage page, bool alwaysNotify)
		{
			var item = ((TabPageHandler)page.Handler).Control;
			if (item.TabView == null)
				return;
			disableSelectedIndexChanged = true;
			try
			{
				var nativeIndex = (int)Control.IndexOf(item);
				var isSelected = Control.Selected == item;
				Control.Remove(item);
				var count = Control.Items.Length;
				if (isSelected && count > 0)
					Control.SelectAt(Math.Min(nativeIndex, count - 1));
				if (Widget.Loaded && (alwaysNotify || isSelected))
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
