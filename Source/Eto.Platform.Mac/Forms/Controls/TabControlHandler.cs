using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class TabControlHandler : MacView<NSTabView, TabControl>, ITabControl
	{
		NSTabView control;

		public TabControlHandler()
		{
			control = new NSTabView();
			control.DidSelect += HandleControlDidSelect;
			//control.B
			Control = control;
		}

		void HandleControlDidSelect (object sender, NSTabViewItemEventArgs e)
		{
			((TabControl)Widget).OnSelectedIndexChanged(e);
		}

		
		#region ITabControl Members

		public int SelectedIndex
		{
			get { return control.IndexOf(control.Selected); }
			set { control.SelectAt(value); }
		}

		public void AddTab(TabPage page)
		{
			control.Add((NSTabViewItem)page.ControlObject);
		}

		public void RemoveTab(TabPage page)
		{
			control.Remove((NSTabViewItem)page.ControlObject);
		}

		#endregion
	}
}
