using System;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class TabControlHandler : WpfContainer<swc.TabControl, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		bool disableSelectedIndexChanged;
		public TabControlHandler()
		{
			Control = new swc.TabControl();
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
