#if TODO_XAML
using System;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class TabControlHandler : WpfContainer<swc.TabControl, TabControl>, ITabControl
	{
		bool disableSelectedIndexChanged;
		public TabControlHandler ()
		{
			Control = new swc.TabControl ();
			Control.Loaded += delegate {
				Control.SelectionChanged += delegate {
					if (!disableSelectedIndexChanged)
						Widget.OnSelectedIndexChanged (EventArgs.Empty);
				};
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

		public void InsertTab (int index, TabPage page)
		{
			if (index == -1)
				Control.Items.Add (page.ControlObject);
			else
				Control.Items.Insert (index, page.ControlObject);
			if (Widget.Loaded && Control.Items.Count == 1)
				SelectedIndex = 0;
		}

		public void ClearTabs ()
		{
			Control.Items.Clear ();
		}

		public void RemoveTab (int index, TabPage page)
		{
			disableSelectedIndexChanged = true;
			try {
				Control.Items.Remove (page.ControlObject);
				if (Widget.Loaded)
					Widget.OnSelectedIndexChanged (EventArgs.Empty);
			} finally {
				disableSelectedIndexChanged = false;
			}
		}

		public override void Remove(Windows.UI.Xaml.FrameworkElement child)
		{
			// no need
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			foreach (var tab in Widget.TabPages)
			{
				var handler = tab.GetWpfFrameworkElement();
				if (handler != null)
					handler.SetScale(xscale, yscale);
			}
		}
	}
}
#endif