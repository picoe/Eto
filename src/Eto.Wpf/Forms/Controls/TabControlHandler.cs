using Eto.Wpf.CustomControls;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoTabControl : swc.TabControl, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			var desired = Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
			// once loaded, act as normal
			if (IsLoaded)
				return desired;

			// not loaded yet, let's calculate size based on all tabs
			var size = new sw.Size(0, 0);
			var selectedSize = new sw.Size(0, 0);
			var selected = SelectedItem as swc.TabItem;

			foreach (var tab in Items.Cast<swc.TabItem>())
			{
				var tabContent = tab.Content as sw.FrameworkElement;
				if (tabContent == null)
					continue;
				tabContent.Measure(constraint);
				var tabSize = tabContent.DesiredSize;
				if (tab == selected)
					selectedSize = tabSize;
				size = size.Max(tabSize);
			}
			// calculate size of the border around the content based on selected tab's content size
			var borderSize = desired.Subtract(selectedSize);
			// return max height with border
			return size.Add(borderSize);
		}
	}

	public class TabControlHandler : WpfContainer<swc.TabControl, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		bool disableSelectedIndexChanged;
		public TabControlHandler()
		{
			Control = new EtoTabControl();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			Control.SelectionChanged += (sender, ee) =>
			{
				if (ReferenceEquals(ee.Source, Control) && !disableSelectedIndexChanged)
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			};
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public override Size ClientSize { get; set; } // TODO

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set
			{
				// a collapsed TabItem can still be selected, showing a page without a tab
				if (value >= 0 && value < Widget.Pages.Count && !Widget.Pages[value].Visible)
					return;
				Control.SelectedIndex = value;
			}
		}

		public DockPosition TabPosition
		{
			get { return Control.TabStripPlacement.ToEtoTabPosition(); }
			set { Control.TabStripPlacement = value.ToWpf(); }
		}

		public void InsertTab(int index, TabPage page)
		{
			if (index == -1)
				Control.Items.Add(page.ControlObject);
			else
				Control.Items.Insert(index, page.ControlObject);
			if (Widget.Loaded)
				page.GetWpfFrameworkElement()?.SetScale(XScale, YScale);
			if (Control.SelectedIndex == -1 && page.Visible)
				Control.SelectedItem = page.ControlObject;
		}

		internal void SetPageVisible(TabPage page, bool visible)
		{
			if (visible)
			{
				if (Control.SelectedIndex == -1)
					Control.SelectedItem = page.ControlObject;
				return;
			}
			if (!ReferenceEquals(Control.SelectedItem, page.ControlObject))
				return;
			// prefer the next visible page, like the other platforms
			var index = Widget.Pages.IndexOf(page);
			var next = Widget.Pages.Skip(index + 1).FirstOrDefault(r => r.Visible)
				?? Widget.Pages.Take(index).LastOrDefault(r => r.Visible);
			if (next != null)
				Control.SelectedItem = next.ControlObject;
			else
				Control.SelectedIndex = -1;
		}

		public void ClearTabs()
		{
			Control.Items.Clear();
		}

		public void RemoveTab(int index, TabPage page)
		{
			disableSelectedIndexChanged = true;
			try
			{
				var isSelected = SelectedIndex == index;
				Control.Items.Remove(page.ControlObject);
				if (Widget.Loaded && isSelected)
					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			}
			finally
			{
				disableSelectedIndexChanged = false;
			}
		}

		public override void Remove(System.Windows.FrameworkElement child)
		{
			// no need
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			foreach (var tab in Widget.Pages)
			{
				var handler = tab.GetWpfFrameworkElement();
				if (handler != null)
					handler.SetScale(xscale, yscale);
			}
		}
	}
}
