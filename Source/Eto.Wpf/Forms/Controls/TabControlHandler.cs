using System;
using System.Linq;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.CustomControls;

namespace Eto.Wpf.Forms.Controls
{
	public class TabControlHandler : WpfContainer<swc.TabControl, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		class EtoTabControl : swc.TabControl
		{
			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				// once loaded, act as normal
				if (IsLoaded)
					return base.MeasureOverride(constraint);

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
					size.Width = Math.Max(tabSize.Width, size.Width);
					size.Height = Math.Max(tabSize.Height, size.Height);
				}
				var baseSize = base.MeasureOverride(constraint);
				// calculate size of the border around the content based on selected tab's content size
				var borderSize = new sw.Size(baseSize.Width - selectedSize.Width, baseSize.Height - selectedSize.Height);
				// return max height with border
				return new sw.Size(size.Width + borderSize.Width, size.Height + borderSize.Height);
			}
		}

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
			set { Control.SelectedIndex = value; }
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
			if (Control.Items.Count == 1)
				SelectedIndex = 0;
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
